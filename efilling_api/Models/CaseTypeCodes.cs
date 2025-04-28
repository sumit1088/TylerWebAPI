using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efilling_api.Models
{
    public class CaseTypeCodes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? casecategory { get; set; }
        public string? initial { get; set; }
        public string? fee { get; set; }
        public string? willfileddate { get; set; }
        public string? casestreetaddress { get; set; }
        public string? efspcode { get; set; }
    }
}
