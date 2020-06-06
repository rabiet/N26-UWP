using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N26.Classes
{
    public class Product
    {
        public enum TypeCode
        {
            TRANSACTIONAL,
            RECURRING
        } //Not confirmed as the only ones, just the only ones listed on my account

        public enum StatusCode
        {
            ACTIVE,
            INACTIVE
        } //These are only guesses, I've never discovered anything different from ACTIVE

        public enum AccountStatusCode
        {
            PRIMARY,
            NON_PRIMARY
        }

        public string group { get; set; }

        public string productId { get; set; }

        public TypeCode type { get; set; }

        public string country { get; set; }

        public StatusCode status { get; set; }

        public string userId { get; set; }

        public string merchantCountryOption { get; set; }

        public int totalFreeAtms { get; set; }

        public int usedAtms { get; set; }

        public DateTime endOfPeriod { get; set; }

        public DateTime endOfGracePeriod { get; set; }

        public AccountStatusCode accountStatus { get; set; }

        public bool hasSalary { get; set; }

        public bool hasIncome { get; set; }

        public bool underAgeThreshold { get; set; }

        public bool inGracePeriod { get; set; }

        public List<string> actions { get; set; }

        public Product (JObject jObject)
        {
            try
            {
                productId = jObject.GetValue("productId").ToString();
                group = jObject.GetValue("group").ToString();

                actions = new List<string>();
                foreach (string now in ((JArray)jObject.GetValue("actions")))
                    actions.Add(now);

                JObject prod = (JObject)jObject.GetValue("product");
                type = (prod.GetValue("type").ToString().Equals("TRANSACTIONAL")) ? TypeCode.TRANSACTIONAL : TypeCode.RECURRING;
                country = prod.GetValue("country").ToString();
                status = (prod.GetValue("status").ToString().Equals("ACTIVE")) ? StatusCode.ACTIVE : StatusCode.INACTIVE;

                JObject groupDetails = (JObject)prod.GetValue("groupDetails");
                if (groupDetails == null)
                    return;

                if (groupDetails.ContainsKey("userId"))
                    userId = groupDetails.GetValue("userId").ToString();
                if (groupDetails.ContainsKey("merchantCountryOption"))
                    merchantCountryOption = groupDetails.GetValue("merchantCountryOption").ToString();
                if (groupDetails.ContainsKey("totalFreeAtms"))
                    totalFreeAtms = (int)groupDetails.GetValue("totalFreeAtms");
                if (groupDetails.ContainsKey("usedAtms"))
                    usedAtms = (int)groupDetails.GetValue("usedAtms");
                if (groupDetails.ContainsKey("endOfPeriod"))
                    endOfPeriod = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((int)groupDetails.GetValue("endOfPeriod"));
                if (groupDetails.ContainsKey("endOfGracePeriod"))
                    endOfGracePeriod = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((int)groupDetails.GetValue("endOfGracePeriod"));
                if (groupDetails.ContainsKey("accountStatus"))
                    accountStatus = groupDetails.GetValue("accountStatus").ToString().Equals("PRIMARY") ? AccountStatusCode.PRIMARY : AccountStatusCode.NON_PRIMARY;
                if (groupDetails.ContainsKey("hasSalary"))
                    hasSalary = (bool)groupDetails.GetValue("hasSalary");
                if (groupDetails.ContainsKey("hasIncome"))
                    hasIncome = (bool)groupDetails.GetValue("hasIncome");
                if (groupDetails.ContainsKey("underAgeThreshold"))
                    underAgeThreshold = (bool)groupDetails.GetValue("underAgeThreshold");
                if (groupDetails.ContainsKey("inGracePeriod"))
                    inGracePeriod = (bool)groupDetails.GetValue("inGracePeriod");
            } catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
        }



    }
}
