using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace University_Grant_Application_System.Pages
{
    public class AllocationRowViewModel
    {

        public int Id { get; set; }
        public string Title { get; set; }

        public string SubmittedBy { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string PrincipalInvestigator { get; set; }

        public int AccountIndex { get; set; }

        public decimal RspgTotal { get; set; }
        public decimal OtherTotal { get; set; }
        public double OverallScore { get; set; }
    }

    public class AllocationPageModel : PageModel
    {
        private readonly University_Grant_Application_SystemContext _context;

        public AllocationPageModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<int> SelectedApplicationIds { get; set; }

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
                                  .Include(f => f.PersonnelExpenses)
                                  .Include(f => f.EquipmentExpenses)
                                  .Include(f => f.TravelExpenses)
                                  .Include(f => f.OtherExpenses)
                                  join s in scoresQuery
                                      on f.Id equals s.FormTableId into scoreGroup
                                  
                                  from sg in scoreGroup.DefaultIfEmpty()
                                  where f.ApplicationStatus == "PendingAllocation"
                                  select new AllocationRowViewModel
                                  {
                                      Id = f.Id,
                                      Title = f.Title,

                                      FirstName = _context.Users
                                        .Where(u => u.UserId == f.UserId)
                                        .Select(u => u.FirstName)
                                        .FirstOrDefault(),

                                      LastName = _context.Users
                                        .Where(u => u.UserId == f.UserId)
                                        .Select(u => u.LastName)
                                        .FirstOrDefault(),

                                      AccountIndex = _context.Users
                                        .Where(u => u.UserId == f.UserId)
                                        .Select(u => u.AccountID)
                                        .FirstOrDefault(),

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
                                        (decimal)(f.OtherFunding4Amount ?? 0) +
                                        (decimal)(f.PersonnelExpenses
                                        .Where(p => p.FormTableId == f.Id)
                                        .Select(p => p.OtherAmount1)
                                        .FirstOrDefault() ?? 0) +
                                        (decimal)(f.PersonnelExpenses
                                        .Where(p => p.FormTableId == f.Id)
                                        .Select(p => p.OtherAmount2)
                                        .FirstOrDefault() ?? 0) +
                                        (decimal)(f.TravelExpenses
                                        .Where(t => t.FormTableId == f.Id)
                                        .Select(t => t.OtherAmount1)
                                        .FirstOrDefault() ?? 0) +
                                        (decimal)(f.TravelExpenses
                                        .Where(t => t.FormTableId == f.Id)
                                        .Select(t => t.OtherAmount2)
                                        .FirstOrDefault() ?? 0) +
                                        (decimal)(f.EquipmentExpenses
                                        .Where(e => e.FormTableId == f.Id)
                                        .Select(e => e.OtherAmount1)
                                        .FirstOrDefault() ?? 0) +
                                        (decimal)(f.EquipmentExpenses
                                        .Where(e => e.FormTableId == f.Id)
                                        .Select(e => e.OtherAmount2)
                                        .FirstOrDefault() ?? 0) +
                                        (decimal)(f.OtherExpenses
                                        .Where(o => o.FormTableId == f.Id)
                                        .Select(o => o.OtherAmount1)
                                        .FirstOrDefault() ?? 0) +
                                        (decimal)(f.OtherExpenses
                                        .Where(o => o.FormTableId == f.Id)
                                        .Select(o => o.OtherAmount2)
                                        .FirstOrDefault() ?? 0),

                                      OverallScore = sg != null
                                        ? (sg.AverageScore / 100.0)
                                        : 0
                                  }
                                  ).ToListAsync();

            TotalMoneyAvailable = 10_000m; // static value

            TotalMoneyRequested = Applications.Sum(a => a.RspgTotal);
        }
        public async Task<IActionResult> OnPostFinalizeAsync()
        {
            if (SelectedApplicationIds == null || !SelectedApplicationIds.Any())
            {
                return RedirectToPage();
            }

            var applications = await _context.FormTable
                .Where(f => SelectedApplicationIds.Contains(f.Id) && f.ApplicationStatus == "PendingAllocation")
                .ToListAsync();

            foreach (var app in applications)
            {
                app.ApplicationStatus = "Approved";
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
