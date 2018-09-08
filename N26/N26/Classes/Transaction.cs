using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N26.Classes
{
    class Transaction
    {
        public string id;
        public string userId;
        public string type;
        public double amount;
        public string currencyCode;
        public double originalAmount;
        public string originalCurrency;
        public double exchangeRate;
        public string merchantCity;
        public DateTime visibleTS;
        public long mmc;
        public long mmcGroup;
        public bool recurring;
        public string partnerBic;
        public bool partnerAccountIsSepa;
        public string partnerName;
        public string accountId;
        public string partnerIban;
        public string category;
        public string cardId;
        public string referenceText;
        public DateTime userCertified;
        public bool pending;
        public string transactionNature;
        public string transactionTerminal;
        public DateTime createdTS;
        public long merchantCountry;
        public string mandateId;
        public string creditorIdentifier;
        public string creditorName;
        public string smartLinkId;
        public string linkId;
        public DateTime confirmed;
        public string paymentScheme;
        public string merchantName;

        public Transaction(JObject jObject)
        {
            id = jObject.GetValue("id").ToString();
            userId = jObject.GetValue("userId").ToString();
            type = jObject.GetValue("type").ToString();
            amount = (double)jObject.GetValue("amount");
            currencyCode = jObject.GetValue("currencyCode").ToString();
            if (jObject.ContainsKey("originalAmount"))
                originalAmount = (double)jObject.GetValue("originalAmount");
            if (jObject.ContainsKey("originalCurrency"))
                originalCurrency = jObject.GetValue("originalCurrency").ToString();
            if (jObject.ContainsKey("exchangeRate"))
                exchangeRate = (double)jObject.GetValue("exchangeRate");
            if (jObject.ContainsKey("merchantCity"))
                merchantCity = jObject.GetValue("merchantCity").ToString();
            visibleTS = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            visibleTS = visibleTS.AddMilliseconds((long) jObject.GetValue("visibleTS"));
            if (jObject.ContainsKey("mmc"))
                mmc = (long)jObject.GetValue("mmc");
            if (jObject.ContainsKey("mmcGroup"))
                mmcGroup = (long)jObject.GetValue("mmcGroup");
            if (jObject.ContainsKey("recurring"))
                recurring = (bool)jObject.GetValue("recurring");
            if (jObject.ContainsKey("partnerBic"))
                partnerBic = jObject.GetValue("partnerBic").ToString();
            if (jObject.ContainsKey("partnerAccountIsSepa"))
                partnerAccountIsSepa = (bool)jObject.GetValue("partnerAccountIsSepa");
            if (jObject.ContainsKey("partnerName"))
                partnerName = jObject.GetValue("partnerName").ToString();
            if (jObject.ContainsKey("accountId"))
                accountId = jObject.GetValue("accountId").ToString();
            if (jObject.ContainsKey("partnerIban"))
                partnerIban = jObject.GetValue("partnerIban").ToString();
            if (jObject.ContainsKey("category"))
                category = jObject.GetValue("category").ToString();
            if (jObject.ContainsKey("cardId"))
                cardId = jObject.GetValue("cardId").ToString();
            if (jObject.ContainsKey("referenceText"))
                referenceText = jObject.GetValue("referenceText").ToString();
            if (jObject.ContainsKey("userCertified"))
            {
                userCertified = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                userCertified = userCertified.AddMilliseconds((long)jObject.GetValue("userCertified"));
            }
            if (jObject.ContainsKey("pending"))
                pending = (bool)jObject.GetValue("pending");
            if (jObject.ContainsKey("transactionNature"))
                transactionNature = jObject.GetValue("transactionNature").ToString();
            if (jObject.ContainsKey("transactionTerminal"))
                transactionTerminal = jObject.GetValue("transactionTerminal").ToString();
            if (jObject.ContainsKey("createdTS"))
            {
                createdTS = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                createdTS = createdTS.AddMilliseconds((long)jObject.GetValue("createdTS"));
            }
            if (jObject.ContainsKey("merchantCountry"))
                merchantCountry = (long)jObject.GetValue("merchantCountry");
            if (jObject.ContainsKey("mandateId"))
                mandateId = jObject.GetValue("mandateId").ToString();
            if (jObject.ContainsKey("creditorIdentifier"))
                creditorIdentifier = jObject.GetValue("creditorIdentifier").ToString();
            if (jObject.ContainsKey("creditorName"))
                creditorName = jObject.GetValue("creditorName").ToString();
            if (jObject.ContainsKey("smartLinkId"))
                smartLinkId = jObject.GetValue("smartLinkId").ToString();
            if (jObject.ContainsKey("linkId"))
                linkId = jObject.GetValue("linkId").ToString();
            if (jObject.ContainsKey("confirmed"))
            {
                confirmed = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                confirmed = confirmed.AddMilliseconds((long)jObject.GetValue("confirmed"));
            }
            if (jObject.ContainsKey("paymentScheme"))
                paymentScheme = jObject.GetValue("paymentScheme").ToString();
            if (jObject.ContainsKey("merchantName"))
                merchantName = jObject.GetValue("merchantName").ToString();
        }

        public string GetName()
        {
            if (partnerName != null)
                return partnerName;
            if (merchantName != null)
                return merchantName;
            if (creditorName != null)
                return creditorName;

            return "N26";
        }

        public string GetDate()
        {
            return visibleTS.ToString("dd. MMM, yyyy");
        }

        public string GetReference()
        {
            if (referenceText != null)
                return referenceText;

            return "";
        }
    }
}
