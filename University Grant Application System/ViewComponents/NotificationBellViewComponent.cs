using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;

namespace University_Grant_Application_System.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly University_Grant_Application_SystemContext _context;

        public NotificationBellViewComponent(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var notifications = new List<NotificationItem>();
            var userEmail = HttpContext.User.Identity?.Name;

            if (userEmail == null)
                return View(notifications);

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser == null)
                return View(notifications);

            // Application due date (placeholder — 14 days from today, matching dashboard logic)
            var dueDate = DateTime.Today.AddDays(14);
            var daysUntilDue = (dueDate - DateTime.Today).Days;

            if (daysUntilDue <= 7 && daysUntilDue >= 0)
            {
                notifications.Add(new NotificationItem
                {
                    Icon = "clock",
                    Message = $"Applications due in {daysUntilDue} day{(daysUntilDue != 1 ? "s" : "")}!",
                    Type = "warning"
                });
            }
            else if (daysUntilDue < 0)
            {
                notifications.Add(new NotificationItem
                {
                    Icon = "exclamation",
                    Message = "Applications are past due!",
                    Type = "danger"
                });
            }

            var userId = currentUser.UserId;
            var userType = currentUser.userType?.ToLower() ?? "";
            var isAdmin = currentUser.isAdmin;

            // Faculty-specific notifications
            if (userType == "faculty" || userType == "chair")
            {
                var savedDrafts = await _context.FormTable
                    .CountAsync(f => f.UserId == userId && f.ApplicationStatus == "Saved");

                if (savedDrafts > 0)
                {
                    notifications.Add(new NotificationItem
                    {
                        Icon = "draft",
                        Message = $"You have {savedDrafts} saved draft{(savedDrafts != 1 ? "s" : "")} to complete.",
                        Type = "info"
                    });
                }

                var pendingApps = await _context.FormTable
                    .CountAsync(f => f.UserId == userId &&
                        (f.ApplicationStatus == "PendingDeptChair" || f.ApplicationStatus == "PendingCommittee"));

                if (pendingApps > 0)
                {
                    notifications.Add(new NotificationItem
                    {
                        Icon = "review",
                        Message = $"{pendingApps} application{(pendingApps != 1 ? "s" : "")} under review.",
                        Type = "info"
                    });
                }
            }

            // Dept Chair — applications awaiting their review
            if (userType == "chair")
            {
                var deptId = currentUser.DepartmentId;
                var pendingReview = await (from f in _context.FormTable
                    join pi in _context.Users on f.PrincipalInvestigatorID equals pi.UserId
                    where f.ApplicationStatus == "PendingDeptChair" && pi.DepartmentId == deptId
                    select f.Id).CountAsync();

                if (pendingReview > 0)
                {
                    notifications.Add(new NotificationItem
                    {
                        Icon = "action",
                        Message = $"{pendingReview} application{(pendingReview != 1 ? "s" : "")} awaiting your department review.",
                        Type = "warning"
                    });
                }
            }

            // Committee Member / Committee Chair — pending reviews
            if (userType == "committee" || currentUser.committeeMemberStatus?.ToLower() == "active")
            {
                var pendingReviews = await _context.Reviews
                    .CountAsync(r => r.ReviewerId == userId && r.ReviewDone == false);

                if (pendingReviews > 0)
                {
                    notifications.Add(new NotificationItem
                    {
                        Icon = "action",
                        Message = $"{pendingReviews} review{(pendingReviews != 1 ? "s" : "")} assigned to you.",
                        Type = "warning"
                    });
                }
            }

            return View(notifications);
        }
    }

    public class NotificationItem
    {
        public string Icon { get; set; } = "info";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "info"; // info, warning, danger
    }
}
