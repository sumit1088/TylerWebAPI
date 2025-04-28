using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class ServeOnlyCase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CreatedBy { get; set; }
        public int? ClientMatterNo { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
    }
}
