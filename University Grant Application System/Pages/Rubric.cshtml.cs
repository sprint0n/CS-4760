using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;
namespace University_Grant_Application_System.Pages
{
    public class RubricModel : PageModel
    {
        private readonly University_Grant_Application_SystemContext _context;

        public RubricModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }
        // Area One

        [BindProperty] public double AreaOneScore { get; set; }

        // Area Two

        [BindProperty] public double AreaTwoScore { get; set; }

        // Area Three
        [BindProperty] public double ProcedureScore { get; set; }
        [BindProperty] public double TimelineScore { get; set; }
        [BindProperty] public double EvaluationScore { get; set; }
        [BindProperty] public double EvidenceScore { get; set; }
        [BindProperty(Name = "id", SupportsGet = true)]
        public int ApplicationId { get; set; }

        public double TotalScore { get; set; }

        public void OnGet() {
           
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userEmail = User.Identity?.Name;


            var currentUser = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

           
         


            TotalScore = AreaOneScore * 3 + AreaTwoScore * 5  + ProcedureScore +
                         TimelineScore + EvaluationScore * 3 + EvidenceScore;


            var reviewToUpdate = await _context.Reviews
                .FirstOrDefaultAsync(r => r.FormTableId == ApplicationId && r.ReviewerId == currentUser.UserId);
            if (reviewToUpdate != null)
            {
                reviewToUpdate.totalScore = TotalScore;
                reviewToUpdate.ReviewDone = true;
            }
            else
            {
                var newReview = new Review
                {
                    ReviewerId = currentUser.UserId,
                    FormTableId = ApplicationId,
                    totalScore = TotalScore,
                    ReviewDone = true
                };
                _context.Reviews.Add(newReview);
            }
            await _context.SaveChangesAsync();

            bool pendingReviews = await _context.Reviews
                .AnyAsync(r => r.FormTableId == ApplicationId && r.ReviewDone == false);

            if (!pendingReviews)
            {
                var application = await _context.FormTable.FindAsync(ApplicationId);
                if (application != null)
                {
                    application.ApplicationStatus = "PendingDeanApproval";
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage("/CommitteeDashboard/CommitteeDashboard");
        }
    }
}

/*
 * 
 * 
 * 
 *         var newUser = new User
        {
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Birthday = Input.Birthday,
            Email = Input.Email,
            HashedPassword = hashedPassword,
            AccountID = Input.AccountIndex,
            CollegeId = Input.SelectedCollegeId,
            SchoolId = Input.SelectedSchoolId,
            DepartmentId = Input.SelectedDepartmentId,
            userType = "Teacher"
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
*/