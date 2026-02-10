using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;

namespace University_Grant_Application_System.Pages.FacultyDashboard
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

            // Retrieve saved drafts
            SavedApplications = await _context.FormTable
                .Where(f => f.UserId == userId && f.ApplicationStatus == "Saved")
                .Select(f => new ApplicationCard
                {
                    Id = f.Id,
                    Title = f.Title,
                    Status = f.ApplicationStatus
                })
                .ToListAsync();

            // Retrieve submitted / in-review applications
            InReviewApplications = await _context.FormTable
                .Where(f => f.UserId == userId && f.ApplicationStatus == "Pending")
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

            // Example placeholder for due date
            ApplicationDueDate = DateTime.Today.AddDays(14);
        }

        public async Task<IActionResult> OnPostDeleteDraftAsync(int draftId)
        {
            var userEmail = User.Identity?.Name;
            if (userEmail == null)
                return RedirectToPage(); // or show error

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (currentUser == null)
                return RedirectToPage();

            var draft = await _context.FormTable
                .Include(f => f.PersonnelExpenses)
                .Include(f => f.EquipmentExpenses)
                .Include(f => f.TravelExpenses)
                .Include(f => f.OtherExpenses)
                .FirstOrDefaultAsync(f => f.Id == draftId && f.UserId == currentUser.UserId);

            if (draft != null)
            {
                // Remove child expenses first (if needed)
                _context.PersonnelExpenses.RemoveRange(draft.PersonnelExpenses);
                _context.EquipmentExpenses.RemoveRange(draft.EquipmentExpenses);
                _context.TravelExpenses.RemoveRange(draft.TravelExpenses);
                _context.OtherExpenses.RemoveRange(draft.OtherExpenses);

                // Remove the draft itself
                _context.FormTable.Remove(draft);

                await _context.SaveChangesAsync();
                TempData["Message"] = $"Draft '{draft.Title}' deleted successfully!";
            }

            return RedirectToPage(); // Refresh the page
        }
    }

    public class ApplicationCard
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Status { get; set; } = "";
    }

    public class ApprovedGrant
    {
        public string Title { get; set; } = "";
        public decimal Amount { get; set; }
    }

}