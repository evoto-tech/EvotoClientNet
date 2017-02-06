using System.Windows;
using GalaSoft.MvvmLight;

namespace Models
{
    public abstract class EvotoViewModelBase : ViewModelBase
    {
        protected void Ui(OnUiThreadDelegate uiDelegate)
        {
            Application.Current.Dispatcher.Invoke(uiDelegate);
        }

        protected delegate void OnUiThreadDelegate();
    }
}