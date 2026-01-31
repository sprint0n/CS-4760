using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages.Departments
{
    public class EditModel : PageModel
    {
        private readonly University_Grant_Application_System.Data.University_Grant_Application_SystemContext _context;

        public EditModel(University_Grant_Application_System.Data.University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Department Department { get; set; } = default!;

        [BindProperty]
        public int SelectedChairId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department =  await _context.Departments.FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }
            Department = department;

            var currentChair = await _context.Users
                .FirstOrDefaultAsync(u => u.DepartmentId == id && u.userType == "chair");

            if (currentChair != null)
            {
                SelectedChairId = currentChair.UserId;
            }
            
            var deptUsers = _context.Users
                .Where(u => u.DepartmentId == id)
                .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName })
                .ToList();

            ViewData["ChairIdList"] = new SelectList(deptUsers, "UserId", "FullName");
            ViewData["SchoolId"] = new SelectList(_context.Schools, "SchoolId", "SchoolId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Department).State = EntityState.Modified;


            var oldChair = await _context.Users
                .FirstOrDefaultAsync(u => u.DepartmentId == Department.DepartmentId && u.userType == "chair");

            var newChair = await _context.Users.FindAsync(SelectedChairId); 

            if (newChair != null && (oldChair == null || oldChair.UserId != newChair.UserId))
            {
           
                if (oldChair != null)
                {
                    oldChair.userType = "teacher";
                }
                // Promote the new chair
                newChair.userType = "chair";
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(Department.DepartmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }
    }
}
