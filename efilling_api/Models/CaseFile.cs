using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class CaseFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? CaseTypeId { get; set; }
        public string? CourtId { get; set; }
        public string? CaseId { get; set; }
        public string? Role { get; set; }
        public string? PartyType { get; set; }
        public string? CompanyName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public bool? AddressUnknown { get; set; }
        public bool? InternationalAddress { get; set; }
        public bool? SaveToAddressBook { get; set; }
        public int? FeeExemption { get; set; }
        public string? Interpreter { get; set; }
        public int? RepresentingAttorneyId { get; set; }
        public string? FilingParty { get; set; }
        public string? FilingCode { get; set; }
        public string? DocumentDescription { get; set; }
        public string? FilingType { get; set; }
        public string? FileName { get; set; }
        public string? FileContent { get; set; } // Base64 string
        public IFormFile? File { get; set; }

        //public AttorneyDetails? AttorneyDetails { get; set; }

    }
}
