namespace University_Grant_Application_System.Models;

public class OtherExpense
{
    public int Id { get; set; }
    public string? OtherExpenseName { get; set; }
    public decimal? Amount { get; set; } // This is the RSPG Amount
    public decimal? OtherAmount1 { get; set; } // Add this
    public decimal? OtherAmount2 { get; set; } // Add this
    public int FormTableId { get; set; }
}