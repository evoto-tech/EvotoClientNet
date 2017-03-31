using System;
using System.Diagnostics;
using System.Windows;
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

        private string _status;

        public string Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        #endregion

        #region Methods

        public void InvokeLogin(object caller, UserDetails details)
        {
            OnLogin?.Invoke(caller, details);
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

        public void SetStatus(string status)
        {
            Status = status;
        }

        #endregion
    }
}