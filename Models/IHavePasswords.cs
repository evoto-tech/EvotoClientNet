using System.Security;

namespace Models
{
    public interface IHavePasswords
    {
        SecureString SecurePassword { get; }
        SecureString SecurePasswordConfirm { get; }
    }
}