using N26.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
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
    public sealed partial class SettingsPage : Page
    {
        APIHelper api;
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            api = (APIHelper)e.Parameter;

            SavingsAnswer answer = await api.LoadSavings();

            Account account = await api.LoadAccount();
            PersonalInfo personalInfo = await api.LoadMe();

            UserNameTextBlock.Text = string.Format("{0} {1}", personalInfo.firstName, personalInfo.lastName);
            ibanTextBlock.Text = System.Text.RegularExpressions.Regex.Replace(account.iban, ".{4}", "$0 ");
            bicTextBlock.Text = account.bic;

            WithdrawalsThisMonth.Text = string.Format("{0}/{1} free ATM withdrawals", await api.remainingMonthlyATMWithdrawals(), await api.maxMonthlyATMWithdrawals());
        }

        private void IbanCopyButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage ibanPackage = new DataPackage();
            ibanPackage.SetText(ibanTextBlock.Text);
            Clipboard.SetContent(ibanPackage);
        }

        private void BicCopyButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage bicPackage = new DataPackage();
            bicPackage.SetText(bicTextBlock.Text);
            Clipboard.SetContent(bicPackage);
        }
    }
}
