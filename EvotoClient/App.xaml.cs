using System;
using System.Collections.Generic;
using System.Windows;
using EvotoClient.ViewModel;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;

namespace EvotoClient
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "Evoto_Client";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                DispatcherHelper.Initialize();
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var vm = ServiceLocator.Current.GetInstance<MultiChainViewModel>();
            if (vm.Connected)
                vm.Cleanup();
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            CustomUriHandler.HandleArgs(args);

            return true;
        }
    }
}