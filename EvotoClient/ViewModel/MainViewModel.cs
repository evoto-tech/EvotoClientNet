using System;
using System.Diagnostics;
using Blockchain;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using Models;

namespace EvotoClient.ViewModel
{
    public enum EvotoView
    {
        Login,
        Home,
        Register,
        Vote
    }

    public class MainViewModel : EvotoViewModelBase
    {
        private readonly LoginViewModel _loginVm = ServiceLocator.Current.GetInstance<LoginViewModel>();

        public MainViewModel()
        {
            MultiChainTools.SubDirectory = "client";
            CurrentView = _loginVm;
        }

        #region Events

        public event EventHandler<UserDetails> OnLogin;

        #endregion

        #region Properties

        private ViewModelBase _currentView;

        public ViewModelBase CurrentView
        {
            get { return _currentView; }
            set { Set(ref _currentView, value); }
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
                switch (view)
                {
                    case EvotoView.Login:
                        CurrentView = ServiceLocator.Current.GetInstance<LoginViewModel>();
                        break;
                    case EvotoView.Home:
                        CurrentView = ServiceLocator.Current.GetInstance<HomeViewModel>();
                        break;
                    case EvotoView.Register:
                        CurrentView = ServiceLocator.Current.GetInstance<RegisterViewModel>();
                        break;
                    case EvotoView.Vote:
                        CurrentView = ServiceLocator.Current.GetInstance<VoteViewModel>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(view), view, null);
                }
            });
        }

        #endregion
    }
}