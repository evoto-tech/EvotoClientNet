using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using EvotoClient.Transitions;
using EvotoClient.ViewModel;

namespace EvotoClient.Views
{
    public partial class PageTransition : UserControl
    {
        public static readonly DependencyProperty TransitionTypeProperty = DependencyProperty.Register("TransitionType",
            typeof(PageTransitionType),
            typeof(PageTransition), new PropertyMetadata(PageTransitionType.SlideAndFade));

        private readonly Stack<EvotoViewModelBase> pages = new Stack<EvotoViewModelBase>();

        private static bool _forwards = true;

        public PageTransition()
        {
            InitializeComponent();
        }

        public PageTransitionType TransitionType
        {
            get { return (PageTransitionType) GetValue(TransitionTypeProperty); }
            set { SetValue(TransitionTypeProperty, value); }
        }

        public void ShowPage(EvotoViewModelBase newPage, bool forwards = true)
        {
            _forwards = forwards;
            pages.Push(newPage);

            Task.Factory.StartNew(ShowNewPage);
        }

        private void ShowNewPage()
        {
            Dispatcher.Invoke(delegate
            {
                if (contentPresenter.Content != null)
                {
                    var oldPage = contentPresenter.Content as EvotoViewModelBase;

                    if (oldPage != null)
                    {
                        UnloadPage();
                    }
                }
                else
                {
                    ShowNextPage();
                }
            });
        }

        private void ShowNextPage()
        {
            var newPage = pages.Pop();

            contentPresenter.Content = newPage;

            var showNewPage = Resources[$"{TransitionType}In"] as Storyboard;

            if (showNewPage == null)
            {
                var direction = (_forwards) ? "Right" : "Left";
                showNewPage = Resources[$"{TransitionType}In{direction}"] as Storyboard;
            }

            if (showNewPage == null)
            {
                Debug.WriteLine("Transition not found!");
                return;
            }

            showNewPage.Begin(contentPresenter);
        }

        private void UnloadPage()
        {
            var transition = Resources[$"{TransitionType}Out"] as Storyboard;

            if (transition == null)
            {
                var direction = (!_forwards) ? "Right" : "Left";
                transition = Resources[$"{TransitionType}Out{direction}"] as Storyboard;
            }

            if (transition == null)
            {
                Debug.WriteLine("Transition not found");
                return;
            }

            var hidePage = transition.Clone();

            hidePage.Completed += hidePage_Completed;

            hidePage.Begin(contentPresenter);
        }

        private void hidePage_Completed(object sender, EventArgs e)
        {
            contentPresenter.Content = null;

            ShowNextPage();
        }
    }
}