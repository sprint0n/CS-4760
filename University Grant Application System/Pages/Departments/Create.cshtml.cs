using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages.Departments
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

            var allUsers = _context.Users
                .Select(u => new
                {
                    u.UserId,
                    FullName = u.FirstName + " " + u.LastName
                })
                .ToList();

            ViewData["UserList"] = new SelectList(allUsers, "UserId", "FullName");
            return Page();
        }

        [BindProperty]
        public Department Department { get; set; } = default!;
        [BindProperty]
        public int? SelectedChairId { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["SchoolId"] = new SelectList(_context.Schools, "SchoolId", "SchoolName");
                return Page();
            }

            _context.Departments.Add(Department);
            await _context.SaveChangesAsync();

            if (SelectedChairId.HasValue)
            {
                var user = await _context.Users.FindAsync(SelectedChairId.Value);
                if (user != null)
                {
                    user.userType = "chair";
                    user.DepartmentId = Department.DepartmentId;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
            }


            return RedirectToPage("./Index");
        }
    }
}