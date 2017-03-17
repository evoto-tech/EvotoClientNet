using System.Security;

namespace Models.Forms
{
    public interface IHavePassword
    {
        SecureString SecurePassword { get; }
    }
}