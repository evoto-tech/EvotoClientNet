using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using Api;
using Api.Clients;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;
using Models;

namespace EvotoClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly LoginClient _loginClient;
        private string _errorMessage;
        private bool _loading;
        private MainViewModel _mainVm;
        private string _email;

        public LoginViewModel()
        {
            RegisterCommand = new RelayCommand(DoRegister, CanRegister);
            _loginClient = new LoginClient();
            LoginCommand = new RelayCommand<object>(Login, CanLogin);
            CanSubmit = CanExecuteChanged;
        }

        private MainViewModel MainVm => _mainVm ?? (_mainVm = ServiceLocator.Current.GetInstance<MainViewModel>());

        public RelayCommand<object> LoginCommand { get; }

        public RelayCommand RegisterCommand { get; }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                Set(ref _loading, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { Set(ref _errorMessage, value); }
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
            var passwordContainer = parameter as IHavePassword;
            if (passwordContainer == null)
                return;

            Loading = true;

            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        ErrorMessage = "";
                        await _loginClient.Login(Username, ConvertToUnsecureString(passwordContainer.SecurePassword));
                    }
                    catch (ApiException e)
                    {
                        if (e.StatusCode == HttpStatusCode.Forbidden)
                            ErrorMessage = "Invalid Username or Password";
                        else
                            ErrorMessage = "An Unknown Error Occurred";
                        Loading = false;
                    }
                });
        }

        private void DoRegister()
        {
            MainVm.ChangeView(EvotoView.Register);
        }

        private bool CanLogin(object data)
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

        public string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null) return string.Empty;

            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        //TODO: Pull from registrar
        public bool RegisterEnabled => true;
    }
}