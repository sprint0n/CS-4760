namespace University_Grant_Application_System.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName {  get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;
        public string College { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int AccountID { get; set; } = 0;

        public string HashedPassword { get; set; } = string.Empty;

        public DateOnly Birthday { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddYears(-20));

        public bool isAdmin { get; set; } = false;
    }
}
