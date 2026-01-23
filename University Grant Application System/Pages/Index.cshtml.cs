using System.ComponentModel.DataAnnotations;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;
using BCrypt.Net;


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

        var user = await _context.User
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

        bool isAdmin = user.isAdmin;

        if(isAdmin)
        {
            return RedirectToPage("/AdminDashboard/Index");
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
