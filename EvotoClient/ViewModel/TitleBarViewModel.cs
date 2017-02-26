using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace EvotoClient.ViewModel
{
    public class TitleBarViewModel : ViewModelBase
    {
        private bool _maximizeVisible;

        private bool _minimizeVisible;

        public TitleBarViewModel()
        {
            ToggleWindowStateCommand = new RelayCommand(ToggleWindowState);
            MinimizeCommand = new RelayCommand(MinimizeWindow);
            MaximizeCommand = new RelayCommand(MaximizeWindow);
            RestoreCommand = new RelayCommand(RestoreWindow);
            CloseCommand = new RelayCommand(ExitApplication);
            DragWindowCommand = new RelayCommand(DragWindow);

            if ((Application.Current.MainWindow != null) &&
                (Application.Current.MainWindow.WindowState == WindowState.Maximized))
                MinimizeVisible = true;
            else
                MaximizeVisible = true;
        }

        public string Title
        {
            get { return "Evoto"; }
        }

        public RelayCommand ToggleWindowStateCommand { get; }
        public RelayCommand MinimizeCommand { get; }
        public RelayCommand MaximizeCommand { get; }
        public RelayCommand RestoreCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand DragWindowCommand { get; }

        public bool MaximizeVisible
        {
            get { return _maximizeVisible; }
            set { Set(ref _maximizeVisible, value); }
        }

        public bool MinimizeVisible
        {
            get { return _minimizeVisible; }
            set { Set(ref _minimizeVisible, value); }
        }

        private static void DragWindow()
        {
            Application.Current.MainWindow.DragMove();
        }

        private void ToggleWindowState()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                RestoreWindow();
            else
                MaximizeWindow();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.DragMove();
        }

        private static void MinimizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;

            MaximizeVisible = false;
            MinimizeVisible = true;
        }

        private void RestoreWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;

            MaximizeVisible = true;
            MinimizeVisible = false;
        }

        private static void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}