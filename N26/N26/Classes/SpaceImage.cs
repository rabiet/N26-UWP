using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N26.Classes
{
    public class SpaceImage
    {
        public string id;
        public string url;
        public string altText;

        public SpaceImage(JObject jObject)
        {
            id = jObject.GetValue("id").ToString();
            url = jObject.GetValue("url").ToString();
            altText = jObject.GetValue("altText").ToString();
        }
    }
}
