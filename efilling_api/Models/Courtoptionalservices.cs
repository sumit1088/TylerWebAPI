using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class Courtoptionalservices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? displayorder { get; set; }
        public string? fee { get; set; }
        public string? filingcodeid { get; set; }
        public bool? multiplier { get; set; }
        public string? altfeedesc { get; set; }
        public string? hasfeeprompt { get; set; }
        public string? feeprompttext { get; set; }
        public string? required { get; set; }
        public string? ismprff { get; set; }
        public string? efspcode { get; set; }
    }
}
