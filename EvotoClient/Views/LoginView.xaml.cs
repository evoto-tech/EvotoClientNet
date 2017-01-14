using System.Windows.Controls;
using EvotoClient.ViewModel;
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

        public System.Security.SecureString SecurePassword => Password.SecurePassword;
    }
}