
namespace University_Grant_Application_System.Models;

public class EquipmentExpense
{
    public int Id { get; set; }
    public string? EquipmentName { get; set; }
    public decimal? RSPGAmount { get; set; } // Match the HTML
    public decimal? OtherAmount1 { get; set; } // Add this
    public decimal? OtherAmount2 { get; set; } // Add this
    public int FormTableId { get; set; }
}









