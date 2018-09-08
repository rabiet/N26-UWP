using N26.Classes;
using System;
using System.Collections.Generic;
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

            List<Transaction> transactions = await api.LoadTransactions();
            List<Classes.Containers.Transaction> showTransactions = new List<Classes.Containers.Transaction>();
            foreach (Transaction now in transactions)
            {
                Classes.Containers.Transaction transaction = new Classes.Containers.Transaction();
                transaction.Amount = now.amount + now.currencyCode;
                transaction.AmountColor = (now.amount < 0.0) ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                transaction.Name = now.GetName();
                transaction.Date = now.GetDate();
                transaction.ReferenceText = now.GetReference();
                transaction.Category = string.Format("/Assets/Categories/icon-category-{0}.png", Regex.Split(now.category, "v2-")[1]); // TODO Change these to use SVG once scaling problems are fixed

                showTransactions.Add(transaction);
            }
            Transactions_ListView.ItemsSource = showTransactions;
        }
    }
}
