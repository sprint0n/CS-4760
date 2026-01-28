using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Grant_Application_System.Models
{
    public class FormTable
    {
        [Key]
        public int Id { get; set; }

        public int PrincipalInvestigatorID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        public string GrantPurpose { get; set; }

 
        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Procedure { get; set; } = string.Empty;

        public bool isIRB { get; set; } = false;

        public Guid uploadedFile { get; set; }

        [Required]
        public string ApplicationStatus {  get; set; } = string.Empty;

        public float TotalBudget { get; set; }

        public float PersonalExpense {  get; set; }

        public float TravelExpense { get; set; }

        public float EquipmentExpense { get; set; }
        // Foreign Key
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}