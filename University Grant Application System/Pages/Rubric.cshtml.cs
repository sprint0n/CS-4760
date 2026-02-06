using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace University_Grant_Application_System.Pages
{
    public class RubricModel : PageModel
    {
        // Area One
        [BindProperty] public int BudgetScore { get; set; }
        [BindProperty] public int SupportScore { get; set; }

        // Area Two
        [BindProperty] public int ReputationScore { get; set; }
        [BindProperty] public int InnovationScore { get; set; }
        [BindProperty] public int CommunityScore { get; set; }

        // Area Three
        [BindProperty] public int ProcedureScore { get; set; }
        [BindProperty] public int TimelineScore { get; set; }
        [BindProperty] public int EvaluationScore { get; set; }
        [BindProperty] public int EvidenceScore { get; set; }

        public int TotalScore { get; set; }

        public void OnGet() { }

        public void OnPost()
        {
            // Simple addition of all bound properties
            TotalScore = BudgetScore + SupportScore + ReputationScore +
                         InnovationScore + CommunityScore + ProcedureScore +
                         TimelineScore + EvaluationScore + EvidenceScore;
        }
    }
}
