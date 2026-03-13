using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models;

public class OtherExpense
{
    public int Id { get; set; }
    public string? OtherExpenseName { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Amount { get; set; } // This is the RSPG Amount

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OtherAmount1 { get; set; } // Add this

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OtherAmount2 { get; set; } // Add this

    public int FormTableId { get; set; }
}