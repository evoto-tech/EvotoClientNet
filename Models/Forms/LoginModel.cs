namespace Models.Forms
{
    public class LoginModel : PasswordForm
    {
        public LoginModel(string email, string password)
        {
            Email = email?.Trim();
            Password = password?.Trim();
        }

        public string Email { get; }
    }
}