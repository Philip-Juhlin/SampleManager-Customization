using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Text;
using System.Threading.Tasks;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;

namespace Customization.Tasks
{
    [SampleManagerTask("GeneralExcelReport", "WorkflowCallback")]
    public class GeneralExcelReport : SampleManagerTask
    {
        private string path;
        protected override async void SetupTask()
        {
            base.SetupTask();
            var path = EntityManager.Select<ConfigHeader>("ITK_EXCEL_OUTPUT_FOLDER").Value;
            JobHeader job = Context.SelectedItems[0] as JobHeader;
            if (job != null)
            {
                try
                {
                    //StringBuilder Msg = new StringBuilder();
                    //Msg.AppendLine($"Creating Excel for {job}.");
                    //Library.Utils.FlashMessage(Msg.ToString(), "task");

                    await Task.Run(() => CreateReport(job));

                }
                catch (Exception ex)
                {
                    StringBuilder Msg = new StringBuilder();
                    Msg.AppendLine($"Task not run for {job}. Exception: {ex.Message}");
                    Library.Utils.FlashMessage(Msg.ToString(), "task not run");
                }
            }
        }
        private void CreateReport(JobHeader job)
        {
            string filePath = path + $"\\{job.Name} Report.xlsx";

            var q = EntityManager.CreateQuery<SampJobTestResult>();
            q.AddEquals(SampJobTestResult.EntityName, job.JobName);
            EntityManager.Select(q);


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
                    Name = "Results"
                };
                sheets.Append(sheet);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();


                // Add general info to specific cells in column A (rows 1-4)
                Row row1 = new Row() { RowIndex = 1 };
                row1.Append(new Cell() { CellReference = "A1", CellValue = new CellValue("Customer Number:"), DataType = CellValues.String });
                row1.Append(new Cell() { CellReference = "B1", CellValue = new CellValue(job.CustomerId.ToString()), DataType = CellValues.String });
                sheetData.Append(row1);

                Row row2 = new Row() { RowIndex = 2 };
                row2.Append(new Cell() { CellReference = "A2", CellValue = new CellValue("Customer:"), DataType = CellValues.String });
                row2.Append(new Cell() { CellReference = "B2", CellValue = new CellValue(job.CustomerId.Name), DataType = CellValues.String });
                sheetData.Append(row2);

                Row row3 = new Row() { RowIndex = 3 };
                row3.Append(new Cell() { CellReference = "A3", CellValue = new CellValue("Report Date:"), DataType = CellValues.String });
                row3.Append(new Cell() { CellReference = "B3", CellValue = new CellValue(DateTime.Now.ToString()), DataType = CellValues.String });
                sheetData.Append(row3);

                Row row4 = new Row() { RowIndex = 4 };
                row4.Append(new Cell() { CellReference = "A4", CellValue = new CellValue("Order Number:"), DataType = CellValues.String });
                row4.Append(new Cell() { CellReference = "B4", CellValue = new CellValue(job.JobName), DataType = CellValues.String });
                sheetData.Append(row4);

                Row row5 = new Row() { RowIndex = 5 };
                row5.Append(new Cell() { CellReference = "A5", CellValue = new CellValue("Customer Project:"), DataType = CellValues.String });
                row5.Append(new Cell() { CellReference = "B5", CellValue = new CellValue(job.CustomerProject), DataType = CellValues.String });
                sheetData.Append(row5);



                //first check the top level entity_template_id 

                foreach (Sample sample in job.RootSamples)
                {
                    if (sample != null)
                    {
                        if (sample.EntityTemplateId.Equals(EntityManager.SelectByName(EntityTemplateBase.EntityName, "PCR_SNP_GENOTYPING") as EntityTemplate))
                        {
                            int countPrep;
                            foreach (Test test in sample.Tests)
                            {
                                if (test.Analysis.ItkArticle != null)
                                {

                                }
                            }

                        }
                    }


                }
                workbookPart.Workbook.Save();
                document.Close();
            }
        }
    }
}
