using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class CaseCategoryCodes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? ecfcasetype { get; set; }
        public string? procedureremedyinitial { get; set; }
        public string? procedureremedysubsequent { get; set; }
        public string? damageamountinitial { get; set; }
        public string? damageamountsubsequent { get; set; }
        public string? efspcode { get; set; }
    }
}
