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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace N26.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TransactionsPage : Page
    {
        APIHelper api;

        public TransactionsPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            api = (APIHelper)e.Parameter;

            CurrentBalanceBlock.Text = (await api.LoadAccount()).availableBalance.ToString("0.00") + "€";

            List<Transaction> transactions = await api.LoadTransactions();
            List<Classes.Containers.Transaction> showTransactions = new List<Classes.Containers.Transaction>();
            foreach (Transaction now in transactions)
            {
                Classes.Containers.Transaction transaction = new Classes.Containers.Transaction();
                transaction.Amount = now.amount.ToString("0.00") + now.currencyCode.Replace("EUR", "€");
                transaction.AmountColor = (now.amount < 0.0) ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                transaction.Name = now.GetName();
                transaction.Date = now.GetDate();
                transaction.ReferenceText = now.GetReference();
                transaction.Category = string.Format("/Assets/Categories-Dark/icon-category-{0}.png", Regex.Split(now.category, "v2-")[1]); // TODO Change these to use SVG once scaling problems are fixed

                showTransactions.Add(transaction);
            }
            MasterDetail.ItemsSource = showTransactions;
        }
    }
}
