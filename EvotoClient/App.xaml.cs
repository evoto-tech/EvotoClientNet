using System.Windows;
using EvotoClient.ViewModel;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var vm = ServiceLocator.Current.GetInstance<MultiChainViewModel>();
            if (vm.Connected)
                vm.Cleanup();
        }
    }
}