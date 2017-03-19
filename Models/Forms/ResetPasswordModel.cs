namespace Models.Forms
{
    public class ResetPasswordModel
    {
        public ResetPasswordModel(string email, string password, string confirmPassword, string token)
        {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
            Token = token;
        }
        
        public string Email { get; }
        public string Password { get; }
        public string ConfirmPassword { get; }
        public string Token { get; }
    }
}