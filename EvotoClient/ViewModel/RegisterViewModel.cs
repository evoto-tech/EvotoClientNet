using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register, CanRegister);
            ReturnToLoginCommand = new RelayCommand(BackToLogin);
            CanSubmit = CanExecuteChanged;
        }

        private MainViewModel _mainVm;
        private MainViewModel MainVm => _mainVm ?? (_mainVm = ServiceLocator.Current.GetInstance<MainViewModel>());

        public RelayCommand RegisterCommand { get; }
        public RelayCommand ReturnToLoginCommand { get; }

        public KeyEventHandler CanSubmit { get; }

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set { Set(ref _loading, value); }
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set { Set(ref _firstName, value); }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set { Set(ref _lastName, value); }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { Set(ref _email, value); }
        }

        private string _idNumber;

        public string IdNumber
        {
            get { return _idNumber; }
            set { Set(ref _idNumber, value); }
        }

        private bool CanRegister()
        {
            return (Email?.Length > 0) && !Loading;
        }

        private void CanExecuteChanged(object sender, KeyEventArgs e)
        {
            RegisterCommand.RaiseCanExecuteChanged();
        }


        private void Register()
        {
            Loading = true;

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                MainVm.ChangeView(EvotoView.Home);
            });
        }

        private void BackToLogin()
        {
            MainVm.ChangeView(EvotoView.Login);
        }
    }
}