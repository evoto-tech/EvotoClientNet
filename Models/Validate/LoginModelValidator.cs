using FluentValidation;

namespace Models.Validate
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Password).Length(6, 30).Must(ValidPassword);
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid Email Address")
                .NotEmpty().WithMessage("Invalid Email Address");
        }

        public static bool ValidPassword(string password)
        {
            return true;
        }
    }
}