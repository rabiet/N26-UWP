using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace N26.Classes.Converter
{
    class PerMonthConverter : IValueConverter
    {
        private const string currency = "€"; // for later implementation of multiple currencies
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format("{0} {1}/mo", value, currency);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
