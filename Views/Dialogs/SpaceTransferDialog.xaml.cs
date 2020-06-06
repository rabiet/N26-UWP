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
    public sealed partial class SpaceTransferDialog : ContentDialog
    {
        APIHelper api;
        List<Space> spaces = new List<Space>();
        public Space fromSpace;
        public Space toSpace;
        public double amount;
        public SpaceTransferDialog(APIHelper apihelper)
        {
            this.InitializeComponent();
            api = apihelper;
            putSpaces();
        }

        private async void putSpaces()
        {
            spaces = await api.LoadSpaces();
            foreach (Space space in spaces)
            {
                FromCombo.Items.Add(space.name);
                ToCombo.Items.Add(space.name);
            }
            FromCombo.SelectedIndex = 0;
            ToCombo.SelectedIndex = 1;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            fromSpace = spaces[FromCombo.SelectedIndex];
            toSpace = spaces[ToCombo.SelectedIndex];
            amount = double.Parse(AmountBox.Text);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
