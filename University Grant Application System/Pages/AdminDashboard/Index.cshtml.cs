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

        public IList<AdminType> AdminList { get;set; } = default!;
        public List<AdminType> Staff {  get;set; }

        public async Task OnGetAsync()
        {
            AdminList = new List<AdminType> 
            { 
                new AdminType 
                { 
                    Name = "Jane Doe", 
                    School = "Engineering", 
                    College = "College of Engineering", 
                    Department = "Computer Science" 
                }, 
                new AdminType 
                { 
                    Name = "John Smith", 
                    School = "Business", 
                    College = "College of Business", 
                    Department = "Accounting" 
                } 
            };

            // ADD THIS WHEN DB IS READY, REPLACE WITH ABOVE. -- AdminList = await _context.Admin.ToListAsync();


        }
    }
}
