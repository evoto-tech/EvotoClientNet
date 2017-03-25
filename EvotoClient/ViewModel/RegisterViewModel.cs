using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Clients;
using Api.Exceptions;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Models;
using Models.Forms;
using Models.Validate;

namespace EvotoClient.ViewModel
{
    public class RegisterViewModel : EvotoViewModelBase
    {
        private readonly UserClient _userClient;
        private readonly RegisterModelValidator _validator;

        public RegisterViewModel()
        {
            _validator = new RegisterModelValidator();
            _userClient = new UserClient();
            RegisterCommand = new RelayCommand<object>(Register);
            ReturnToLoginCommand = new RelayCommand(BackToLogin);

            CustomFields = new ObservableRangeCollection<CustomUserFieldViewModel>();

            Loaded += (sender, args) => { Task.Run(async () => { await LoadCustomFields(); }); };
        }

        #region Commands

        public RelayCommand<object> RegisterCommand { get; }
        public RelayCommand ReturnToLoginCommand { get; }

        #endregion

        #region Properties

        private bool _loading = true;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                Set(ref _loading, value);
                RaisePropertyChanged(nameof(ShowFields));
            }
        }

        public bool ShowFields => !Loading;

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

        public ObservableRangeCollection<CustomUserFieldViewModel> CustomFields { get; }

        #endregion

        #region Methods

        private bool IsFormValid(object parameter, out RegisterModel registerModel)
        {
            registerModel = null;
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

            registerModel = new RegisterModel(Email, p1, p2);
            var v = _validator.Validate(registerModel);
            if (!v.IsValid)
            {
                errorMessages.AddRange(v.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            if (!valid)
                ErrorMessage = string.Join("\n", errorMessages);
            return valid;
        }

        private void Register(object parameter)
        {
            RegisterModel registerModel;
            if (!IsFormValid(parameter, out registerModel))
                return;

            Loading = true;
            ErrorMessage = "";
            Task.Run(async () =>
            {
                try
                {
                    await _userClient.Register(registerModel);
                    MainVm.ChangeView(EvotoView.Login);
                    var loginVm = ServiceLocator.Current.GetInstance<LoginViewModel>();
                    loginVm.VerifyEmail(Email);
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

        private async Task LoadCustomFields()
        {
            Ui(() => { Loading = true; });

            try
            {
                var fields = await _userClient.GetCustomFields();

                Ui(() =>
                {
                    CustomFields.Clear();
                    CustomFields.AddRange(fields.Select(f => new CustomUserFieldViewModel(f)));

                    Loading = false;
                });
            }
            catch (ApiException e)
            {
                Ui(() =>
                {
                    ErrorMessage = e.Message;
                    Loading = false;
                });
            }
        }

        private void BackToLogin()
        {
            MainVm.ChangeView(EvotoView.Login);
        }

        #endregion
    }
}