using FluentValidation;
using Models.Forms;

namespace Models.Validate
{
    public class RegisterModelValidator : PasswordValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid Email Address")
                .NotEmpty().WithMessage("Invalid Email Address");
            ValidPassword();
            RuleFor(x => x.Password).Equal(x => x.ConfirmPassword).WithMessage("Passwords must match");
        }
    }
}