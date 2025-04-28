using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class AttorneyDetails
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? BarId { get; set; }
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
        public string? ZipCode { get; set; }
        public string? PhoneNo { get; set; }
        public bool? MakeUserLogin { get; set; }
        public bool? MakeServiceContact { get; set; }
        public bool? MakeServiceContactPublic { get; set; }
        public bool? MakeFirmAdmin { get; set; }
        public bool? RecFilingStatusEmails { get; set; }
        public int? Status { get; set; }
        public string? AttorneyID { get; set; }

        // public virtual UserLogin? User { get; set; }
    }


    public class UpdateAttorney
    {
        public string AttorneyID { get; set; }
        public string BarNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FirmID { get; set; }
    }


    public class ApiResponse
    {
        public string AttorneyID { get; set; }
        public Error Error { get; set; }
    }
    
}

