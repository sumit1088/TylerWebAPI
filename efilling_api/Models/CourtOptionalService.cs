using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class CourtOptionalService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal? OpSrvFee { get; set; }
        public int? CourtOpSrvType { get; set; }
        public int? CourtDocId { get; set; }

        public virtual CourtDocument? CourtDoc { get; set; }
    }
}
