// class for interpreting salesorderjsons for OdooAPI.
using System;
using System.Runtime.Serialization;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.WebApiTasks.Mobile.Data;


namespace Customization.Tasks
{
    /// <summary>
    /// The suignature.
    /// </summary>
    [DataContract(Name ="Jsonstring")]
    [Serializable]

   
    public class SaleOrderJson : MobileObject
    {
        /// <summary>
        /// Gets or sets the sale order ID.
        /// </summary>
        /// <value>
        /// The sale order ID.
        /// </value>
        [DataMember(Name = "SaleOrderID")]
        public int SaleOrderId { get; set; }

        /// <summary>
        /// Gets or set the sale order name
        /// </summary>
        /// <value>
        /// the sale order name
        /// </value>
        [DataMember(Name = "SaleOrder")]
        public string SaleOrderName {  get; set; }

        ///<summary>
        /// gets or sets the  JobType PhraseID
        /// </summary>
        ///<value>
        /// the JobTypePhraseID
        /// </value>
        [DataMember(Name = "JobTypePhraseID")]
        public string JobTypePhraseID {  get; set; }


        ///<summary>
        /// gets or sets the JobType
        /// </summary>
        ///<value>
        /// the jobtype
        /// </value>
        [DataMember(Name = "JobType")]
        public string jobType { get; set; }


        ///<summary>
        /// gets or sets the ConfirmationEmail
        /// </summary>
        ///<value>
        /// the ConfirmationEmail
        /// </value>
        [DataMember(Name = "ConfirmationEmail")]
        public string ConfirmationEmail { get; set; }

        ///<summary>
        /// gets or sets the ReportEmail
        /// </summary>
        ///<value>
        /// the ReportEmail
        /// </value>
        [DataMember(Name = "ReportEmail")]
        public string ReportEmail { get; set; }

        ///<summary>
        /// gets or sets the PoNumber
        /// </summary>
        ///<value>
        /// the PoNumber
        /// </value>
        [DataMember(Name = "PoNumber")]
        public string PoNumber { get; set; }

        ///<summary>
        /// gets or sets the Contact Pers Invoice
        /// </summary>
        ///<value>
        /// the ContactPersInvoic
        /// </value>
        [DataMember(Name = "ContactPersInvoic")]
        public string ContactPersInvoic { get; set; }

        ///<summary>
        /// gets or sets the Contact Pers Report
        /// </summary>
        ///<value>
        /// the ContactPersReport
        /// </value>
        [DataMember(Name = "ContactPersReport")]
        public string ContactPersReport { get; set; }

        ///<summary>
        /// gets or sets the Contact Pers Confirmation
        /// </summary>
        ///<value>
        /// the ContactPersConfirm
        /// </value>
        [DataMember(Name = "ContactPersConfirm")]
        public string ContactPersConfirm { get; set; }

        ///<summary>
        /// gets or sets the TurnAroundTimePhraseID
        /// </summary>
        ///<value>
        /// the TurnAroundTimePhraseID
        /// </value>
        [DataMember(Name = "TurnAroundTimePhraseID")]
        public string serviceLevel { get; set; }

        ///<summary>
        /// gets or sets the BrowseDescription
        /// </summary>
        ///<value>
        /// the BrowseDescription
        /// </value>
        [DataMember(Name = "BrowseDescription")]
        public string BrowseDescription { get; set; }

        ///<summary>
        /// gets or sets the JobComments
        /// </summary>
        ///<value>
        /// the JobComments
        /// </value>
        [DataMember(Name = "JobComments")]
        public string JobComments { get; set; }

        ///<summary>
        /// gets or sets the TurnAroundDaysPhraseID
        /// </summary>
        ///<value>
        /// the TurnAroundDaysPhraseID
        /// </value>
        [DataMember(Name = "TurnAroundDaysPhraseID")]
        public string serviceLevelDays { get; set; }

        ///<summary>
        /// gets or sets the CustomerName
        /// </summary>
        ///<value>
        /// the CustomerName
        /// </value>
        [DataMember(Name = "CustomerName")]
        public string customerName { get; set; }

        /// <summary>
        /// Gets or sets the sale order samples.
        /// </summary>
        /// <value>
        /// The sale order samples.
        /// </value>
        [DataMember(Name = "SaleOrderSamples")]
        public SaleOrderSample[] SaleOrderSamples { get; set; }
    }

    [DataContract]
    [Serializable]
    public class SaleOrderSample
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        [DataMember(Name = "SamplesID")]
        public int Id { get; set; }


        /// <summary>
        /// Gets or sets the SamplesTypeIdentity.
        /// </summary>
        /// <value>
        /// The sample type.
        /// </value>
        [DataMember(Name = "SamplesTypeIdentity")]
        public string SampleType { get; set; }

        /// <summary>
        /// Gets or sets the SamplesDescription.
        /// </summary>
        /// <value>
        /// The SamplesDescription.
        /// </value>
        [DataMember(Name = "SamplesDescription")]
        public string SampleDescription { get; set; }

        /// <summary>
        /// Gets or sets the SamplesName.
        /// </summary>
        /// <value>
        /// The SamplesName.
        /// </value>
        [DataMember(Name = "SamplesName")]
        public string SampleName { get; set; }


        /// <summary>
        /// Gets or sets the external reference.
        /// </summary>
        /// <value>
        /// The external reference.
        /// </value>
        [DataMember(Name = "SamplesExternalReference")]
        public string ExternalReference { get; set; }

        /// <summary>
        /// Gets or sets the sample size.
        /// </summary>
        /// <value>
        /// The sample size.
        /// </value>
        [DataMember(Name = "SamplesSize")]
        public double SampleSize { get; set; }

        /// <summary>
        /// Gets or sets the sample size unit.
        /// </summary>
        /// <value>
        /// The sample size unit.
        /// </value>
        [DataMember(Name = "SamplesSizeUnitPhraseID")]
        public string SampleSizeUnit { get; set; }

        /// <summary>
        /// Gets or sets the sample density.
        /// </summary>
        /// <value>
        /// The sample density.
        /// </value>
        [DataMember(Name = "SamplesDensity")]
        public double SampleDensity { get; set; }

        /// <summary>
        /// Gets or sets the sample density unit.
        /// </summary>
        /// <value>
        /// The sample density unit.
        /// </value>
        [DataMember(Name = "SamplesDensitySizePhraseID")]
        public string SampleDensityUnit { get; set; }

        /// <summary>
        /// Gets or sets the sample subtype.
        /// </summary>
        /// <value>
        /// The sample subtype.
        /// </value>
        [DataMember(Name = "SamplesSubTypeIdentity")]
        public string SampSubtype { get; set; }


        /// <summary>
        /// Gets or sets the variety.
        /// </summary>
        /// <value>
        /// the variety
        /// </value>
        [DataMember(Name = "variety")]
        public string Variety { get; set; }

        /// <summary>
        /// Gets or sets the sample batch.
        /// </summary>
        /// <value>
        /// the sample batch
        /// </value>
        [DataMember(Name = "SamplesBatch")]
        public string SampleBatch { get; set; }

        /// <summary>
        /// Gets or sets if SamplesRequired.
        /// </summary>
        /// <value>
        /// the SamplesRequired 
        /// </value>
        [DataMember(Name = "SamplesRequired")]
        public Boolean SamplingReq { get; set; }

        /// <summary>
        /// Gets or sets SamplesRemarkPhraseID.
        /// </summary>
        /// <value>
        /// the SamplesRemarkPhraseID 
        /// </value>
        [DataMember(Name = "SamplesRemarkPhraseID")]
        public string SamplesRemarkPhraseID { get; set; }

        /// <summary>
        /// Gets or sets SamplesRemark.
        /// </summary>
        /// <value>
        /// the SamplesRemark 
        /// </value>
        [DataMember(Name = "SamplesRemark")]
        public string SamplesRemark { get; set; }

        /// <summary>
        /// Gets or sets SamplesPlateName.
        /// </summary>
        /// <value>
        /// the SamplesPlateName 
        /// </value>
        [DataMember(Name = "SamplesPlateName")]
        public string SamplesPlateName { get; set; }

        /// <summary>
        /// Gets or sets SamplesPlateWell.
        /// </summary>
        /// <value>
        /// the SamplesPlateWell 
        /// </value>
        [DataMember(Name = "SamplesPlateWell")]
        public string SamplesPlateWell { get; set; }

        /// <summary>
        /// Gets or sets if SamplesPooledSamples.
        /// </summary>
        /// <value>
        /// the SamplesPooledSamples 
        /// </value>
        [DataMember(Name = "SamplesPooledSamples")]
        public Boolean SamplesPooledSamples { get; set; }

        /// <summary>
        /// Gets or sets if SamplesCustomerITKXMLReport.
        /// </summary>
        /// <value>
        /// the SamplesCustomerITKXMLReport 
        /// </value>
        [DataMember(Name = "SamplesCustomerITKXMLReport")]
        public Boolean SamplesCustomerITKXMLReport { get; set; }


        /// <summary>
        /// Gets or sets the split sample method.
        /// </summary>
        /// <value>
        /// The split sample method.
        /// </value>
        [DataMember(Name = "SamplesSplitMethodPhraseID")]
        public string SplitSampMethod { get; set; }

        ///// <summary>
        ///// Gets or sets the workflow.
        ///// </summary>
        ///// <value>
        ///// The workflow.
        ///// </value>
        //[DataMember(Name = "SamplesWorkflow")]
        //public string SampleWorkflow { get; set; }

        /// <summary>
        /// Gets or sets the limit of detection.
        /// </summary>
        /// <value>
        /// The limit of detection.
        /// </value>
        [DataMember(Name = "LOD")]
        public double Lod { get; set; }


        /// <summary>
        /// Gets or sets the testschedule.
        /// </summary>
        /// <value>
        /// The create testschedule.
        /// </value>
        [DataMember(Name = "SamplesTestScheduleIdentity")]
        public string TestSchedule { get; set; }

        /// <summary>
        /// Gets or sets the test list.
        /// </summary>
        /// <value>
        /// The test list.
        /// </value>
        [DataMember(Name = "Tests")]
        public TestList[] TestList { get; set; }
    }

    [DataContract]
    [Serializable]
    public class TestList
    {
        /// <summary>
        /// Gets or sets the analysis name.
        /// </summary>
        /// <value>
        /// The analysis name.
        /// </value>
        [DataMember(Name = "AnalysisName")]
        public string AnalysisName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this test is part of a schedule.
        /// </summary>
        /// <value>
        ///   <c>true</c> if test is scheduled; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "TestSchedule")]
        public string TestSchedule { get; set; }
    }
}


//namespace Customization.Tasks
//{
//    /// <summary>
//    /// The suignature.
//    /// </summary>
//    [DataContract(Name = "Jsonstring")]
//    public class SaleOrderJson : MobileObject
//    {
//        /// <summary>
//        /// Gets or sets the sale order ID.
//        /// </summary>
//        /// <value>
//        /// The sale order ID.
//        /// </value>
//        [DataMember(Name = "sale_order_id")]
//        public int SaleOrderId { get; set; }

//        /// <summary>
//        /// Gets or sets the sale order name.
//        /// </summary>
//        /// <value>
//        /// The sale order name.
//        /// </value>
//        [DataMember(Name = "sale_order_name")]
//        public string SaleOrderName { get; set; }

//        /// <summary>
//        /// Gets or sets the sale order customer ID.
//        /// </summary>
//        /// <value>
//        /// The sale order customer ID.
//        /// </value>
//        [DataMember(Name = "sale_order_customer_id")]
//        public int SaleOrderCustomerId { get; set; }

//        /// <summary>
//        /// Gets or sets the sale order customer name.
//        /// </summary>
//        /// <value>
//        /// The sale order customer name.
//        /// </value>
//        [DataMember(Name = "sale_order_customer_name")]
//        public string SaleOrderCustomerName { get; set; }

//        /// <summary>
//        /// Gets or sets the sale order customer street.
//        /// </summary>
//        /// <value>
//        /// The sale order customer street.
//        /// </value>
//        [DataMember(Name = "sale_order_customer_street")]
//        public string SaleOrderCustomerStreet { get; set; }

//        /// <summary>
//        /// Gets or sets the sale order customer city.
//        /// </summary>
//        /// <value>
//        /// The sale order customer city.
//        /// </value>
//        [DataMember(Name = "sale_order_customer_city")]
//        public string SaleOrderCustomerCity { get; set; }

//        /// <summary>
//        /// Gets or sets the sale order customer zip code.
//        /// </summary>
//        /// <value>
//        /// The sale order customer zip code.
//        /// </value>
//        [DataMember(Name = "sale_order_customer_zip")]
//        public string SaleOrderCustomerZip { get; set; }

//        /// <summary>
//        /// Gets or sets the sale order customer country.
//        /// </summary>
//        /// <value>
//        /// The sale order customer country.
//        /// </value>
//        [DataMember(Name = "sale_order_customer_country")]
//        public string SaleOrderCustomerCountry { get; set; }

//        /// <summary>
//        /// Gets or sets the browse description.
//        /// </summary>
//        /// <value>
//        /// The browse description.
//        /// </value>
//        [DataMember(Name = "browse_description")]
//        public string BrowseDescription { get; set; }

//        /// <summary>
//        /// Gets or sets the PO numbers.
//        /// </summary>
//        /// <value>
//        /// The PO numbers.
//        /// </value>
//        [DataMember(Name = "po_numbers")]
//        public string PoNumbers { get; set; }

//        /// <summary>
//        /// Gets or sets the customer project.
//        /// </summary>
//        /// <value>
//        /// The customer project.
//        /// </value>
//        [DataMember(Name = "customer_project")]
//        public string CustomerProject { get; set; }

//        /// <summary>
//        /// Gets or sets the report email.
//        /// </summary>
//        /// <value>
//        /// The report email.
//        /// </value>
//        [DataMember(Name = "report_email")]
//        public string ReportEmail { get; set; }

//        /// <summary>
//        /// Gets or sets the confirmation email.
//        /// </summary>
//        /// <value>
//        /// The confirmation email.
//        /// </value>
//        [DataMember(Name = "confirmation_email")]
//        public string ConfirmationEmail { get; set; }

//        /// <summary>
//        /// Gets or sets the contact person for invoice.
//        /// </summary>
//        /// <value>
//        /// The contact person for invoice.
//        /// </value>
//        [DataMember(Name = "contact_person_invoice")]
//        public string ContactPersonInvoice { get; set; }

//        /// <summary>
//        /// Gets or sets the contact person for report.
//        /// </summary>
//        /// <value>
//        /// The contact person for report.
//        /// </value>
//        [DataMember(Name = "contact_person_report")]
//        public string ContactPersonReport { get; set; }

//        /// <summary>
//        /// Gets or sets the contact person for confirmation.
//        /// </summary>
//        /// <value>
//        /// The contact person for confirmation.
//        /// </value>
//        [DataMember(Name = "contact_person_confirmation")]
//        public string ContactPersonConfirmation { get; set; }

//        /// <summary>
//        /// Gets or sets the contact person for return material.
//        /// </summary>
//        /// <value>
//        /// The contact person for return material.
//        /// </value>
//        [DataMember(Name = "contact_person_return_material")]
//        public string ContactPersonReturnMaterial { get; set; }

//        /// <summary>
//        /// Gets or sets the Kraken ID.
//        /// </summary>
//        /// <value>
//        /// The Kraken ID.
//        /// </value>
//        [DataMember(Name = "kraken_id")]
//        public string KrakenId { get; set; }

//        /// <summary>
//        /// Gets or sets the job type.
//        /// </summary>
//        /// <value>
//        /// The job type.
//        /// </value>
//        [DataMember(Name = "job_type")]
//        public string JobType { get; set; }

//        /// <summary>
//        /// Gets or sets the job comment.
//        /// </summary>
//        /// <value>
//        /// The job comment.
//        /// </value>
//        [DataMember(Name = "job_comment")]
//        public string JobComment { get; set; }

//        /// <summary>
//        /// Gets or sets the additional service.
//        /// </summary>
//        /// <value>
//        /// The additional service.
//        /// </value>
//        [DataMember(Name = "additional_service")]
//        public string AdditionalService { get; set; }

//        /// <summary>
//        /// Gets or sets the sale order samples.
//        /// </summary>
//        /// <value>
//        /// The sale order samples.
//        /// </value>
//        [DataMember(Name = "sale_order_samples")]
//        public SaleOrderSample[] SaleOrderSamples { get; set; }
//    }

//    [DataContract]
//    public class SaleOrderSample
//    {
//        /// <summary>
//        /// Gets or sets the ID.
//        /// </summary>
//        /// <value>
//        /// The ID.
//        /// </value>
//        [DataMember(Name = "id")]
//        public int Id { get; set; }

//        /// <summary>
//        /// Gets or sets the sale order ID.
//        /// </summary>
//        /// <value>
//        /// The sale order ID.
//        /// </value>
//        [DataMember(Name = "sale_order_id")]
//        public object[] SaleOrderId { get; set; }

//        /// <summary>
//        /// Gets or sets the sample type.
//        /// </summary>
//        /// <value>
//        /// The sample type.
//        /// </value>
//        [DataMember(Name = "sample_type")]
//        public string SampleType { get; set; }

//        /// <summary>
//        /// Gets or sets the name.
//        /// </summary>
//        /// <value>
//        /// The name.
//        /// </value>
//        [DataMember(Name = "name")]
//        public string SampleName { get; set; }

//        /// <summary>
//        /// Gets or sets the description.
//        /// </summary>
//        /// <value>
//        /// The description.
//        /// </value>
//        [DataMember(Name = "description")]
//        public string Description { get; set; }

//        /// <summary>
//        /// Gets or sets the template.
//        /// </summary>
//        /// <value>
//        /// The template.
//        /// </value>
//        [DataMember(Name = "template")]
//        public bool Template { get; set; }

//        /// <summary>
//        /// Gets or sets the external reference.
//        /// </summary>
//        /// <value>
//        /// The external reference.
//        /// </value>
//        [DataMember(Name = "external_reference")]
//        public string ExternalReference { get; set; }

//        /// <summary>
//        /// Gets or sets the sample size.
//        /// </summary>
//        /// <value>
//        /// The sample size.
//        /// </value>
//        [DataMember(Name = "sample_size")]
//        public double SampleSize { get; set; }

//        /// <summary>
//        /// Gets or sets the sample size unit.
//        /// </summary>
//        /// <value>
//        /// The sample size unit.
//        /// </value>
//        [DataMember(Name = "sample_size_unit")]
//        public string SampleSizeUnit { get; set; }

//        /// <summary>
//        /// Gets or sets the sample density.
//        /// </summary>
//        /// <value>
//        /// The sample density.
//        /// </value>
//        [DataMember(Name = "sample_density")]
//        public double SampleDensity { get; set; }

//        /// <summary>
//        /// Gets or sets the sample density unit.
//        /// </summary>
//        /// <value>
//        /// The sample density unit.
//        /// </value>
//        [DataMember(Name = "sample_density_unit")]
//        public string SampleDensityUnit { get; set; }

//        /// <summary>
//        /// Gets or sets the sample type.
//        /// </summary>
//        /// <value>
//        /// The sample type.
//        /// </value>
//        [DataMember(Name = "samp_type")]
//        public string SampType { get; set; }

//        /// <summary>
//        /// Gets or sets the sample subtype.
//        /// </summary>
//        /// <value>
//        /// The sample subtype.
//        /// </value>
//        [DataMember(Name = "samp_subtype")]
//        public string SampSubtype { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether this <see cref="SaleOrderSample"/> is a variety.
//        /// </summary>
//        /// <value>
//        ///   <c>true</c> if variety; otherwise, <c>false</c>.
//        /// </value>
//        [DataMember(Name = "variety")]
//        public string Variety { get; set; }

//        /// <summary>
//        /// Gets or sets the sample batch.
//        /// </summary>
//        /// <value>
//        /// The sample batch.
//        /// </value>
//        [DataMember(Name = "sample_batch")]
//        public string SampleBatch { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether sampling is required.
//        /// </summary>
//        /// <value>
//        ///   <c>true</c> if sampling required; otherwise, <c>false</c>.
//        /// </value>
//        [DataMember(Name = "sampling_required")]
//        public bool SamplingRequired { get; set; }

//        /// <summary>
//        /// Gets or sets the split sample method.
//        /// </summary>
//        /// <value>
//        /// The split sample method.
//        /// </value>
//        [DataMember(Name = "split_samp_method")]
//        public object[] SplitSampMethod { get; set; }

//        /// <summary>
//        /// Gets or sets the limit of detection.
//        /// </summary>
//        /// <value>
//        /// The limit of detection.
//        /// </value>
//        [DataMember(Name = "lod")]
//        public double Lod { get; set; }

//        /// <summary>
//        /// Gets or sets the remark.
//        /// </summary>
//        /// <value>
//        /// The remark.
//        /// </value>
//        [DataMember(Name = "remark")]
//        public string Remark { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether this sample is pooled.
//        /// </summary>
//        /// <value>
//        ///   <c>true</c> if pooled sample; otherwise, <c>false</c>.
//        /// </value>
//        [DataMember(Name = "pooled_sample")]
//        public bool PooledSample { get; set; }

//        /// <summary>
//        /// Gets or sets the customer comment.
//        /// </summary>
//        /// <value>
//        /// The customer comment.
//        /// </value>
//        [DataMember(Name = "customer_comment")]
//        public string CustomerComment { get; set; }

//        /// <summary>
//        /// Gets or sets the customer plate name.
//        /// </summary>
//        /// <value>
//        /// The customer plate name.
//        /// </value>
//        [DataMember(Name = "customer_plate_name")]
//        public string CustomerPlateName { get; set; }

//        /// <summary>
//        /// Gets or sets the customer plate well.
//        /// </summary>
//        /// <value>
//        /// The customer plate well.
//        /// </value>
//        [DataMember(Name = "customer_plate_well")]
//        public string CustomerPlateWell { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether this sample has an ITK XML report.
//        /// </summary>
//        /// <value>
//        ///   <c>true</c> if ITK XML report; otherwise, <c>false</c>.
//        /// </value>
//        [DataMember(Name = "itk_xml_report")]
//        public bool ItkXmlReport { get; set; }

//        /// <summary>
//        /// Gets or sets the test IDs.
//        /// </summary>
//        /// <value>
//        /// The test IDs.
//        /// </value>
//        [DataMember(Name = "test_ids")]
//        public int[] TestIds { get; set; }

//        /// <summary>
//        /// Gets or sets the analysis IDs.
//        /// </summary>
//        /// <value>
//        /// The analysis IDs.
//        /// </value>
//        [DataMember(Name = "analysis_ids")]
//        public int[] AnalysisIds { get; set; }

//        /// <summary>
//        /// Gets or sets the display name.
//        /// </summary>
//        /// <value>
//        /// The display name.
//        /// </value>
//        [DataMember(Name = "display_name")]
//        public string DisplayName { get; set; }

//        /// <summary>
//        /// Gets or sets the create UID.
//        /// </summary>
//        /// <value>
//        /// The create UID.
//        /// </value>
//        [DataMember(Name = "create_uid")]
//        public object[] CreateUid { get; set; }

//        /// <summary>
//        /// Gets or sets the create date.
//        /// </summary>
//        /// <value>
//        /// The create date.
//        /// </value>
//        [DataMember(Name = "create_date")]
//        public DateTime CreateDate { get; set; }

//        /// <summary>
//        /// Gets or sets the write UID.
//        /// </summary>
//        /// <value>
//        /// The write UID.
//        /// </value>
//        [DataMember(Name = "write_uid")]
//        public object[] WriteUid { get; set; }

//        /// <summary>
//        /// Gets or sets the write date.
//        /// </summary>
//        /// <value>
//        /// The write date.
//        /// </value>
//        [DataMember(Name = "write_date")]
//        public DateTime WriteDate { get; set; }

//        /// <summary>
//        /// Gets or sets the testschedule.
//        /// </summary>
//        /// <value>
//        /// The create testschedule.
//        /// </value>
//        [DataMember(Name = "test_schedule")]
//        public string TestSchedule { get; set; }

//        /// <summary>
//        /// Gets or sets the test list.
//        /// </summary>
//        /// <value>
//        /// The test list.
//        /// </value>
//        [DataMember(Name = "test_list")]
//        public Test[] TestList { get; set; }
//    }

//    [DataContract]
//    public class Test
//    {
//        /// <summary>
//        /// Gets or sets the analysis name.
//        /// </summary>
//        /// <value>
//        /// The analysis name.
//        /// </value>
//        [DataMember(Name = "analysis_name")]
//        public string AnalysisName { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether this test is part of a schedule.
//        /// </summary>
//        /// <value>
//        ///   <c>true</c> if test is scheduled; otherwise, <c>false</c>.
//        /// </value>
//        [DataMember(Name = "test_schedule")]
//        public string TestSchedule { get; set; }
//    }
//}