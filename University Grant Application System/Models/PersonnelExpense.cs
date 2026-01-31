using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class PersonnelExpense
    {
        [Key]
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        public string Role { get; set; } // "Student" or "Teacher"

        public string Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TaxedAmount { get; set; }
    }
}
