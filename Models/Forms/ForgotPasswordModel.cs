namespace Models.Forms
{
    public class ForgotPasswordModel
    {
        public ForgotPasswordModel(string email)
        {
            Email = email?.Trim();
        }

        public string Email { get; }
    }
}