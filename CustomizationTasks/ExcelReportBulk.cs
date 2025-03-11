/*
 * 
 * Author P.Juhlin
 * Method : ExcelReportBulk
 * Description : Method for creating and excel report for bulk samples, with only the top level.
 * 
 */

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.ObjectModel;
namespace Customization.Tasks
{
    [SampleManagerTask("ExcelReportBulk", "WorkflowCallback")]
    public class ExcelReportBulk : SampleManagerTask
    {

        private string path;

        protected override async void SetupTask()
        {
            base.SetupTask();
            path = GetConfigHeader("ITK_EXCEL_OUTPUT_FOLDER");

            JobHeader job = Context.SelectedItems[0] as JobHeader;
            if (job != null)
            {
                try
                {
                   

                    await Task.Run(() => CreateExcelReport(job));

                }
                catch (Exception ex)
                {
                    StringBuilder Msg = new StringBuilder();
                    Msg.AppendLine($"Task not run for {job}. Exception: {ex.Message}");
                    Library.Utils.FlashMessage(Msg.ToString(), "task not run");
                }
            }
        }
        public void CreateExcelReport(JobHeader job)
        {
            string filePath = path + $"\\{job.Name} Report.xlsx";
           
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                StringBuilder Msg = new StringBuilder();
                Msg.AppendLine($"path not found for {path}.");
                Msg.AppendLine($"creating {path}.");
                Library.Utils.FlashMessage(Msg.ToString(), "path");
            }
           
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(new SheetData());

                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "SampleManager LIMS"
                };
                sheets.Append(sheet);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();


                Row customerInfo= new Row();
                customerInfo.Append(
                        new Cell() { CellReference = "A3", CellValue = new CellValue("Customer No.:"), DataType = CellValues.String },
                        new Cell() { CellReference = "B3", CellValue = new CellValue(job.CustomerId.ToString()), DataType = CellValues.String },
                        new Cell() { CellReference = "A4", CellValue = new CellValue("Customer:"), DataType = CellValues.String },
                        new Cell() { CellReference = "B4", CellValue = new CellValue(), DataType = CellValues.String },
                        new Cell() { CellReference = "A5", CellValue = new CellValue("Report Date:"), DataType = CellValues.String },
                        new Cell() { CellReference = "B5", CellValue = new CellValue(DateTime.Now.ToString("dd MMM yyyyy")), DataType = CellValues.String },
                        new Cell() { CellReference = "A6", CellValue = new CellValue("Order No.:"), DataType = CellValues.String },
                        new Cell() { CellReference = "B6", CellValue = new CellValue(job.JobName), DataType = CellValues.String },
                        new Cell() { CellReference = "A7", CellValue = new CellValue("Customer Project:"), DataType = CellValues.String },
                        new Cell() { CellReference = "B7", CellValue = new CellValue(job.CustomerProject), DataType = CellValues.String }
                );

                sheetData.Append(customerInfo);

                
                Row headerRow = new Row();
                headerRow.Append(
                    new Cell() { CellReference = "A" + 10 ,CellValue = new CellValue("ITK Sample Name"), DataType = CellValues.String },
                    new Cell() { CellReference = "B" + 10 ,CellValue = new CellValue("Sample Name"), DataType = CellValues.String },
                    new Cell() { CellReference = "C" + 10 ,CellValue = new CellValue("Sample Batch"), DataType = CellValues.String },
                    new Cell() { CellReference = "D" + 10 ,CellValue = new CellValue("Variety"), DataType = CellValues.String }
                );



                sheetData.Append(headerRow);

                int rowNumber = 11; 
                foreach (Sample sample in job.RootSamples)
                {
                    if (sample.Status.Equals(PhraseSampStat.PhraseIdA) && sample.EntityTemplateId.Equals("PCR_BULK_SAMPLE"))
                    {


                        Row row = new Row();
                        row.Append(
                            new Cell() { CellReference = "A" + rowNumber, CellValue = new CellValue(sample.IdText), DataType = CellValues.String },
                            new Cell() { CellReference = "B" + rowNumber, CellValue = new CellValue(sample.SampleName), DataType = CellValues.String },
                            new Cell() { CellReference = "C" + rowNumber, CellValue = new CellValue(sample.SampleBatch), DataType = CellValues.String },
                            new Cell() { CellReference = "D" + rowNumber, CellValue = new CellValue(sample.Variety), DataType = CellValues.String }
                        );

                        foreach (Test test in sample.Tests)
                        {
                            if (test.Status.Equals(PhraseTestStat.PhraseIdA))
                            {
                                foreach (Result result in test.Results)
                                {
                                    if (result.RepControl.Contains("E") && result.Status.Equals(PhraseReslStat.PhraseIdA))
                                    {


                                        row.Append(

                                               

                                            );



                                    }

                                }
                            }

                            sheetData.Append(row);

                            rowNumber++; 

                        }
                    }

                    workbookPart.Workbook.Save();
                    document.Close();
                   
                }

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



