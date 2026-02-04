using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Pages.FacultyDashboard;

namespace University_Grant_Application_System.Pages
{
    public class CommitteeDashboardModel : PageModel
    {
        private readonly University_Grant_Application_SystemContext _context;

        public CommitteeDashboardModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }
        public List<Grant> InProgressApplications { get; set; } = new();
        public List<Grant> InReviewApplications { get; set; } = new();
        public void OnGet()
        {
            // Pull all "In Progress" applications
            InProgressApplications = _context.FormTable
                .Where(a => a.ApplicationStatus == "Pending")
                .Select(a => new Grant
                {
                    Title = a.Title,
                    Amount = a.TotalBudget
                })
                .ToList();
        }

        public class Grant
        {
            public string Title { get; set; } = "";
            public decimal Amount { get; set; }
        }
    }
}
