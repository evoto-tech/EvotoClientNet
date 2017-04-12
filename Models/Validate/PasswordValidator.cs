using FluentValidation;
using Models.Forms;

namespace Models.Validate
{
    public abstract class PasswordValidator<T> : AbstractValidator<T> where T : PasswordForm
    {       
        protected void ValidPassword()
        {
            RuleFor(x => x.Password).Length(6, 30)
                .WithMessage("Invalid Password (must have between 6 and 30 chars)");
        }
    }
}