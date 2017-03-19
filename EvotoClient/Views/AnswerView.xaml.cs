using System.Windows;
using System.Windows.Controls;
using EvotoClient.ViewModel;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for AnswerView.xaml
    /// </summary>
    public partial class AnswerView : UserControl
    {
        public AnswerView()
        {
            InitializeComponent();
            Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            ((EvotoViewModelBase) DataContext).ViewLoaded(sender, e);
        }
    }
}