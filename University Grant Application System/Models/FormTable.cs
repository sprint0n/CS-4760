using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class FormTable
    {
        [Key]
        public int Id { get; set; }

        // -----------------------------
        // Required / non-null columns
        // -----------------------------

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [Required]
        public string ApplicationStatus { get; set; } = string.Empty;

        // -----------------------------
        // Nullable text columns
        // -----------------------------

        public string? Description { get; set; }
        public string? Procedure { get; set; }
        public string? GrantPurpose { get; set; }
        public string? Timeline { get; set; }
        public string? pastBudgets { get; set; }

        // -----------------------------
        // Nullable numerics (REAL / DECIMAL)
        // -----------------------------

        public float? DisseminationBudget { get; set; }

        public float? OtherFunding1Amount { get; set; }
        public float? OtherFunding2Amount { get; set; }
        public float? OtherFunding3Amount { get; set; }
        public float? OtherFunding4Amount { get; set; }

        public decimal? TotalBudget { get; set; }

        // -----------------------------
        // Nullable names
        // -----------------------------

        public string? OtherFunding1Name { get; set; }
        public string? OtherFunding2Name { get; set; }
        public string? OtherFunding3Name { get; set; }
        public string? OtherFunding4Name { get; set; }

        // -----------------------------
        // Nullable booleans (BIT NULL)
        // -----------------------------

        public bool? isIRB { get; set; }
        public bool? pastFunding { get; set; }

        // -----------------------------
        // Nullable FK (INT NULL)
        // -----------------------------

        public int? PrincipalInvestigatorID { get; set; }

        // -----------------------------
        // Navigation properties
        // -----------------------------

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public virtual ICollection<PersonnelExpense> PersonnelExpenses { get; set; } = new List<PersonnelExpense>();
        public virtual ICollection<TravelExpense> TravelExpenses { get; set; } = new List<TravelExpense>();
        public virtual ICollection<EquipmentExpense> EquipmentExpenses { get; set; } = new List<EquipmentExpense>();
        public virtual ICollection<OtherExpense> OtherExpenses { get; set; } = new List<OtherExpense>();
        public virtual ICollection<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();
    }
}