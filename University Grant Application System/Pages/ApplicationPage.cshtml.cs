using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations; 

namespace University_Grant_Application_System.Pages.Shared
{
    public class IndexModel : PageModel
    {

        [BindProperty]
        [Required(ErrorMessage = "Please enter your full name")]
        public string ApplicantName { get; set; }

        [BindProperty]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Grant Purpose")]
        public string GrantPurpose { get; set; }

        public void OnGet()
        {

        }


        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {

                return Page();
            }


            return Content("Success! Your application has been submitted.");
        }
    }
}