using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;

namespace University_Grant_Application_System.Pages.DeptChairDashboard
{
    public class IndexModel : PageModel
    {
        private readonly University_Grant_Application_SystemContext _context;
        public IndexModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        public DateTime ApplicationDueDate { get; set; }
        public List<ApplicationCard> SavedApplications { get; set; } = new();
        public List<ApplicationCard> InReviewApplications { get; set; } = new();
        public List<ApprovedGrant> ApprovedGrants { get; set; } = new();

        public List<ApplicationCard> ApplicationToReview { get; set; } = new();
        public async Task OnGetAsync()
        {
            // Get the logged-in user's email
            var userEmail = User.Identity?.Name;

            if (userEmail == null)
            {
                // No logged-in user, redirect or just return empty lists
                SavedApplications = new List<ApplicationCard>();
                InReviewApplications = new List<ApplicationCard>();
                ApprovedGrants = new List<ApprovedGrant>();
                return;
            }

            // Get current user's ID from the database
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser == null)
            {
                // User not found
                SavedApplications = new List<ApplicationCard>();
                InReviewApplications = new List<ApplicationCard>();
                ApprovedGrants = new List<ApprovedGrant>();
                return;
            }

            var userId = currentUser.UserId;
            var deptId = currentUser.DepartmentId;

            // Retrieve saved drafts
            SavedApplications = await _context.FormTable
                .Where(f => f.UserId == userId && f.ApplicationStatus == "Saved")
                .Select(f => new ApplicationCard
                {
                    ApplicationId = f.Id,
                    Title = f.Title,
                    Status = f.ApplicationStatus,
                    PrimaryInvestigator = _context.Users
                        .Where(u => u.UserId == f.PrincipalInvestigatorID)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "N/A"
                })
                .ToListAsync();

            // Retrieve submitted / in-review applications
            InReviewApplications = await _context.FormTable
                .Where(f => f.UserId == userId && (f.ApplicationStatus == "PendingDeptChair" || f.ApplicationStatus == "PendingCommittee"))
                .Select(f => new ApplicationCard
                {
                    Title = f.Title,
                    Status = f.ApplicationStatus
                })
                .ToListAsync();

            // Optional: Approved grants (if you have a separate table or flag)
            ApprovedGrants = await _context.FormTable
                .Where(f => f.UserId == userId && f.ApplicationStatus == "Approved")
                .Select(f => new ApprovedGrant
                {
                    Title = f.Title,
                    Amount = f.TotalBudget ?? 0m // or your approved amount field
                })
                .ToListAsync();

            ApplicationToReview = await _context.FormTable
                 .Include(f => f.User) // Include the applicant user data
                 .Where(f => f.ApplicationStatus == "PendingDeptChair" &&
                             f.User.DepartmentId == deptId)
                 .Select(f => new ApplicationCard
                 {
                     ApplicationId = f.Id,
                     Title = f.Title,
                     Status = "Pending Review",
                     PrimaryInvestigator = _context.Users
                        .Where(u => u.UserId == f.PrincipalInvestigatorID)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "N/A"
                 })
                 .ToListAsync();
             // Example placeholder for due date
             ApplicationDueDate = DateTime.Today.AddDays(14);
        }
    }

    public class ApplicationCard
    {
        public int ApplicationId { get; set; }
        public string Title { get; set; } = "";
        public string Status { get; set; } = "";

        public string PrimaryInvestigator {  get; set; } = " ";
        public decimal Amount { get; set; } = 0;
    }

    public class ApprovedGrant
    {
        public int ApplicationId { get; set; }

        public string Title { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
