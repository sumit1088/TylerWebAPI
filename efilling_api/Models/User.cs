using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class User
    {
        public User()
        {
            Cases = new HashSet<Case>();
            ServeOnlyCases = new HashSet<ServeOnlyCase>();
            UserAttorneys = new HashSet<UserAttorney>();
            UserParties = new HashSet<UserParty>();
            UserPaymentAccounts = new HashSet<UserPaymentAccount>();
            UserServiceContacts = new HashSet<UserServiceContact>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordEncrypted { get; set; } = null!;
        public string? SecurityQuestion { get; set; }
        public string? SecurityAnswer { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
        public string? Organization { get; set; }
        public string? PhoneNo { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? CountryCode { get; set; }
        public string? FirmName { get; set; }
        public bool? IsActivated { get; set; }
        public int? Status { get; set; }
        // updated the name from FilingStatus to RecieveFilingStatus
        public bool? RecieveFilingStatus { get; set; }
        public string? CcEmails { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Case> Cases { get; set; }
        public virtual ICollection<ServeOnlyCase> ServeOnlyCases { get; set; }
        public virtual ICollection<UserAttorney> UserAttorneys { get; set; }
        public virtual ICollection<UserParty> UserParties { get; set; }
        public virtual ICollection<UserPaymentAccount> UserPaymentAccounts { get; set; }
        public virtual ICollection<UserServiceContact> UserServiceContacts { get; set; }
    }
}
