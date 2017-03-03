namespace Models
{
    public class RegisterModel
    {
        public RegisterModel(string email, string firstName, string lastName, string companyId, string password,
            string confirmPassword)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            CompanyId = companyId;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string CompanyId { get; }
        public string Password { get; }
        public string ConfirmPassword { get; }
    }
}