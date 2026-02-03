using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages.AdminDashboard
{
    public class CreateModel : PageModel
    {
        private readonly University_Grant_Application_System.Data.University_Grant_Application_SystemContext _context;

        public CreateModel(University_Grant_Application_System.Data.University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["SchoolId"] = new SelectList(_context.Schools, "SchoolId", "SchoolName");
            ViewData["CollegeId"] = new SelectList(_context.Colleges, "CollegeId", "CollegeName");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");

            return Page();
        }

        [BindProperty]
        public User Users { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["SchoolId"] = new SelectList(_context.Schools, "SchoolId", "SchoolName");
                ViewData["CollegeId"] = new SelectList(_context.Colleges, "CollegeId", "CollegeName");
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");

                return Page();
            }

            _context.Users.Add(Users);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
