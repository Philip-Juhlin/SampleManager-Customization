using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.ObjectModel;
using System.Xml.Serialization;
using System.Xml;
using Thermo.SampleManager.Common.Data;


namespace Customization.Tasks
{

    [SampleManagerTask("ITKCustomerExcelTask", "WorkflowCallback")]
    public class ITKCustomerExcelTask : SampleManagerTask
    {
        private string path;
        protected override void SetupTask()
        {
            base.SetupTask();
            path = GetConfigHeader("ITK_XML_OUTPUT_FOLDER");
            if (Context.SelectedItems.Count > 0 && Context.SelectedItems[0] is JobHeader job)
            {/*
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Data");
                    // Add the header to the data sheet
                    worksheet.Cell(1, 1).Value = "Provstatus";
                    worksheet.Cell(1, 2).Value = "Datum (Skickat)";
                    worksheet.Cell(1, 3).Value = "Provid";
                    worksheet.Cell(1, 4).Value = "Inv.plats";
                    worksheet.Cell(1, 5).Value = "Län";
                    worksheet.Cell(1, 6).Value = "Inventerare";
                    worksheet.Cell(1, 7).Value = "Datum for inventering";
                    worksheet.Cell(1, 8).Value = "Skadegörare";
                    worksheet.Cell(1, 9).Value = "Växtart";
                    worksheet.Cell(1, 10).Value = "Prov.mtrl.";
                    worksheet.Cell(1, 11).Value = "Prioriterad";
                    worksheet.Cell(1, 12).Value = "Provsvar";
                    worksheet.Cell(1, 13).Value = "Provsvar-Kommentar";

                    int row = 2;

                    foreach (Sample sample in job.Samples)
                    {
                        string resultText;
                        bool sampleRes = sample.DetectedResult;
                        if (sampleRes)
                        {
                            resultText = "Positiv";
                        }
                        else
                        {
                            resultText = "Negative";
                        }

                        worksheet.Cell(row, 1).Value = "Skickad";
                        worksheet.Cell(row, 2).Value = DateTime.Now.ToString("yyyy-dd-M_HH-mm");
                        worksheet.Cell(row, 3).Value = sample.SampleName;
                        worksheet.Cell(row, 12).Value = resultText;

                        row++;
                    }


                    // Save the workbook
                    workbook.SaveAs(path + $"\\{job.JobName + DateTime.Now.ToString("_yyyy-dd-M_HH-mm-ss")}.xlsx");
                }

                */


            }
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
