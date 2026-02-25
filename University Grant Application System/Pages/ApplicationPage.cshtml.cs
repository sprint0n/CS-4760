using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Packaging;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private decimal CalculateTotalBudget(FormTable form)
        {
            decimal total = 0;

            // Updated to include the new OtherAmount fields in the total budget calculation
            total += form.PersonnelExpenses.Sum(p => (p.Amount ?? 0) + (p.OtherAmount1 ?? 0) + (p.OtherAmount2 ?? 0));
            total += form.EquipmentExpenses.Sum(e => (e.RSPGAmount ?? 0) + (e.OtherAmount1 ?? 0) + (e.OtherAmount2 ?? 0));
            total += form.TravelExpenses.Sum(t => (t.RSPGAmount ?? 0) + (t.OtherAmount1 ?? 0) + (t.OtherAmount2 ?? 0));
            total += form.OtherExpenses.Sum(o => (o.Amount ?? 0) + (o.OtherAmount1 ?? 0) + (o.OtherAmount2 ?? 0));

            return total;
        }

        private readonly University_Grant_Application_SystemContext _context;

        public ApplicationPageModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

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
        [Display(Name = "Select Grant Type")]
        public string SelectedGrantTypeOption { get; set; }
     
        public List<SelectListItem> GrantTypeOptions { get; set; }

        [BindProperty]
        [Display(Name = "Select Staff Type")]
        public string SelectedStaffTypeOption { get; set; }
        public List<SelectListItem> StaffTypeOptions { get; set; }

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
        public int? DraftId { get; set; }

        [BindProperty]
        public bool HasExistingRequiredDoc { get; set; }

        [BindProperty]
        public bool HasExistingIRB { get; set; }

        public List<UploadedFile> ExistingFiles { get; set; } = new();

        private List<SelectListItem> GetSortedGrantTypeOptions()
        {
            var allOptions = new List<(int Id, string Text)>
            {
                (1, "Hemingway Adjunct Faculty Grant - Spring Semester"),
                (2, "Hemingway Collaborative Award - Spring Semester"),
                (3, "Hemingway Excellence Award - Spring Semester"),
                (4, "Hemingway New Faculty Grant - Spring Semester"),
                (5, "Hemingway Faculty Vitality Grant - Fall and Spring"),
                (6, "Creative Works Grant - Fall and Spring"),
                (7, "Research Grant - Fall and Spring"),
                (8, "Travel Grant - Fall, Spring, and Summer")
            };

            int currentMonth = DateTime.Now.Month;


            return allOptions.OrderByDescending(opt =>
            {
                if (currentMonth >= 8 || currentMonth <= 1) 
                {
                    if (opt.Text.Contains("Fall")) return 2;
                    if (opt.Text.Contains("Spring")) return 0;
                }
                else if (currentMonth >= 2 && currentMonth <= 7) 
                {
                    if (opt.Text.Contains("Spring")) return 2;
                    if (opt.Text.Contains("Fall")) return 0;
                }
                return 1; 
            })
            .ThenBy(opt => opt.Id) 
            .Select(opt => new SelectListItem
            {
                Value = opt.Id.ToString(),
                Text = opt.Text
            })
            .ToList();
        }

        public async Task<IActionResult> OnGetAsync(int? draftId)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail)) return Page();

            var currentUser = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser == null) return Page();

            Name = $"{currentUser.FirstName} {currentUser.LastName}";
            IndexNumber = currentUser.AccountID;
            Department = currentUser.Department?.DepartmentName ?? "No assigned department";

            await PopulateSelectListsAsync();

            GrantTypeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Hemingway adjunct faculty grant spring semester" },
                new SelectListItem { Value = "2", Text = "Hemingway collaborative award spring semester" },
                new SelectListItem { Value = "3", Text = "Hemingway excellence award spring semester" },
                new SelectListItem { Value = "4", Text = "Hemingway new faculty grant spring semester" },
                new SelectListItem { Value = "5", Text = "Hemingway faculty vitality grant fall and spring" },
                new SelectListItem { Value = "6", Text = "Creative works grant fall and spring" },
                new SelectListItem { Value = "7", Text = "Research grant fall and spring" },
                new SelectListItem { Value = "8", Text = "Travel grant fall spring and summer" }
            };
            StaffTypeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Contracted faculty" },
                new SelectListItem { Value = "2", Text = "Instructure / tenure" },
                new SelectListItem { Value = "3", Text = "Tenure track new faculty(first two years of tenure track)" },
                new SelectListItem { Value = "4", Text = "Adjunct faculty" }
            };

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
                    DraftId = draft.Id;
                    GrantTitle = draft.Title;
                    Procedure = draft.Procedure;
                    GrantPurpose = draft.GrantPurpose;
                    PastBudget = draft.pastBudgets;
                    HasPastFunding = draft.pastFunding;
                    Timeline = draft.Timeline;
                    DisseminationBudget = draft.DisseminationBudget?.ToString();
                    HumansOrAnimals = draft.isIRB;

                    PrimaryInvestigator = await _context.Users
                        .Where(u => u.UserId == draft.PrincipalInvestigatorID)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefaultAsync();

                    ExistingFiles = await _context.UploadedFiles
                       .Where(u => u.FormTableId == draft.Id)
                       .ToListAsync();

                    PersonnelExpenses = draft.PersonnelExpenses.ToList();
                    EquipmentExpenses = draft.EquipmentExpenses.ToList();
                    TravelExpenses = draft.TravelExpenses.ToList();
                    OtherExpenses = draft.OtherExpenses.ToList();

                    if (!PersonnelExpenses.Any()) PersonnelExpenses.Add(new PersonnelExpense());
                    if (!EquipmentExpenses.Any()) EquipmentExpenses.Add(new EquipmentExpense());
                    if (!TravelExpenses.Any()) TravelExpenses.Add(new TravelExpense());
                    if (!OtherExpenses.Any()) OtherExpenses.Add(new OtherExpense());

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

            IncomeSources.Add(new IncomeSource { SourceName = "RSPG", Amount = 0 });
            if (!PersonnelExpenses.Any()) PersonnelExpenses.Add(new PersonnelExpense());
            if (!EquipmentExpenses.Any()) EquipmentExpenses.Add(new EquipmentExpense());
            if (!TravelExpenses.Any()) TravelExpenses.Add(new TravelExpense());
            if (!OtherExpenses.Any()) OtherExpenses.Add(new OtherExpense());

            return Page();
        }

        private async Task PopulateSelectListsAsync()
        {
            AllUsers = await _context.Users
                .Select(u => u.FirstName + " " + u.LastName)
                .ToListAsync();
        }

        private async Task<string?> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;
            var uploadFolder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadFolder);
            var extension = Path.GetExtension(file.FileName);
            var uniqueName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadFolder, uniqueName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }
            return uniqueName;
        }

        private void RecordFileUpload(int formId, string originalName, string uniqueName, AttachmentType category, string contentType, long size)
        {
            var upload = new UploadedFile
            {
                ID = Guid.NewGuid(),
                FileName = originalName,
                StoredFileName = uniqueName,
                Category = category,
                ContentType = contentType,
                FileSize = size,
                FormTableId = formId,
                UploadedAt = DateTime.UtcNow,
            };
            _context.UploadedFiles.Add(upload);
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            bool isSubmit = action == "Submit";

            if (HasExistingRequiredDoc) ModelState.Remove(nameof(RequiredDocument));
            if (HasExistingIRB && HumansOrAnimals == true) ModelState.Remove(nameof(UploadFile));

            if (string.IsNullOrWhiteSpace(GrantTitle))
                ModelState.AddModelError(nameof(GrantTitle), "Grant Title is required.");

            if (isSubmit)
            {
                if (string.IsNullOrWhiteSpace(Procedure)) ModelState.AddModelError(nameof(Procedure), "Procedure is required.");
                if (string.IsNullOrWhiteSpace(GrantPurpose)) ModelState.AddModelError(nameof(GrantPurpose), "Grant Purpose is required.");
                if (string.IsNullOrWhiteSpace(DisseminationBudget)) ModelState.AddModelError(nameof(DisseminationBudget), "Dissemination Budget is required.");
                if (string.IsNullOrWhiteSpace(Timeline)) ModelState.AddModelError(nameof(Timeline), "Timeline is required.");
                if (string.IsNullOrWhiteSpace(PrimaryInvestigator)) ModelState.AddModelError(nameof(PrimaryInvestigator), "Primary Investigator is required.");
                if (!HasExistingRequiredDoc && (RequiredDocument == null || RequiredDocument.Length == 0))
                    ModelState.AddModelError(nameof(RequiredDocument), "Required supporting document must be uploaded.");
                if (HumansOrAnimals == true && !HasExistingIRB && (UploadFile == null || UploadFile.Length == 0))
                    ModelState.AddModelError(nameof(UploadFile), "IRB Documentation is required.");
                if (IncomeSources.Count > 4)
                    ModelState.AddModelError("", "You may enter a maximum of four income sources.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync();
                return Page();
            }

            var userEmail = User.Identity?.Name;
            var currentUser = userEmail != null ? await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail) : null;
            var userId = currentUser?.UserId ?? 1;

            FormTable formEntry;
            if (DraftId.HasValue)
            {
                formEntry = await _context.FormTable
                    .Include(f => f.PersonnelExpenses)
                    .Include(f => f.EquipmentExpenses)
                    .Include(f => f.TravelExpenses)
                    .Include(f => f.OtherExpenses)
                    .FirstOrDefaultAsync(f => f.Id == DraftId.Value && f.UserId == userId);

                if (formEntry == null)
                {
                    formEntry = new FormTable { UserId = userId };
                    _context.FormTable.Add(formEntry);
                }
                else
                {
                    formEntry.PersonnelExpenses.Clear();
                    formEntry.EquipmentExpenses.Clear();
                    formEntry.TravelExpenses.Clear();
                    formEntry.OtherExpenses.Clear();
                }
            }
            else
            {
                formEntry = new FormTable { UserId = userId };
                _context.FormTable.Add(formEntry);
            }

            formEntry.ApplicationStatus = isSubmit ? "PendingDeptChair" : "Saved";
            formEntry.Title = GrantTitle;
            formEntry.Procedure = Procedure;
            formEntry.Timeline = Timeline;
            formEntry.GrantPurpose = GrantPurpose;
            if (decimal.TryParse(DisseminationBudget, out decimal parsedDissemination))
            {
                formEntry.DisseminationBudget = (float)parsedDissemination;
            }
            formEntry.pastBudgets = PastBudget ?? string.Empty;
            formEntry.pastFunding = HasPastFunding ?? false;
            formEntry.isIRB = HumansOrAnimals ?? false;
            formEntry.Description = $"{GrantPurpose} | Inv: {PrimaryInvestigator} | Time: {Timeline}";
            formEntry.PrincipalInvestigatorID = _context.Users
                .Where(u => (u.FirstName + " " + u.LastName) == PrimaryInvestigator)
                .Select(u => u.UserId)
                .FirstOrDefault();

            // ---------------------------------------------------------
            // STEP 2: REFINED BUDGET LOGIC (RSPG TOTAL -> GLOBAL TAX)
            // ---------------------------------------------------------

            // 1. Calculate Grand Total RSPG across all sub-tabs for tax calculation
            decimal grandTotalRspg = 0;
            grandTotalRspg += PersonnelExpenses.Sum(p => p.Amount ?? 0);
            grandTotalRspg += EquipmentExpenses.Sum(e => e.RSPGAmount ?? 0);
            grandTotalRspg += TravelExpenses.Sum(t => t.RSPGAmount ?? 0);
            grandTotalRspg += OtherExpenses.Sum(o => o.Amount ?? 0);

            // 2. Determine tax rate based on the role of the first Personnel entry (Default to Student)
            var primaryRole = PersonnelExpenses.FirstOrDefault()?.Role ?? "Student";
            decimal globalTaxRate = (primaryRole == "Teacher") ? 0.225m : 0.0825m;

            // 3. Save filtered Personnel Expenses (RSPG Tax remains in Personnel model if needed)
            foreach (var p in PersonnelExpenses.Where(p => !string.IsNullOrWhiteSpace(p.Role) || p.Amount > 0))
            {
                p.TaxedAmount = Math.Round((p.Amount ?? 0) * globalTaxRate, 2);
                formEntry.PersonnelExpenses.Add(p);
            }

            // 4. Save Equipment, Travel, and Other with new fields
            formEntry.EquipmentExpenses.AddRange(EquipmentExpenses.Where(e => !string.IsNullOrWhiteSpace(e.EquipmentName) || e.RSPGAmount > 0));
            formEntry.TravelExpenses.AddRange(TravelExpenses.Where(t => !string.IsNullOrWhiteSpace(t.TravelName) || t.RSPGAmount > 0));
            formEntry.OtherExpenses.AddRange(OtherExpenses.Where(o => !string.IsNullOrWhiteSpace(o.OtherExpenseName) || o.Amount > 0));

            // 5. Final Grand Total calculation for formEntry
            formEntry.TotalBudget = CalculateTotalBudget(formEntry);

            // Save Income Sources (Max 4)
            var funding = IncomeSources.Where(f => !string.IsNullOrWhiteSpace(f.SourceName)).Take(4).Concat(Enumerable.Repeat(new IncomeSource(), 4)).Take(4).ToList();
            formEntry.OtherFunding1Name = funding[0].SourceName ?? string.Empty;
            formEntry.OtherFunding1Amount = (float)funding[0].Amount;
            formEntry.OtherFunding2Name = funding[1].SourceName ?? string.Empty;
            formEntry.OtherFunding2Amount = (float)funding[1].Amount;
            formEntry.OtherFunding3Name = funding[2].SourceName ?? string.Empty;
            formEntry.OtherFunding3Amount = (float)funding[2].Amount;
            formEntry.OtherFunding4Name = funding[3].SourceName ?? string.Empty;
            formEntry.OtherFunding4Amount = (float)funding[3].Amount;

            await _context.SaveChangesAsync();

            // Handle file uploads recording
            if (RequiredDocument != null)
            {
                var reqName = await SaveFileAsync(RequiredDocument);
                if (reqName != null) RecordFileUpload(formEntry.Id, RequiredDocument.FileName, reqName, AttachmentType.SupportingDoc, RequiredDocument.ContentType, RequiredDocument.Length);
            }
            if (UploadFile != null)
            {
                var irbName = await SaveFileAsync(UploadFile);
                if (irbName != null) RecordFileUpload(formEntry.Id, UploadFile.FileName, irbName, AttachmentType.IRB, UploadFile.ContentType, UploadFile.Length);
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = isSubmit ? "Application submitted successfully!" : "Draft saved successfully!";

            if (currentUser != null)
            {
                if (currentUser.isAdmin) return RedirectToPage("/AdminDashboard/Index");
                if (currentUser.committeeMemberStatus == "member" || currentUser.committeeMemberStatus == "chair") return RedirectToPage("/CommitteeDashboard/CommitteeDashboard");
                if (currentUser.userType == "chair") return RedirectToPage("/DeptChairDashboard/DeptChairDashboard");
                return RedirectToPage("/FacultyDashboard/Index");
            }

            return RedirectToPage("/Index");
        }
    }
}