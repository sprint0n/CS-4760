using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly University_Grant_Application_System.Data.University_Grant_Application_SystemContext _context;

        public IndexModel(University_Grant_Application_System.Data.University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        public IList<Department> Department { get; set; } = default!;

        public Dictionary<int, string> DepartmentChairs { get; set; } = new();
        public async Task OnGetAsync()
        {
            Department = await _context.Departments
                .Include(d => d.School)
                .ToListAsync();

            var chairs = await _context.Users
                .Where(u => u.userType.ToLower() == "chair" && u.DepartmentId != null)
                .ToListAsync();

            foreach (var dept in Department)
            {
                var chair = chairs.FirstOrDefault(u => u.DepartmentId == dept.DepartmentId);
                DepartmentChairs[dept.DepartmentId] = chair != null
                ? $"{chair.FirstName} {chair.LastName}"
                : "No Chair Assigned";
            }

        }
    }
}