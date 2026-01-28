using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class College
    {
        [Key]
        [Required]
        public int CollegeId { get; set; }

        [Required]
        public string CollegeName { get; set; }
    }
}
