using System.Windows;
using System.Windows.Controls;
using EvotoClient.ViewModel;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Description for CustomUserFieldView.
    /// </summary>
    public partial class CustomUserFieldView : UserControl
    {
        /// <summary>
        ///     Initializes a new instance of the CustomUserFieldView class.
        /// </summary>
        public CustomUserFieldView()
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