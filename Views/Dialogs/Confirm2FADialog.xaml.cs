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
    public sealed partial class Confirm2FADialog : ContentDialog
    {
        APIHelper api;
        public Confirm2FADialog(APIHelper apiHelper)
        {
            this.InitializeComponent();
            api = apiHelper;
            WaitForConfirmation();
        }

        private async void WaitForConfirmation()
        {
            if (await api.Complete2FA())
                this.PrimaryButtonText = "Finish";
            else
                this.SecondaryButtonText = "Cancel";
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
