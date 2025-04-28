using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class CaseTypeParty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CaseTypeId { get; set; }
        public int? DemandedParties { get; set; }

        public virtual CaseType? CaseType { get; set; }
    }
}
