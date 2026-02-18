using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace University_Grant_Application_System.Pages
{

    public class AllocationRowViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubmittedBy { get; set; }
        public string PrincipalInvestigator { get; set; }
        public decimal RspgTotal { get; set; }
        public decimal OtherTotal { get; set; }
        public double OverallScore { get; set; } // assuming you calculate/store this somewhere
    }
    public class AllocationPageModel : PageModel
    {
        private readonly University_Grant_Application_SystemContext _context;

        public AllocationPageModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        public decimal TotalMoneyAvailable { get; set; }
        public decimal TotalMoneyRequested { get; set; }

        public List<AllocationRowViewModel> Applications { get; set; }

        public async Task OnGetAsync()
        {
            var scoresQuery = _context.Reviews
                .Where(r => r.ReviewDone)
                .GroupBy(r => r.FormTableId)
                .Select(g => new
                {
                    FormTableId = g.Key,
                    AverageScore = g.Average(r => r.totalScore)
                });

            Applications = await (from f in _context.FormTable
                                  join s in scoresQuery
                                      on f.Id equals s.FormTableId into scoreGroup
                                  from sg in scoreGroup.DefaultIfEmpty()
                                  where f.ApplicationStatus == "PendingDeanApproval"
                                  select new AllocationRowViewModel
                                  {
                                      Id = f.Id,
                                      Title = f.Title,
                                      SubmittedBy = _context.Users
                                          .Where(u => u.UserId == f.UserId)
                                          .Select(u => u.FirstName + " " + u.LastName)
                                          .FirstOrDefault(),

                                      PrincipalInvestigator = _context.Users
                                          .Where(u => u.UserId == f.PrincipalInvestigatorID)
                                          .Select(u => u.FirstName + " " + u.LastName)
                                          .FirstOrDefault(),

                                      RspgTotal = f.TotalBudget ?? 0,

                                      OtherTotal =
                                          (decimal)(f.OtherFunding1Amount ?? 0) +
                                          (decimal)(f.OtherFunding2Amount ?? 0) +
                                          (decimal)(f.OtherFunding3Amount ?? 0) +
                                          (decimal)(f.OtherFunding4Amount ?? 0),

                                      OverallScore = sg != null
                                            ? (sg.AverageScore / 18.0)
                                            : 0
                                  })
                                  .ToListAsync();

            TotalMoneyAvailable = 100_000m; // static value

            TotalMoneyRequested = Applications.Sum(a => a.RspgTotal);
        }
    }
}
