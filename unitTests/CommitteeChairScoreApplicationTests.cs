using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;
using University_Grant_Application_System.Pages;

namespace unitTests
{
	[TestClass]
	public sealed class CommitteeChairScoreApplicationTests
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
			context.Users.Add(new User
			{
				UserId = 301,
				FirstName = "Bob",
				LastName = "CommChair",
				Email = "chair@test.com",
				userType = "Teacher",
				committeeMemberStatus = "chair"
			});

			context.Users.Add(new User
			{
				UserId = 300,
				FirstName = "Alice",
				LastName = "Member",
				Email = "member@test.com",
				userType = "Teacher",
				committeeMemberStatus = "member"
			});

			context.FormTable.Add(new FormTable
			{
				Id = 1,
				Title = "Test Grant",
				ApplicationStatus = "PendingCommittee"
			});

			context.Reviews.Add(new Review
			{
				FormTableId = 1,
				ReviewerId = 301,
				ReviewDone = false,
				totalScore = 0
			});

			context.Reviews.Add(new Review
			{
				FormTableId = 1,
				ReviewerId = 300,
				ReviewDone = false,
				totalScore = 0
			});

			context.SaveChanges();
		}

		private RubricModel CreateModelWithUser(University_Grant_Application_SystemContext context, string email)
		{
			var model = new RubricModel(context);

			var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };
			var identity = new ClaimsIdentity(claims, "TestAuthType");
			var claimsPrincipal = new ClaimsPrincipal(identity);

			model.PageContext = new PageContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			return model;
		}

		[TestMethod]
		public async Task OnPost_CalculatesWeightedScoreCorrectly()
		{
			using var context = CreateContext(nameof(OnPost_CalculatesWeightedScoreCorrectly));
			SeedBaseData(context);
			var model = CreateModelWithUser(context, "chair@test.com");

			model.ApplicationId = 1;
			model.AreaOneScore = 2;
			model.AreaTwoScore = 2;
			model.ProcedureScore = 2;
			model.TimelineScore = 2;
			model.EvaluationScore = 2;
			model.EvidenceScore = 2;

			await model.OnPostAsync();

			var review = await context.Reviews.FirstOrDefaultAsync(r => r.ReviewerId == 301 && r.FormTableId == 1);
			Assert.IsNotNull(review);
			Assert.AreEqual(100.0, review.totalScore);
			Assert.IsTrue(review.ReviewDone);
		}

		[TestMethod]
		public async Task OnPost_ChairIsRedirectedToCommitteeDashboard()
		{
			using var context = CreateContext(nameof(OnPost_ChairIsRedirectedToCommitteeDashboard));
			SeedBaseData(context);
			var model = CreateModelWithUser(context, "chair@test.com");
			model.ApplicationId = 1;

			var result = await model.OnPostAsync();

			Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
			var redirect = (RedirectToPageResult)result;
			Assert.AreEqual("/CommitteeDashboard/CommitteeDashboard", redirect.PageName);
		}

		[TestMethod]
		public async Task OnPost_WhenOtherReviewsArePending_ApplicationStatusRemainsUnchanged()
		{
			using var context = CreateContext(nameof(OnPost_WhenOtherReviewsArePending_ApplicationStatusRemainsUnchanged));
			SeedBaseData(context);
			var model = CreateModelWithUser(context, "chair@test.com");
			model.ApplicationId = 1;

			await model.OnPostAsync();

			var application = await context.FormTable.FindAsync(1);
			Assert.AreEqual("PendingCommittee", application!.ApplicationStatus);
		}

		[TestMethod]
		public async Task OnPost_WhenLastReviewIsFinished_ApplicationStatusChangesToPendingDeanApproval()
		{
			using var context = CreateContext(nameof(OnPost_WhenLastReviewIsFinished_ApplicationStatusChangesToPendingDeanApproval));
			SeedBaseData(context);

			var memberReview = await context.Reviews.FirstAsync(r => r.ReviewerId == 300);
			memberReview.ReviewDone = true;
			await context.SaveChangesAsync();

			var model = CreateModelWithUser(context, "chair@test.com");
			model.ApplicationId = 1;

			await model.OnPostAsync();

			var application = await context.FormTable.FindAsync(1);
			Assert.AreEqual("PendingDeanApproval", application!.ApplicationStatus);
		}
	}
}