using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Clients;
using Api.Exceptions;
using GalaSoft.MvvmLight.Command;
using Models.Forms;
using Models.Validate;

namespace EvotoClient.ViewModel
{
    public class ResetPasswordViewModel : EvotoViewModelBase
    {
        private readonly UserClient _userClient;
        private readonly ResetPasswordModelValidator _validator;

        public ResetPasswordViewModel()
        {
            _validator = new ResetPasswordModelValidator();
            _userClient = new UserClient();
            ResetCommand = new RelayCommand<object>(DoSendEmail);
            BackCommand = new RelayCommand(DoBack);
            ReturnToLoginCommand = new RelayCommand(BackToLogin);
        }

        #region Commands

        public RelayCommand<object> ResetCommand { get; }

        public RelayCommand ReturnToLoginCommand { get; }

        public RelayCommand BackCommand { get; }

        #endregion

        #region Properties

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                Set(ref _loading, value);
                ResetCommand.RaiseCanExecuteChanged();
            }
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

        public string _token;

        public string Token
        {
            get { return _token; }
            set { Set(ref _token, value); }
        }

        #endregion

        #region Methods

        private bool IsFormValid(object parameter, out ResetPasswordModel model)
        {
            model = null;
            var valid = true;
            var errorMessages = new List<string>();

            var passwordContainer = parameter as IHavePasswords;
            if (passwordContainer == null)
                return false;

            var p1 = LoginViewModel.ConvertToUnsecureString(passwordContainer.SecurePassword);
            var p2 = LoginViewModel.ConvertToUnsecureString(passwordContainer.SecurePasswordConfirm);

            if (p1 != p2)
            {
                errorMessages.Add("Passwords do not match");
                valid = false;
            }

            model = new ResetPasswordModel(Email, p1, p2, Token);
            var v = _validator.Validate(model);
            if (!v.IsValid)
            {
                errorMessages.AddRange(v.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            if (!valid)
                ErrorMessage = string.Join("\n", errorMessages);
            return valid;
        }

        private void DoSendEmail(object parameter)
        {
            ResetPasswordModel resetPasswordModel;
            if (!IsFormValid(parameter, out resetPasswordModel))
                return;

            Loading = true;
            ErrorMessage = "";
            Task.Run(async () =>
            {
                try
                {
                    await _userClient.ResetPassword(resetPasswordModel);
                    MainVm.ChangeView(EvotoView.Home);

                    Ui(() =>
                    {
                        Loading = false;
                    });
                }
                catch (BadRequestException e)
                {
                    Ui(() =>
                    {
                        ErrorMessage = e.Message;
                        Loading = false;
                    });
                }
                catch (ApiException)
                {
                    Ui(() =>
                    {
                        ErrorMessage = "An Unknown Error Occurred";
                        Loading = false;
                    });
                }
            });
        }

        private void BackToLogin()
        {
            MainVm.ChangeView(EvotoView.Login);
        }

        private void DoBack()
        {
            MainVm.ChangeView(EvotoView.ForgotPassword);
        }

        public void SetToken(string token)
        {
            Ui(() =>
            {
                Token = token;
            });
        }

        public void SetEmail(string email)
        {
            Ui(() =>
            {
                Email = email;
            });
        }

        #endregion
    }
}