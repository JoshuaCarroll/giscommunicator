using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace gisserver.map
{
    /// <summary>
    /// Summary description for checkins
    /// </summary>
    public class checkins : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string kml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"" xmlns:kml=""http://www.opengis.net/kml/2.2"" xmlns:atom=""http://www.w3.org/2005/Atom""><Document>";

            string strP = context.Request.QueryString["p"];
            if (strP != null && strP != "")
            {
                Parameters p = Parameters.FromString(strP);

                XDocument xml = XDocument.Load(p.NetloggerUrl);
                foreach (XElement checkin in xml.Descendants("Checkin"))
                {
                    if (checkin.Element("Callsign").Value.Trim() != string.Empty && checkin.Element("CityCountry").Value.Trim() != string.Empty)
                    {
                        string name = checkin.Element("FirstName").Value + " " + checkin.Element("Callsign").Value;
                        string address = checkin.Element("Street").Value + ", " + checkin.Element("CityCountry").Value + ", " + checkin.Element("State").Value + " " + checkin.Element("Zip").Value;

                        string s = string.Format(@"<Placemark><name>{0}</name><address>{1}</address></Placemark>", name, address);
                        kml += s;
                    }
                }
            }

            kml += "</Document></kml>";

            context.Response.ContentType = "application/vnd.google-earth.kml+xml";
            context.Response.Write(kml);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}