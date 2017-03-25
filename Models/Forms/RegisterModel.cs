namespace Models.Forms
{
    public class RegisterModel
    {
        public RegisterModel(string email, string password, string confirmPassword)
        {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        public string Email { get; }
        public string Password { get; }
        public string ConfirmPassword { get; }
    }
}