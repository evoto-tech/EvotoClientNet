using System.Security;
using System.Windows.Controls;
using Models;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl, IHavePasswords
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        public SecureString SecurePassword => Password.SecurePassword;

        public SecureString SecurePasswordConfirm => PasswordConfirm.SecurePassword;
    }
}