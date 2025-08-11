# SampleManager Customization

This repository contains customizations and extensions for Thermo Fisher SampleManager LIMS, including object models, task automation, and reporting tools.

## Project Structure

- **CustomizationObjectModel/**  
  Contains C# classes extending the SampleManager object model, such as [`ExtendedHazard.cs`](CustomizationObjectModel/ExtendedHazard.cs).

- **CustomizationTasks/**  
  Contains C# tasks and utilities for automating SampleManager processes, generating reports, and integrating with external systems. Notable files include:
  - [`CreatePecSamples.cs`](CustomizationTasks/CreatePecSamples.cs): Task for creating PEC samples.
  - [`ExcelReportBulk.cs`](CustomizationTasks/ExcelReportBulk.cs): Bulk Excel report generation.
  - [`FindDuplicateLoginsTask.cs`](CustomizationTasks/FindDuplicateLoginsTask.cs): Utility for finding duplicate logins.
  - [`JbvExcelReport.cs`](CustomizationTasks/JbvExcelReport.cs), [`JbvXmlResultTask.cs`](CustomizationTasks/JbvXmlResultTask.cs): JBV-specific reporting tasks.
  - [`OdooAPI.cs`](CustomizationTasks/OdooAPI.cs): Integration with Odoo API.
  - [`PcrPlateFormTask.cs`](CustomizationTasks/PcrPlateFormTask.cs): PCR plate form automation.
  - [`SaleOrderJson.cs`](CustomizationTasks/SaleOrderJson.cs): Sale order JSON processing.
  - [`TrainingRecordCustomCheckTask.cs`](CustomizationTasks/TrainingRecordCustomCheckTask.cs): Custom training record checks.

- **Form/**  
  Contains form files, such as [`GettingStarted.frm`](Form/GettingStarted.frm), for SampleManager UI customization.

- **JBVcustomtask/**  
  Contains JBV-specific custom tasks and utilities, such as:
  - [`Class1.cs`](JBVcustomtask/Class1.cs)
  - [`ITKCustomerExcelTask.cs`](JBVcustomtask/ITKCustomerExcelTask.cs)

- **logs/**  
  Contains log files for debugging and monitoring, e.g., `xvba_debug.log`.

## Getting Started

1. Open `Customization.sln` in Visual Studio.
2. Restore NuGet packages.
3. Build the solution.
4. Deploy the compiled DLLs and scripts to your SampleManager LIMS environment as required.

## Requirements

- .NET Framework (version as specified in the `.csproj` files)
- Visual Studio (recommended for development)

## Contributing

Contributions are welcome! Please open issues or submit pull requests for bug fixes or new features.

## License

[Specify your license here, e.g., MIT, Apache 2.0, etc.]

---

For more details, see the individual project folders and source files.