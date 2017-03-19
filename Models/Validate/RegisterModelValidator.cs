using FluentValidation;
using Models.Forms;

namespace Models.Validate
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleFor(x => x.FirstName).Length(1, 20);
            RuleFor(x => x.LastName).Length(1, 20);
            RuleFor(x => x.CompanyId).Length(2, 10);
            RuleFor(x => x.Password).Length(6, 30).Must(LoginModelValidator.ValidPassword);
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid Email Address")
                .NotEmpty().WithMessage("Invalid Email Address");
        }
    }
}