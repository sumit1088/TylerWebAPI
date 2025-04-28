using System;
using System.Collections.Generic;

namespace efilling_api.Models
{
    // Shared Classes
    public class Document
    {  
        
        public string documentType { get; set; }
        public string documentDescription { get; set; }
        public string fileName { get; set; }
        public string fileBase64 { get; set; }
        public string? securityTypes { get; set; }
        public decimal? fee { get; set; }
        public List<OptionalService>? optionalServicesSelections { get; set; }
    }

    public class Party
    {        
        public string selectedPartyType { get; set; }
        public string roleType { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string suffix { get; set; }
        public string companyName { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public bool addressUnknown { get; set; }
        public string internationalAddress { get; set; }
        public bool saveToAddressBook { get; set; }
        public string selectedAttorney { get; set; }
        public List<string> selectedBarNumbers { get; set; }
    }

    public class SelectedParty
    {        
        public string partyName { get; set; }
        public string partyType { get; set; }
        public string role { get; set; }
    }

    public class OptionalService
    {        
        public string? value { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? fee { get; set; }
        public string? label { get; set; }
        public bool? multiplier { get; set; }
    }

    // Combined Models
    public class SubSequentFeeCalRequest
    {        
        public string selectedCourt { get; set; }
        public string caseNumber { get; set; }
        public string paymentAccount { get; set; }
        public string? selectedAttorneySec { get; set; }
        public string? createdBy { get; set; }
        public string? courtesyemail { get; set; }
        public string? note { get; set; }
        public List<Document> documents { get; set; }
        public List<Party> parties { get; set; }
        public List<SelectedParty> selectedParties { get; set; }
    }

    public class FeeCalculationRequest
    {
        public bool isExistingCase { get; set; } // ✅ Add this field
        public string? caseNumber { get; set; }
        public string selectedCourt { get; set; }
        public string selectedCategory { get; set; }
        public string selectedCaseType { get; set; }
        public string paymentAccount { get; set; }
        public string selectedAttorneySec { get; set; }
        public string createdBy { get; set; }
        public string courtesyemail { get; set; }
        public string note { get; set; }
        public string? caseTitle { get; set; }        
        public Int32 filingID { get; set; }
        public string? courtLocation { get; set; }
        public string? caseTrackingID { get; set; }
        public int? caseId { get; set; }

        public List<Document> documents { get; set; }
        public List<Party> parties { get; set; }
        public List<SelectedParty> selectedParties { get; set; }
    }

    public class FeesCalculationRequest
    {
        public string selectedCourt { get; set; }
        public string selectedCategory { get; set; }
        public string selectedCaseType { get; set; }
        public string paymentAccount { get; set; }
        public List<Document> documents { get; set; }
        public List<Party> parties { get; set; }
        public List<SelectedParty> selectedParties { get; set; }
    }

    public class ServeFilingRequest
    {     
        //public string? caseNumber { get; set; }
        public string selectedCourt { get; set; }
        public string selectedCaseType { get; set; }
        public string CaseTrackingID { get; set; }
        public string selectedCaseCategoryId { get; set; }
        public string paymentAccount { get; set; }
        public string selectedAttorneySec { get; set; }
        public string createdBy { get; set; }
        public string courtesyemail { get; set; }
        public string note { get; set; }

        public List<Document> documents { get; set; }
        public List<Party> parties { get; set; }
        public List<SelectedParty> selectedParties { get; set; }


        public class ReservedRequest
        {
            public bool isExistingCase { get; set; } // ✅ Add this field
            public string? caseNumber { get; set; }
            public string selectedCourt { get; set; }
            public string selectedCategory { get; set; }
            public string selectedCaseType { get; set; }
            public string paymentAccount { get; set; }
            public string selectedAttorneySec { get; set; }
            public string createdBy { get; set; }
            public string courtesyemail { get; set; }
            public string note { get; set; }
            public string? caseTitle { get; set; }
            
        }
    }
}
