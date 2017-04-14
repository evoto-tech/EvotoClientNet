using GalaSoft.MvvmLight.Command;
using Models;

namespace EvotoClient.ViewModel
{
    public class UserBarViewModel : EvotoViewModelBase
    {
        public UserBarViewModel()
        {
            LogoutCommand = new RelayCommand(DoLogout, CanLogout);

            MainVm.OnLogin += (sender, details) => { UpdateDetails(details); };
        }

        #region Commands

        public RelayCommand LogoutCommand { get; }

        #endregion

        #region Methods

        private void UpdateDetails(UserDetails details)
        {
            Ui(() => { Email = $"Logged in as: {details.Email}"; });
        }

        private void DoLogout()
        {
            Email = "";
            MainVm.Logout(this);
        }

        private bool CanLogout()
        {
            return !LogoutDisabled;
        }

        #endregion

        #region Properties

        private string _email;

        public string Email
        {
            get { return _email; }
            set { Set(ref _email, value); }
        }

        private bool _logoutDisabled;

        public bool LogoutDisabled
        {
            get { return _logoutDisabled; }
            set
            {
                Set(ref _logoutDisabled, value);
                LogoutCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion
    }
}