using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        public int UserId { get; set; }

        //public int? UserAccountType { get; set; }
        public int? role { get; set; }

        public virtual User? User { get; set; }
    }
}
