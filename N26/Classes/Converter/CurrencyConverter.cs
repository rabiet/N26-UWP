using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace N26.Classes.Converter
{
    class CurrencyConverter : IValueConverter
    {
        private const string currency = "€"; // For later implementation of more currencies
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double)
                return string.Format("{0} {1}", ((double)value).ToString("0.00"), currency);

            return string.Format("{0} {1}", value.ToString(), currency);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
