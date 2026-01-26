using System.ComponentModel.DataAnnotations;

namespace University_Grant_Application_System.Models
{
    public class College
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string CollegeName { get; set; }
    }
}
