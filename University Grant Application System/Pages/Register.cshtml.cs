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

    public List<SelectListItem> SchoolOptions { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDropdowns();
    }

    private async Task LoadDropdowns()
    {
        var colleges = await _context.Colleges.ToListAsync();
        var departments = await _context.Departments.ToListAsync();
        var schools = await _context.Schools.ToListAsync();

        try
        {
            CollegeOptions = colleges
              .Select(c => new SelectListItem {
                Value = c.CollegeId.ToString(),
                Text = c.CollegeName
              }).ToList();

            DepartmentOptions = departments
              .Select(c => new SelectListItem
              {
                  Value = c.DepartmentId.ToString(),
                  Text = c.DepartmentName
              }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Database query failed: " + ex.Message);
        }

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
            HashedPassword = hashedPassword,
            AccountID = Input.AccountIndex,
            CollegeId = Input.SelectedCollegeId,
            SchoolId = Input.SelectedSchoolId,
            DepartmentId = Input.SelectedDepartmentId,
            userType = "Teacher"
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();



        return RedirectToPage("/Index");
    }

    public async Task<JsonResult> OnGetSchoolsAsync(int collegeId)
    {
        var schools = await _context.Schools
            .Where(s => s.CollegeId == collegeId)
            .Select(s => new { s.SchoolId, s.SchoolName })
            .ToListAsync();
        return new JsonResult(schools);
    }

    public async Task<JsonResult> OnGetDepartmentsAsync(int schoolId)
    {
        var departments = await _context.Departments
            .Where(s => s.SchoolId == schoolId)
            .Select(s => new { s.DepartmentId, s.DepartmentName })
            .ToListAsync();
        return new JsonResult(departments);
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
        public int SelectedCollegeId { get; set; }

        [Required(ErrorMessage = "Please select a department")]
        [Display(Name = "Department")]
        public int SelectedDepartmentId { get; set; }

        [Required(ErrorMessage = "Please select a school")]
        [Display(Name = "School")]
        public int SelectedSchoolId { get; set; }
    }
}
