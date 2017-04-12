using System;
using System.Collections.Generic;
using System.Linq;
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
        private static string[] _args;

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // Skip first argument (filename.exe)
            CustomUriHandler.HandleArgs(args.Skip(1).ToList());

            return true;
        }

        [STAThread]
        public static void Main(string[] args)
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                DispatcherHelper.Initialize();
                var application = new App();

                _args = args;

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        public void HandleArgsCallback()
        {
            if (_args != null)
                CustomUriHandler.HandleArgs(_args);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var vm = ServiceLocator.Current.GetInstance<MultiChainViewModel>();
            if (vm.Connected)
                vm.Cleanup();
        }
    }
}