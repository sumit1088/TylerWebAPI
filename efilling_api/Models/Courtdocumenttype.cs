using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class Courtdocumenttype
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? filingcodeid { get; set; }
        public string? iscourtuseonly { get; set; }
        public string? isdefault { get; set; }
        public string? promptforconfirmation { get; set; }
        public string? efspcode { get; set; }
    }
}
