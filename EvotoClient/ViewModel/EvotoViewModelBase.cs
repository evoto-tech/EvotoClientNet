using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient.ViewModel
{
    public abstract class EvotoViewModelBase : ViewModelBase
    {
        private MainViewModel _mainVm;

        public event EventHandler<EventArgs> Loaded;

        public bool IsLoaded { get; private set; }

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

        public void ViewLoaded(object sender, EventArgs e)
        {
            Loaded?.Invoke(sender, e);
            IsLoaded = true;
        }
    }
}