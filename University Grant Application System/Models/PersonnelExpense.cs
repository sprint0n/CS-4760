using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models;

public class PersonnelExpense
{
    public int Id { get; set; }
    public string? Role { get; set; }
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Amount { get; set; } // This is the RSPG Amount

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OtherAmount1 { get; set; } // Add this

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OtherAmount2 { get; set; } // Add this

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TaxedAmount { get; set; }
    public int FormTableId { get; set; }
}