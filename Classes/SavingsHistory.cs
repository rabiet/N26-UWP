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
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public double Profit { get; set; }
        public double ProfitPercentage { get; set; }

        public SavingsHistory(JObject jObject)
        {
            Name = jObject.GetValue("name").ToString();

            Date = DateTime.ParseExact(jObject.GetValue("date").ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            Value = (double) jObject.GetValue("value");
            Profit = (double)jObject.GetValue("profit");
            ProfitPercentage = (double)jObject.GetValue("profitPercentage");
        }
    }
}
