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
    public class IndexModel : PageModel
    {
        private readonly University_Grant_Application_System.Data.University_Grant_Application_SystemContext _context;

        public IndexModel(University_Grant_Application_System.Data.University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        public IList<College> College { get;set; } = default!;

        public async Task OnGetAsync()
        {
            // Temporary placeholder data until the database is ready
            College = new List<College> 
            { 
                new College { Id = 1, CollegeName = "College of Engineering" }, 
                new College { Id = 2, CollegeName = "College of Business" }, 
                new College { Id = 3, CollegeName = "College of Arts & Humanities" }, 
                new College { Id = 4, CollegeName = "College of Science" } 
            };
            // College = await _context.Colleges.ToListAsync(); -- use later with database
        }
    }
}
