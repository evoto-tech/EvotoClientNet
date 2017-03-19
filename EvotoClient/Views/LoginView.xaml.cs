using System.Security;
using System.Windows;
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
            Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            ((EvotoViewModelBase)DataContext).ViewLoaded(sender, e);
        }

        public SecureString SecurePassword => Password.SecurePassword;
    }
}