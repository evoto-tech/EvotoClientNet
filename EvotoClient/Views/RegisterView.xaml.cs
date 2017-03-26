using System.Security;
using System.Windows;
using System.Windows.Controls;
using EvotoClient.ViewModel;
using Models;
using Models.Forms;

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
            Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            ((EvotoViewModelBase)DataContext).ViewLoaded(sender, e);
        }

        public SecureString SecurePassword => Password.SecurePassword;

        public SecureString SecurePasswordConfirm => PasswordConfirm.SecurePassword;
    }
}