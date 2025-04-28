using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class CaseServiceContact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CaseId { get; set; }
        public int? UserAttorneyId { get; set; }
        public int? UserServiceContactId { get; set; }
        //public string? FirstName { get; set; }
        //public string? MiddleName { get; set; }
        //public string? LastName { get; set; }
        //public string? Email { get; set; }
        //public string? AdminCopyEmail { get; set; }
        //public string? Address1 { get; set; }
        //public string? Address2 { get; set; }
        //public string? City { get; set; }
        //public string? State { get; set; }
        //public short? ZipCode { get; set; }
        //public string? PhoneNo { get; set; }
        //public bool? IsPublic { get; set; }
        public bool? EServe { get; set; }
        public string? ServiceType { get; set; }
        public string? Status { get; set; }
        //new // not needed as these info will already exist in user service contact id 
        //public string? Firm { get; set; }

        public virtual Case? Case { get; set; }
        public virtual UserAttorney? UserAttorney { get; set; }
        public virtual UserServiceContact? UserServiceContact { get; set; }
    }
}
