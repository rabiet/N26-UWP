using N26.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace N26
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        APIHelper api;
        PersonalInfo personalInfo;

        public LoginPage()
        {
            this.InitializeComponent();
            api = new APIHelper();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            personalInfo = await api.LoadMe();
            if (personalInfo != null)
            {
                WelcomeBlock.Text = string.Format("Welcome back, {0}!", personalInfo.firstName);
                UsernameBox.Text = personalInfo.email;
                CreateAccountButton.Content = "Login using Windows Hello";
            } else
                return;

            var loginCredential = GetCredentialFromLocker();
            if (loginCredential == null)
                return;

            
            UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(string.Format("Welcome back, {0}!", personalInfo.firstName));
            if (consentResult.Equals(UserConsentVerificationResult.Verified))
            {
                if (await api.RenewToken())
                    LoadData();
                else
                {
                    await new MessageDialog("Could not refresh authentication token. Please log in again!").ShowAsync();
                    new StorageHelper().DeleteAll();
                    Frame.Navigate(typeof(LoginPage));
                    Frame.BackStack.Clear();

                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            SendLogin(UsernameBox.Text, PasswordBox.Password);
        }

        private async void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string uriToLaunch = @"https://app.n26.com/forgotten-password";
            var uri = new Uri(uriToLaunch);

            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (personalInfo == null)
            {
                var uri = new Uri(@"https://app.n26.com/register");
                await Windows.System.Launcher.LaunchUriAsync(uri);
            } else {
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync("Please authenticate to log in!");
                if (consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    SendLogin(GetCredentialFromLocker().UserName, GetCredentialFromLocker().Password);
                }
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SendLogin(UsernameBox.Text, PasswordBox.Password);
            }
        }

        private async void SendLogin(string username, string password)
        {
            PasswordVault vault = new PasswordVault();
            vault.Add(new PasswordCredential("N26", username, password));
            ProgressWorking.Visibility = Visibility.Visible;
            if (await api.GetToken(username, password) != true)
            {
                await new MessageDialog("Login failed!").ShowAsync();
                return;
            }
            LoadData();
        }

        private async void LoadData()
        {
            await api.GetMe(true);
            await api.GetAccount(true);
            await api.GetSpaceImages(true);
            await api.GetSpaces(true);
            await api.GetTransactions(true);
            await api.GetSavings(false);
            Frame.Navigate(typeof(MainPage), api);
            Frame.BackStack.Clear();
        }

        private PasswordCredential GetCredentialFromLocker()
        {
            try
            {
                var vault = new PasswordVault();
                var credentialList = vault.FindAllByResource("N26");
                if (credentialList.Count > 0)
                {
                    PasswordCredential credential = credentialList[0];
                    credential.RetrievePassword();
                    return credential;
                }
            }
            catch (Exception) { }
            return null;
        }
    }
}
