namespace Models.Forms
{
    public class ResetPasswordModel : PasswordForm
    {
        public ResetPasswordModel(string email, string password, string confirmPassword, string token)
        {
            Email = email.Trim();
            Password = password.Trim();
            ConfirmPassword = confirmPassword.Trim();
            Token = token.Trim();
        }
        
        public string Email { get; }
        public string ConfirmPassword { get; }
        public string Token { get; }
    }
}