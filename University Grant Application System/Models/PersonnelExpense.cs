using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class PersonnelExpense
    {
        [Key]
        public int Id { get; set; }

        public int FormTableId { get; set; }
        [ForeignKey("FormTableId")]
        public virtual FormTable? FormTable { get; set; }

        // Optional, can be left blank
        public string? Role { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TaxedAmount { get; set; }
    }
}