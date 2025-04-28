using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class Courtpartytype
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public bool? isavailablefornewparties { get; set; }
        public string? casetypeid { get; set; }
        public bool? isrequired { get; set; }
        public string? amount { get; set; }
        public string? numberofpartiestoignore { get; set; }
        public string? sendforredaction { get; set; }
        public string? dateofdeath { get; set; }
        public string? displayorder { get; set; }
        public string? efspcode { get; set; }
    }
}
