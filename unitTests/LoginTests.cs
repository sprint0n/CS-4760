using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace unitTests
{
    [TestClass]
    public class LoginTests
    {
        private University_Grant_Application_SystemContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<University_Grant_Application_SystemContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new University_Grant_Application_SystemContext(options);
        }

        private void SeedUser(University_Grant_Application_SystemContext context, bool isAdmin = false)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");

            context.Users.Add(new User
            {
                UserId = 1,
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                HashedPassword = hashedPassword,
                userType = "Teacher",
                isAdmin = isAdmin,
                committeeMemberStatus = "member"
            });

            context.SaveChanges();
        }

        private IndexModel CreateModel(University_Grant_Application_SystemContext context)
        {
            var model = new IndexModel(context);

            var httpContext = new DefaultHttpContext();

            // Required for SignInAsync to not crash
            httpContext.RequestServices = new ServiceCollection()
                .AddLogging()
                .AddAuthentication("MyCookieAuth")
                .AddCookie("MyCookieAuth")
                .Services
                .BuildServiceProvider();

            model.PageContext = new PageContext
            {
                HttpContext = httpContext
            };

            return model;
        }

        //Valid login
        [TestMethod]
        public async Task OnPost_ValidLogin_RedirectsToFacultyDashboard()
        {
            using var context = CreateContext(nameof(OnPost_ValidLogin_RedirectsToFacultyDashboard));
            SeedUser(context);

            var model = CreateModel(context);

            model.Input = new IndexModel.LoginInputModel
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var result = await model.OnPost();

            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));

            var redirect = (RedirectToPageResult)result;
            Assert.AreEqual("/ComMemberDashboard/ComMemberDashboard", redirect.PageName);
        }

        //Invalid email
        [TestMethod]
        public async Task OnPost_InvalidEmail_ReturnsPage()
        {
            using var context = CreateContext(nameof(OnPost_InvalidEmail_ReturnsPage));

            var model = CreateModel(context);

            model.Input = new IndexModel.LoginInputModel
            {
                Email = "wrong@test.com",
                Password = "password123"
            };

            var result = await model.OnPost();

            Assert.IsInstanceOfType(result, typeof(PageResult));
        }

        //Wrong password
        [TestMethod]
        public async Task OnPost_WrongPassword_ReturnsPage()
        {
            // Arrange
            using var context = CreateContext(nameof(OnPost_WrongPassword_ReturnsPage));
            SeedUser(context);

            var model = CreateModel(context);

            model.Input = new IndexModel.LoginInputModel
            {
                Email = "test@test.com",
                Password = "wrongpassword"
            };

            var result = await model.OnPost();

            Assert.IsInstanceOfType(result, typeof(PageResult));
        }

        //Admin redirect
        [TestMethod]
        public async Task OnPost_AdminUser_RedirectsToAdminDashboard()
        {
            using var context = CreateContext(nameof(OnPost_AdminUser_RedirectsToAdminDashboard));
            SeedUser(context, isAdmin: true);

            var model = CreateModel(context);

            model.Input = new IndexModel.LoginInputModel
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var result = await model.OnPost();

            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));

            var redirect = (RedirectToPageResult)result;
            Assert.AreEqual("/AdminDashboard/Index", redirect.PageName);
        }

        // Committee chair redirect
        [TestMethod]
        public async Task OnPost_CommitteeChair_RedirectsToCommitteeDashboard()
        {
            using var context = CreateContext(nameof(OnPost_CommitteeChair_RedirectsToCommitteeDashboard));

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");

            context.Users.Add(new User
            {
                UserId = 1,
                Email = "chair@test.com",
                HashedPassword = hashedPassword,
                userType = "Teacher",
                isAdmin = false,
                committeeMemberStatus = "chair"
            });

            context.SaveChanges();

            var model = CreateModel(context);

            model.Input = new IndexModel.LoginInputModel
            {
                Email = "chair@test.com",
                Password = "password123"
            };

            var result = await model.OnPost();

            var redirect = result as RedirectToPageResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("/CommitteeDashboard/CommitteeDashboard", redirect.PageName);
        }

        // Department chair redirect
        [TestMethod]
        public async Task OnPost_DeptChair_RedirectsToDeptChairDashboard()
        {
            using var context = CreateContext(nameof(OnPost_DeptChair_RedirectsToDeptChairDashboard));

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");

            context.Users.Add(new User
            {
                UserId = 1,
                Email = "deptchair@test.com",
                HashedPassword = hashedPassword,
                userType = "chair",
                isAdmin = false,
                committeeMemberStatus = "none"
            });

            context.SaveChanges();

            var model = CreateModel(context);

            model.Input = new IndexModel.LoginInputModel
            {
                Email = "deptchair@test.com",
                Password = "password123"
            };

            var result = await model.OnPost();

            var redirect = result as RedirectToPageResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("/DeptChairDashboard/DeptChairDashboard", redirect.PageName);
        }
    }
}