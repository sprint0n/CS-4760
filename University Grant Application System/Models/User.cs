using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace University_Grant_Application_System.Models
{
    public class User
    {
        [Key]
        [Required]
        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName {  get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public int AccountID { get; set; } = 0;

        public string HashedPassword { get; set; } = string.Empty;

        public DateOnly Birthday { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddYears(-20));

        public string userType { get; set; } = string.Empty;


        public int? SchoolId { get; set; }
        [ForeignKey("SchoolId")]
        public School? School { get; set; }

        // Foreign Key
        public int? CollegeId { get; set; }
        [ForeignKey("CollegeId")]
        public College? College { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
    }
}
