using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N26.Classes
{
    public class SavingsHistory
    {
        public string name;
        public string date;
        public double value;
        public double profit;
        public double profitPercentage;

        public SavingsHistory(JObject jObject)
        {
            name = jObject.GetValue("name").ToString();
            date = jObject.GetValue("date").ToString();
            value = (double) jObject.GetValue("value");
            profit = (double)jObject.GetValue("profit");
            profitPercentage = (double)jObject.GetValue("profitPercentage");
        }
    }
}
