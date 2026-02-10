using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
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

            total += form.PersonnelExpenses.Sum(p => p.Amount ?? 0);
            total += form.EquipmentExpenses.Sum(e => e.Amount ?? 0);
            total += form.TravelExpenses.Sum(t => t.Amount ?? 0);
            total += form.OtherExpenses.Sum(o => o.Amount ?? 0);

            return total;
        }

        private readonly University_Grant_Application_SystemContext _context;

        public ApplicationPageModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        // User info and dropdowns
        [BindProperty]
        public int IndexNumber { get; set; }

        [BindProperty]
        public string? TypeOfUser { get; set; }

        [BindProperty]
        public string? Department { get; set; }

        public List<string> AllUsers { get; set; } = new List<string>();

        public List<string> UserTypes { get; } = new List<string>
{
    "PrimaryUser", "Student", "Faculty", "Staff", "External Researcher"
};

        // Core application fields
        [BindProperty]
        public string? Procedure { get; set; }

        [BindProperty]
        [Display(Name = "Primary Investigator")]
        public string? PrimaryInvestigator { get; set; }

        [BindProperty]
        [Display(Name = "Grant Title")]
        public string? GrantTitle { get; set; }

        [BindProperty]
        [Display(Name = "Grant Purpose")]
        public string? GrantPurpose { get; set; }

        [BindProperty]
        public string? Name { get; set; }

        [BindProperty]
        [Display(Name = "Past Budget")]
        public string? PastBudget { get; set; }

        [BindProperty]
        [Display(Name = "Dissemination Budget")]
        public string? DisseminationBudget { get; set; }

        [BindProperty]
        [Display(Name = "Timeline")]
        public string? Timeline { get; set; }

        [BindProperty]
        public bool? HasPastFunding { get; set; }

        [BindProperty]
        public bool? HumansOrAnimals { get; set; }

        // File uploads
        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        [BindProperty]
        [Display(Name = "Required supporting document")]
        public IFormFile? RequiredDocument { get; set; }

        [BindProperty]
        [Display(Name = "Optional document 1")]
        public IFormFile? OptionalDocument1 { get; set; }

        [BindProperty]
        [Display(Name = "Optional document 2")]
        public IFormFile? OptionalDocument2 { get; set; }

        // Expenses and income
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

        [BindProperty]
        public int? DraftId { get; set; }  // Add this property to bind hidden draft ID from the form

        public async Task<IActionResult> OnGetAsync(int? draftId)
        {
            // -----------------------------
            // Resolve current user
            // -----------------------------
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(userEmail))
                return Page();

            var currentUser = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser == null)
                return Page();

            // -----------------------------
            // Populate user info
            // -----------------------------
            Name = $"{currentUser.FirstName} {currentUser.LastName}";
            IndexNumber = currentUser.AccountID;
            Department = currentUser.Department?.DepartmentName ?? "No assigned department";

            await PopulateSelectListsAsync();

            // -----------------------------
            // Load draft if requested
            // -----------------------------
            if (draftId.HasValue)
            {
                var draft = await _context.FormTable
                    .Include(f => f.PersonnelExpenses)
                    .Include(f => f.EquipmentExpenses)
                    .Include(f => f.TravelExpenses)
                    .Include(f => f.OtherExpenses)
                    .FirstOrDefaultAsync(f =>
                        f.Id == draftId.Value &&
                        f.UserId == currentUser.UserId &&
                        f.ApplicationStatus == "Saved");

                if (draft != null)
                {
                    GrantTitle = draft.Title;
                    Procedure = draft.Procedure;
                    GrantPurpose = draft.GrantPurpose;
                    PastBudget = draft.pastBudgets;
                    HasPastFunding = draft.pastFunding;
                    Timeline = draft.Timeline;
                    HumansOrAnimals = draft.isIRB;

                    PrimaryInvestigator = await _context.Users
                        .Where(u => u.UserId == draft.PrincipalInvestigatorID)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefaultAsync();

                    PersonnelExpenses = draft.PersonnelExpenses.ToList();
                    EquipmentExpenses = draft.EquipmentExpenses.ToList();
                    TravelExpenses = draft.TravelExpenses.ToList();
                    OtherExpenses = draft.OtherExpenses.ToList();

                    // ===============================
                    // Add placeholder rows if empty
                    // ===============================
                    if (!PersonnelExpenses.Any()) PersonnelExpenses.Add(new PersonnelExpense());
                    if (!EquipmentExpenses.Any()) EquipmentExpenses.Add(new EquipmentExpense());
                    if (!TravelExpenses.Any()) TravelExpenses.Add(new TravelExpense());
                    if (!OtherExpenses.Any()) OtherExpenses.Add(new OtherExpense());

                    // Populate IncomeSources
                    IncomeSources = new List<IncomeSource>
                    {
                        new IncomeSource { SourceName = draft.OtherFunding1Name, Amount = (decimal)(draft.OtherFunding1Amount ?? 0) },
                        new IncomeSource { SourceName = draft.OtherFunding2Name, Amount = (decimal)(draft.OtherFunding2Amount ?? 0) },
                        new IncomeSource { SourceName = draft.OtherFunding3Name, Amount = (decimal)(draft.OtherFunding3Amount ?? 0) },
                        new IncomeSource { SourceName = draft.OtherFunding4Name, Amount = (decimal)(draft.OtherFunding4Amount ?? 0) },
                    }.Where(f => !string.IsNullOrWhiteSpace(f.SourceName) || f.Amount > 0).ToList();

                    return Page();
                }
            }

            // -----------------------------
            // Default initial state (new application)
            // -----------------------------
            IncomeSources.Add(new IncomeSource
            {
                SourceName = "RSPG",
                Amount = 0
            });

            // ===============================
            // Add placeholder rows for expenses
            // ===============================
            if (!PersonnelExpenses.Any()) PersonnelExpenses.Add(new PersonnelExpense());
            if (!EquipmentExpenses.Any()) EquipmentExpenses.Add(new EquipmentExpense());
            if (!TravelExpenses.Any()) TravelExpenses.Add(new TravelExpense());
            if (!OtherExpenses.Any()) OtherExpenses.Add(new OtherExpense());

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
            bool isSubmit = action == "Submit";

            // -----------------------------
            // Conditional validation for Submit
            // -----------------------------
            if (string.IsNullOrWhiteSpace(GrantTitle))
                ModelState.AddModelError(nameof(GrantTitle), "Grant Title is required.");

            if (isSubmit)
            {
                if (string.IsNullOrWhiteSpace(Procedure))
                    ModelState.AddModelError(nameof(Procedure), "Procedure is required.");

                if (string.IsNullOrWhiteSpace(GrantPurpose))
                    ModelState.AddModelError(nameof(GrantPurpose), "Grant Purpose is required.");

                if (string.IsNullOrWhiteSpace(DisseminationBudget))
                    ModelState.AddModelError(nameof(DisseminationBudget), "Dissemination Budget is required.");

                if (string.IsNullOrWhiteSpace(Timeline))
                    ModelState.AddModelError(nameof(Timeline), "Timeline is required.");

                if (string.IsNullOrWhiteSpace(PrimaryInvestigator))
                    ModelState.AddModelError(nameof(PrimaryInvestigator), "Primary Investigator is required.");

                if (RequiredDocument == null || RequiredDocument.Length == 0)
                    ModelState.AddModelError(nameof(RequiredDocument), "Required supporting document must be uploaded.");

                if (IncomeSources.Count > 4)
                    ModelState.AddModelError("", "You may enter a maximum of four income sources.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync();
                return Page();
            }

            // -----------------------------
            // Handle file uploads
            // -----------------------------
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

            // -----------------------------
            // Resolve current user
            // -----------------------------
            var userEmail = User.Identity?.Name;
            var currentUser = userEmail != null
                ? await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail)
                : null;
            var userId = currentUser?.UserId ?? 1;

            // -----------------------------
            // Load or create FormTable entry
            // -----------------------------
            FormTable formEntry;

            if (DraftId.HasValue)
            {
                // Editing an existing draft
                formEntry = await _context.FormTable
                    .Include(f => f.PersonnelExpenses)
                    .Include(f => f.EquipmentExpenses)
                    .Include(f => f.TravelExpenses)
                    .Include(f => f.OtherExpenses)
                    .FirstOrDefaultAsync(f => f.Id == DraftId.Value && f.UserId == userId);

                if (formEntry == null)
                {
                    // Fallback if the draft was deleted
                    formEntry = new FormTable { UserId = userId };
                    _context.FormTable.Add(formEntry);
                }
                else
                {
                    // Clear old expenses so we can replace them
                    formEntry.PersonnelExpenses.Clear();
                    formEntry.EquipmentExpenses.Clear();
                    formEntry.TravelExpenses.Clear();
                    formEntry.OtherExpenses.Clear();
                }
            }
            else
            {
                // New application
                formEntry = new FormTable { UserId = userId };
                _context.FormTable.Add(formEntry);
            }

            // -----------------------------
            // Populate core fields
            // -----------------------------
            formEntry.ApplicationStatus = isSubmit ? "Pending" : "Saved";
            formEntry.Title = GrantTitle;
            formEntry.Procedure = Procedure;
            formEntry.Timeline = Timeline;
            formEntry.GrantPurpose = GrantPurpose;
            formEntry.pastBudgets = PastBudget ?? string.Empty;
            formEntry.pastFunding = HasPastFunding ?? false;
            formEntry.isIRB = HumansOrAnimals ?? false;
            formEntry.Description = $"{GrantPurpose} | Inv: {PrimaryInvestigator} | Time: {Timeline}";
            formEntry.PrincipalInvestigatorID = _context.Users
                .Where(u => (u.FirstName + " " + u.LastName) == PrimaryInvestigator)
                .Select(u => u.UserId)
                .FirstOrDefault();

            // -----------------------------
            // Add expenses
            // -----------------------------
            foreach (var p in PersonnelExpenses.Where(p =>
                !string.IsNullOrWhiteSpace(p.Role) &&
                !string.IsNullOrWhiteSpace(p.Description) &&
                p.Amount.HasValue))
            {
                var rate = (p.Role == "Student") ? 0.0825m : 0.225m;
                p.TaxedAmount = Math.Round(p.Amount.Value * rate, 2);
                formEntry.PersonnelExpenses.Add(p);
            }

            formEntry.EquipmentExpenses.AddRange(
                EquipmentExpenses.Where(e =>
                    !string.IsNullOrWhiteSpace(e.EquipmentName) &&
                    e.Amount.HasValue
                )
            );

            formEntry.TravelExpenses.AddRange(
                TravelExpenses.Where(t =>
                    !string.IsNullOrWhiteSpace(t.TravelName) &&
                    t.Amount.HasValue
                )
            );

            formEntry.OtherExpenses.AddRange(
                OtherExpenses.Where(o =>
                    !string.IsNullOrWhiteSpace(o.OtherExpenseName) &&
                    o.Amount.HasValue
                )
            );

            // -----------------------------
            // Calculate total budget
            // -----------------------------
            formEntry.TotalBudget = CalculateTotalBudget(formEntry);

            // === INCOME SOURCES: max 4, pad if necessary ===
            var funding = IncomeSources
                .Where(f => !string.IsNullOrWhiteSpace(f.SourceName)) // ignore empty rows
                .Take(4)
                .Concat(Enumerable.Repeat(new IncomeSource(), 4))
                .Take(4)
                .ToList();

            formEntry.OtherFunding1Name = funding[0].SourceName ?? string.Empty;
            formEntry.OtherFunding1Amount = (float?)funding[0].Amount;

            formEntry.OtherFunding2Name = funding[1].SourceName ?? string.Empty;
            formEntry.OtherFunding2Amount = (float?)funding[1].Amount;

            formEntry.OtherFunding3Name = funding[2].SourceName ?? string.Empty;
            formEntry.OtherFunding3Amount = (float?)funding[2].Amount;

            formEntry.OtherFunding4Name = funding[3].SourceName ?? string.Empty;
            formEntry.OtherFunding4Amount = (float?)funding[3].Amount;

            // -----------------------------
            // Persist to database
            // -----------------------------
            await _context.SaveChangesAsync();

            TempData["Message"] = isSubmit ? "Application submitted successfully!" : "Draft saved successfully!";

            // -----------------------------
            // Redirect based on role
            // -----------------------------
            if (currentUser != null)
            {
                if (currentUser.isAdmin)
                    return RedirectToPage("/AdminDashboard/Index");
                if (currentUser.committeeMemberStatus == "member" || currentUser.committeeMemberStatus == "chair")
                    return RedirectToPage("/CommitteeDashboard/CommitteeDashboard");
                if (currentUser.userType == "chair")
                    return RedirectToPage("/DeptChairDashboard/DeptChairDashboard");

                return RedirectToPage("/FacultyDashboard/Index");
            }

            return RedirectToPage("/Index");
        }
    }
}