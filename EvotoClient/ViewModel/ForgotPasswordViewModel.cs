using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Clients;
using Api.Exceptions;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Models.Forms;
using Models.Validate;

namespace EvotoClient.ViewModel
{
    public class ForgotPasswordViewModel : EvotoViewModelBase
    {
        private readonly UserClient _userClient;
        private readonly ForgotPasswordModelValidator _validator;

        public ForgotPasswordViewModel()
        {
            _validator = new ForgotPasswordModelValidator();
            _userClient = new UserClient();
            SendEmailCommand = new RelayCommand(DoSendEmail);
            ContinueCommand = new RelayCommand(DoContinue);
            ReturnToLoginCommand = new RelayCommand(BackToLogin);
        }

        #region Commands

        public RelayCommand SendEmailCommand { get; }

        public RelayCommand ReturnToLoginCommand { get; }

        public RelayCommand ContinueCommand { get; }

        #endregion

        #region Properties

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                Set(ref _loading, value);
                SendEmailCommand.RaiseCanExecuteChanged();
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

        #endregion

        #region Methods

        private bool IsFormValid(out ForgotPasswordModel model)
        {
            var valid = true;
            var errorMessages = new List<string>();

            model = new ForgotPasswordModel(Email);
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

        private void DoSendEmail()
        {
            ForgotPasswordModel forgotPasswordModel;
            if (!IsFormValid(out forgotPasswordModel))
                return;

            Loading = true;
            ErrorMessage = "";
            Task.Run(async () =>
            {
                try
                {
                    await _userClient.ForgotPassword(forgotPasswordModel);
                    var resetVm = ServiceLocator.Current.GetInstance<ResetPasswordViewModel>();
                    resetVm.SetEmail(Email);
                    MainVm.ChangeView(EvotoView.ResetPassword);

                    Ui(() =>
                    {
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
                catch (UnconfirmedEmailException)
                {
                    Ui(() =>
                    {
                        ErrorMessage = "This email has not been verified. Please contact an administrator.";
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

        private void DoContinue()
        {
            MainVm.ChangeView(EvotoView.ResetPassword);
            if (!string.IsNullOrWhiteSpace(Email))
            {
                var resetVm = ServiceLocator.Current.GetInstance<ResetPasswordViewModel>();
                resetVm.SetEmail(Email, false);
            }
        }

        public void SetEmail(string email)
        {
            Email = email;
        }

        #endregion
    }
}