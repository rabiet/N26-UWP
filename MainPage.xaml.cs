﻿using N26.Classes;
using N26.Views;
using N26.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace N26
{
    public sealed partial class MainPage : Page
    {
        APIHelper api;
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            api = (APIHelper)e.Parameter;
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            mainNavView.SelectedItem = accountItem;
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            if (args.IsSettingsSelected)
            {
                contentFrame.Navigate(typeof(SettingsPage), api);
                mainNavView.PaneTitle = "Settings";
                return;
            }
            switch (item.Tag)
            {
                case "account":
                    contentFrame.Navigate(typeof(TransactionsPage), api);
                    mainNavView.PaneTitle = "Account";
                    break;
                case "spaces":
                    contentFrame.Navigate(typeof(SpacesPage), api);
                    mainNavView.PaneTitle = "Spaces";
                    break;
                case "credit":
                    Debug.WriteLine("Not implemented yet");
                    break;
            }
            contentFrame.BackStack.Clear();
        }
    }
}
