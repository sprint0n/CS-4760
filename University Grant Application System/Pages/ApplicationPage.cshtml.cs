using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace University_Grant_Application_System.Pages
{
    public class IncomeSource
    {
        [Required]
        public string SourceName { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        public decimal TaxAmount { get; set; }

    }
    public class ApplicationPageModel : PageModel
    {

        [BindProperty]
        [Required]
        [Display(Name = "Index number")]
        public string IndexNumber { get; set; }

        //This is for the dropdown menu for Primaryuser || Type of user

        [BindProperty]
        [Required]
        [Display(Name = "User")]
        public string TypeOfUser { get; set; }

        public List<string> UserTypes { get; } = new List<string>
        {
            "PrimaryUser",
            "Student",
            "Faculty",
            "Staff",
            "External Researcher"
        };

        //This is the name for the Procedure
        [BindProperty]
        [Required]
        [Display(Name = "Procedures")]
        public string Procedure { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Primary Investigator is required")]
        public string PrimaryInvestigator { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Please enter in the title")]
        public string GrantTitle { get; set; }


        [BindProperty]
        [Required]
        [Display(Name = "Grant Purpose")]
        public string GrantPurpose { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Timeline")]
        public string Timeline { get; set; }


        /// <summary>
        /// This is used for the Humans or Animals checkbox
        /// True opens file upload
        /// </summary>
        [BindProperty]
        public bool HumansOrAnimals { get; set; }

        // Supporting documents (one required, two optional)
        [BindProperty]
        [Display(Name = "Required supporting document")]
        public IFormFile RequiredDocument { get; set; }

        [BindProperty]
        [Display(Name = "Optional document 1")]
        public IFormFile? OptionalDocument1 { get; set; }

        [BindProperty]
        [Display(Name = "Optional document 2")]
        public IFormFile? OptionalDocument2 { get; set; }

        [BindProperty]
        public List<IncomeSource> IncomeSources { get; set; } = new();
        public void OnGet()
        {
            // Preload RSPG as the main application funding
            IncomeSources.Add(new IncomeSource
            {
                SourceName = "RSPG",
                Amount = 0
            });
        }

        public IActionResult OnPost()
        {
            // Ensure required file present
            if (RequiredDocument == null || RequiredDocument.Length == 0)
            {
                ModelState.AddModelError(nameof(RequiredDocument), "The required supporting document must be uploaded.");
            }

            if (IncomeSources.Count > 4)
            {
                ModelState.AddModelError("", "You may enter a maximum of four income sources.");
            }

            if (!ModelState.IsValid)
            {

                return Page();
            }

            // TODO: calculate and display taxes
            // TODO: process/save uploaded files (RequiredDocument, OptionalDocument1, OptionalDocument2)

            return Content("Success! Your application has been submitted.");
        }
    }
}