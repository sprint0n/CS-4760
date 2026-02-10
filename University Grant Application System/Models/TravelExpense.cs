using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class TravelExpense
    {
        [Key]
        public int TravelExpenseId { get; set; }

        public int FormTableId { get; set; }
        [ForeignKey("FormTableId")]
        public virtual FormTable? FormTable { get; set; }

        public string? TravelName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
    }
}