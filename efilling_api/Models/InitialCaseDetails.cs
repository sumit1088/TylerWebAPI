namespace efilling_api.Models
{
    public class InitialCaseDetails
    {
        public class Cases
        {
            public int Id { get; set; }
            public string? SelectedCourt { get; set; }
            public string? SelectedCategory { get; set; }
            public string? SelectedCaseType { get; set; }
            public string? PaymentAccount { get; set; }
            public decimal TotalFees { get; set; }
            public string? EnvelopeNo { get; set; }
            public string? NoteToClerk { get; set; }
            public string? CreatedBy { get; set; }
            public string? SubmittedDate { get; set; }
            public string? selectedAttorneySec { get; set; }
            public string? courtesyemail { get; set; }
            public Int32 FilingID { get; set; }
            public bool IsDraft { get; set; }
            public string? DraftSavedAt { get; set; }
            public string? CaseIDResp { get; set; }
            public bool? IsExistingCase { get; set; }
            public string? CaseNumber { get; set; }
            public string? caseTitle { get; set; }
            public string? courtLocation { get; set; }
            public string? caseTrackingID { get; set; }
            public string? caseFilingId { get; set; }   

            public virtual ICollection<Documents> Documents { get; set; } = new List<Documents>();
            public virtual ICollection<Parties> Parties { get; set; } = new List<Parties>();
            public virtual ICollection<SelectedParties> SelectedParties { get; set; } = new List<SelectedParties>();
        }

        public class Documents
        {
            public int Id { get; set; }
            public int CaseId { get; set; }
            public string? EnvelopeNo { get; set; }
            public string? DocumentType { get; set; }
            public string? DocumentDescription { get; set; }
            public string? FileName { get; set; }
            public string? FileBase64 { get; set; }
            public string? SecurityTypes { get; set; }
            public decimal? fee { get; set; }

            public virtual Cases Case { get; set; }

            public virtual ICollection<OptionalServices> OptionalServices { get; set; } = new List<OptionalServices>();
        }

        public class OptionalServices
        {
            public int Id { get; set; }
            public int DocumentId { get; set; }
            public decimal? Quantity { get; set; }
            public int CaseId { get; set; }
            public string? OptionalServiceId { get; set; }
            public bool? multiplier { get; set; }
            public string? EnvelopeNo { get; set; }
            public string? DocumentTypeId { get; set; }
            public decimal? fee { get; set; }
            public string? label { get; set; }

            public virtual Documents Document { get; set; }
        }

        public class Parties
        {
            public int Id { get; set; }
            public int CaseId { get; set; }
            public int CasesId { get; set; }
            public string? SelectedPartyType { get; set; }
            public string? RoleType { get; set; }
            public string? LastName { get; set; }
            public string? FirstName { get; set; }
            public string? MiddleName { get; set; }
            public string? Suffix { get; set; }
            public string? CompanyName { get; set; }
            public string? Address { get; set; }
            public string? Address2 { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Zip { get; set; }
            public bool AddressUnknown { get; set; }
            public string? InternationalAddress { get; set; }
            public bool SaveToAddressBook { get; set; }
            public string? SelectedAttorney { get; set; }
            public string? EnvelopeNo { get; set; }

            public virtual ICollection<SelectedBarNumber> SelectedBarNumbers { get; set; } = new List<SelectedBarNumber>();
        }

        public class SelectedBarNumber  
        {
            public int Id { get; set; }
            public int PartyId { get; set; }
            public string? BarNumber { get; set; }

            public virtual Parties Party { get; set; }
        }

        public class SelectedParties
        {
            public int Id { get; set; }
            public int CaseId { get; set; }
            public string? PartyName { get; set; }
            public string? PartyType { get; set; }
            public string? Role { get; set; }
            public string? EnvelopeNo { get; set; }

            public virtual Cases Case { get; set; }
        }

    }
}
