using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages.Colleges
{
    public class DetailsModel : PageModel
    {
        private readonly University_Grant_Application_System.Data.University_Grant_Application_SystemContext _context;

        public DetailsModel(University_Grant_Application_System.Data.University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        public College College { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var college = await _context.Colleges.FirstOrDefaultAsync(m => m.CollegeId == id);

            if (college is not null)
            {
                College = college;

                return Page();
            }

            return NotFound();
        }
    }
}
