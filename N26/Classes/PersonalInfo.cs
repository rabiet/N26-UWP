using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N26.Classes
{
    public class PersonalInfo
    {
        public string id;
        public string email;
        public string firstName;
        public string lastName;
        public string kycFirstName;
        public string kycLastName;
        public string title;
        public string gender;
        public long birthDate;
        public bool signupCompleted;
        public string nationality;
        public string mobile;
        public string shadowUserID;
        public bool transferWiseTermsAccepted;

        public PersonalInfo(JObject jObject)
        {
            id = jObject.GetValue("id").ToString();
            email = jObject.GetValue("email").ToString();
            firstName = jObject.GetValue("firstName").ToString();
            lastName = jObject.GetValue("lastName").ToString();
            kycFirstName = jObject.GetValue("kycFirstName").ToString();
            kycLastName = jObject.GetValue("kycLastName").ToString();
            title = jObject.GetValue("title").ToString();
            gender = jObject.GetValue("gender").ToString();
            birthDate = (long) jObject.GetValue("birthDate");
            signupCompleted = (bool) jObject.GetValue("signupCompleted");
            nationality = jObject.GetValue("nationality").ToString();
            mobile = jObject.GetValue("mobilePhoneNumber").ToString();
            shadowUserID = jObject.GetValue("shadowUserId").ToString();
            transferWiseTermsAccepted = (bool) jObject.GetValue("transferWiseTermsAccepted");
        }
    }
}
