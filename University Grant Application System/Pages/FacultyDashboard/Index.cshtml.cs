using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace University_Grant_Application_System.Pages.FacultyDashboard
{

    public class IndexModel : PageModel
    {
        public DateTime ApplicationDueDate { get; set; }

        public List<ApplicationCard> InProgressApplications { get; set; } = new();
        public List<ApplicationCard> InReviewApplications { get; set; } = new();
        public List<ApprovedGrant> ApprovedGrants { get; set; } = new();

        public void OnGet()
        {
            // Placeholder data
            ApplicationDueDate = DateTime.Today.AddDays(14);

            InProgressApplications = new List<ApplicationCard>
        {
            new() { Title = "RSPG Application A", Status = "Draft" },
            new() { Title = "RSPG Application B", Status = "In Progress" }
        };

            InReviewApplications = new List<ApplicationCard>
        {
            new() { Title = "Education Grant", Status = "Under Review" },
            new() { Title = "Community Grant", Status = "Submitted" }
        };

            ApprovedGrants = new List<ApprovedGrant>
        {
            new() { Title = "STEM Outreach Grant", Amount = 25000 },
            new() { Title = "Arts Initiative Grant", Amount = 10000 }
        };
        }
    }

    public class ApplicationCard
    {
        public string Title { get; set; } = "";
        public string Status { get; set; } = "";
    }

    public class ApprovedGrant
    {
        public string Title { get; set; } = "";
        public decimal Amount { get; set; }
    }

}