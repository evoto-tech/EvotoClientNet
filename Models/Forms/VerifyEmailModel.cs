namespace Models.Forms
{
    public class VerifyEmailModel
    {
        public VerifyEmailModel(string email, string token)
        {
            Email = email;
            Token = token;
        }
        
        public string Email { get; }
        public string Token { get; }
    }
}