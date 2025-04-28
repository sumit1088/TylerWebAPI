using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class UserInfo
    {
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
        public string? RegistrationType { get; set; }
        public string? BarId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateUser
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
        public string Email { get; set; } = null!;
        public string? FirmID { get; set; }
        public string? UserID { get; set; }
         

    }
    public class UpdateFirm
    {
        public string? FirmName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string State { get; set; } = null!;
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirmID { get; set; }


    }
}
