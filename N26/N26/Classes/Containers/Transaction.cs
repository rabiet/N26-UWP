using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace N26.Classes.Containers
{
    class Transaction
    {
        public string Name
        {
            get;
            set;
        }

        public string Amount
        {
            get;
            set;
        }

        public SolidColorBrush AmountColor
        {
            get;
            set;
        }

        public string ReferenceText
        {
            get;
            set;
        }

        public string Date
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }
    }
}
