using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace N26.Classes.Converter
{
    class InvestOptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value.ToString())
            {
                case "Vaamo_Medium":
                    return "Invest 60";
                case "Vaamo_High":      // Not confirmed
                    return "Invest 80";
                case "Vaamo_Low":       // Not confirmed
                    return "Invest 40";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
