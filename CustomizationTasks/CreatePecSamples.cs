
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Common.Workflow;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;
using Thermo.SampleManager.Server.Workflow;


namespace Customization.Tasks
{

    [SampleManagerTask("CreatePecSamples", "WorkflowCallback")]
    public class CreatePecSamples : SampleManagerTask
    {

        protected override void SetupTask()
        {
            base.SetupTask();


            if (Context.SelectedItems.Count > 0 && Context.SelectedItems[0] is Sample sample)
            {
                JobHeader job = sample.JobName as JobHeader;

                string wfName = "ITK_BULK_COMPOSITE_WF";

                Workflow sampwf = EntityManager.SelectByName(Workflow.EntityName, GetConfigHeader(wfName)) as Workflow;
                Workflow sampleWorkflow = EntityManager.SelectLatestVersion(Workflow.EntityName, sampwf?.WorkflowGuid) as Workflow;

                var childSamples = new List<Sample>();
                foreach (Sample childSample in sample.ChildSamples)
                {
                    childSamples.Add(childSample);
                }
                try
                {
                    MatchSamples(sample, childSamples, job, sampleWorkflow);
                    EntityManager.Commit();
                }
                catch (Exception ex)
                {
                    StringBuilder Msg = new StringBuilder();
                    //below lines instant kill previous sessions 20240910 remove and uncomment below commands for msg options
                    Msg.AppendLine($"cannot create additional composit samples for {sample.IdText}.");
                    Msg.AppendLine($"This sample appears to already have composites");
                    Msg.AppendLine("result : no samples created");
                    Library.Utils.FlashMessage(Msg.ToString(), "Error");
                }

            }
            Exit();
        }

        private void MatchSamples(Sample parentSample, IList<Sample> samples, JobHeader job, Workflow workflow)
        {
            var matches = new List<Tuple<Sample, Sample>>();
            for (int i = 0; i < samples.Count; i++)
            {
                for (int j = i + 1; j < samples.Count; j++)
                {
                    if (IsMatch(samples[i], samples[j]))
                    {
                        matches.Add(Tuple.Create(samples[i], samples[j]));
                    }
                }
            }
            // Process matches as needed
            for (int i = 0; i < matches.Count; i++)
            {
                var parentsampName = parentSample.IdText.ToString();
                IList<IEntity> samplesList = RunWorkflowForEntity(job, workflow, 1);


                foreach (Sample sample in samplesList)
                {


                    string name = $"{parentsampName}-C-0{i + 1}";
                    Sample sample1 = matches[i].Item1;
                    Sample sample2 = matches[i].Item2;

                    sample.ItkCompositeRef = $"{sample1.IdText}, {sample2.IdText}";
                    sample.IdText = name;
                    sample.IsComposite = true;
                    sample.CompositeParents.Add(matches[i].Item1);
                    sample.CompositeParents.Add(matches[i].Item2);
                    sample.JobName = job;
                    sample.Variety = parentSample.Variety;
                    sample.SampleBatch = parentSample.SampleBatch;
                    sample.SampleName = parentSample.SampleName;
                    sample.ExternalReference = parentSample.ExternalReference;

                    var qtest = EntityManager.CreateQuery<TestSchedHeaderBase>();
                    qtest.AddEquals(TestSchedHeaderPropertyNames.Identity, "TPPF001");

                    if (EntityManager.Select(qtest)?.GetFirst() is TestSchedHeaderBase testSchedHeaderBase)
                    {
                        sample.AddTests(testSchedHeaderBase);
                    }
                }
            }


        }

        private bool IsMatch(Sample sample1, Sample sample2)
        {
            // Implement your matching logic here
            // For example, matching based on specific criteria
            return (sample1.IdText.EndsWith("1") && sample2.IdText.EndsWith("6")) ||
                   (sample1.IdText.EndsWith("2") && sample2.IdText.EndsWith("7")) ||
                   (sample1.IdText.EndsWith("3") && sample2.IdText.EndsWith("8")) ||
                   (sample1.IdText.EndsWith("4") && sample2.IdText.EndsWith("9")) ||
                   (sample1.IdText.EndsWith("5") && sample2.IdText.EndsWith("0"));
        }



        private string GetConfigHeader(string entityId)
        {
            return EntityManager.Select<ConfigHeader>(entityId).Value;

        }

        private CustomerSampleTypeBase GetCustomerSampleType(Customer customer, string sampleType)
        {
            IQuery q = EntityManager.CreateQuery<CustomerSampleTypeBase>();
            q.AddEquals(CustomerSampleTypePropertyNames.Customer, customer);
            q.AddAnd();
            q.AddEquals(CustomerSampleTypePropertyNames.XmlSampleType, sampleType);
            return EntityManager.Select(q).GetFirst() as CustomerSampleTypeBase;
        }

        private CustomerSampleSubtypeBase GetCustomerSampleSubtype(string species, Customer customer)
        {
            IQuery q = EntityManager.CreateQuery<CustomerSampleSubtypeBase>();
            q.AddEquals(CustomerSampleSubtypePropertyNames.Customer, customer);
            q.AddAnd();
            q.AddEquals(CustomerSampleSubtypePropertyNames.Species, species);
            return EntityManager.Select(q).GetFirst() as CustomerSampleSubtypeBase;
        }

        private CustomerServLevBase GetCustomerServLev(string service, Customer customer)
        {
            IQuery q = EntityManager.CreateQuery<CustomerServLevBase>();
            q.AddEquals(CustomerServLevPropertyNames.Customer, customer);
            q.AddAnd();
            q.AddEquals(CustomerServLevPropertyNames.Service, service);
            return EntityManager.Select(q).GetFirst() as CustomerServLevBase;
        }

        private CustomerSizeUnitBase GetCustomerSizeUnit(Customer customer, string sampleType)
        {
            IQuery q = EntityManager.CreateQuery<CustomerSizeUnitBase>();
            q.AddEquals(CustomerSizeUnitPropertyNames.Customer, customer);
            q.AddAnd();
            q.AddEquals(CustomerSizeUnitPropertyNames.Unit, sampleType);
            return EntityManager.Select(q).GetFirst() as CustomerSizeUnitBase;
        }



        private SampleSubtypeBase GetSampSubtype(string species)
        {
            var q = EntityManager.CreateQuery<SampleSubtypeBase>();
            q.AddEquals(SampleSubtypePropertyNames.SampleSubtypeName, species);

            var entity = EntityManager.Select(q)?.GetFirst() as SampleSubtypeBase;

            return entity;
        }



        /// <summary>
        /// Generates the property bag for the passed entity.
        /// </summary>
        /// <returns></returns>
        private static IWorkflowPropertyBag GeneratePropertyBag(IEntity entity)
        {
            // Generate context for the workflow

            IWorkflowPropertyBag propertyBag = new WorkflowPropertyBag();

            if (entity != null)
            {
                // Pass in the selected parent
                propertyBag.Add(entity.EntityType, entity);
            }

            return propertyBag;
        }

        public static T FromXmlString<T>(string xml) where T : class
        {
            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Runs the workflow for entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="selectedWorkflow">The selected workflow.</param>
        /// <param name="count">The count.</param>
        /// <returns>List of newly created entities.</returns>
        private IList<IEntity> RunWorkflowForEntity(IEntity entity, Workflow selectedWorkflow, int count)
        {
            List<IEntity> newEntities = new List<IEntity>();

            for (int j = 0; j < count; j++)
            {
                // Run the workflow with a property bag for results - used passed in Parameters if available.

                IWorkflowPropertyBag propertyBag;

                if (selectedWorkflow.Properties == null)
                {
                    propertyBag = GeneratePropertyBag(entity);
                }
                else
                {
                    propertyBag = selectedWorkflow.Properties;
                }

                // Counters

                propertyBag.Set("$WORKFLOW_MAX", count);
                propertyBag.Set("$WORKFLOW_COUNT", j + 1);

                // Perform

                PerformWorkflow(selectedWorkflow, propertyBag);

                // Keep track of the newly created entities.

                IList<IEntity> entities = propertyBag.GetEntities(selectedWorkflow.TableName);
                newEntities.AddRange(entities);
            }

            return newEntities;
        }


        /// <summary>
        /// Performs the workflow.
        /// </summary>
        /// <param name="workflow">The workflow.</param>
        /// <param name="propertyBag">The property bag.</param>
        /// <returns></returns>
        protected bool PerformWorkflow(Workflow workflow, IWorkflowPropertyBag propertyBag)
        {
            if (propertyBag == null)
            {
                // Perform the workflow & validate it's output

                propertyBag = workflow.Perform();
            }
            else
            {
                workflow.Perform(propertyBag);
            }

            // Make sure the workflow generated something

            if (propertyBag.Count == 0)
            {
                // Un-supported entity type

                //string message = Library.Message.GetMessage("GeneralMessages", "EmptyWorkflowOutput");
                //Library.Utils.FlashMessage(message, m_Title, MessageButtons.OK, MessageIcon.Error, MessageDefaultButton.Button1);

                return false;
            }

            // Exit if there are errors

            if (propertyBag.Errors.Count > 0)
            {
                //Library.Utils.FlashMessage(propertyBag.Errors[0].Message, m_Title, MessageButtons.OK, MessageIcon.Error, MessageDefaultButton.Button1);
                return false;
            }

            return true;
        }

    }


}
