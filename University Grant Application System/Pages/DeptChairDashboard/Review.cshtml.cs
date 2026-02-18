using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; 
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages.DeptChairDashboard { 
public class ReviewModel : PageModel
{
    private readonly University_Grant_Application_SystemContext _context;
    public ReviewModel (University_Grant_Application_SystemContext context) => _context = context;

    public FormTable Application { get; set; }

        public List<UploadedFile> AttachedFiles { get; set; } = new();
        public string InvestigatorName { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Application = await _context.FormTable
                .Include(f => f.User)
                    .ThenInclude(u => u.Department)
                .Include(f => f.PersonnelExpenses)
                .Include(f => f.EquipmentExpenses)
                .Include(f => f.TravelExpenses)
                .Include(f => f.OtherExpenses)
                .FirstOrDefaultAsync(m => m.Id == id);

            AttachedFiles = await _context.UploadedFiles
                .Where(f => f.FormTableId == id)
                .ToListAsync();

            if (Application == null) return NotFound();

            // Safely resolve the Investigator Name
            if (Application.User != null)
            {
                InvestigatorName = $"{Application.User.FirstName} {Application.User.LastName}";
            }
            else
            {
                InvestigatorName = "Unknown";
            }

            return Page();
        }
        public async Task<IActionResult> OnGetDownloadFile(Guid fileId)
        {
            var fileRecord = await _context.UploadedFiles.FirstOrDefaultAsync(u => u.ID == fileId);

            if (fileRecord == null) return NotFound("Database record not found.");

            if (string.IsNullOrEmpty(fileRecord.StoredFileName))
                return NotFound("This record was created before unique naming was implemented.");

            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string filePath = Path.Combine(rootPath, "uploads", fileRecord.StoredFileName);

            if (!System.IO.File.Exists(filePath))
            {
            
                return NotFound($"File not found on disk. Path: {filePath}");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, fileRecord.ContentType, fileRecord.FileName);
        }
        public async Task<IActionResult> OnPostActionAsync(int id, string decision)
    {
            var app = await _context.FormTable.FindAsync(id);
            if (app == null) return NotFound();

            if (decision == "Approve")
            {
                app.ApplicationStatus = "PendingCommittee";
                app.approvedByDeptChair = true;

                var committeMembers = await _context.Users
                    .Where(u => u.committeeMemberStatus == "member" || u.committeeMemberStatus == "chair")
                    .ToListAsync();

                foreach (var member in committeMembers)
                {
                    bool exists = await _context.Reviews.AnyAsync(r => r.FormTableId == id && r.ReviewerId == member.UserId);
                    if (!exists)
                    {
                        _context.Reviews.Add(new Review
                        {
                            FormTableId = id,
                            ReviewerId = member.UserId,
                            ReviewDone = false,
                            totalScore = 0
                        });
                    }
                }
            }
            else
            {
                app.ApplicationStatus = "DeclinedByChair";
                app.approvedByDeptChair = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/DeptChairDashboard/DeptChairDashboard");
        }
    }
}