using N26.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

            CurrentBalanceBlock.Text = (await api.LoadAccount()).availableBalance + "€";

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
            SpacesGridView.ItemsSource = showSpaces;
        }
    }
}
