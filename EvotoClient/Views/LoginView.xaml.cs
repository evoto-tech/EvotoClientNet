using System.Security;
using System.Windows.Controls;
using Models;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, IHavePassword
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public SecureString SecurePassword => Password.SecurePassword;
    }
}