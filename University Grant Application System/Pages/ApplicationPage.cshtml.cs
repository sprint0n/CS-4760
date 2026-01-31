using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages
{
    // Helper class for the View (Income rows)
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
        private readonly University_Grant_Application_SystemContext _context;

        public ApplicationPageModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required]
        [Display(Name = "Index number")]
        public int IndexNumber { get; set; }

        // Dropdown menu for User Type
        [BindProperty]
        [Required]
        [Display(Name = "User")]
        public string TypeOfUser { get; set; }

        public List<string> UserTypes { get; } = new List<string>
        {
            "PrimaryUser", "Student", "Faculty", "Staff", "External Researcher"
        };

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

        [BindProperty]
        public bool HumansOrAnimals { get; set; }

        [BindProperty]
        public IFormFile UploadFile { get; set; }

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

        // YOUR BUDGET PROPERTY
        [BindProperty]
        public List<PersonnelExpense> PersonnelExpenses { get; set; } = new List<PersonnelExpense>();

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Prefill user info from the database
            var userEmail = User.Identity?.Name;

            if (userEmail != null)
            {
                var currentUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userEmail);

                if (currentUser != null)
                {
                    PrimaryInvestigator = $"{currentUser.FirstName} {currentUser.LastName}";
                    IndexNumber = currentUser.AccountID;
                }
            }

            // 2. Preload RSPG as the main application funding
            // (Moved inside the method to fix the red errors)
            IncomeSources.Add(new IncomeSource
            {
                SourceName = "RSPG",
                Amount = 0
            });

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // 1. Validation
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

            // 2. Handle File Upload
            if (UploadFile != null && UploadFile.Length > 0)
            {
                var uniqueName = $"{UploadFile.FileName}_{Guid.NewGuid()}";
                var uploadPath = Path.Combine("wwwroot/uploads", uniqueName);

                using (var stream = System.IO.File.Create(uploadPath))
                {
                    await UploadFile.CopyToAsync(stream);
                }

                TempData["UploadSuccess"] = $"Successfully uploaded: {UploadFile.FileName}";
            }

            // 3. Save to Database (Using FormTable)
            // We combine missing columns (Timeline, Inv) into Description so they are saved.
            var formEntry = new FormTable
            {
                Title = GrantTitle,          // Maps to FormTable.Title
                Procedure = Procedure,       // Maps to FormTable.Procedure
                UserId = 1,                  // Placeholder ID (Update this logic later if needed)
                Description = $"{GrantPurpose} | Inv: {PrimaryInvestigator} | Time: {Timeline}"
            };

            _context.FormTable.Add(formEntry);
            await _context.SaveChangesAsync(); // Generates the ID

            // 4. Save Personnel Expenses
            if (PersonnelExpenses != null && PersonnelExpenses.Count > 0)
            {
                foreach (var person in PersonnelExpenses)
                {
                    // Link the expense to the FormTable ID we just created
                    person.ApplicationId = formEntry.Id;
                    _context.PersonnelExpenses.Add(person);
                }
                await _context.SaveChangesAsync();
            }

            // Note: We are not saving IncomeSources to the DB yet because
            // 'IncomeSource' needs to be added to Database Context first.
            // For now, only the Budget (Personnel) is saving to the database.

            return Content("Success! Application and Budget saved.");
        }
    }
}