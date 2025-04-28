using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class UserResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UserID { get; set; }
        public string? FirmID { get; set; }
        public string? PasswordHash { get; set; }
        public bool ActivationRequired { get; set; }
        public string? ExpirationDateTime { get; set; }
        public string? Email { get; set; }
        public Error? Error { get; set; }
    }

    [NotMapped]
    public class Error
    {
        public string? ErrorCode { get; set; }
        public string? ErrorText { get; set; }
    }
    
}
