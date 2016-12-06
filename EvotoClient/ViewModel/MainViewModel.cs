using System;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient.ViewModel
{
    public enum EvotoView
    {
        Login,
        Home
    }

    public class MainViewModel : ViewModelBase
    {
        private readonly LoginViewModel _loginVm = new LoginViewModel();
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

        public void ChangeView(EvotoView view)
        {
            switch (view)
            {
                case EvotoView.Login:
                    CurrentView = ServiceLocator.Current.GetInstance<LoginViewModel>();
                    break;
                case EvotoView.Home:
                    CurrentView = ServiceLocator.Current.GetInstance<HomeViewModel>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(view), view, null);
            }
        }
    }
}