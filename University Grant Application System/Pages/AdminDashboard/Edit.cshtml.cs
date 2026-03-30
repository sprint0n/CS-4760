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

namespace University_Grant_Application_System.Pages.AdminDashboard
{
    public class EditModel : PageModel
    {
        private readonly University_Grant_Application_System.Data.University_Grant_Application_SystemContext _context;

        public EditModel(University_Grant_Application_System.Data.University_Grant_Application_SystemContext context)
        {
            _context = context;
        }
        public List<SelectListItem> CollegeOptions { get; set; }
        public List<SelectListItem> DepartmentOptions { get; set; }

        public List<SelectListItem> SchoolOptions { get; set; }

        [BindProperty]
        public User Users { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =  await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            Users = user;

            await LoadDropdowns();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.

        private async Task LoadDropdowns()
        {
            var colleges = await _context.Colleges.ToListAsync();
            var departments = await _context.Departments.ToListAsync();
            var schools = await _context.Schools.ToListAsync();

            CollegeOptions = colleges.Select(c => new SelectListItem
            {
                Value = c.CollegeId.ToString(),
                Text = c.CollegeName,
                Selected = c.CollegeId == Users.CollegeId 
            }).ToList();

            SchoolOptions = schools.Select(s => new SelectListItem
            {
                Value = s.SchoolId.ToString(),
                Text = s.SchoolName,
                Selected = s.SchoolId == Users.SchoolId
            }).ToList();

            DepartmentOptions = departments.Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName,
                Selected = d.DepartmentId == Users.DepartmentId
            }).ToList();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(Users.UserId))
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
        public async Task<JsonResult> OnGetSchoolsAsync(int collegeId)
        {
            var schools = await _context.Schools
                .Where(s => s.CollegeId == collegeId)
                .Select(s => new { s.SchoolId, s.SchoolName })
                .ToListAsync();
            return new JsonResult(schools);
        }

        public async Task<JsonResult> OnGetDepartmentsAsync(int schoolId)
        {
            var departments = await _context.Departments
                .Where(s => s.SchoolId == schoolId)
                .Select(s => new { s.DepartmentId, s.DepartmentName })
                .ToListAsync();
            return new JsonResult(departments);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
