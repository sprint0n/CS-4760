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

namespace University_Grant_Application_System.Pages.Schools
{
    public class CreateModel : PageModel
    {
        private readonly University_Grant_Application_System.Data.University_Grant_Application_SystemContext _context;

        public CreateModel(University_Grant_Application_System.Data.University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        public List<SelectListItem> CollegeOptions { get; set; }
        public List<SelectListItem> DepartmentOptions { get; set; }

        public List<SelectListItem> SchoolOptions { get; set; }


        public async Task OnGetAsync()
        {
            await LoadDropdowns();
        }

        private async Task LoadDropdowns()
        {
            var colleges = await _context.Colleges.ToListAsync();
            var departments = await _context.Departments.ToListAsync();
            var schools = await _context.Schools.ToListAsync();

                CollegeOptions = colleges
                  .Select(c => new SelectListItem
                  {
                      Value = c.CollegeId.ToString(),
                      Text = c.CollegeName
                  }).ToList();

                DepartmentOptions = departments
                  .Select(c => new SelectListItem
                  {
                      Value = c.DepartmentId.ToString(),
                      Text = c.DepartmentName
                  }).ToList();
            
         }
        

        [BindProperty]
        public School School { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Schools.Add(School);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
