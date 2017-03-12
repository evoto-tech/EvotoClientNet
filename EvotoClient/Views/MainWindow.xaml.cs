using System.Windows;
using EvotoClient.ViewModel;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        ///     Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            ((EvotoViewModelBase)DataContext).ViewLoaded(sender, e);
        }
    }
}