using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Api.Clients;
using Api.Exceptions;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;
using Models.Forms;
using Models.Validate;

namespace EvotoClient.ViewModel
{
    public class LoginViewModel : EvotoViewModelBase
    {
        private readonly UserClient _userClient;
        private readonly LoginModelValidator _validator;

        public LoginViewModel()
        {
            _userClient = new UserClient();
            _validator = new LoginModelValidator();

            RegisterCommand = new RelayCommand(DoRegister);
            LoginCommand = new RelayCommand<object>(DoLogin);
            ForgotPasswordCommand = new RelayCommand(DoForgotPassword);
            ResendCommand = new RelayCommand(DoResend);

            if (IsLoaded)
                OnLoad(this, null);
            else
                Loaded += OnLoad;
        }

        #region Commands 

        public RelayCommand<object> LoginCommand { get; }

        public RelayCommand RegisterCommand { get; }

        public RelayCommand ForgotPasswordCommand { get; }

        public RelayCommand ResendCommand { get; }

        #endregion

        #region Properties

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                Set(ref _loading, value);
                RaisePropertyChanged(nameof(CanSubmit));
            }
        }

        private bool _registerLoading;

        public bool RegisterLoading
        {
            get { return _registerLoading; }
            set { Set(ref _registerLoading, value); }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { Set(ref _errorMessage, value); }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { Set(ref _email, value); }
        }

        private bool _registerEnabled;

        public bool RegisterEnabled
        {
            get { return _registerEnabled; }
            set { Set(ref _registerEnabled, value); }
        }

        private bool _showConfirmEmail;

        public bool ShowConfirmEmail
        {
            get { return _showConfirmEmail; }
            set { Set(ref _showConfirmEmail, value); }
        }

        private string _emailToken;

        public string EmailToken
        {
            get { return _emailToken; }
            set { Set(ref _emailToken, value); }
        }

        public bool CanSubmit => !Loading;

        #endregion

        #region Methods

        public void OnLoad(object sender, EventArgs e)
        {
            RegisterLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    var canRegister = await _userClient.RegisterEnabled();

                    Ui(() =>
                    {
                        RegisterEnabled = canRegister;
                        RegisterLoading = false;
                    });
                }
                catch (ApiException)
                {
                    Ui(() =>
                    {
                        RegisterEnabled = true;
                        RegisterLoading = false;
                    });
                }
            });
        }

        private bool IsFormValid(object parameter, out LoginModel loginModel)
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

            if (ShowConfirmEmail)
                if (string.IsNullOrWhiteSpace(EmailToken))
                {
                    errorMessages.Add("Email Token is required");
                    valid = false;
                }

            if (!valid)
                ErrorMessage = string.Join("\n", errorMessages);
            return valid;
        }

        private void DoLogin(object parameter)
        {
            LoginModel loginModel;
            if (!IsFormValid(parameter, out loginModel))
                return;

            MainVm.LoggedIn = false;
            Loading = true;
            ErrorMessage = "";
            Task.Run(async () =>
            {
                try
                {
                    // Verify email first
                    if (ShowConfirmEmail)
                    {
                        var model = new VerifyEmailModel(Email, EmailToken);
                        await _userClient.VerifyEmail(model);
                    }
                    await _userClient.LoginAsync(loginModel.Email, loginModel.Password);
                    var userDetails = await _userClient.GetCurrentUserDetails();

                    ShowConfirmEmail = false;
                    Loading = false;
                    Email = "";

                    MainVm.ChangeView(EvotoView.Home);
                    MainVm.LoggedIn = true;
                    MainVm.InvokeLogin(this, userDetails);
                }
                catch (IncorrectLoginException)
                {
                    Ui(() =>
                    {
                        ErrorMessage = "Invalid Username or Password";
                        Loading = false;
                    });
                }
                catch (EmailVerificationNeededException)
                {
                    Ui(() =>
                    {
                        ShowConfirmEmail = true;
                        ErrorMessage =
                            "This email is unconfirmed. Please enter the verification token sent to this email.";
                        Loading = false;
                    });
                }
                catch (UnauthorizedException)
                {
                    Ui(() =>
                    {
                        ErrorMessage = "Invalid Token";
                        Loading = false;
                    });
                }
                catch (ApiException e)
                {
                    Ui(() =>
                    {
                        ErrorMessage = "An Unknown Error Occurred";
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

        private void DoForgotPassword()
        {
            ErrorMessage = "";
            if (!string.IsNullOrWhiteSpace(Email))
            {
                var forgotPasswordVM = ServiceLocator.Current.GetInstance<ForgotPasswordViewModel>();
                forgotPasswordVM.SetEmail(Email);
            }
            MainVm.ChangeView(EvotoView.ForgotPassword);
        }

        private void DoResend()
        {
            if (!ShowConfirmEmail)
                return;

            Loading = true;
            ErrorMessage = "";
            Task.Run(async () =>
            {
                try
                {
                    await _userClient.ResendVerificationEmail(Email);

                    Ui(() =>
                    {
                        ErrorMessage = "Email sent!";
                        Loading = false;
                    });
                }
                catch (TokenDelayException e)
                {
                    Ui(() =>
                    {
                        ErrorMessage =
                            $"Please wait {e.Message} before sending another email. Be sure to check your spam folder";
                        Loading = false;
                    });
                }
                catch (ApiException)
                {
                    Ui(() =>
                    {
                        ErrorMessage = "Could not resend verification email";
                        Loading = false;
                    });
                }
            });
        }

        public static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                return string.Empty;

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

        public void VerifyEmail(string email)
        {
            Email = email;
            ShowConfirmEmail = true;
            ErrorMessage = "Please enter your email verification token that was sent to the above email address.";
        }

        public void SetToken(string email, string token)
        {
            Email = email;
            ShowConfirmEmail = true;
            EmailToken = token;
        }

        #endregion
    }
}