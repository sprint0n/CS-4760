using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages.AdminDashboard
{
    public class IndexModel : PageModel
    {
        private readonly University_Grant_Application_System.Data.University_Grant_Application_SystemContext _context;

        public IndexModel(University_Grant_Application_System.Data.University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        public IList<User> Users { get;set; } = default!;
        
        public IList<User> CommitteeMembers { get; set; }

        [BindProperty]
        public IFormFile UploadFile { get; set; }


        public async Task OnGetAsync()
        {
            CommitteeMembers = await _context.Users
                .Where(u => u.committeeMemberStatus == "member" || u.committeeMemberStatus == "chair")
                .Include(u => u.College)
                .Include(u => u.School)
                .Include(u => u.Department)
                .OrderByDescending(u => u.committeeMemberStatus == "chair")
                .ThenBy(u => u.LastName)
                .ToListAsync();

            Users = await _context.Users
                .Where(u => u.committeeMemberStatus != "member" && u.committeeMemberStatus != "chair")
                .Include(u => u.School)
                .Include(u => u.College)
                .Include(u => u.Department)
                .OrderBy(u => u.LastName)
                .ToListAsync();

        }

        public async Task<IActionResult> OnPostAddToCommitteeAsync(int userId) 
        { 
            var user = await _context.Users.FindAsync(userId); 
            if (user != null) 
            { 
                user.committeeMemberStatus = "member";
                await _context.SaveChangesAsync(); 
            } 
            return RedirectToPage(); 
        }

        public async Task<IActionResult> OnPostRemoveFromCommitteeAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.committeeMemberStatus = "";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMakeChairAsync(int userId)
        {
            // Find the user being promoted
            var newChair = await _context.Users.FindAsync(userId);
            if (newChair == null)
                return RedirectToPage();

            // Demote any existing chair
            var currentChair = await _context.Users
                .Where(u => u.committeeMemberStatus == "chair")
                .ToListAsync();

            foreach (var user in currentChair)
            {
                user.committeeMemberStatus = "member";
            }

            // Promote the selected user
            newChair.committeeMemberStatus = "chair";

            // Save changes
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (UploadFile != null && UploadFile.Length > 0)
            {
                // Create a unique filename while preserving the original name in case of uploading file with same name
                var uniqueName = $"{UploadFile.FileName}_{Guid.NewGuid()}";
                var uploadPath = Path.Combine("wwwroot/uploads", uniqueName); // upload to wwwroot/uploads folder

                using (var stream = System.IO.File.Create(uploadPath))
                {
                    await UploadFile.CopyToAsync(stream);
                }

                // Show original filename to user
                TempData["UploadSuccess"] = $"Successfully uploaded: {UploadFile.FileName}";
            }
            // Reload page after upload
            return RedirectToPage();
        }


    }
}
