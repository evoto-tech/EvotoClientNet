namespace Models.Forms
{
    public class VerifyEmailModel
    {
        public VerifyEmailModel(string email, string token)
        {
            Email = email.Trim();
            Token = token.Trim();
        }
        
        public string Email { get; }
        public string Token { get; }
    }
}