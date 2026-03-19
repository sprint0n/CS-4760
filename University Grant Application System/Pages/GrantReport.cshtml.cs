using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages
{
    public class GrantReportModel : PageModel
    {
        private readonly University_Grant_Application_SystemContext _context;
        private readonly IWebHostEnvironment _env;

        public GrantReportModel(University_Grant_Application_SystemContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Auto-populated fields (read-only display)
        public string GrantTitle { get; set; } = string.Empty;
        public string GrantType { get; set; } = string.Empty;
        public decimal GrantAmount { get; set; }
        public string OriginalDescription { get; set; } = string.Empty;
        public string OriginalFundingUse { get; set; } = string.Empty;

        public int FormTableId { get; set; }

        // Editable fields
        [BindProperty]
        public string NewDescription { get; set; } = string.Empty;

        [BindProperty]
        public string NewFundingUse { get; set; } = string.Empty;

        [BindProperty]
        public int GrantId { get; set; }

        [BindProperty]
        public IFormFile? ReportFile1 { get; set; }

        [BindProperty]
        public IFormFile? ReportFile2 { get; set; }

        // Existing report data (for re-display if already saved)
        public Report? ExistingReport { get; set; }

        public async Task<IActionResult> OnGetAsync(int grantId)
        {
            var userEmail = User.Identity?.Name;
            if (userEmail == null) return RedirectToPage("/FacultyDashboard/Index");

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (currentUser == null) return RedirectToPage("/FacultyDashboard/Index");

            var grant = await _context.FormTable
                .FirstOrDefaultAsync(f => f.Id == grantId && f.UserId == currentUser.UserId && f.ApplicationStatus == "Approved");

            if (grant == null) return RedirectToPage("/FacultyDashboard/Index");

            PopulateFromGrant(grant);

            // Check if a report already exists for this grant
            ExistingReport = await _context.Reports
                .Include(r => r.ReportFiles)
                .FirstOrDefaultAsync(r => r.FormTableId == grantId);

            if (ExistingReport != null)
            {
                NewDescription = ExistingReport.newDescription;
                NewFundingUse = ExistingReport.newFundingUse;
            }
            else
            {
                // Pre-fill with original grant data
                NewDescription = grant.Description ?? string.Empty;
                NewFundingUse = grant.GrantPurpose ?? string.Empty;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userEmail = User.Identity?.Name;
            if (userEmail == null) return RedirectToPage("/FacultyDashboard/Index");

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (currentUser == null) return RedirectToPage("/FacultyDashboard/Index");

            var grant = await _context.FormTable
                .FirstOrDefaultAsync(f => f.Id == GrantId && f.UserId == currentUser.UserId && f.ApplicationStatus == "Approved");

            if (grant == null) return RedirectToPage("/FacultyDashboard/Index");

            // Find or create the report
            var report = await _context.Reports
                .Include(r => r.ReportFiles)
                .FirstOrDefaultAsync(r => r.FormTableId == GrantId);

            if (report == null)
            {
                report = new Report
                {
                    FormTableId = GrantId,
                    newDescription = NewDescription,
                    newFundingUse = NewFundingUse
                };
                _context.Reports.Add(report);
                await _context.SaveChangesAsync();
            }
            else
            {
                report.newDescription = NewDescription;
                report.newFundingUse = NewFundingUse;
            }

            // Handle file uploads
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            await SaveReportFile(ReportFile1, report, grant.Id, uploadsFolder, "ReportFile1");
            await SaveReportFile(ReportFile2, report, grant.Id, uploadsFolder, "ReportFile2");

            // Move grant to Closed status
            grant.ApplicationStatus = "Closed";

            await _context.SaveChangesAsync();

            TempData["Message"] = "Grant report submitted successfully. Grant has been moved to Closed.";
            return RedirectToPage("/FacultyDashboard/Index");
        }

        private async Task SaveReportFile(IFormFile? file, Report report, int formTableId, string uploadsFolder, string label)
        {
            if (file == null || file.Length == 0) return;

            var originalName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(originalName);
            var storedName = $"{Path.GetFileNameWithoutExtension(originalName)}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, storedName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var uploadedFile = new UploadedFile
            {
                ID = Guid.NewGuid(),
                FileName = originalName,
                StoredFileName = storedName,
                Category = AttachmentType.SupportingDoc,
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow,
                FormTableId = formTableId,
                ReportId = report.ReportID
            };

            _context.UploadedFiles.Add(uploadedFile);
        }

        private void PopulateFromGrant(FormTable grant)
        {
            FormTableId = grant.Id;
            GrantId = grant.Id;
            GrantTitle = grant.Title;
            GrantType = grant.grantTypeSelection ?? "N/A";
            GrantAmount = grant.TotalBudget ?? 0m;
            OriginalDescription = grant.Description ?? string.Empty;
            OriginalFundingUse = grant.GrantPurpose ?? string.Empty;
        }
    }
}
