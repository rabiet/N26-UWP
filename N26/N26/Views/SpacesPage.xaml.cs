using N26.Classes;
using N26.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
        List<Space> spaces;
        public SpacesPage()
        {
            this.InitializeComponent();

            
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            api = (APIHelper)e.Parameter;

            SpacesTotalBalanceBlock.Text = (await api.LoadSpacesTotalBalance()).ToString("0.00") + "€";

            spaces = await api.LoadSpaces();
            List<Classes.Containers.Space> showSpaces = new List<Classes.Containers.Space>();
            foreach (Space now in spaces)
            {
                Classes.Containers.Space space = new Classes.Containers.Space();
                space.IMGURL = now.image;
                space.Name = now.name;
                space.ID = now.id;
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

        private async void AddSpaceButton_Click(object sender, RoutedEventArgs e)
        {
            Tuple<int, bool> uF = await api.LoadSpacesUserFeatures();
            if (uF.Item1 == 0)
            {
                if (uF.Item2 == true)
                    await new MessageDialog("With N26 Black and Metal you can make up to 10 spaces to stay more flexible.", "Upgrade for more spaces").ShowAsync();
                else
                    await new MessageDialog("You have reached the maximum of 10 spaces. Unfortunately you cannot upgrade anymore.", "No more upgrades left").ShowAsync(); //TODO Find proper Message

                return;
            }
            Frame.Navigate(typeof(CreateSpacePage), api);
        }

        private async void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            SpaceTransferDialog dialog = new SpaceTransferDialog(api);

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                //await new MessageDialog("From: " + dialog.fromSpace.name + " - " + dialog.fromSpace.accountID + "\nTo: " + dialog.toSpace.name + " - " + dialog.toSpace.accountID + "\nAmount: " + dialog.amount).ShowAsync();
                if (await api.MakeSpaceTransfer(dialog.fromSpace.id, dialog.toSpace.id, dialog.amount) == false)
                {
                    await new MessageDialog("Transaction was not successful!").ShowAsync();
                    return;
                }
                await api.GetTransactions(true);
                await api.GetSpaces(true);
                await api.GetAccount(true);
                Frame.Navigate(typeof(SpacesPage), api);
                Frame.BackStack.Clear();
            }
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            string id = ((MenuFlyoutItem)sender).Tag.ToString();
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = "Edit Space Name";
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Ok";
            dialog.SecondaryButtonText = "Cancel";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                Space tapped = spaces.Find((Space now) => { return now.id.Equals(id); });
                if (await api.EditSpace(id, inputTextBox.Text, tapped.imageId) == false)
                {
                    await new MessageDialog("Could not edit Space").ShowAsync();
                    return;
                }
                await api.GetSpaces(true);
                Frame.Navigate(typeof(SpacesPage), api);
                Frame.BackStack.Clear();
            }
        }
    }
}
