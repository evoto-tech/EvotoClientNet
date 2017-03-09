using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
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

        protected void Ui(Action uiDelegate)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(uiDelegate);
        }

        protected T GetVm<T>() where T : EvotoViewModelBase
        {
            return ServiceLocator.Current.GetInstance<T>();
        }
    }
}