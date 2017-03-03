using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient.ViewModel
{
    public abstract class EvotoViewModelBase : ViewModelBase
    {
        protected EvotoViewModelBase()
        {
            if (this is MainViewModel)
                _mainVm = (MainViewModel)this;
        }

        protected void Ui(OnUiThreadDelegate uiDelegate)
        {
            Application.Current.Dispatcher.Invoke(uiDelegate);
        }

        protected delegate void OnUiThreadDelegate();

        private MainViewModel _mainVm;
        protected MainViewModel MainVm => _mainVm ?? (_mainVm = GetVm<MainViewModel>());

        protected T GetVm<T>() where T : EvotoViewModelBase
        {
            return ServiceLocator.Current.GetInstance<T>();
        }
    }
}