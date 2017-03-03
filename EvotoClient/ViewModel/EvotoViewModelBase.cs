using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient.ViewModel
{
    public abstract class EvotoViewModelBase : ViewModelBase
    {
        private MainViewModel _mainVm;

        protected EvotoViewModelBase()
        {
            if (this is MainViewModel)
                _mainVm = (MainViewModel) this;
        }

        protected MainViewModel MainVm => _mainVm ?? (_mainVm = GetVm<MainViewModel>());

        protected void Ui(OnUiThreadDelegate uiDelegate)
        {
            Application.Current.Dispatcher.Invoke(uiDelegate);
        }

        protected T GetVm<T>() where T : EvotoViewModelBase
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        protected delegate void OnUiThreadDelegate();
    }
}