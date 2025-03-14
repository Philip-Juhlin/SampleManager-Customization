

/*
 * 
 * Author P.Juhlin
 * Method : OdooAPI
 * Description : Method for processing JSON files sent from ODOO: SalesModule, salesorder.
 * External Url to DEV method : http://ESWEALNDEV001:56105/api/input/salesorder
 * External Url to PROD method : http://ESWEALNAPP001:56105/api/input/salesorder
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Common.Workflow;
using Thermo.SampleManager.Internal.ObjectModel;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;
using Thermo.SampleManager.Server.Workflow;
using Thermo.SampleManager.WebApiTasks.Mobile;
using Thermo.SampleManager.WebApiTasks.Mobile.Data;

namespace Customization.Tasks

{

    /// <summary>

    /// Provides REST Endpoint for logging in stuff via Odoo

    /// </summary>
    //[DataContract(Name = "OdooAPI")]
    [SampleManagerWebApi("OdooAPI")]
    [MobileFeature(FunctionAuxiliary.FeatureName)]
    public class OdooAPI : SampleManagerWebApiTask

    {
        #region Processsaleorder

        /// <summary>Input json.</summary>
        ///
        /// <remarks></remarks>
        /// <url>http://localhost/api/input/salesorder</url>
        /// <verb>POST</verb>
        /// <param name="json">   The values. </param>
        ///
        /// <returns>   The status. </returns>

        //[WebInvoke(UriTemplate = "api/input/salesorder", Method = "POST")]
        [WebInvoke(UriTemplate = "api/input/salesorder", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Creates a salesorder in SM from ODOO")]


        // public get json input
        public void ProcessSaleOrder(SaleOrderJson json)
        {
            try
            {
                string jobname = json.SaleOrderName;


                //check if job exists.

                if (EntityManager.SelectByName(JobHeader.EntityName, jobname) is JobHeader)
                {
                    SetHttpStatus(HttpStatusCode.BadRequest, $"job: {jobname} already exist in LIMS");
                    return;
                }



                //Check for a workflow..
                Workflow wf = EntityManager.SelectByName(Workflow.EntityName, GetConfigHeader("ITK_STANDARD_JOB")) as Workflow;
                if (!(EntityManager.SelectLatestVersion(Workflow.EntityName, wf?.WorkflowGuid) is Workflow jobWorkFlow))
                {
                    SetHttpStatus(HttpStatusCode.BadRequest, $"workflow not found {wf}");
                    return;
                }

                //run the workflow for creating the job 1 time
                IList<IEntity> jobs = RunWorkflowForEntity(null, jobWorkFlow, 1);
                JobHeader job;

                // if jobs.count is not equal to 1 then the workflow did not create a job or produced the wrong number... 
                if (jobs.Count != 1)
                {
                    // send error and return.
                    SetHttpStatus(HttpStatusCode.BadRequest, $"workflow {jobWorkFlow} did'nt produce any job");
                    return;
                }


                //Set the data on the job based on the saleorder information
                job = jobs[0] as JobHeader;
                job.JobName = jobname;
                job.CustomerId = GetCustomer(json.customerName);
                job.BrowseDescription = json.BrowseDescription;
                job.JobComments = json.JobComments;
                job.ConfirmationEmail = json.ConfirmationEmail;
                job.ReportEmail = json.ReportEmail;
                var customer = EntityManager.Select<Customer>(job.CustomerId.Identity);
                job.ContactPersonConfirmation = GetCustomerAdress(json.ContactPersConfirm, customer);
                job.ContactPersonReport = GetCustomerAdress(json.ContactPersReport, customer);
                job.ContactPersonInvoice = GetCustomerAdress(json.ContactPersInvoic, customer);
                job.JobType = EntityManager.SelectByName(PhraseBase.EntityName, json.JobTypePhraseID) as PhraseBase;
                job.PoNumber = json.PoNumber;
                job.ServiceLevel = EntityManager.SelectByName(PhraseBase.EntityName, json.serviceLevel) as PhraseBase;
                job.ServiceLevelDays = EntityManager.SelectByName(PhraseBase.EntityName, json.serviceLevelDays) as PhraseBase;
                job.SetItkWorkProgress("J01");

                /*
                 * need to choose the correct customer in lims based on data from odoo, the identity in SM does not exist for all of the customers, 
                 * if we could make it so that odoo create the customer then the ODOO identity could be added to all the customer and the that could be the key.
                 * 
                 */

                // some sloppy method of desciding the workflow for the samples based on the jobtype..
                var workflow = GetWorkflow(json.jobType);


                //Create the samples in Lims on the created job
                CreateSamples(json, workflow, job);

                //Split the samples to subsamples 
                SplitSamples(job);


                // now check for all the tests that do not have a component list
                CleanupTask(job);

                SetHttpStatus(HttpStatusCode.OK, $"Successfully logged in {job} with {job.Samples.Count} samples");
                EntityManager.Commit();
            }
            catch (Exception ex)
            {
                SetHttpStatus(HttpStatusCode.BadRequest, ex.Message);
            }




        }
        #endregion
        

        #region privatemethodssaleorder


        private Workflow GetWorkflow(string jobtype)
        {
            //The below dictonary uses the Job type to choose the config header item for the sample workflow
            Dictionary<string, string> sampleWorkFlowDictonary = new Dictionary<string, string>
                {
                    {"Bulk","ITK_BULK_SAMP_WF" },
                    {"Bulk Potato","ITK_BULK_SAMP_WF" },
                    {"Single seed","ITK_SINGLE_SAMP_WF" },
                    {"DArT","ITK_BULK_SAMP_WF" },
                    {"SNP genotyping","ITK_SNP_SAMP_WF" },
                    {"SNP verification","ITK_SNP_SAMP_WF" },
                    {"ADS: KASP Design","ITK_ADS_SAMP_WF" },
                    {"ADS: Reagent Shipment","ITK_ADS_SAMP_WF" },
                    {"ADS: Material Shipment","ITK_ADS_SAMP_WF" },
                    {"ADS: Consulting Services","ITK_ADS_SAMP_WF" },
                    {"ADS: DNA Extraction","ITK_ADS_SAMP_WF" }
                };

            //if the job type is not in the dictonary then send error and exit code
            if (!sampleWorkFlowDictonary.ContainsKey(jobtype))
            {
                SetHttpStatus(HttpStatusCode.BadRequest, $"Unable to find workflow key for sample with job type {jobtype}");
                return null;
            }
            //yse tge key to get the config header name
            string wfName = sampleWorkFlowDictonary[jobtype];

            Workflow sampwf = EntityManager.SelectByName(Workflow.EntityName, GetConfigHeader(wfName)) as Workflow;
            // check if the workflow is found
            if (!(EntityManager.SelectLatestVersion(Workflow.EntityName, sampwf?.WorkflowGuid) is Workflow sampleWorkflow))
            {
                SetHttpStatus(HttpStatusCode.BadRequest, $"workflow not found {jobtype}");
                return null;
            }
            else
            {
                return sampleWorkflow;
            }
        }


        private Task CreateSamples(SaleOrderJson saleorderjson, Workflow sampleWorkflow, JobHeader job)
        {
            // currently running this with a shitty dict to determine the workflow as soon as i have an update i should do 
            
          

            int sampleCount = saleorderjson.SaleOrderSamples.Length;
            // run the workflow n times based on the number of samples in the json
            //IList<IEntity> samplesList = RunWorkflowForEntity(job, sampleWorkflow, sampleCount);

            IList<IEntity> samplesList = new List<IEntity>();

            for(int i = 0; i < sampleCount; i++)
            {
                var iSample = saleorderjson.SaleOrderSamples[i];

                if (!string.IsNullOrEmpty(iSample.SampleDescription))
                {
                    Workflow sampwf = EntityManager.SelectByName(Workflow.EntityName, GetConfigHeader(iSample.SampleDescription)) as Workflow;
                    if (!(EntityManager.SelectLatestVersion(Workflow.EntityName, sampwf?.WorkflowGuid) is Workflow workflow))
                    {
                        SetHttpStatus(HttpStatusCode.BadRequest, $"workflow not found {iSample.SampleDescription} on sample {i}");
                        return Task.CompletedTask;
                    }
                    else
                    {
                        
                        IEntity sample = RunWorkflowForEntity(job, sampwf, 1).FirstOrDefault();
                        samplesList.Add(sample);
                    }

                }
                else
                {
                    SetHttpStatus(HttpStatusCode.BadRequest, $"sample {i} does not contain a workflow");
                    return Task.CompletedTask;
                }


            }
            //if the workflow count is not equal to the number of samples then send error
            if (samplesList.Count != sampleCount)
            {
                SetHttpStatus(HttpStatusCode.BadRequest, $"workflow {sampleWorkflow} produces wrong number of samples");
                return Task.CompletedTask;
            }


            var saleOrderSamples = saleorderjson.SaleOrderSamples;


            try
            {


                foreach (var i in samplesList.Zip(saleOrderSamples, (n, w) => new { Sample = n as Sample, InputSample = w }))
                {
                    var sample = i.Sample;
                    var inputSample = i.InputSample;
                    sample.JobName = job;
                    sample.SetSplitSampMethod(inputSample.SplitSampMethod);
                    sample.SampleName = inputSample.SampleName;
                    sample.SampleBatch = inputSample.SampleBatch;
                    sample.ExternalReference = inputSample.ExternalReference;
                    sample.Variety = inputSample.Variety;
                    sample.Lod = inputSample.Lod;
                    sample.SampSubtype = EntityManager.Select<SampleSubtypeBase>(inputSample.SampSubtype); 
                    sample.SampType = EntityManager.Select<SampleTypeBase>(inputSample.SampleType);
                    sample.SampleSize = inputSample.SampleSize;
                    sample.SampleSizeUnit = EntityManager.SelectByName(PhraseBase.EntityName, inputSample.SampleSizeUnit) as PhraseBase;
                    sample.SampleDensity = inputSample.SampleDensity;
                    sample.SampleDensityUnit = EntityManager.SelectByName(PhraseBase.EntityName, inputSample.SampleDensityUnit) as PhraseBase;
                    sample.TestSchedule = EntityManager.SelectByName(TestSchedHeaderBase.EntityName, inputSample.TestSchedule) as TestSchedHeaderBase;
                    sample.Remark = EntityManager.SelectByName(PhraseBase.EntityName, inputSample.SamplesRemarkPhraseID) as PhraseBase;
                    sample.PooledSample = inputSample.SamplesPooledSamples;
                    sample.ItkXmlReport = inputSample.SamplesCustomerITKXMLReport;
                    sample.CustomerPlateName = inputSample.SamplesPlateName;
                    sample.CustomerPlateWell = inputSample.SamplesPlateWell;
                    sample.SamplingRequired = inputSample.SamplingReq;

                    //if the sample contain a testpackages then add all the tests from that.

                    if (inputSample.TestSchedule != null)
                    {
                        var qtest = EntityManager.CreateQuery<TestSchedHeaderBase>();
                        qtest.AddEquals(TestSchedHeaderPropertyNames.Identity, inputSample.TestSchedule);

                        if (EntityManager.Select(qtest)?.GetFirst() is TestSchedHeaderBase testSchedHeaderBase)
                        {
                            sample.AddTests(testSchedHeaderBase);
                        }

                    }

                    var analysisList = inputSample.TestList;
                    if (analysisList != null)
                    {

                        foreach (var analysis in analysisList)
                        {
                            {
                                var q = EntityManager.CreateQuery<VersionedAnalysis>();
                                q.AddEquals(VersionedAnalysisPropertyNames.Identity, analysis.AnalysisName);

                                if (EntityManager.Select(q)?.GetFirst() is VersionedAnalysis versionedAnalysis)
                                {
                                    List<TestInternal> tests = i.Sample.AddTest(versionedAnalysis);

                                    if (analysis.TestSchedule != null)
                                    {
                                        for (int j = 0; j < tests.Count; j++)
                                        {
                                            var qtest = EntityManager.CreateQuery<TestSchedHeaderBase>();
                                            qtest.AddEquals(TestSchedHeaderPropertyNames.Identity, analysis.TestSchedule);

                                            if (EntityManager.Select(qtest)?.GetFirst() is TestSchedHeaderBase testSchedHeaderBase)
                                            {
                                                var test = tests[j];
                                                test.TestSchedule = testSchedHeaderBase;
                                            }
                                            else
                                            {
                                                SetHttpStatus(HttpStatusCode.BadRequest, $"testschedule for analysis{analysis} is not correct on sample; {sample.IdText}");
                                            }

                                        }
                                    }

                                }

                                else
                                {

                                    SetHttpStatus(HttpStatusCode.BadRequest, $"missing analysis {analysis.AnalysisName}");
                                    return Task.CompletedTask;
                                }
                            }
                        }
                    }

                }

                EntityManager.Commit();
                return Task.CompletedTask;

            }


            catch (Exception ex)
            {
                SetHttpStatus(HttpStatusCode.BadRequest, $"Error processing samples: {ex}");
                return Task.CompletedTask;
            }
        }


        private Task SplitSamples(JobHeader job)
        {

            // Create a copy of the samples in job to avoid modification errors during iteration
            var samplesCopy = new List<Sample>();
            foreach (Sample sample in job.Samples)
            {
                samplesCopy.Add(sample);
            }

            try
            {
                foreach (Sample sample in samplesCopy)
                {
                    sample.PerformAction("SPLIT_TO_AB");
                    // Create a list of the subsamples
                    var childSampleList = new List<Sample>();
                    foreach (Sample subsample in sample.ChildSamples)
                    {
                        childSampleList.Add(subsample);
                    }

                    foreach (Sample subsample in childSampleList)
                    {
                        foreach (TestBase analysis in sample.Tests)
                        {
                            VersionedAnalysisBase testName = analysis.Analysis;
                            var q = EntityManager.CreateQuery<VersionedAnalysis>();
                            q.AddEquals(VersionedAnalysisPropertyNames.Identity, testName);

                            if (EntityManager.Select(q)?.GetFirst() is VersionedAnalysis versionedAnalysis)
                            {
                                List<TestInternal> tests = subsample.AddTest(versionedAnalysis);
                            }
                        }

                        //if (Regex.IsMatch(sample.SplitSampMethod.ToString(), @".*_A_B"))
                        //{
                        //    subsample.PerformAction("SPLIT_TO_AB");

                        var childSampleChildList = new List<Sample>();
                        foreach (Sample childsample in subsample.ChildSamples)
                        {
                            childSampleChildList.Add(childsample);
                        }
                        if (childSampleChildList.Count > 0)
                        {
                            foreach (Sample childSampleChild in childSampleChildList)
                            {
                                foreach (TestBase analysis in subsample.Tests)
                                {
                                    VersionedAnalysisBase testName = analysis.Analysis;
                                    var q = EntityManager.CreateQuery<VersionedAnalysis>();
                                    q.AddEquals(VersionedAnalysisPropertyNames.Identity, testName);

                                    if (EntityManager.Select(q)?.GetFirst() is VersionedAnalysis versionedAnalysis)
                                    {
                                        List<TestInternal> tests = childSampleChild.AddTest(versionedAnalysis);
                                    }
                                }
                            }
                        }

                    }
                }


                EntityManager.Commit();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                SetHttpStatus(HttpStatusCode.BadRequest, $"Error splitting samples: {ex}");
                return Task.CompletedTask;
            }
        }

        private Task CleanupTask(JobHeader job)
        {

            foreach (Sample sample in job.Samples)
            {
                IList<Test> remove = new List<Test>();

                foreach (Test test in sample.Tests)
                {

                    if (test.HasResultList) continue;

                    remove.Add(test);
                }

                foreach (Test test in remove)
                {
                    sample.Tests.Remove(test);

                    test.TriggerDeleted();

                    EntityManager.Transaction.Remove(test);
                }
            }
            EntityManager.Commit();
            return Task.CompletedTask;
        }


        //private void CollectRequiredKeys(SaleOrderSample[] saleOrderSamples, out HashSet<string> analysisNames, out HashSet<string> testSchedules)
        //{
        //    analysisNames = new HashSet<string>();
        //    testSchedules = new HashSet<string>();

        //    foreach (var sample in saleOrderSamples)
        //    {
        //        foreach (var test in sample.Tests)
        //        {
        //            analysisNames.Add(test.AnalysisName);
        //            if (test.TestSchedule != null)
        //            {
        //                testSchedules.Add(test.TestSchedule);
        //            }
        //        }
        //    }
        //}

        private List<AnalysisWithSchedule> ExecuteBatchQueries(HashSet<string> analysisNames, HashSet<string> testSchedules)
        {
            var versionedAnalysesQuery = EntityManager.CreateQuery<VersionedAnalysis>();
            foreach (var analysisName in analysisNames)
            {
                versionedAnalysesQuery.AddEquals(VersionedAnalysisPropertyNames.Identity, analysisName);
            }
            var versionedAnalyses = new List<VersionedAnalysis>();
            foreach (var result in EntityManager.Select(versionedAnalysesQuery))
            {
                if (result is VersionedAnalysis versionedAnalysis)
                {
                    versionedAnalyses.Add(versionedAnalysis);
                }
            }

            var testSchedHeadersQuery = EntityManager.CreateQuery<TestSchedHeaderBase>();
            foreach (var testSchedule in testSchedules)
            {
                testSchedHeadersQuery.AddEquals(TestSchedHeaderPropertyNames.Identity, testSchedule);
            }
            var testSchedHeaders = new List<TestSchedHeaderBase>();
            foreach (var result in EntityManager.Select(testSchedHeadersQuery))
            {
                if (result is TestSchedHeaderBase testSchedHeaderBase)
                {
                    testSchedHeaders.Add(testSchedHeaderBase);
                }
            }

            // Create lookup dictionaries
            var versionedAnalysisDict = versionedAnalyses.ToDictionary(va => va.Identity);
            var testSchedHeaderDict = testSchedHeaders.ToDictionary(ts => ts.Identity);

            // Combine data into a list of AnalysisWithSchedule
            var combinedList = new List<AnalysisWithSchedule>();
            foreach (var analysisName in analysisNames)
            {
                var combinedItem = new AnalysisWithSchedule
                {
                    VersionedAnalysis = versionedAnalysisDict.TryGetValue(analysisName, out var versionedAnalysis) ? versionedAnalysis : null,
                    TestSchedule = testSchedHeaderDict.TryGetValue(analysisName, out var testSchedHeaderBase) ? testSchedHeaderBase : null
                };

                // Only add non-null combined items
                if (combinedItem.VersionedAnalysis != null)
                {
                    combinedList.Add(combinedItem);
                }
            }

            return combinedList;
        }




        #endregion



        #region privatemethods
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

        private CustomerAddressBase GetCustomerAdress(string contact, Customer customer)
        {
            IQuery q = EntityManager.CreateQuery<CustomerAddressBase>();
            q.AddEquals(CustomerAddressPropertyNames.CustomerAddressName, contact);
            q.AddAnd();
            q.AddEquals(CustomerAddressPropertyNames.Customer, customer.Identity);
            return EntityManager.Select(q).GetFirst() as CustomerAddressBase;
        }

        private CustomerBase GetCustomer(string customerName)
        {
            IQuery q = EntityManager.CreateQuery<CustomerBase>();
            q.AddEquals(CustomerPropertyNames.CompanyName, customerName);
            q.AddAnd();
            q.AddNotEquals(CustomerPropertyNames.Removeflag, true);
            var customer = EntityManager.Select(q).GetFirst() as CustomerBase;
            if (customer == null)
            {
                SetHttpStatus(HttpStatusCode.BadRequest, $"{customerName} not found as customer in lims");
                return null;
            }
            return customer;
        }

        private CustomerSizeUnitBase GetCustomerSizeUnit(Customer customer, string sampleType)
        {
            IQuery q = EntityManager.CreateQuery<CustomerSizeUnitBase>();
            q.AddEquals(CustomerSizeUnitPropertyNames.Customer, customer);
            q.AddAnd();
            q.AddEquals(CustomerSizeUnitPropertyNames.Unit, sampleType);
            return EntityManager.Select(q).GetFirst() as CustomerSizeUnitBase;
        }

        //private PhraseBase GetSampleSizeUnit(string textId)
        //{
        //    var q = EntityManager.CreateQuery<PhraseBase>();
        //    q.AddEquals(PhrasePropertyNames.PhraseType, PhraseSizeDenU.Identity);
        //    q.AddAnd();
        //    q.AddEquals(PhrasePropertyNames.PhraseText, textId);

        //    return EntityManager.Select(q).GetFirst() as PhraseBase;
        //}

        //private PhraseBase GetServiceLevelDays(string service, Customer customer, bool singleSeed)
        //{
        //    var q = EntityManager.CreateQuery<CustomerServLevBase>();
        //    q.AddEquals(CustomerServLevPropertyNames.Customer, customer);
        //    q.AddAnd();
        //    q.AddEquals(CustomerServLevPropertyNames.Service, service);

        //    var entity = EntityManager.Select(q)?.GetFirst() as CustomerServLevBase;
        //    // Fråga Jolanta hur hantera sakande värde här?
        //    return entity == null ? null : singleSeed ? entity.SsServiceDays : entity.BulkServiceDays;
        //}

        private string GetConfigHeader(string entityId)
        {
            return EntityManager.Select<ConfigHeader>(entityId).Value;
        }

        private SampleSubtypeBase GetSampSubtype(string species)
        {
            var q = EntityManager.CreateQuery<SampleSubtypeBase>();
            q.AddEquals(SampleSubtypePropertyNames.SampleSubtypeName, species);

            var entity = EntityManager.Select(q)?.GetFirst() as SampleSubtypeBase;

            return entity;
        }

        //private void SetServiceLevelPhraseOnJob(JobHeader job, string service, bool singleSeed)
        //{
        //    var q = EntityManager.CreateQuery<CustomerServLevBase>();
        //    q.AddEquals(CustomerServLevPropertyNames.Service, service);

        //    var entity = EntityManager.Select(q)?.GetFirst() as CustomerServLevBase;
        //    job.ServiceLevel = singleSeed  ? entity.SsServiceLevel : entity.BulkServiceLevel;
        //}

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
        #endregion
    }
}




#region publicmethods
public class AnalysisWithSchedule
{
    public VersionedAnalysis VersionedAnalysis { get; set; }
    public TestSchedHeaderBase TestSchedule { get; set; }
}

public class JobAlreadyExistsException : Exception
{
    public JobAlreadyExistsException(string message) : base(message) { }
}

public class WorkflowNotFoundException : Exception
{
    public WorkflowNotFoundException(string message) : base(message) { }
}

public class WorkflowExecutionException : Exception
{
    public WorkflowExecutionException(string message) : base(message) { }
}


#endregion

