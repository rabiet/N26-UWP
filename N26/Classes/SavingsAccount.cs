using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N26.Classes
{
    public class SavingsAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long MonthlyAmount { get; set; }
        public DateTime NextDate { get; set; }
        public List<SavingsHistory> History { get; set; }
        public List<SavingsForecast> Forecasts { get; set; }
        public Uri RiskDisclaimerUrl { get; set; }
        public Uri ForecastDisclaimerUrl { get; set; }
        public string OptionId { get; set; }
        public DateTime StartingDate { get; set; }
        public double Balance { get; set; }
        public long TotalDeposit { get; set; }
        public double Performance { get; set; }
        public double Profit { get; set; }
        public string Status { get; set; }

        public SavingsAccount(JObject jObject)
        {
            Id = jObject.GetValue("id").ToString();
            Name = jObject.GetValue("name").ToString();
            MonthlyAmount = (long)jObject.GetValue("monthlyAmount");
            NextDate = DateTime.ParseExact(jObject.GetValue("nextDate").ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            History = new List<SavingsHistory>();
            foreach (JObject history in jObject.GetValue("history"))
                History.Add(new SavingsHistory(history));
            Forecasts = new List<SavingsForecast>();
            foreach (JObject forecast in jObject.GetValue("forecasts"))
                Forecasts.Add(new SavingsForecast(forecast));
            RiskDisclaimerUrl = new Uri(jObject.GetValue("riskDisclaimerUrl").ToString());
            ForecastDisclaimerUrl = new Uri(jObject.GetValue("forecastDisclaimerUrl").ToString());
            OptionId = jObject.GetValue("optionId").ToString();
            StartingDate = DateTime.ParseExact(jObject.GetValue("startingDate").ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            Balance = (double)jObject.GetValue("balance");
            TotalDeposit = (long)jObject.GetValue("totalDeposit");
            Performance = (double)jObject.GetValue("performance");
            Profit = (double)jObject.GetValue("profit");
            Status = jObject.GetValue("status").ToString();
        }
    }
}
