using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class OtherExpense
    {
        [Key]
        public int OtherExpensesId { get; set; }

        public int FormTableId { get; set; }
        [ForeignKey("FormTableId")]
        public virtual FormTable? FormTable { get; set; }

        [Required]
        public string OtherExpenseName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

    }
}

