using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class CaseParty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CaseId { get; set; }
        public int? Role { get; set; }
        public int? Type { get; set; }
        public string? CompanyName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public short? ZipCode { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public bool? AddressUnknown { get; set; }
        public bool? InternationalAddress { get; set; }
        public bool? SaveToAddressBook { get; set; }
        public int? FeeExemption { get; set; }
        public string? Interpreter { get; set; }
        public int? RepresentingAttorneyId { get; set; }
        public bool? FilingParty { get; set; }

        public virtual Case? Case { get; set; }
        public virtual UserAttorney? RepresentingAttorney { get; set; }
    }
}
