using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations; 

namespace University_Grant_Application_System.Pages
{
    public class ApplicationPageModel : PageModel
    {


        [BindProperty]
        [Required]
        [Display(Name = "Index number")]
        public string IndexNumber { get; set; }

        //This is for the dropdown menu for Primaryuser || Type of user

        [BindProperty]
        [Required]
        [Display(Name = "User")]
        public string TypeOfUser { get; set; }

        public List<string> UserTypes { get; } = new List<string>
        {
            "PrimaryUser",
            "Student",
            "Faculty",
            "Staff",
            "External Researcher"
        };

        //This is the name for the Procedure
        [BindProperty]
        [Required]
        [Display(Name = "Procedures")]
        public string Procedure { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Please enter in the title")]
        public string GrantTitle { get; set; }


        [BindProperty]
        [Required]
        [Display(Name = "Grant Purpose")]
        public string GrantPurpose { get; set; }

        /// <summary>
        /// This is used for the Humans or Animals checkbox
        /// True opens file upload
        /// </summary>
        [BindProperty]
        public bool HumansOrAnimals { get; set; }





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