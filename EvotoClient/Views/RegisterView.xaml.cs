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

        public System.Security.SecureString SecurePassword => Password.SecurePassword;

        public System.Security.SecureString SecurePasswordConfirm => PasswordConfirm.SecurePassword;
    }
}