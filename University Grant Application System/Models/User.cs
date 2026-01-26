using System.ComponentModel.DataAnnotations;

namespace University_Grant_Application_System.Models
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName {  get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;
        public string College { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public int AccountID { get; set; } = 0;

        public string HashedPassword { get; set; } = string.Empty;

        public DateOnly Birthday { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddYears(-20));

        public bool isAdmin { get; set; } = false;
    }
}
