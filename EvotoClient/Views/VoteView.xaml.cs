﻿using System.Windows;
using System.Windows.Controls;
using EvotoClient.ViewModel;

namespace EvotoClient.Views
{
    /// <summary>
    ///     Interaction logic for VoteView.xaml
    /// </summary>
    public partial class VoteView : UserControl
    {
        public VoteView()
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