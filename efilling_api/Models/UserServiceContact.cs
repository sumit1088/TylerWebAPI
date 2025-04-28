using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class UserServiceContact
    {
        public UserServiceContact()
        {
            CaseServiceContacts = new HashSet<CaseServiceContact>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? AdminCopyEmail { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public short? ZipCode { get; set; }
        public string? PhoneNo { get; set; }
        public bool? IsPublic { get; set; }
        // new 
        public string? Firm { get; set; }
        // new 2
        public bool? IsFirm { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<CaseServiceContact> CaseServiceContacts { get; set; }
    }
}
