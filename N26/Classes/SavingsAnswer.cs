using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N26.Classes
{
    public class SavingsAnswer
    {
        public float TotalBalance { get; set; }
        public bool CanOpenMore { get; set; }
        public List<SavingsAccount> Accounts { get; set; }
        public object[] PendingAccounts { get; set; }


        public SavingsAnswer(JObject jObject)
        {
            TotalBalance = (float)jObject.GetValue("totalBalance");
            CanOpenMore = (bool)jObject.GetValue("canOpenMore");
            Accounts = new List<SavingsAccount>();
            foreach (JObject account in jObject.GetValue("accounts"))
                Accounts.Add(new SavingsAccount(account));

        }
    }
}
