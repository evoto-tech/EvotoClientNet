using FluentValidation;
using Models.Forms;

namespace Models.Validate
{
    public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordModel>
    {
        public ResetPasswordModelValidator()
        {
            RuleFor(x => x.Password).Length(6, 30).Must(LoginModelValidator.ValidPassword);
            RuleFor(x => x.Token).NotEmpty();
        }
    }
}