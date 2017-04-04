using System.Windows;
using System.Windows.Controls;
using EvotoClient.ViewModel;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for PostVoteView.xaml
    /// </summary>
    public partial class PostVoteView : UserControl
    {
        public PostVoteView()
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