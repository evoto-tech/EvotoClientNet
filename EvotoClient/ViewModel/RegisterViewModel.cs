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
        private bool _registerDisabled;

        public RegisterViewModel()
        {
            _validator = new RegisterModelValidator();
            _userClient = new UserClient();
            RegisterCommand = new RelayCommand<object>(DoRegister);
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
            get { return _fieldsLoading; }
            set
            {
                Set(ref _fieldsLoading, value);
                RaisePropertyChanged(nameof(ShowFields));
                RaisePropertyChanged(nameof(LoadingSpinner));
                RaisePropertyChanged(nameof(CanRegister));
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

        public bool CanRegister => !FieldsLoading && !_registerDisabled;

        #endregion

        #region Methods

        private bool IsFormValid(object parameter, out RegisterModel registerModel)
        {
            registerModel = null;

            var errorMessages = new List<string>();
            var customErrors = false;

            var passwordContainer = parameter as IHavePasswords;
            if (passwordContainer == null)
                return false;

            // Pull out our passwords
            var p1 = LoginViewModel.ConvertToUnsecureString(passwordContainer.SecurePassword);
            var p2 = LoginViewModel.ConvertToUnsecureString(passwordContainer.SecurePasswordConfirm);

            // Validate the form
            registerModel = new RegisterModel(Email, p1, p2);
            var v = _validator.Validate(registerModel);
            if (!v.IsValid)
            {
                // Put the errors into our error list, ensuring email goes first, followed by custom, followed by password
                // This is the same order as the view
                foreach (var msg in v.Errors)
                {
                    if ((msg.PropertyName != nameof(registerModel.Email)) && !customErrors)
                    {
                        customErrors = true;
                        errorMessages.AddRange(GetCustomErrors());
                    }
                    errorMessages.Add(msg.ErrorMessage);
                }
            }

            if (!customErrors)
                errorMessages.AddRange(GetCustomErrors());

            // Display error message(s) if invalid
            if (errorMessages.Any())
            {
                ErrorMessage = string.Join("\n", errorMessages);
                return false;
            }
            ErrorMessage = "";
            return true;
        }

        private IEnumerable<string> GetCustomErrors()
        {
            return CustomFields.Where(cf => cf.Required && string.IsNullOrWhiteSpace(cf.Value))
                .Select(f => $"{f.Name} is Required");
        }

        private void ResetForm(object parameter)
        {
            var passwordContainer = parameter as IHavePasswords;
            if (passwordContainer == null)
                return;

            passwordContainer.SecurePassword.Clear();
            passwordContainer.SecurePasswordConfirm.Clear();

            Email = "";
            foreach (var f in CustomFields)
                f.Value = "";
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
                    MainVm.ChangeView(EvotoView.Login);
                    var loginVm = ServiceLocator.Current.GetInstance<LoginViewModel>();

                    Ui(() =>
                    {
                        Loading = false;
                        ResetForm();
                    });
                    
                    // Autofill their email and ask them to verify it
                    loginVm.VerifyEmail(Email);
                }
                catch (RegisterDisabledException)
                {
                    _registerDisabled = true;
                    RaisePropertyChanged(nameof(CanRegister));
                    Ui(() =>
                    {
                        ErrorMessage = "Sorry, registration is not enabled at this time";
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