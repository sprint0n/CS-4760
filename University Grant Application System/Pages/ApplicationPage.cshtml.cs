using System.ComponentModel.DataAnnotations; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;

namespace University_Grant_Application_System.Pages
{
    public class ApplicationPageModel : PageModel
    {
        private readonly University_Grant_Application_SystemContext _context;

        public ApplicationPageModel(University_Grant_Application_SystemContext context)
        {
            _context = context;
        }


        [BindProperty]
        [Required]
        [Display(Name = "Index number")]
        public int IndexNumber { get; set; }

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
        [Required(ErrorMessage = "Primary Investigator is required")]
        public string PrimaryInvestigator { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Please enter in the title")]
        public string GrantTitle { get; set; }


        [BindProperty]
        [Required]
        [Display(Name = "Grant Purpose")]
        public string GrantPurpose { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Timeline")]
        public string Timeline { get; set; }


        /// <summary>
        /// This is used for the Humans or Animals checkbox
        /// True opens file upload
        /// </summary>
        [BindProperty]
        public bool HumansOrAnimals { get; set; }

        [BindProperty]
        public IFormFile UploadFile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userEmail = User.Identity?.Name;

            if (userEmail != null)
            {
                var currentUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userEmail);
                
                if (currentUser != null)
                {
                    PrimaryInvestigator = $"{currentUser.FirstName} {currentUser.LastName}";
                    IndexNumber = currentUser.AccountID;
                }
            }
            return Page();
        }


        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (UploadFile != null && UploadFile.Length > 0)
            {
                var uniqueName = $"{UploadFile.FileName}_{Guid.NewGuid()}";
                var uploadPath = Path.Combine("wwwroot/uploads", uniqueName);

                using (var stream = System.IO.File.Create(uploadPath))
                {
                    await UploadFile.CopyToAsync(stream);
                }

                TempData["UploadSuccess"] = $"Successfully uploaded: {UploadFile.FileName}";
            }

            // TODO: Save the application data to the database here
            // var application = new Application { ... };
            // _context.Applications.Add(application);
            // await _context.SaveChangesAsync();

            return Content("Success! Your application has been submitted.");
        }

    }
}