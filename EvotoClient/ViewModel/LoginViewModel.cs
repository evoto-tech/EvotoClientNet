using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Api;
using Api.Clients;
using Api.Exceptions;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;
using Models;
using Models.Validate;

namespace EvotoClient.ViewModel
{
    public class LoginViewModel : EvotoViewModelBase
    {
        private readonly UserClient _userClient;
        private readonly LoginModelValidator _validator;
        private string _email;

        private string _errorMessage;
        private bool _loading;
        private MainViewModel _mainVm;

        public LoginViewModel()
        {
            _userClient = new UserClient();
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

            Loading = true;
            ErrorMessage = "";
            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        await _userClient.LoginAsync(loginModel.Email, loginModel.Password);
                        var userDetails = await _userClient.GetCurrentUserDetails();
                        MainVm.InvokeLogin(this, userDetails);
                        MainVm.ChangeView(EvotoView.Home);
                    }
                    catch (IncorrectLoginException)
                    {
                        Ui(() => {
                            ErrorMessage = "Invalid Username or Password";
                            Loading = false;
                        });
                    }
                    catch (ApiException e)
                    {
                        Ui(() =>
                        {
                            ErrorMessage = "An Unknown Error Occurred";
#if DEBUG
                            // Just override debug message, as ELSE gives annoying compiler warnings
                            ErrorMessage = e.Message;
#endif
                            Loading = false;
                        });
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