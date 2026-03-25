using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace unitTests
{
    [TestClass]
    public class CreateNewUserTests
    {
        private University_Grant_Application_SystemContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<University_Grant_Application_SystemContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new University_Grant_Application_SystemContext(options);
        }

        private void SeedDropdownData(University_Grant_Application_SystemContext context)
        {
            context.Colleges.Add(new College
            {
                CollegeId = 1,
                CollegeName = "Science"
            });

            context.Schools.Add(new School
            {
                SchoolId = 1,
                SchoolName = "Engineering",
                CollegeId = 1
            });

            context.Departments.Add(new Department
            {
                DepartmentId = 1,
                DepartmentName = "Computer Science",
                SchoolId = 1
            });

            context.SaveChanges();
        }

        // Successful user creation
        [TestMethod]
        public async Task OnPostAsync_CreatesUserSuccessfully()
        {
            using var context = CreateContext(nameof(OnPostAsync_CreatesUserSuccessfully));
            SeedDropdownData(context);

            var model = new RegisterModel(context);

            model.Input = new RegisterModel.RegisterInputModel
            {
                FirstName = "Max",
                LastName = "Harker",
                Birthday = new DateOnly(2000, 1, 1),
                Email = "test@test.com",
                Password = "password123",
                ConfirmPassword = "password123",
                AccountIndex = 123456,
                SelectedCollegeId = 1,
                SelectedSchoolId = 1,
                SelectedDepartmentId = 1
            };

            var result = await model.OnPostAsync();

            var user = await context.Users.FirstOrDefaultAsync();

            Assert.IsNotNull(user);
            Assert.AreEqual("Max", user.FirstName);
            Assert.AreEqual("Harker", user.LastName);
            Assert.AreEqual("test@test.com", user.Email);

            // Password should be hashed (NOT plain text)
            Assert.AreNotEqual("password123", user.HashedPassword);

            // Assert redirect
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            var redirect = (RedirectToPageResult)result;
            Assert.AreEqual("/Index", redirect.PageName);
        }

        //Invalid model should return Page()
        [TestMethod]
        public async Task OnPostAsync_InvalidModel_ReturnsPage()
        {
            using var context = CreateContext(nameof(OnPostAsync_InvalidModel_ReturnsPage));
            var model = new RegisterModel(context);

            model.ModelState.AddModelError("Email", "Required");

            var result = await model.OnPostAsync();

            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.AreEqual(0, context.Users.Count());
        }

        //Password is properly hashed
        [TestMethod]
        public async Task OnPostAsync_PasswordIsHashed()
        {
            using var context = CreateContext(nameof(OnPostAsync_PasswordIsHashed));
            SeedDropdownData(context);

            var model = new RegisterModel(context);

            model.Input = new RegisterModel.RegisterInputModel
            {
                FirstName = "Test",
                LastName = "User",
                Birthday = new DateOnly(2000, 1, 1),
                Email = "hash@test.com",
                Password = "mypassword",
                ConfirmPassword = "mypassword",
                AccountIndex = 654321,
                SelectedCollegeId = 1,
                SelectedSchoolId = 1,
                SelectedDepartmentId = 1
            };

            await model.OnPostAsync();

            var user = await context.Users.FirstOrDefaultAsync();

            Assert.IsNotNull(user);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("mypassword", user.HashedPassword));
        }
    }
}
