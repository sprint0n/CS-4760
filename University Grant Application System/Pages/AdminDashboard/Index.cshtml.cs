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

        [BindProperty]
        public IFormFile UploadFile { get; set; }


        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
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
