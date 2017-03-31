using System.Windows;
using System.Windows.Controls;
using EvotoClient.ViewModel;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for ChartView.xaml
    /// </summary>
    public partial class ChartView : UserControl
    {
        public ChartView()
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