using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Api;
using Blockchain;
using Microsoft.Practices.ServiceLocation;
using Models;

namespace EvotoClient.ViewModel
{
    public enum EvotoView
    {
        Login,
        Register,
        Vote,
        PostVote,
        FindVote,
        Results,
        ForgotPassword,
        ResetPassword,
        Home
    }

    public class MainViewModel : EvotoViewModelBase
    {
        private readonly LoginViewModel _loginVm = ServiceLocator.Current.GetInstance<LoginViewModel>();

        public MainViewModel()
        {
            MultiChainTools.SubDirectory = "client";
            CurrentView = _loginVm;

            Loaded += (sender, args) =>
            {
                var app = Application.Current as App;
                app?.HandleArgsCallback();
            };
        }

        #region Events

        public event EventHandler<UserDetails> OnLogin;

        public event EventHandler OnLogout;

        #endregion

        #region Properties

        private EvotoViewModelBase _currentView;

        public EvotoViewModelBase CurrentView
        {
            get { return _currentView; }
            set { Set(ref _currentView, value); }
        }

        private bool _loggedIn;

        public bool LoggedIn
        {
            get { return _loggedIn; }
            set { Set(ref _loggedIn, value); }
        }

        #endregion

        #region Methods

        public void Login(object caller, UserDetails details)
        {
            LoggedIn = true;
            ChangeView(EvotoView.Home);
            OnLogin?.Invoke(caller, details);
        }

        public void Logout(object caller)
        {
            LoggedIn = false;
            ApiClient.ClearAuth();
            MainVm.ChangeView(EvotoView.Login);
            OnLogout?.Invoke(caller, EventArgs.Empty);
        }

        public void ChangeView(EvotoView view)
        {
            Debug.WriteLine($"Changing view to: {view}");
            Ui(() =>
            {
                EvotoViewModelBase newView;
                switch (view)
                {
                    case EvotoView.Login:
                        newView = ServiceLocator.Current.GetInstance<LoginViewModel>();
                        break;
                    case EvotoView.Register:
                        newView = ServiceLocator.Current.GetInstance<RegisterViewModel>();
                        break;
                    case EvotoView.ForgotPassword:
                        newView = ServiceLocator.Current.GetInstance<ForgotPasswordViewModel>();
                        break;
                    case EvotoView.ResetPassword:
                        newView = ServiceLocator.Current.GetInstance<ResetPasswordViewModel>();
                        break;
                    case EvotoView.Home:
                        newView = ServiceLocator.Current.GetInstance<HomeViewModel>();
                        break;
                    case EvotoView.Vote:
                        newView = ServiceLocator.Current.GetInstance<VoteViewModel>();
                        break;
                    case EvotoView.PostVote:
                        newView = ServiceLocator.Current.GetInstance<PostVoteViewModel>();
                        break;
                    case EvotoView.FindVote:
                        newView = ServiceLocator.Current.GetInstance<FindVoteViewModel>();
                        break;
                    case EvotoView.Results:
                        newView = ServiceLocator.Current.GetInstance<ResultsViewModel>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(view), view, null);
                }

                // Check we're not already on the right view before switching
                if (CurrentView != newView)
                    CurrentView = newView;
            });
        }

        #endregion
    }
}