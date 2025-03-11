/*
 * 
 * Author P.Juhlin
 * Method : JbvXmlResultTask
 * Description : Method for creating and xml report for JBV.
 * 
 */

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;



namespace Customization.Tasks
{
    /* Created: 2024-09-18
     * Author : P.Juhlin
     * Site   : Alnarp, Sweden
     * Details: code produced to create sample xml report for customer Jordbruksverket, not applicable to other customers as the setup
     *          has hardcoded values and only works for job with single sample. Code is specific to customer project
     */


    [SampleManagerTask("JbvXmlResultTask", "WorkflowCallback")]
    public class JbvXmlResultTask : SampleManagerTask
    {
        private string path;

        protected override void SetupTask()
        {
            base.SetupTask();

            path = GetConfigHeader("ITK_XML_JBV_OUTPUT_FOLDER");
            if (Context.SelectedItems.Count > 0 && Context.SelectedItems[0] is Sample sample)
            {
                // requirement for boolean field sample.ItkXmlReport can be added to statement if needed or necessary
                if (sample.Status.Equals(PhraseSampStat.PhraseIdA))
                {

                    XmlDocument doc = new XmlDocument();
                    XmlComment comment = doc.CreateComment("Comments");
                    comment.InnerText = "Sample result xml from Intertek";
                    doc.AppendChild(comment);
                    XmlElement root = doc.CreateElement("Sample");
                    doc.AppendChild(root);

                    // Create Sampleproperties-elementet
                    XmlElement sampleProperties = doc.CreateElement("Sampleproperties");
                    root.AppendChild(sampleProperties);
                    // ... sub-element under Sampleproperties ...
                    XmlElement customerSampleID = doc.CreateElement("SampleIDSJV");
                    customerSampleID.InnerText = sample.SampleName;
                    sampleProperties.AppendChild(customerSampleID);

                    XmlElement sampleID = doc.CreateElement("LabratorySampleID");
                    sampleID.InnerText = sample.IdText;
                    sampleProperties.AppendChild(sampleID);

                    XmlElement labratory = doc.CreateElement("Labratory");
                    labratory.InnerText = "Intertek";
                    sampleProperties.AppendChild(labratory);

                    XmlElement samplingdate = doc.CreateElement("SamplingsDate");
                    sampleProperties.AppendChild(samplingdate);

                    XmlElement sampletakeby = doc.CreateElement("SampleTakenBy");
                    sampleProperties.AppendChild(sampletakeby);

                    // Create Result-element
                    XmlElement result = doc.CreateElement("Result");
                    root.AppendChild(result);
                    // ... sub-element under Result ...

                    foreach (TestBase test in sample.Tests)
                    {
                        if (test.Status.Equals(PhraseTestStat.PhraseIdA))
                        {
                            foreach (ResultBase lresult in test.Results)
                            {
                                if (lresult.RepControl.Contains("M") && lresult.Status.Equals(PhraseReslStat.PhraseIdA))
                                {
                                    XmlElement sampleresult = doc.CreateElement("SampleResult");
                                    result.AppendChild(sampleresult);

                                    XmlElement receivedate = doc.CreateElement("ReceiveDate");
                                    receivedate.InnerText = sample.RecdDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                                    sampleresult.AppendChild(receivedate);

                                    XmlElement resultdate = doc.CreateElement("ResultDate");
                                    resultdate.InnerText = sample.DateAuthorised.Value.ToString("yyyy-MM-dd HH:mm:ss");
                                    sampleresult.AppendChild(resultdate);

                                    // result values Positivt = Detected, Negativt = Not Detected, Makulerat(not sure how to use this, not in current system)
                                    XmlElement analysis = doc.CreateElement("Analysis");
                                    analysis.InnerText = test.Analysis.Description;
                                    sampleresult.AppendChild(analysis);

                                    // result values Positivt = Detected, Negativt = Not Detected, Makulerat(not sure how to use this, not in current system)
                                    XmlElement resultValue = doc.CreateElement("ResultValue");
                                    resultValue.InnerText = lresult.Text;
                                    sampleresult.AppendChild(resultValue);

                                    XmlElement resultComment = doc.CreateElement("SampleComment");
                                    resultComment.InnerText = lresult.Comments;
                                    sampleresult.AppendChild(resultComment);
                                }
                            }
                        }
                    }

                    // Create Requestor-element
                    XmlElement requestor = doc.CreateElement("Requestor");
                    root.AppendChild(requestor);
                    // ...  sub-element under Requestor ...

                    XmlElement requestorname = doc.CreateElement("Requestorname");
                    requestorname.InnerText = "Jordbruksverket";
                    requestor.AppendChild(requestorname);

                    XmlElement adress1 = doc.CreateElement("Adress1");
                    adress1.InnerText = "Skeppsbrogatan 2";
                    requestor.AppendChild(adress1);

                    XmlElement adress2 = doc.CreateElement("Adress2");
                    adress2.InnerText = "Växtavdelningen";
                    requestor.AppendChild(adress2);

                    XmlElement postcode = doc.CreateElement("Postcode");
                    postcode.InnerText = "55329";
                    requestor.AppendChild(postcode);

                    XmlElement phonenumber = doc.CreateElement("Phonenumber");
                    requestor.AppendChild(phonenumber);

                    XmlElement reportemail = doc.CreateElement("ReportEmail");
                    reportemail.InnerText = sample.JobName.ReportEmail;
                    requestor.AppendChild(reportemail);

                    XmlElement countrycode = doc.CreateElement("Countrycode");
                    countrycode.InnerText = "SE";
                    requestor.AppendChild(countrycode);


                    Save(doc, path + $"\\INTERTEK_105488_{sample.SampleName + "_" + sample.IdText + DateTime.Now.ToString("_yyyy-MM-dd")}.xml");


                }

            }

            Exit();
        }

        private string GetConfigHeader(string entityId)
        {
            return EntityManager.Select<ConfigHeader>(entityId).Value;
        }

        public void Save<T>(T xml, String path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StreamWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, xml);
            }
        }

    }
}


