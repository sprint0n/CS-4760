using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_Grant_Application_System.Data;
using University_Grant_Application_System.Models;
using BCrypt.Net;

public class RegisterModel : PageModel
{
    private readonly University_Grant_Application_SystemContext _context;

    public RegisterModel(University_Grant_Application_SystemContext context)
    {
        _context = context;
    }

    [BindProperty]
    public RegisterInputModel Input { get; set; }

    public List<SelectListItem> CollegeOptions { get; set; }
    public List<SelectListItem> DepartmentOptions { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDropdowns();
    }

    private async Task LoadDropdowns()
    {
        var collegeList = new List<string> { "Engineering", "Mathematics" };
        var departmentList = new List<string> { "Computer Science", "Statistics" };

        try
        {
            var dbColleges = await _context.User
                .Select(u => u.College)
                .Distinct()
                .ToListAsync();

            var dbDepts = await _context.User
                .Select(u => u.Department)
                .Distinct()
                .ToListAsync();

            collegeList = collegeList.Union(dbColleges).ToList();
            departmentList = departmentList.Union(dbDepts).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Database query failed: " + ex.Message);
        }

        CollegeOptions = collegeList.Where(x => !string.IsNullOrEmpty(x))
            .Select(c => new SelectListItem(c, c)).ToList();
        DepartmentOptions = departmentList.Where(x => !string.IsNullOrEmpty(x))
            .Select(d => new SelectListItem(d, d)).ToList();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        

        if (!ModelState.IsValid) // Fail
        {
            await LoadDropdowns();
            return Page();
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.Password);

        var newUser = new User
        {
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Birthday = Input.Birthday,
            Email = Input.Email,
            Department = Input.SelectedDepartment,
            College = Input.SelectedCollege,
            HashedPassword = hashedPassword,
            School = "Placeholder",
            AccountID = Input.AccountIndex
        };

        _context.User.Add(newUser);
        await _context.SaveChangesAsync();



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
        public DateOnly Birthday { get; set; }

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
        public int AccountIndex { get; set; }

        [Required(ErrorMessage = "Please select a college")]
        [Display(Name = "College")]
        public string SelectedCollege { get; set; }

        [Required(ErrorMessage = "Please select a department")]
        [Display(Name = "Department")]
        public string SelectedDepartment { get; set; }
    }
}
