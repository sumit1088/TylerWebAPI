using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class Courtfilingcomponent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? filingcodeid { get; set; }
        public string? required { get; set; }
        public string? allowmultiple { get; set; }
        public string? displayorder { get; set; }
        public string? allowedfiletypes { get; set; }
        public string? efspcode { get; set; }
    }
}
