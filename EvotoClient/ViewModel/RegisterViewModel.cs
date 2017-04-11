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
        private bool _registerDisabled;

        public RegisterViewModel()
        {
            _validator = new RegisterModelValidator();
            _userClient = new UserClient();
            RegisterCommand = new RelayCommand<object>(DoRegister, CanRegister);
            ReturnToLoginCommand = new RelayCommand(BackToLogin);

            CustomFields = new ObservableRangeCollection<CustomUserFieldViewModel>();

            Loaded += (sender, args) => { Task.Run(async () => { await LoadCustomFields(); }); };
        }

        #region Commands

        public RelayCommand<object> RegisterCommand { get; }
        public RelayCommand ReturnToLoginCommand { get; }

        #endregion

        #region Properties

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                Set(ref _loading, value);
                RegisterCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(LoadingSpinner));
            }
        }

        public bool LoadingSpinner => Loading || FieldsLoading;

        private bool _fieldsLoading;

        public bool FieldsLoading
        {
            get { return _fieldsLoading;}
            set
            {
                Set(ref _fieldsLoading, value);
                RaisePropertyChanged(nameof(ShowFields));
                RaisePropertyChanged(nameof(LoadingSpinner));
            }
        }

        public bool ShowFields => !FieldsLoading;

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

            // Start out with list of custom field messages, as they should be first. Maybe
            var errorMessages =
                CustomFields.Where(cf => cf.Required && string.IsNullOrWhiteSpace(cf.Value))
                    .Select(f => $"{f.Name} is Required").ToList();

            var passwordContainer = parameter as IHavePasswords;
            if (passwordContainer == null)
                return false;

            // Pull out our passwords
            var p1 = LoginViewModel.ConvertToUnsecureString(passwordContainer.SecurePassword);
            var p2 = LoginViewModel.ConvertToUnsecureString(passwordContainer.SecurePasswordConfirm);

            // Check they match
            if (p1 != p2)
            {
                errorMessages.Add("Passwords do not match");
                valid = false;
            }

            // Validate the form
            registerModel = new RegisterModel(Email, p1, p2);
            var v = _validator.Validate(registerModel);
            if (!v.IsValid)
            {
                errorMessages.AddRange(v.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            // Display error message(s) if invalid
            if (!valid)
                ErrorMessage = string.Join("\n", errorMessages);
            return valid;
        }

        private bool CanRegister(object _)
        {
            return !FieldsLoading && !_registerDisabled;
        }

        private void DoRegister(object parameter)
        {
            RegisterModel registerModel;
            // Ensure form is valid
            if (!IsFormValid(parameter, out registerModel))
                return;

            registerModel.CustomFields = CustomFields.Select(cf => cf.GetModel()).ToList();

            Loading = true;
            ErrorMessage = "";
            Task.Run(async () =>
            {
                try
                {
                    // Send registration info to API
                    await _userClient.Register(registerModel);

                    // Redirect to login page
                    // TODO: If email verification off, login?
                    MainVm.ChangeView(EvotoView.Login);
                    var loginVm = ServiceLocator.Current.GetInstance<LoginViewModel>();

                    // Autofill their email and ask them to verify it
                    loginVm.VerifyEmail(Email);
                }
                catch (RegisterDisabledException)
                {
                    Ui(() =>
                    {
                        ErrorMessage = "Sorry, registration is not enabled at this time";
                        Loading = false;
                    });
                }
                catch (BadRequestException e)
                {
                    _registerDisabled = true;
                    RegisterCommand.RaiseCanExecuteChanged();
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
            Ui(() => { FieldsLoading = true; });

            try
            {
                // Get custom fields from API
                var fields = await _userClient.GetCustomFields();

                Ui(() =>
                {
                    // Display custom fields
                    CustomFields.Clear();
                    CustomFields.AddRange(fields.Select(f => new CustomUserFieldViewModel(f)));

                    FieldsLoading = false;
                });
            }
            catch (ApiException e)
            {
                Ui(() =>
                {
                    // Oh no!
                    ErrorMessage = e.Message;
                    FieldsLoading = false;
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