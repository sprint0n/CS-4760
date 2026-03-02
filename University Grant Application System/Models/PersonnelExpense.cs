namespace University_Grant_Application_System.Models;

public class PersonnelExpense
{
    public int Id { get; set; }
    public string? Role { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; } // This is the RSPG Amount
    public decimal? OtherAmount1 { get; set; } // Add this
    public decimal? OtherAmount2 { get; set; } // Add this
    public decimal? TaxedAmount { get; set; }
    public int FormTableId { get; set; }
}