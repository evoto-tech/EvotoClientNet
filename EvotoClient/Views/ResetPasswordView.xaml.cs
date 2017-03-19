using System.Security;
using System.Windows.Controls;
using Models.Forms;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for ResetPasswordView.xaml
    /// </summary>
    public partial class ResetPasswordView : UserControl, IHavePasswords
    {
        public ResetPasswordView()
        {
            InitializeComponent();
        }

        public SecureString SecurePassword => Password.SecurePassword;

        public SecureString SecurePasswordConfirm => PasswordConfirm.SecurePassword;
    }
}