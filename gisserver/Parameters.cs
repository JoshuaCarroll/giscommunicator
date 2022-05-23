using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gisserver
{
    public class Parameters
    {
        public bool Winlink { get; set; }
        public bool Weather { get; set; }
        public bool Netlogger { get; set; }
        public string WinlinkRecipient { get; set; }
        public string WeatherState { get; set; }
        public string NetloggerNetName { get; set; }
        public string NetloggerUrl { get; set; }
        public string NetloggerServer { get; set; }

        public static Parameters FromString(string encodedString)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(encodedString);
            string strParameters = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            Parameters p = JsonConvert.DeserializeObject<Parameters>(strParameters);
            return p;
        }

        public override string ToString()
        {
            string plainText = JsonConvert.SerializeObject(this);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}