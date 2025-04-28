using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class UserAttorney
    {
        public UserAttorney()
        {
            CaseParties = new HashSet<CaseParty>();
            CaseServiceContacts = new HashSet<CaseServiceContact>();
            Cases = new HashSet<Case>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? BarId { get; set; }
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FirmID { get; set; }
        public string? Suffix { get; set; }
        public string? Email { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public short? ZipCode { get; set; }
        public string? PhoneNo { get; set; }
        public bool? MakeUserLogin { get; set; }
        public bool? MakeServiceContact { get; set; }
        public bool? MakeServiceContactPublic { get; set; }
        public bool? MakeFirmAdmin { get; set; }
        public bool? RecFilingStatusEmails { get; set; }
        public int? Status { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<CaseParty> CaseParties { get; set; }
        public virtual ICollection<CaseServiceContact> CaseServiceContacts { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
    }
}
