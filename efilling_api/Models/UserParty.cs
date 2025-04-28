using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class UserParty
    {
        public UserParty()
        {
            CaseDocuments = new HashSet<CaseDocument>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? Role { get; set; }
        public string? Type { get; set; }
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

        public virtual User? User { get; set; }
        public virtual ICollection<CaseDocument> CaseDocuments { get; set; }
    }
}
