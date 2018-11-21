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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace N26.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SavingsInvestPage : Page
    {
        APIHelper api;
        public SavingsInvestPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            api = (APIHelper)e.Parameter;

            SavingsAnswer answer = await api.LoadSavings();

            MasterDetail.ItemsSource = answer.Accounts;
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Uri link = (Uri)((HyperlinkButton)sender).Tag;
            await Windows.System.Launcher.LaunchUriAsync(link);
        }
    }
}
