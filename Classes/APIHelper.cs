﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.Threading;

namespace N26.Classes
{
    public class APIHelper
    {
        private string Token;
        private string RefreshToken;
        private string TokenType;
        private bool authenticated = false;
        
        private string mfaToken = "";
        DateTime mfaTimestamp;


        public APIHelper()
        {
            loadAPI();
        }

        private async void loadAPI()
        {
            try
            {
                JObject token = JObject.Parse(await new StorageHelper().ReadValue("authentication"));
                if (token == null)
                {
                    await new StorageHelper().DeleteAll();
                    Debug.WriteLine("Could not load authentication values from storage. Reseting everything...");
                    return;
                }
                RefreshToken = token.GetValue("refresh_token").ToString();
                Token = token.GetValue("access_token").ToString();
                TokenType = token.GetValue("token_type").ToString();
            } catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
        }

        public async Task<bool> Start2FA(string username, string password)
        {
            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                body.Add(new KeyValuePair<string, string>("grant_type", "password"));
                body.Add(new KeyValuePair<string, string>("username", username));
                body.Add(new KeyValuePair<string, string>("password", password));
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Basic bXktdHJ1c3RlZC13ZHBDbGllbnQ6c2VjcmV0");
                var response = await client.PostAsync(new Uri("https://api.tech26.de/oauth/token"), new HttpFormUrlEncodedContent(body));
                Debug.WriteLine("Response:\n" + response.Content.ToString());
                JObject jResponse = JObject.Parse(response.Content.ToString());

                int status = (int) jResponse.GetValue("status");

                if (status != 403 || !jResponse.GetValue("type").ToString().Equals("mfa_required"))
                {
                    Debug.WriteLine("Something went wrong when starting the 2FA process");
                    return false;
                }

                mfaToken = jResponse.GetValue("mfaToken").ToString();

                return await Approve2FA();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }

        private async Task<bool> Approve2FA()
        {
            try
            {
                JObject data = new JObject();
                data.Add("challengeType", "oob");   // TODO implement SMS Approval
                data.Add("mfaToken", mfaToken);

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Basic bXktdHJ1c3RlZC13ZHBDbGllbnQ6c2VjcmV0");
                var response = await client.PostAsync(new Uri("https://api.tech26.de/api/mfa/challenge"), new HttpStringContent(data.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json"));
                Debug.WriteLine("Response:\n" + response.Content.ToString());
                mfaTimestamp = DateTime.Now;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }

        public async Task<bool> Complete2FA()
        {
            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                body.Add(new KeyValuePair<string, string>("grant_type", "mfa_oob"));
                body.Add(new KeyValuePair<string, string>("mfaToken", mfaToken));
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Basic bXktdHJ1c3RlZC13ZHBDbGllbnQ6c2VjcmV0");
                var response = await client.PostAsync(new Uri("https://api.tech26.de/oauth/token"), new HttpFormUrlEncodedContent(body));
                Debug.WriteLine("Response:\n" + response.Content.ToString());

                JObject jResponse = JObject.Parse(response.Content.ToString());

                if (!jResponse.ContainsKey("access_token"))
                {
                    if (!jResponse.GetValue("error").ToString().Equals("authorization_pending"))
                        return false;

                    if ((DateTime.Now - mfaTimestamp).TotalMinutes > 1.0)
                        return false;

                    Thread.Sleep(5000);
                    return await Complete2FA();
                }

                await new StorageHelper().WriteValue("authentication", response.Content.ToString());

                Token = jResponse.GetValue("access_token").ToString();
                TokenType = jResponse.GetValue("token_type").ToString();
                RefreshToken = jResponse.GetValue("refresh_token").ToString();
                authenticated = true;

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }

        public async Task<bool> RenewToken()
        {
            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                body.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
                body.Add(new KeyValuePair<string, string>("refresh_token", RefreshToken));
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Basic bXktdHJ1c3RlZC13ZHBDbGllbnQ6c2VjcmV0");
                DateTime RequestTime = DateTime.Now;
                var response = await client.PostAsync(new Uri("https://api.tech26.de/oauth/token"), new HttpFormUrlEncodedContent(body));
                Debug.WriteLine("Response:\n" + response.Content.ToString());
                await new StorageHelper().WriteValue("authentication", response.Content.ToString());

                JObject jResponse = JObject.Parse(response.Content.ToString());

                Token = jResponse.GetValue("access_token").ToString();
                TokenType = jResponse.GetValue("token_type").ToString();
                authenticated = true;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }

        public async Task<Account> GetAccount(bool onlyCache)
        {
            if (!authenticated)
                return null;
            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                DateTime RequestTime = DateTime.Now;
                var response = await client.GetAsync(new Uri("https://api.tech26.de/api/accounts"));
                Debug.WriteLine("Response:\n" + response.Content);
                await new StorageHelper().WriteValue("account", response.Content.ToString());

                if (onlyCache)
                    return null;

                JObject jResponse = JObject.Parse(response.Content.ToString());

                Account account = new Account();
                account.id = jResponse.GetValue("id").ToString();
                account.iban = jResponse.GetValue("iban").ToString();
                account.bic = jResponse.GetValue("bic").ToString();
                account.bankName = jResponse.GetValue("bankName").ToString();
                account.availableBalance = double.Parse(jResponse.GetValue("availableBalance").ToString());
                account.usableBalance = double.Parse(jResponse.GetValue("usableBalance").ToString());
                account.bankBalance = double.Parse(jResponse.GetValue("bankBalance").ToString());

                return account;
            } catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
            return null;
        }

        public async Task<List<Limit>> GetLimits(bool onlyCache)
        {
            if (!authenticated)
                return null;
            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                DateTime RequestTime = DateTime.Now;
                var response = await client.GetAsync(new Uri("https://api.tech26.de/api/settings/account/limits"));
                Debug.WriteLine("Response:\n" + response.Content);
                await new StorageHelper().WriteValue("limits", response.Content.ToString());

                if (onlyCache)
                    return null;

                JArray jResponse = JArray.Parse(response.Content.ToString());
                List<Limit> limitsList = new List<Limit>();
                foreach (JObject limit in jResponse)
                    limitsList.Add(new Limit(limit));
                return limitsList;
            } catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return null;
        }

        public async Task<PersonalInfo> GetMe(bool onlyCache)
        {
            if (!authenticated)
                return null;
            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                DateTime RequestTime = DateTime.Now;
                var response = await client.GetAsync(new Uri("https://api.tech26.de/api/me"));
                Debug.WriteLine("Response:\n" + response.Content);
                await new StorageHelper().WriteValue("personalinfo", response.Content.ToString());

                if (onlyCache)
                    return null;

                JObject jResponse = JObject.Parse(response.Content.ToString());

                return new PersonalInfo(JObject.Parse(response.Content.ToString()));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return null;
        }

        #region Spaces

        public async Task<List<Space>> GetSpaces(bool onlyCache)
        {
            if (!authenticated)
                return null;
            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                DateTime RequestTime = DateTime.Now;
                var response = await client.GetAsync(new Uri("https://api.tech26.de/api/spaces"));
                Debug.WriteLine("Response:\n" + response.Content);
                await new StorageHelper().WriteValue("spaces", response.Content.ToString());

                if (onlyCache)
                    return null;

                JObject jResponse = JObject.Parse(response.Content.ToString());
                List<Space> returnList = new List<Space>();
                JArray spaces = (JArray)jResponse.GetValue("spaces");
                foreach (JObject spac in spaces)
                {
                    Space space = new Space();
                    space.id = spac.GetValue("id").ToString();
                    space.accountID = spac.GetValue("accountID").ToString();
                    space.name = spac.GetValue("name").ToString();
                    space.image = spac.GetValue("imageUrl").ToString();

                    JObject balance = (JObject)spac.GetValue("balance");
                    space.amount = (double) balance.GetValue("availableBalance");
                    space.currency = balance.GetValue("currency").ToString();
                    space.overdraftAmount = (double) balance.GetValue("overdraftAmount");

                    space.isPrimary = (bool)spac.GetValue("isPrimary");
                    space.isCardAttached = (bool)spac.GetValue("isCardAttached");

                    returnList.Add(space);
                }

                return returnList;
            } catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
            return null;
        }

        public async Task<List<SpaceImage>> GetSpaceImages(bool onlyCache)
        {
            if (!authenticated)
                return null;
            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                DateTime RequestTime = DateTime.Now;
                var response = await client.GetAsync(new Uri("https://api.tech26.de/api/spaces/creationDetails"));
                Debug.WriteLine("Response:\n" + response.Content);
                await new StorageHelper().WriteValue("space-images", response.Content.ToString());

                if (onlyCache)
                    return null;

                List<SpaceImage> returnList = new List<SpaceImage>();
                JArray images = (JArray) (JObject.Parse(response.Content.ToString())).GetValue("images");
                foreach (JObject image in images)
                    returnList.Add(new SpaceImage(image));

                return returnList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return null;
        }

        public async Task<bool> CreateSpace(string name, string imageId)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                HttpStringContent content = new HttpStringContent(string.Format("{{\"name\":\"{0}\",\"imageId\":\"{1}\"}}", name, imageId));
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                DateTime RequestTime = DateTime.Now;
                var response = await client.PostAsync(new Uri("https://api.tech26.de/api/spaces"), content);
                Debug.WriteLine("Response:\n" + response.Content.ToString());

                if (response.Content.ToString().Contains("Error"))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }

        public async Task<bool> EditSpace(string id, string name, string imageId)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                HttpStringContent content = new HttpStringContent(string.Format("{{\"name\":\"{0}\",\"imageId\":\"{1}\"}}", name, imageId));
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                DateTime RequestTime = DateTime.Now;
                var response = await client.PutAsync(new Uri(string.Format("https://api.tech26.de/api/spaces/{0}", id)), content);
                Debug.WriteLine("Response:\n" + response.Content.ToString());

                if (response.Content.ToString().Contains("Error"))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }

        public async Task<bool> DeleteSpace(string id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                DateTime RequestTime = DateTime.Now;
                var response = await client.DeleteAsync(new Uri(string.Format("https://api.tech26.de/api/spaces/{0}", id)));
                Debug.WriteLine("Response:\n" + response.Content.ToString());

                if (response.Content.ToString().Contains("Error"))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }

        public async Task<bool> MakeSpaceTransfer(string fromID, string toID, double amount)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                string body = string.Format("{{\"fromSpaceId\":\"{0}\", \"toSpaceId\":\"{1}\", \"amount\":{2}}}", fromID, toID, amount.ToString("0.00"));
                HttpStringContent content = new HttpStringContent(body);
                content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                Debug.WriteLine(body);
                DateTime RequestTime = DateTime.Now;
                var response = await client.PostAsync(new Uri("https://api.tech26.de/api/spaces/transaction"), content);
                Debug.WriteLine("Response:\n" + response.Content.ToString());
                if (response.Content.ToString().Contains("Error"))
                    return false;

                return true;
            } catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }

            return false;
        }

        #endregion

        public async Task<List<Transaction>> GetTransactions(bool onlyCache, int limit = -1, DateTime from = default(DateTime), DateTime to = default(DateTime))
        {
            if (!authenticated)
                return null;

            try
            {
                List<Transaction> transactions = new List<Transaction>();
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                DateTime RequestTime = DateTime.Now;
                HttpResponseMessage response = null;
                if (limit == -1 && (from.Equals(default(DateTime)) || to.Equals(default(DateTime))))
                    response = await client.GetAsync(new Uri("https://api.tech26.de/api/smrt/transactions"));
                else {
                    if (limit != -1)
                        body.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                    else
                        body.Add(new KeyValuePair<string, string>("limit", int.MaxValue.ToString()));   // If no limit is specified should download ALL the transactions in that timeframe
                    if (!from.Equals(default(DateTime)) && !to.Equals(default(DateTime)))
                    {
                        body.Add(new KeyValuePair<string, string>("from", ((long)(from - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString()));
                        body.Add(new KeyValuePair<string, string>("to", ((long)(to - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString()));
                    }
                    string parameters = "?";
                    foreach (KeyValuePair<string, string> param in body)
                        parameters += string.Format("{0}={1}&", param.Key, param.Value);
                    parameters = parameters.Remove(parameters.Length - 1);
                    Debug.WriteLine(parameters);
                    response = await client.GetAsync(new Uri("https://api.tech26.de/api/smrt/transactions" + parameters));
                }
                Debug.WriteLine("Response:\n" + response.Content);

                await new StorageHelper().WriteValue("transactions", response.Content.ToString());

                if (onlyCache)
                    return null;

                JArray trans = JArray.Parse(response.Content.ToString());
                foreach (JObject transaction in trans)
                    transactions.Add(new Transaction(transaction));

                return transactions;
            }
            catch (FieldAccessException e)
            {
                Debug.WriteLine("Transactions failed:\n" + e.ToString());
            }
            return null;
        }

        public async Task<List<Product>> GetProducts(bool onlyCache)
        {
            if (!authenticated)
                return null;
            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                DateTime RequestTime = DateTime.Now;
                var response = await client.GetAsync(new Uri("https://api.tech26.de/api/users/products"));
                Debug.WriteLine("Response:\n" + response.Content);
                await new StorageHelper().WriteValue("products", response.Content.ToString());

                if (onlyCache)
                    return null;

                List<Product> productList = new List<Product>();
                JArray jResponse = JArray.Parse(response.Content.ToString());

                foreach (JObject now in jResponse)
                    productList.Add(new Product(now));

                return productList;
            } catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return null;
        }

        public async Task<string> GetBranches(bool onlyCache)
        {
            if (!authenticated)
                return null;

            if (await Geolocator.RequestAccessAsync() != GeolocationAccessStatus.Allowed)
                return null;

            Geolocator geolocator = new Geolocator();
            Geoposition pos = await geolocator.GetGeopositionAsync();

            try
            {
                List<KeyValuePair<string, string>> body = new List<KeyValuePair<string, string>>();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", TokenType, Token));
                DateTime RequestTime = DateTime.Now;
                var response = await client.GetAsync(new Uri(string.Format("https://api.tech26.de/api/barzahlen/branches?nelat={0}&nelon={1}&swlat={2}&swlon={3}", pos.Coordinate.Point.Position.Latitude + 0.04, pos.Coordinate.Point.Position.Longitude + 0.04, pos.Coordinate.Point.Position.Latitude - 0.04, pos.Coordinate.Point.Position.Longitude - 0.04)));
                Debug.WriteLine("Response:\n" + response.Content);
                await new StorageHelper().WriteValue("branches", response.Content.ToString());

                if (onlyCache)
                    return null;

                List<Product> productList = new List<Product>();
                JArray jResponse = JArray.Parse(response.Content.ToString());

                foreach (JObject now in jResponse)
                    productList.Add(new Product(now));

                return "";
            } catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return null;
        }

        public async Task<Account> LoadAccount()
        {
            JObject jResponse = JObject.Parse(await new StorageHelper().ReadValue("account"));

            Account account = new Account();
            account.id = jResponse.GetValue("id").ToString();
            account.iban = jResponse.GetValue("iban").ToString();
            account.bic = jResponse.GetValue("bic").ToString();
            account.bankName = jResponse.GetValue("bankName").ToString();
            account.availableBalance = double.Parse(jResponse.GetValue("availableBalance").ToString());
            account.usableBalance = double.Parse(jResponse.GetValue("usableBalance").ToString());
            account.bankBalance = double.Parse(jResponse.GetValue("bankBalance").ToString());

            return account;
        }

        public async Task<List<Limit>> LoadLimits()
        {
            JArray jResponse = JArray.Parse(await new StorageHelper().ReadValue("limits"));
            List<Limit> limitsList = new List<Limit>();
            foreach (JObject limit in jResponse)
                limitsList.Add(new Limit(limit));
            return limitsList;
        }

        public async Task<PersonalInfo> LoadMe()
        {
            try
            {
                JObject jResponse = JObject.Parse(await new StorageHelper().ReadValue("personalinfo"));
                return new PersonalInfo(jResponse);
            } catch {
                return null;
            }
        }

        public async Task<List<Space>> LoadSpaces()
        {
            JObject jResponse = JObject.Parse(await new StorageHelper().ReadValue("spaces"));
            List<Space> returnList = new List<Space>();
            JArray spaces = (JArray)jResponse.GetValue("spaces");
            foreach (JObject spac in spaces)
            {
                JObject balance = (JObject)spac.GetValue("balance");
                Space space = new Space
                {
                    id = spac.GetValue("id").ToString(),
                    accountID = spac.GetValue("accountId").ToString(),
                    name = spac.GetValue("name").ToString(),
                    image = spac.GetValue("imageUrl").ToString(),
                    amount = (double)balance.GetValue("availableBalance"),
                    currency = balance.GetValue("currency").ToString(),
                    isPrimary = (bool)spac.GetValue("isPrimary"),
                    isCardAttached = (bool)spac.GetValue("isCardAttached")
                };

                if (balance.ContainsKey("overdraftAmount"))
                    space.overdraftAmount = (double)balance.GetValue("overdraftAmount");

                returnList.Add(space);
            }

            return returnList;
            
        }

        public async Task<Tuple<int, bool>> LoadSpacesUserFeatures()
        {
            JObject userFeatures = (JObject) JObject.Parse(await new StorageHelper().ReadValue("spaces")).GetValue("userFeatures");
            return new Tuple<int, bool>((int) userFeatures.GetValue("availableSpaces"), (bool) userFeatures.GetValue("canUpgrade"));
        }

        public async Task<double> LoadSpacesTotalBalance()
        {
            JObject spaces = JObject.Parse(await new StorageHelper().ReadValue("spaces"));

            return (double) spaces.GetValue("totalBalance");
        }

        public async Task<List<Transaction>> LoadTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();
            JArray trans = JArray.Parse(await new StorageHelper().ReadValue("transactions"));
            foreach (JObject transaction in trans)
                transactions.Add(new Transaction(transaction));

            return transactions;
        }

        public async Task<List<Product>> LoadProducts()
        {
            List<Product> productList = new List<Product>();
            JArray products = JArray.Parse(await new StorageHelper().ReadValue("products"));

            foreach (JObject now in products)
                productList.Add(new Product(now));

            return productList;
        }

        public async Task<int> maxMonthlyATMWithdrawals()
        {
            foreach (Product now in await LoadProducts())
            {
                Debug.WriteLine(now.productId);
                if (now.productId.Equals("FAIR_USE_ATM"))
                    return now.totalFreeAtms;
            }
            return 0;
        }

        public async Task<int> usedMonthlyATMWithdrawals()
        {
            foreach (Product now in await LoadProducts())
                if (now.productId.Equals("FAIR_USE_ATM"))
                    return now.usedAtms;
            return 0;
        }

        public async Task<int> remainingMonthlyATMWithdrawals()
        {
            return (await maxMonthlyATMWithdrawals()) - (await usedMonthlyATMWithdrawals());
        }

        public async Task<string> LoadImageIdFromUrl(string url)
        {

            List<SpaceImage> returnList = new List<SpaceImage>();
            JArray images = JArray.Parse(await new StorageHelper().ReadValue("space-images"));
            foreach (JObject image in images)
                if (image.GetValue("url").ToString().Equals(url))
                    return image.GetValue("id").ToString();

            return "";
        }
    }
}
