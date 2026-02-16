using System.ComponentModel.DataAnnotations;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt.Net;
using Humanizer;


public class IndexModel : PageModel
{
    private readonly University_Grant_Application_SystemContext _context;
    public IndexModel(University_Grant_Application_SystemContext context)
    {
        _context = context;
    }


    [BindProperty]
    public LoginInputModel Input { get; set; }
    public string ErrorMessage { get; set; }

    public void OnGetAsync()
    {

    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) // Fail
        {
            return Page();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == Input.Email.ToLower());
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(Input.Password, user.HashedPassword);

        if (!isPasswordValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();

        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("UserId", user.UserId.ToString()),
            new Claim("UserType", user.userType),
            new Claim("IsAdmin", user.isAdmin.ToString()),
            new Claim("CommitteeStatus", user.committeeMemberStatus ?? "none")
        };

        if (user.isAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "admin"));
        }
        var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddMinutes(5)
        };
        await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);



        if (user.isAdmin)
        {
            return RedirectToPage("/AdminDashboard/Index");
        }

        string status = user.committeeMemberStatus?.ToLower();

        if (status == "member")
        {
            return RedirectToPage("/ComMemberDashboard/ComMemberDashboard");
        }

        if (status == "chair")
        {
            return RedirectToPage("/CommitteeDashboard/CommitteeDashboard");
        }

        if (user.userType == "chair")
        {
            return RedirectToPage("/DeptChairDashboard/DeptChairDashboard");
        }

        return RedirectToPage("/FacultyDashboard/Index");
    }

    // Class to hold form inputs
    public class LoginInputModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}