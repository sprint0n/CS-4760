using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

public class RegisterModel : PageModel
{
    [BindProperty]
    public RegisterInputModel Input { get; set; }

    public List<SelectListItem> CollegeOptions { get; set; }
    public List<SelectListItem> DepartmentOptions { get; set; }

    public void OnGet()
    {
        LoadDropdowns();
    }

    private void LoadDropdowns()
    {
        // TODO: Load these from a database

        CollegeOptions = new List<SelectListItem>
        {
            new SelectListItem("Engineering", "Engineering"),
            new SelectListItem("Arts & Sciences", "Arts & Sciences"),
            new SelectListItem("Business", "Business"),
            new SelectListItem("Health", "Health")
        };

        DepartmentOptions = new List<SelectListItem>
        {
            new SelectListItem("Computer Science", "Computer Science"),
            new SelectListItem("Mathematics", "Mathematics"),
            new SelectListItem("Physics", "Physics"),
            new SelectListItem("Biology", "Biology"),
            new SelectListItem("Marketing", "Marketing")
        };
    }

    public IActionResult OnPost()
    {
        LoadDropdowns();

        if (!ModelState.IsValid) // Fail
        {
            return Page();
        }

        // Success
        // TODO: Save user to database

        return RedirectToPage("/Index");
    }

    //Class to hold form inputs
    public class RegisterInputModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Account index must be a 6-digit number")]
        [Display(Name = "Account Index")]
        public string AccountIndex { get; set; }

        [Required(ErrorMessage = "Please select a college")]
        [Display(Name = "College")]
        public string SelectedCollege { get; set; }

        [Required(ErrorMessage = "Please select a department")]
        [Display(Name = "Department")]
        public string SelectedDepartment { get; set; }
    }
}
