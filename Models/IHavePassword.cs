using System.Security;

namespace Models
{
    public interface IHavePassword
    {
        SecureString SecurePassword { get; }
    }
}