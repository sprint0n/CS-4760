using System.ComponentModel.DataAnnotations; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

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
        private readonly University_Grant_Application_SystemContext _context;

        public ApplicationPageModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required]
        [Display(Name = "Index number")]
        public int IndexNumber { get; set; }
        
        //This is for the dropdown menu for Primaryuser || Type of user
        [BindProperty]
        [Required]
        [Display(Name = "User")]
        public string TypeOfUser { get; set; }

        //This is the user's department
        [BindProperty]
        public string Department { get; set; }
        
        public List<string> AllUsers { get; set; }

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
        [Required]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter in the title")]
        public string GrantTitle { get; set; }


        [BindProperty]
        [Required]
        [Display(Name = "Grant Purpose")]
        public string GrantPurpose { get; set; }


        [BindProperty]
        public bool HasPastFunding { get; set; }


        [BindProperty]
        [Required]
        public string DissemenationBudget { get; set; }

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

        [BindProperty]
        public IFormFile UploadFile { get; set; }

        [BindProperty]
        public string? PastBudget { get; set; }

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

        public async Task<IActionResult> OnGetAsync()
        {
            // 1️⃣ Prefill user info from the database
            var userEmail = User.Identity?.Name;

            if (userEmail != null)
            {
                var currentUser = await _context.Users
              .Include(u => u.Department)
              .FirstOrDefaultAsync(u => u.Email == userEmail);

                if (currentUser != null)
                {
                    Name = $"{currentUser.FirstName} {currentUser.LastName}";
                    IndexNumber = currentUser.AccountID;

                    Department = currentUser.Department?.DepartmentName ?? " No assigned department";

     
                }
            }

            

            AllUsers = await _context.Users
            .Select(u => u.FirstName + " " + u.LastName)
            .ToListAsync();


            // 2️⃣ Preload RSPG as the main application funding
            IncomeSources.Add(new IncomeSource
            {
                SourceName = "RSPG",
                Amount = 0
            });
            return Page();


     

        }

        private async Task<string?> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            // Ensure upload folder exists
            var uploadFolder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadFolder);

            // Create unique filename with original extension
            var extension = Path.GetExtension(file.FileName);
            var uniqueName = $"{UploadFile.FileName}_{Guid.NewGuid()}";

            var filePath = Path.Combine(uploadFolder, uniqueName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueName; // return stored filename for DB
        }

        public async Task<IActionResult> OnPost()
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

            // Save all uploaded files
            var irbFileName = await SaveFileAsync(UploadFile); 
            var requiredDocFileName = await SaveFileAsync(RequiredDocument); 
            var optional1FileName = await SaveFileAsync(OptionalDocument1); 
            var optional2FileName = await SaveFileAsync(OptionalDocument2);

            // TODO: Save the application data to the database here
            // var application = new Application { ... };
            // _context.Applications.Add(application);
            // await _context.SaveChangesAsync();
            // TODO: calculate and display taxes
            // TODO: process/save uploaded files (RequiredDocument, OptionalDocument1, OptionalDocument2)

            return Content("Success! Your application has been submitted.");
        }

    }
}