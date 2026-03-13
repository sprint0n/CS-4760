using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models;

public class TravelExpense
{
    public int Id { get; set; }
    public string? TravelName { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? RSPGAmount { get; set; } // Match the HTML

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OtherAmount1 { get; set; } // Add this

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OtherAmount2 { get; set; } // Add this
    public int FormTableId { get; set; }
}