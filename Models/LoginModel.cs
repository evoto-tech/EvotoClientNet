namespace Models
{
    public class LoginModel
    {
        public LoginModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; }
        public string Password { get; }
    }
}