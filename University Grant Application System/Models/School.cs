using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class School
    {
        [Key]
        public int SchoolId { get; set; }
        public string SchoolName { get; set; } = string.Empty;

        public int CollegeId { get; set; }
        [ForeignKey("CollegeId")]
        public College? College { get; set; }
    }
}
