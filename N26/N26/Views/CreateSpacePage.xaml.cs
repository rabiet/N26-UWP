using N26.Classes;
using N26.Views.Dialogs;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace N26.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateSpacePage : Page
    {
        APIHelper api;
        SpaceImage image;
        public CreateSpacePage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            api = (APIHelper)e.Parameter;
        }

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SpaceImageDialog dialog = new SpaceImageDialog(api);

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                image = dialog.selectedImage;
                Image.Source = new BitmapImage(new Uri(dialog.selectedImage.url));
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await api.CreateSpace(NameBox.Text, image.id);
            await api.GetSpaces(true);
            Frame.Navigate(typeof(SpacesPage), api);
            Frame.BackStack.Clear();
        }
    }
}
