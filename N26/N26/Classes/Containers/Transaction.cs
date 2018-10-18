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

        public int CategoryIndex
        {
            get;
            set;
        }

        public void setCategory(string category)
        {
            Category = string.Format("/Assets/Categories-Dark/icon-category-{0}.png", category);
            switch (category)
            {
                case "atm":
                    CategoryIndex = 0;
                    break;
                case "bars-restaurants":
                    CategoryIndex = 1;
                    break;
                case "business":
                    CategoryIndex = 2;
                    break;
                case "cash26":
                    CategoryIndex = 3;
                    break;
                case "education":
                    CategoryIndex = 4;
                    break;
                case "family-friends":
                    CategoryIndex = 5;
                    break;
                case "food-groceries":
                    CategoryIndex = 6;
                    break;
                case "healthcare-drugstores":
                    CategoryIndex = 7;
                    break;
                case "household-utilities":
                    CategoryIndex = 8;
                    break;
                case "income":
                    CategoryIndex = 9;
                    break;
                case "insurances-finances":
                    CategoryIndex = 10;
                    break;
                case "leisure-entertainment":
                    CategoryIndex = 11;
                    break;
                case "media-electronics":
                    CategoryIndex = 12;
                    break;
                case "miscellaneous":
                    CategoryIndex = 13;
                    break;
                case "n26-referrals":
                    CategoryIndex = 14;
                    break;
                case "salary":
                    CategoryIndex = 15;
                    break;
                case "savings-investments":
                    CategoryIndex = 16;
                    break;
                case "shopping":
                    CategoryIndex = 17;
                    break;
                case "subscriptions-donations":
                    CategoryIndex = 18;
                    break;
                case "tax-fines":
                    CategoryIndex = 19;
                    break;
                case "transport-car":
                    CategoryIndex = 20;
                    break;
                case "travel-holidays":
                    CategoryIndex = 21;
                    break;
            }
        }
    }
}
