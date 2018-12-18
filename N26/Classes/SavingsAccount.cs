using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Uwp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

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

        public SeriesCollection GraphSeries { get; set; }

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

            LineSeries pastSeries = new LineSeries { Title = "Balance", PointGeometry = DefaultGeometries.None, Values = new ChartValues<double>()};
            LineSeries realisticSeries = new LineSeries { Title = "Realistic Forecast", PointGeometry = DefaultGeometries.None,  Values = new ChartValues<double>()};
            LineSeries optimisticSeries = new LineSeries { Title = "Optimistic Forecast", PointGeometry = DefaultGeometries.None, Values = new ChartValues<double>()};
            LineSeries pessimisticSeries = new LineSeries { Title = "Pessimistic Forecast", PointGeometry = DefaultGeometries.None, Values = new ChartValues<double>()};
            foreach (SavingsHistory now in History)
            {
                pastSeries.Values.Add(now.Value);
                realisticSeries.Values.Add(now.Value);
                optimisticSeries.Values.Add(now.Value);
                pessimisticSeries.Values.Add(now.Value);

            }

            pastSeries.Stroke = new SolidColorBrush(Color.FromArgb(0xff, 0, 0, 0xff));
            realisticSeries.Stroke = new SolidColorBrush(Color.FromArgb(0xaa, 0, 0, 0xff));
            realisticSeries.StrokeDashArray = new DoubleCollection { 5, 5 };
            optimisticSeries.Stroke = new SolidColorBrush(Color.FromArgb(0xaa, 0, 0xff, 0));
            optimisticSeries.StrokeDashArray = new DoubleCollection { 5, 5 };
            pessimisticSeries.Stroke = new SolidColorBrush(Color.FromArgb(0xaa, 0xff, 0, 0));
            pessimisticSeries.StrokeDashArray = new DoubleCollection { 5, 5 };

            pastSeries.Values.Add(Balance);
            realisticSeries.Values.Add(Balance);
            optimisticSeries.Values.Add(Balance);
            pessimisticSeries.Values.Add(Balance);

            double oldReal = Balance, oldOpti = Balance, oldPessi = Balance;
            int steps = (NextDate - DateTime.Now).Days;
            //foreach (SavingsForecast now in Forecasts)
            for (int j = 0; j < 1; j++) // Just load the first month for now. Should probably be changed to something that makes more sense
            {
                SavingsForecast now = Forecasts[j];
                for (int i = 1; i <= steps; i++)
                {
                    realisticSeries.Values.Add(oldReal + (((now.Value - oldReal) / steps) * i) );
                    pessimisticSeries.Values.Add(oldPessi + (((now.PessimisticValue - oldPessi) / steps) * i));
                    optimisticSeries.Values.Add(oldOpti + (((now.OptimisticValue - oldOpti) / steps) * i));
                }
                oldReal = now.Value;
                oldOpti = now.OptimisticValue;
                oldPessi = now.PessimisticValue;
                steps = 31; // This is bad interpolation, needs a fix
            }
            
            GraphSeries = new SeriesCollection();
            GraphSeries.Add(realisticSeries);
            GraphSeries.Add(optimisticSeries);
            GraphSeries.Add(pessimisticSeries);
            GraphSeries.Add(pastSeries);
        }
    }
}
