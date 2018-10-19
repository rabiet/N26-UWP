using N26.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace N26.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpacesPage : Page
    {
        APIHelper api;
        public SpacesPage()
        {
            this.InitializeComponent();

            
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            api = (APIHelper)e.Parameter;

            SpacesTotalBalanceBlock.Text = (await api.LoadSpacesTotalBalance()).ToString("0.00") + "€";

            List<Space> spaces = await api.LoadSpaces();
            List<Classes.Containers.Space> showSpaces = new List<Classes.Containers.Space>();
            foreach (Space now in spaces)
            {
                Classes.Containers.Space space = new Classes.Containers.Space();
                space.IMGURL = now.image;
                space.Name = now.name;
                space.Balance = now.amount.ToString("0.00") + now.currency.Replace("EUR", "€");
                showSpaces.Add(space);
            }
            SpacesGridView.ItemsSource = showSpaces;
        }

        private void SpacesGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                ItemsWrapGrid appItemsPanel = (ItemsWrapGrid)SpacesGridView.ItemsPanelRoot;
                appItemsPanel.ItemWidth = (e.NewSize.Width) / ((int)e.NewSize.Width / 200);
            } catch {
                Debug.WriteLine("GridViewItem Size could not be changed");
            }
        }
    }
}
