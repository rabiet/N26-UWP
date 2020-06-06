using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N26.Classes
{
    public class Limit
    {
        public string limit { get; set; }

        public double amount { get; set; }

        public Limit(JObject jObject)
        {
            limit = jObject.GetValue("limit").ToString();
            amount = (double)jObject.GetValue("amount");
        }
    }
}
