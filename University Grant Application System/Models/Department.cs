using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models

{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public int SchoolId { get; set; }
        [ForeignKey("SchoolId")]
        public School? School { get; set; }
    }
}