using System.Windows;
using System.Windows.Controls;
using EvotoClient.ViewModel;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for FindVote.xaml
    /// </summary>
    public partial class FindVote : UserControl
    {
        public FindVote()
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