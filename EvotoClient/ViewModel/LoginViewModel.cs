using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private bool _loading;

        private string _username;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login, CanLogin);
            CanSubmit = CanExecuteChanged;
        }

        public RelayCommand LoginCommand { get; }

        public bool Loading
        {
            get { return _loading; }
            set { Set(ref _loading, value); }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                Set(ref _username, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public KeyEventHandler CanSubmit { get; }

        private void Login()
        {
            Loading = true;

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
                mainVm.ChangeView(EvotoView.Home);
            });
        }

        private bool CanLogin()
        {
            return (Username?.Length > 0) && !Loading;
        }

        private void CanExecuteChanged(object sender, KeyEventArgs e)
        {
            LoginCommand.RaiseCanExecuteChanged();
        }
    }
}