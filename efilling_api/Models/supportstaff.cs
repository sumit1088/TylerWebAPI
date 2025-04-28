using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace efilling_api.Models
{
    [Table("supportstaff")]
    public class supportstaff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")] // Force lowercase to match PostgreSQL
        public int Id { get; set; }

        [Required]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("middle_name")]
        public string MiddleName { get; set; }

        [Required]
        [Column("last_name")]
        public string LastName { get; set; }

        [Column("suffix")]
        public string Suffix { get; set; }

        [Required]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; }

        [Column("user_created")]
        public DateTime UserCreated { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("user_type")]
        public string UserType { get; set; }

        [Column("user_status")]
        public bool UserStatus { get; set; }
    }
}
