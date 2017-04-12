using FluentValidation;
using Models.Forms;

namespace Models.Validate
{
    public class ResetPasswordModelValidator : PasswordValidator<ResetPasswordModel>
    {
        public ResetPasswordModelValidator()
        {
            ValidPassword();
            RuleFor(x => x.Password).Matches(x => x.ConfirmPassword).WithMessage("Passwords must match");
            RuleFor(x => x.Token).NotEmpty()
                .WithMessage("Invalid Token");
        }
    }
}