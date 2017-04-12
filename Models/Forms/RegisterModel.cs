using System.Collections.Generic;

namespace Models.Forms
{
    public class RegisterModel : PasswordForm
    {
        public RegisterModel(string email, string password, string confirmPassword)
        {
            Email = email?.Trim();
            Password = password?.Trim();
            ConfirmPassword = confirmPassword?.Trim();
        }

        public string Email { get; }
        public string ConfirmPassword { get; }

        public List<CustomUserField> CustomFields { get; set; }
    }
}