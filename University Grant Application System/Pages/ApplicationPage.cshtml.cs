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
        private decimal CalculateTotalBudget(FormTable form)
        {
            decimal total = 0;

            total += form.PersonnelExpenses.Sum(p => p.Amount);
            total += form.EquipmentExpenses.Sum(e => e.Amount);
            total += form.TravelExpenses.Sum(t => t.Amount);
            total += form.OtherExpenses.Sum(o => o.Amount);

            return total;
        }

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

        //This is the user's department
        [BindProperty]
        public string Department { get; set; }
        public List<string> AllUsers { get; set; } = new List<string>();
        public List<string> UserTypes { get; } = new List<string>
        {
            "PrimaryUser", "Student", "Faculty", "Staff", "External Researcher"
        };

        [BindProperty]
        [Required]
        public string Procedure { get; set; }

        [BindProperty]
        [Display(Name = "Primary Investigator")]
        [Required(ErrorMessage = "Primary Investigator is required")]
        public string PrimaryInvestigator { get; set; }

        [BindProperty]
        [Display(Name = "Grant Title")]
        [Required(ErrorMessage = "Please enter in the title")]
        public string GrantTitle { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Grant Purpose")]
        public string GrantPurpose { get; set; }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        [Display(Name = "Past Budget")]
        public string? PastBudget { get; set; }


        [BindProperty]
        public bool HasPastFunding { get; set; }


        [BindProperty]
        [Required]
        [Display(Name = "Dissemenation Budget")]
        public string DissemenationBudget { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Timeline")]
        public string Timeline { get; set; }

        [BindProperty]
        public bool HumansOrAnimals { get; set; }

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

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

        [BindProperty]
        public List<PersonnelExpense> PersonnelExpenses { get; set; } = new();

        [BindProperty]
        public List<EquipmentExpense> EquipmentExpenses { get; set; } = new();

        [BindProperty]
        public List<TravelExpense> TravelExpenses { get; set; } = new();

        [BindProperty]
        public List<OtherExpense> OtherExpenses { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Prefill user info from the database
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

            await PopulateSelectListsAsync();

            IncomeSources.Add(new IncomeSource
            {
                SourceName = "RSPG",
                Amount = 0
            });

            return Page();
        }

        // Populate lists used by the view so they are available on GET and POST
        private async Task PopulateSelectListsAsync()
        {
            AllUsers = await _context.Users
                .Select(u => u.FirstName + " " + u.LastName)
                .ToListAsync();
        }

        private async Task<string?> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadFolder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadFolder);

            var extension = Path.GetExtension(file.FileName);
            var uniqueName = $"{file.FileName}_{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(uploadFolder, uniqueName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueName;
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            // Validation for required files
            if (action == "Submit" && (RequiredDocument == null || RequiredDocument.Length == 0))
            {
                ModelState.AddModelError(nameof(RequiredDocument), "The required supporting document must be uploaded.");
            }

            if (IncomeSources.Count > 4)
            {
                ModelState.AddModelError("", "You may enter a maximum of four income sources.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync();
                return Page();
            }

            // Optionally save uploaded file(s)
            if (UploadFile != null && UploadFile.Length > 0)
            {
                var uniqueName = await SaveFileAsync(UploadFile);
                TempData["UploadSuccess"] = $"Successfully uploaded: {UploadFile.FileName}";
            }

            if (RequiredDocument != null && RequiredDocument.Length > 0)
            {
                var uniqueName = await SaveFileAsync(RequiredDocument);
                TempData["UploadSuccess"] += $", Required document uploaded: {RequiredDocument.FileName}";
            }

            // Resolve current user
            var userEmail = User.Identity?.Name;
            var currentUser = userEmail != null
                ? await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail)
                : null;
            var userId = currentUser?.UserId ?? 1;

            // Determine application status based on which button was clicked
            var status = action == "Submit" ? "Pending" : "Saved";

            var formEntry = new FormTable
            {
                Title = GrantTitle,
                Procedure = Procedure,
                Timeline = Timeline,
                GrantPurpose = GrantPurpose,
                UserId = userId,
                ApplicationStatus = status, // <-- dynamically set
                isIRB = HumansOrAnimals,
                pastFunding = HasPastFunding,
                pastBudgets = PastBudget ?? string.Empty,
                Description = $"{GrantPurpose} | Inv: {PrimaryInvestigator} | Time: {Timeline}"
            };

            // Add expenses
            PersonnelExpenses.ForEach(p => formEntry.PersonnelExpenses.Add(p));
            EquipmentExpenses.ForEach(e => formEntry.EquipmentExpenses.Add(e));
            TravelExpenses.ForEach(t => formEntry.TravelExpenses.Add(t));
            OtherExpenses.ForEach(o => formEntry.OtherExpenses.Add(o));

            formEntry.TotalBudget = CalculateTotalBudget(formEntry);

            // Persist
            _context.FormTable.Add(formEntry);
            await _context.SaveChangesAsync();

            TempData["Message"] = action == "Submit" ? "Application submitted successfully!" : "Draft saved successfully!";

            // Redirect based on user role/claims
            if (currentUser != null) 
            { 
                if (currentUser.isAdmin) 
                { 
                    return RedirectToPage("/AdminDashboard/Index"); 
                } 
                if (currentUser.committeeMemberStatus == "member" || currentUser.committeeMemberStatus == "chair") 
                { 
                    return RedirectToPage("/CommitteeDashboard/CommitteeDashboard"); 
                } 
                if (currentUser.userType == "chair") 
                { 
                    return RedirectToPage("/DeptChairDashboard/DeptChairDashboard"); 
                } 
                // Default for faculty
                return RedirectToPage("/FacultyDashboard/Index"); 
            }

            // Fallback: go to login/index if user info is missing
            return RedirectToPage("/Index");
        }
    }
}