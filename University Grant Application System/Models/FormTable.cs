using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

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
        public string GrantPurpose { get; set; } = string.Empty;
        public float DisseminationBudget { get; set; }

        public string Timeline { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Procedure { get; set; } = string.Empty;

        public bool isIRB { get; set; } = false;


        public bool pastFunding { get; set; }

        public string pastBudgets { get; set; } = string.Empty;

        public virtual ICollection<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();



        public string OtherFunding1Name { get; set; } = string.Empty;
        public float OtherFunding1Amount { get; set; }

        public string OtherFunding2Name { get;set; } = string.Empty;
        public float OtherFunding2Amount { get; set; }

        public string OtherFunding3Name { get; set; } = string.Empty;
        public float OtherFunding3Amount { get; set; }

        public string OtherFunding4Name { get; set; } = string.Empty;

        public float OtherFunding4Amount { get; set; }

        [Required] 

        public string ApplicationStatus {  get; set; } = string.Empty;

        public float TotalBudget { get; set; }

        public virtual ICollection<PersonnelExpense> PersonnelExpenses { get; set; } = new List<PersonnelExpense>();


        public virtual ICollection<TravelExpense> TravelExpenses { get; set; } = new List<TravelExpense>();

        public virtual ICollection<EquipmentExpense> EquipmentExpenses { get; set; } = new List<EquipmentExpense>();

        public virtual ICollection<OtherExpense> OtherExpenses { get; set; } = new List<OtherExpense>();
     

        // Foreign Key
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}