using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Api;
using Api.Clients;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;
using Models;
using Models.Validate;

namespace EvotoClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly LoginClient _loginClient;
        private readonly LoginModelValidator _validator;
        private string _email;

        private string _errorMessage;
        private bool _loading;
        private MainViewModel _mainVm;

        public LoginViewModel()
        {
            _loginClient = new LoginClient();
            _validator = new LoginModelValidator();

            RegisterCommand = new RelayCommand(DoRegister, CanRegister);
            LoginCommand = new RelayCommand<object>(DoLogin);
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

        //TODO: Pull from registrar
        public bool RegisterEnabled => true;

        private bool IsFormValid(object parameter, bool updateErrorMessage, out LoginModel loginModel)
        {
            loginModel = null;
            var valid = true;
            var errorMessages = new List<string>();

            var passwordContainer = parameter as IHavePassword;
            if (passwordContainer == null)
                return false;

            loginModel = new LoginModel(Email, ConvertToUnsecureString(passwordContainer.SecurePassword));
            var v = _validator.Validate(loginModel);
            if (!v.IsValid)
            {
                errorMessages.AddRange(v.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            if (updateErrorMessage && !valid)
                ErrorMessage = string.Join("\n", errorMessages);
            return valid;
        }

        private void DoLogin(object parameter)
        {
            LoginModel loginModel;
            if (!IsFormValid(parameter, true, out loginModel))
                return;

            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        Loading = true;
                        ErrorMessage = "";
                        await _loginClient.Login(loginModel);
                    }
                    catch (ApiException e)
                    {
                        ErrorMessage = e.StatusCode == HttpStatusCode.Forbidden
                            ? "Invalid Username or Password"
                            : "An Unknown Error Occurred";
                        Loading = false;
                    }
                });
        }

        private void DoRegister()
        {
            ErrorMessage = "";
            MainVm.ChangeView(EvotoView.Register);
        }

        private bool CanRegister()
        {
            return RegisterEnabled;
        }

        public static string ConvertToUnsecureString(SecureString securePassword)
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
    }
}