using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public partial class CaseDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? CaseId { get; set; }
        public int? CourtDocId { get; set; }
        public string? NameExtension { get; set; }
        public string? FileName { get; set; }
        public string? Security { get; set; }
        public int? Qty { get; set; }
        public int? CourtReservationNo { get; set; }
        public int? FiledBy { get; set; }

        public virtual Case? Case { get; set; }
        public virtual CourtDocument? CourtDoc { get; set; }
        public virtual UserParty? FiledByNavigation { get; set; }
    }
}
