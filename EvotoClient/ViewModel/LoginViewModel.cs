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
        private MainViewModel _mainVm;
        private string _email;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(DoLogin, CanLogin);
            RegisterCommand = new RelayCommand(DoRegister, CanRegister);
            CanSubmit = CanExecuteChanged;
        }

        private MainViewModel MainVm => _mainVm ?? (_mainVm = ServiceLocator.Current.GetInstance<MainViewModel>());

        public RelayCommand LoginCommand { get; }

        public RelayCommand RegisterCommand { get; }

        public bool Loading
        {
            get { return _loading; }
            set { Set(ref _loading, value); }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                Set(ref _email, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public KeyEventHandler CanSubmit { get; }

        private void DoLogin()
        {
            Loading = true;

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                MainVm.ChangeView(EvotoView.Home);
            });
        }

        private void DoRegister()
        {
            MainVm.ChangeView(EvotoView.Register);
        }

        private bool CanLogin()
        {
            return (Email?.Length > 0) && !Loading;
        }

        private bool CanRegister()
        {
            return RegisterEnabled;
        }

        private void CanExecuteChanged(object sender, KeyEventArgs e)
        {
            LoginCommand.RaiseCanExecuteChanged();
        }

        //TODO: Pull from registrar
        public bool RegisterEnabled => true;
    }
}