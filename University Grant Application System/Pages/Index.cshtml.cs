using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    [BindProperty]
    public LoginInputModel Input { get; set; }
    public string ErrorMessage { get; set; }

    public void OnGet()
    {

    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) // Fail
        {
            return Page();
        }

        //Success
        // TODO: Compare against a user database
        // TODO: Redirect to dashboard based on role

        return RedirectToPage("/AdminDashboard/Index");
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
