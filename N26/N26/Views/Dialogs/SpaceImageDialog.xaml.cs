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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace N26.Views.Dialogs
{
    public sealed partial class SpaceImageDialog : ContentDialog
    {
        List<SpaceImage> images = new List<SpaceImage>();
        public SpaceImage selectedImage;
        public SpaceImageDialog(APIHelper apiHelper)
        {
            this.InitializeComponent();
            putImages(apiHelper);
        }

        private async void putImages(APIHelper api)
        {
            images = await api.GetSpaceImages(false);
            List<string> urls = new List<string>();
            foreach (SpaceImage image in images)
                urls.Add(image.url);

            SpaceImagesGridView.ItemsSource = urls;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void SpaceImagesGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedImage = images[SpaceImagesGridView.SelectedIndex];
        }
    }
}
