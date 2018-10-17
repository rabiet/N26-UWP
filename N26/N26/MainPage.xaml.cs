using N26.Classes;
using N26.Views;
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
using MUXC = Microsoft.UI.Xaml.Controls;

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
            
            /*CurrentBalanceBlock.Text = (await api.LoadAccount()).availableBalance + "€";

            List<Space> spaces = await api.LoadSpaces();
            List<Classes.Containers.Space> showSpaces = new List<Classes.Containers.Space>();
            foreach (Space now in spaces)
            {
                Classes.Containers.Space space = new Classes.Containers.Space();
                space.IMGURL = now.image;
                space.Name = now.name;
                space.Balance = now.amount + now.currency;
                showSpaces.Add(space);
            }
            SpacesGridView.ItemsSource = showSpaces;*/
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(TransactionsPage), api);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            switch (item.Tag)
            {
                case "account":
                    contentFrame.Navigate(typeof(TransactionsPage), api);
                    break;
                case "spaces":
                    Debug.WriteLine("Not implemented yet");
                    break;
                case "credit":
                    Debug.WriteLine("Not implemented yet");
                    break;
                case "savings":
                    Debug.WriteLine("Not implemented yet");
                    break;
            }
        }
    }
}
