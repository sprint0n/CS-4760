using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages
{
    public class ReportPageModel : PageModel
    {
        private readonly University_Grant_Application_SystemContext _context;

        public ReportPageModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int GrantId { get; set; }

        public string GrantTitle { get; set; } = "";
        public string GrantType { get; set; } = "";
        public decimal GrantAmount { get; set; }
        public string ApplicationStatus { get; set; } = "";
        public string PrincipalInvestigator { get; set; } = "";
        public string Description { get; set; } = "";
        public string Timeline { get; set; } = "";
        public bool IsClosed { get; set; }

        [BindProperty]
        public string ReportSummary { get; set; } = "";

        [BindProperty]
        public string ReportOutcomes { get; set; } = "";

        [BindProperty]
        public string ReportExpenditures { get; set; } = "";

        [BindProperty]
        public IFormFile? ReportFile1 { get; set; }

        [BindProperty]
        public IFormFile? ReportFile2 { get; set; }

        public UploadedFile? ExistingReportDoc1 { get; set; }
        public UploadedFile? ExistingReportDoc2 { get; set; }

        public string DashboardPage { get; set; } = "/FacultyDashboard/Index";

        public async Task<IActionResult> OnGetAsync()
        {
            var userEmail = User.Identity?.Name;
            if (userEmail == null) return RedirectToPage("/Index");

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (currentUser == null) return RedirectToPage("/Index");

            DashboardPage = GetDashboardPage(currentUser);

            var grant = await _context.FormTable
                .Include(f => f.User)
                .Include(f => f.UploadedFiles)
                .FirstOrDefaultAsync(f => f.Id == GrantId && f.UserId == currentUser.UserId);

            if (grant == null || (grant.ApplicationStatus != "Approved" && grant.ApplicationStatus != "Closed"))
            {
                return RedirectToPage(DashboardPage);
            }

            PopulateFromGrant(grant);
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitReportAsync()
        {
            var userEmail = User.Identity?.Name;
            if (userEmail == null) return RedirectToPage("/Index");

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (currentUser == null) return RedirectToPage("/Index");

            var dashboardPage = GetDashboardPage(currentUser);

            var grant = await _context.FormTable
                .Include(f => f.User)
                .Include(f => f.UploadedFiles)
                .FirstOrDefaultAsync(f => f.Id == GrantId && f.UserId == currentUser.UserId);

            if (grant == null || grant.ApplicationStatus != "Approved")
            {
                return RedirectToPage(dashboardPage);
            }

            // Handle file uploads
            if (ReportFile1 != null && ReportFile1.Length > 0)
            {
                var storedName = await SaveFileAsync(ReportFile1);
                if (storedName != null)
                {
                    RecordFileUpload(grant.Id, ReportFile1.FileName, storedName,
                        AttachmentType.ReportDoc1, ReportFile1.ContentType, ReportFile1.Length);
                }
            }

            if (ReportFile2 != null && ReportFile2.Length > 0)
            {
                var storedName = await SaveFileAsync(ReportFile2);
                if (storedName != null)
                {
                    RecordFileUpload(grant.Id, ReportFile2.FileName, storedName,
                        AttachmentType.ReportDoc2, ReportFile2.ContentType, ReportFile2.Length);
                }
            }

            // Move the grant to Closed status
            grant.ApplicationStatus = "Closed";
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Report for '{grant.Title}' submitted successfully. Grant is now closed.";
            return RedirectToPage(dashboardPage);
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
            var existingFile = _context.UploadedFiles
                .FirstOrDefault(f => f.FormTableId == formId && f.Category == category);

            if (existingFile != null)
            {
                var filePath = Path.Combine("wwwroot", "uploads", existingFile.StoredFileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.UploadedFiles.Remove(existingFile);
            }

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

        private string GetDashboardPage(Models.User user)
        {
            if (user.isAdmin)
                return "/AdminDashboard/Index";
            if (user.committeeMemberStatus == "chair")
                return "/CommitteeDashboard/CommitteeDashboard";
            if (user.committeeMemberStatus == "member")
                return "/ComMemberDashboard/ComMemberDashboard";
            if (user.userType == "chair")
                return "/DeptChairDashboard/DeptChairDashboard";

            return "/FacultyDashboard/Index";
        }

        private void PopulateFromGrant(Models.FormTable grant)
        {
            GrantTitle = grant.Title;
            GrantType = grant.grantTypeSelection ?? "N/A";
            GrantAmount = grant.TotalBudget ?? 0m;
            ApplicationStatus = grant.ApplicationStatus;
            Description = grant.Description ?? "";
            Timeline = grant.Timeline ?? "";
            IsClosed = grant.ApplicationStatus == "Closed";

            ExistingReportDoc1 = grant.UploadedFiles
                .FirstOrDefault(f => f.Category == AttachmentType.ReportDoc1);
            ExistingReportDoc2 = grant.UploadedFiles
                .FirstOrDefault(f => f.Category == AttachmentType.ReportDoc2);

            if (grant.User != null)
            {
                PrincipalInvestigator = $"{grant.User.FirstName} {grant.User.LastName}";
            }
        }
    }
}
