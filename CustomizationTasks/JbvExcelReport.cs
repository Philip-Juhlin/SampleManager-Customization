/*
 * 
 * Author P.Juhlin
 * Method : JbvExcelReport
 * Description : Method for creating and excel report for JBV.
 * 
 */

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Xml.Serialization;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.ObjectModel;
using Thermo.SampleManager.Tasks;
namespace Customization.Tasks
{
    [SampleManagerTask("JbvExcelReport", "WorkflowCallback")]
    public class JbvExcelReport : SampleManagerTask
    {
      
        private string path;

        protected override async void SetupTask()
        {
            base.SetupTask();
            path = GetConfigHeader("ITK_EXCEL_OUTPUT_FOLDER");

            JobHeader job = Context.SelectedItems[0] as JobHeader;
            if(job != null)
            {
                try
                {
                    //StringBuilder Msg = new StringBuilder();
                    //Msg.AppendLine($"Creating Excel for {job}.");
                    //Library.Utils.FlashMessage(Msg.ToString(), "task");

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
            string filePath = path + $"\\{job.Name + DateTime.Now.ToString("_yyyy-dd-M_HH-mm-ss")}.xlsx";

            // Ensure the directory exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                StringBuilder Msg = new StringBuilder();
                Msg.AppendLine($"path not found for {path}.");
                Msg.AppendLine($"creating {path}.");
                Library.Utils.FlashMessage(Msg.ToString(), "path");
            }
            // Create a new workbook
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
                    Name = "Data"
                };
                sheets.Append(sheet);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Add the header to the data sheet
                Row headerRow = new Row();
                headerRow.Append(
                    new Cell() { CellValue = new CellValue("Provstatus"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Datum (Skickat)"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Provid"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Inv.plats"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Län"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Inventerare"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Datum for inventering"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Skadegörare"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Växtart"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Prov.mtrl."), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Prioriterad"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Checklista kommentar"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Provsvar"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Provsvar-Kommentar"), DataType = CellValues.String }
                );
                sheetData.Append(headerRow);

                int rowNumber = 2; // Start from the second row since the first row is the header
                foreach (Sample sample in job.RootSamples)
                {
                    if (sample.Status.Equals(PhraseSampStat.PhraseIdA))
                    {
                        
                        string resultText = sample.DetectedResult ? "1" : "2";
                        string jobpriority;
                        if (job.ServiceLevel.Equals(PhraseServLev.PhraseIdEXPRESS)) {
                            jobpriority = "JA";
                        }
                        else {
                            jobpriority = "NEJ";
                        }

                            Row row = new Row();
                        row.Append(
                            new Cell() { CellReference = "A" + rowNumber, CellValue = new CellValue("Analyserat"), DataType = CellValues.String },
                            new Cell() { CellReference = "B" + rowNumber, CellValue = new CellValue(DateTime.Now.ToString()), DataType = CellValues.String },
                            new Cell() { CellReference = "C" + rowNumber, CellValue = new CellValue(sample.SampleName), DataType = CellValues.String },
                            new Cell() { CellReference = "K" + rowNumber, CellValue = new CellValue(jobpriority), DataType = CellValues.String },
                            new Cell() { CellReference = "M" + rowNumber, CellValue = new CellValue(resultText), DataType = CellValues.String }
                        );
                        sheetData.Append(row);

                        rowNumber++; // Move to the next row
                    }
                }

                workbookPart.Workbook.Save();
                document.Close();
                //StringBuilder Msg = new StringBuilder();
                //Msg.AppendLine($"document saved succesfully to {filePath}.");
                //Library.Utils.FlashMessage(Msg.ToString(), "saved");
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


