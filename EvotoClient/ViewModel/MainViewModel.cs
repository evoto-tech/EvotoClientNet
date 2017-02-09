using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using Models;

namespace EvotoClient.ViewModel
{
    public enum EvotoView
    {
        Login,
        Home,
        Register
    }

    public class MainViewModel : EvotoViewModelBase
    {
        private readonly LoginViewModel _loginVm = ServiceLocator.Current.GetInstance<LoginViewModel>();
        private ViewModelBase _currentView;

        public MainViewModel()
        {
            CurrentView = _loginVm;
        }

        public MultiChainViewModel MultiChainVm { get; } = new MultiChainViewModel();

        public ViewModelBase CurrentView
        {
            get { return _currentView; }
            set { Set(ref _currentView, value); }
        }

        public void InvokeLogin(object caller, UserDetails details)
        {
            OnLogin?.Invoke(caller, details);
        }

        public event EventHandler<UserDetails> OnLogin;

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
                    default:
                        throw new ArgumentOutOfRangeException(nameof(view), view, null);
                }
            });
        }
    }
}