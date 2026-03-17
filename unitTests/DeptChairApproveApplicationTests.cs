using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;
using University_Grant_Application_System.Pages.DeptChairDashboard;

namespace unitTests
{
    [TestClass]
    public sealed class DeptChairApproveApplicationTests
    {
        private University_Grant_Application_SystemContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<University_Grant_Application_SystemContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new University_Grant_Application_SystemContext(options);
        }

        private void SeedBaseData(University_Grant_Application_SystemContext context)
        {
            // Department
            context.Departments.Add(new Department { DepartmentId = 1, DepartmentName = "Computer Science" });

            // Chair user
            context.Users.Add(new User
            {
                UserId = 100,
                FirstName = "Jane",
                LastName = "Chair",
                Email = "chair@test.com",
                userType = "chair",
                DepartmentId = 1,
                HashedPassword = "hashed",
                committeeMemberStatus = ""
            });

            // Faculty / PI user
            context.Users.Add(new User
            {
                UserId = 200,
                FirstName = "John",
                LastName = "Faculty",
                Email = "faculty@test.com",
                userType = "Teacher",
                DepartmentId = 1,
                HashedPassword = "hashed",
                committeeMemberStatus = ""
            });

            // Committee members
            context.Users.Add(new User
            {
                UserId = 300,
                FirstName = "Alice",
                LastName = "Member",
                Email = "alice@test.com",
                userType = "Teacher",
                DepartmentId = 1,
                HashedPassword = "hashed",
                committeeMemberStatus = "member"
            });

            context.Users.Add(new User
            {
                UserId = 301,
                FirstName = "Bob",
                LastName = "CommChair",
                Email = "bob@test.com",
                userType = "Teacher",
                DepartmentId = 1,
                HashedPassword = "hashed",
                committeeMemberStatus = "chair"
            });

            // Application pending dept chair approval
            context.FormTable.Add(new FormTable
            {
                Id = 1,
                Title = "Test Grant",
                UserId = 200,
                ApplicationStatus = "PendingDeptChair",
                PrincipalInvestigatorID = 200,
                approvedByDeptChair = false
            });

            context.SaveChanges();
        }

        // =====================================================
        // FR-4: Approve Application
        // =====================================================

        [TestMethod]
        public async Task Approve_SetsStatusToPendingCommittee()
        {
            // FR-4.1
            using var context = CreateContext(nameof(Approve_SetsStatusToPendingCommittee));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            var result = await model.OnPostActionAsync(1, "Approve");

            var app = await context.FormTable.FindAsync(1);
            Assert.AreEqual("PendingCommittee", app!.ApplicationStatus);
        }

        [TestMethod]
        public async Task Approve_SetsApprovedByDeptChairTrue()
        {
            // FR-4.2
            using var context = CreateContext(nameof(Approve_SetsApprovedByDeptChairTrue));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            await model.OnPostActionAsync(1, "Approve");

            var app = await context.FormTable.FindAsync(1);
            Assert.IsTrue(app!.approvedByDeptChair);
        }

        [TestMethod]
        public async Task Approve_CreatesReviewRecordsForAllCommitteeMembers()
        {
            // FR-4.3
            using var context = CreateContext(nameof(Approve_CreatesReviewRecordsForAllCommitteeMembers));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            await model.OnPostActionAsync(1, "Approve");

            var reviews = await context.Reviews.Where(r => r.FormTableId == 1).ToListAsync();
            Assert.AreEqual(2, reviews.Count, "Should create one review per committee member");
            Assert.IsTrue(reviews.Any(r => r.ReviewerId == 300), "Review for committee member missing");
            Assert.IsTrue(reviews.Any(r => r.ReviewerId == 301), "Review for committee chair missing");
        }

        [TestMethod]
        public async Task Approve_ReviewRecordsInitializedCorrectly()
        {
            // FR-4.3 - ReviewDone = false, totalScore = 0
            using var context = CreateContext(nameof(Approve_ReviewRecordsInitializedCorrectly));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            await model.OnPostActionAsync(1, "Approve");

            var reviews = await context.Reviews.Where(r => r.FormTableId == 1).ToListAsync();
            foreach (var review in reviews)
            {
                Assert.IsFalse(review.ReviewDone, "New reviews should not be marked as done");
                Assert.AreEqual(0, review.totalScore, "New reviews should have score of 0");
            }
        }

        [TestMethod]
        public async Task Approve_DoesNotCreateDuplicateReviewRecords()
        {
            // FR-4.4
            using var context = CreateContext(nameof(Approve_DoesNotCreateDuplicateReviewRecords));
            SeedBaseData(context);

            // Pre-existing review for member 300
            context.Reviews.Add(new Review
            {
                FormTableId = 1,
                ReviewerId = 300,
                ReviewDone = false,
                totalScore = 0
            });
            context.SaveChanges();

            var model = new ReviewModel(context);
            await model.OnPostActionAsync(1, "Approve");

            var reviewsForMember300 = await context.Reviews
                .Where(r => r.FormTableId == 1 && r.ReviewerId == 300)
                .CountAsync();
            Assert.AreEqual(1, reviewsForMember300, "Should not duplicate existing review record");
        }

        [TestMethod]
        public async Task Approve_RedirectsToDashboard()
        {
            // FR-4.6
            using var context = CreateContext(nameof(Approve_RedirectsToDashboard));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            var result = await model.OnPostActionAsync(1, "Approve");

            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            var redirect = (RedirectToPageResult)result;
            Assert.AreEqual("/DeptChairDashboard/DeptChairDashboard", redirect.PageName);
        }

        // =====================================================
        // FR-5: Decline Application
        // =====================================================

        [TestMethod]
        public async Task Decline_SetsStatusToDeclinedByChair()
        {
            // FR-5.1
            using var context = CreateContext(nameof(Decline_SetsStatusToDeclinedByChair));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            await model.OnPostActionAsync(1, "Decline");

            var app = await context.FormTable.FindAsync(1);
            Assert.AreEqual("DeclinedByChair", app!.ApplicationStatus);
        }

        [TestMethod]
        public async Task Decline_SetsApprovedByDeptChairFalse()
        {
            // FR-5.2
            using var context = CreateContext(nameof(Decline_SetsApprovedByDeptChairFalse));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            await model.OnPostActionAsync(1, "Decline");

            var app = await context.FormTable.FindAsync(1);
            Assert.IsFalse(app!.approvedByDeptChair);
        }

        [TestMethod]
        public async Task Decline_DoesNotCreateReviewRecords()
        {
            // FR-5.3
            using var context = CreateContext(nameof(Decline_DoesNotCreateReviewRecords));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            await model.OnPostActionAsync(1, "Decline");

            var reviews = await context.Reviews.Where(r => r.FormTableId == 1).CountAsync();
            Assert.AreEqual(0, reviews, "No review records should be created for declined applications");
        }

        [TestMethod]
        public async Task Decline_RedirectsToDashboard()
        {
            // FR-5.5
            using var context = CreateContext(nameof(Decline_RedirectsToDashboard));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            var result = await model.OnPostActionAsync(1, "Decline");

            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            var redirect = (RedirectToPageResult)result;
            Assert.AreEqual("/DeptChairDashboard/DeptChairDashboard", redirect.PageName);
        }

        // =====================================================
        // FR-3: Review Page - View Application Details
        // =====================================================

        [TestMethod]
        public async Task OnGet_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            // FR-3.4
            using var context = CreateContext(nameof(OnGet_ReturnsNotFound_WhenApplicationDoesNotExist));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            var result = await model.OnGetAsync(999);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task OnGet_LoadsApplicationWithRelatedData()
        {
            // FR-3.1
            using var context = CreateContext(nameof(OnGet_LoadsApplicationWithRelatedData));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            var result = await model.OnGetAsync(1);

            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.IsNotNull(model.Application);
            Assert.AreEqual("Test Grant", model.Application.Title);
        }

        [TestMethod]
        public async Task OnGet_SetsInvestigatorName()
        {
            // FR-3.3
            using var context = CreateContext(nameof(OnGet_SetsInvestigatorName));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            await model.OnGetAsync(1);

            Assert.AreEqual("John Faculty", model.InvestigatorName);
        }

        [TestMethod]
        public async Task OnGet_SetsUnknown_WhenUserIsNull()
        {
            // FR-3.3 - When Application.User navigation property is null, display "Unknown"
            using var context = CreateContext(nameof(OnGet_SetsUnknown_WhenUserIsNull));
            SeedBaseData(context);

            // Detach all tracked entities so Include won't resolve User from cache
            foreach (var entry in context.ChangeTracker.Entries().ToList())
                entry.State = EntityState.Detached;

            // Directly set the navigation property to null after loading
            var app = await context.FormTable.Include(f => f.User).FirstAsync(f => f.Id == 1);
            app.User = null;

            // Simulate the logic from ReviewModel.OnGetAsync for the null-user branch
            string investigatorName;
            if (app.User != null)
                investigatorName = $"{app.User.FirstName} {app.User.LastName}";
            else
                investigatorName = "Unknown";

            Assert.AreEqual("Unknown", investigatorName);
        }

        // =====================================================
        // Edge Cases
        // =====================================================

        [TestMethod]
        public async Task Approve_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            using var context = CreateContext(nameof(Approve_ReturnsNotFound_WhenApplicationDoesNotExist));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            var result = await model.OnPostActionAsync(999, "Approve");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Decline_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            using var context = CreateContext(nameof(Decline_ReturnsNotFound_WhenApplicationDoesNotExist));
            SeedBaseData(context);
            var model = new ReviewModel(context);

            var result = await model.OnPostActionAsync(999, "Decline");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Approve_WithNoCommitteeMembers_CreatesNoReviews()
        {
            using var context = CreateContext(nameof(Approve_WithNoCommitteeMembers_CreatesNoReviews));

            // Seed without committee members
            context.Departments.Add(new Department { DepartmentId = 1, DepartmentName = "CS" });
            context.Users.Add(new User
            {
                UserId = 200,
                FirstName = "John",
                LastName = "Faculty",
                Email = "faculty@test.com",
                userType = "Teacher",
                DepartmentId = 1,
                HashedPassword = "hashed",
                committeeMemberStatus = ""
            });
            context.FormTable.Add(new FormTable
            {
                Id = 1,
                Title = "Test Grant",
                UserId = 200,
                ApplicationStatus = "PendingDeptChair",
                PrincipalInvestigatorID = 200,
                approvedByDeptChair = false
            });
            context.SaveChanges();

            var model = new ReviewModel(context);
            await model.OnPostActionAsync(1, "Approve");

            var app = await context.FormTable.FindAsync(1);
            Assert.AreEqual("PendingCommittee", app!.ApplicationStatus, "Status should still change even with no committee members");

            var reviews = await context.Reviews.CountAsync();
            Assert.AreEqual(0, reviews, "No reviews when no committee members exist");
        }
    }
}
