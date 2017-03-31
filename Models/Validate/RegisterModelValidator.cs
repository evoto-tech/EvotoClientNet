using FluentValidation;
using Models.Forms;

namespace Models.Validate
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleFor(x => x.Password).Length(6, 30).Must(LoginModelValidator.ValidPassword);
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid Email Address")
                .NotEmpty().WithMessage("Invalid Email Address");
        }
    }
}