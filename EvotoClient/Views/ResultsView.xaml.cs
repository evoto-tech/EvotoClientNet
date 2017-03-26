using System.Windows;
using System.Windows.Controls;
using EvotoClient.ViewModel;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for ResultsView.xaml
    /// </summary>
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();
            Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            ((EvotoViewModelBase)DataContext).ViewLoaded(sender, e);
        }
    }
}