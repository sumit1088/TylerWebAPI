using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class UserNotification
    {
        
        public int? UserId { get; set; }
        //public string Notification { get; set; } = null!;
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string? title { get; set; }

        public string? description { get; set; }

        public DateTime? Created_at { get; set; }

        public Boolean? isRead { get; set; }

        //new2
        public int? CaseId { get; set; }

        public virtual Case? Case { get; set; }
        public virtual User? User { get; set; }
    }
}
