using System.Security;

namespace Models.Forms
{
    public interface IHavePasswords
    {
        SecureString SecurePassword { get; }
        SecureString SecurePasswordConfirm { get; }
    }
}