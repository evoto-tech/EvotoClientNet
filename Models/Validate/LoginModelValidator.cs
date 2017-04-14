using FluentValidation;
using Models.Forms;

namespace Models.Validate
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid Email Address")
                .NotEmpty().WithMessage("Invalid Email Address");
            ValidPassword();
        }

        public void ValidPassword()
        {
            RuleFor(x => x.Password).Length(6, 30)
                .WithMessage("Invalid Password (must have between 6 and 30 chars)");
        }
    }
}