using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Data
{
    public class University_Grant_Application_SystemContext : DbContext
    {
        public University_Grant_Application_SystemContext (DbContextOptions<University_Grant_Application_SystemContext> options)
            : base(options)
        {
        }

        public DbSet<University_Grant_Application_System.Models.User> Users { get; set; } = default!;
        public DbSet<University_Grant_Application_System.Models.FormTable> FormTable { get; set; } = default!;
        public DbSet<University_Grant_Application_System.Models.College> Colleges { get; set; } = default!;
        public DbSet<University_Grant_Application_System.Models.Department> Departments { get; set; } = default!;

        public DbSet<University_Grant_Application_System.Models.School> Schools { get; set; } = default!;
        public DbSet<University_Grant_Application_System.Models.PersonnelExpense> PersonnelExpenses { get; set; } = default!;

    }
}
