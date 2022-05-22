using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace gisserver.map
{
    /// <summary>
    /// Summary description for map
    /// </summary>
    public class map : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string thisServer = HttpContext.Current.Request.Url.AbsoluteUri;
            thisServer = thisServer.Substring(0, thisServer.LastIndexOf(HttpContext.Current.Request.Url.AbsolutePath));

            string strP = context.Request.QueryString["p"];

            if (strP != null && strP != "")
            {
                Parameters p = Parameters.FromString(strP);

                string kml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"" xmlns:kml=""http://www.opengis.net/kml/2.2"" xmlns:atom=""http://www.w3.org/2005/Atom""><Document>";

                if (p.Winlink)
                {
                    kml += @"<NetworkLink>
	<name>Winlink Data</name>
	<open>0</open>
	<description>Amateur radio messages and forms</description>
	<Link>";
                    kml += String.Format("<href>https://giscommunicator.azurewebsites.net/api/gisreceiver?code=xNjuM3fBnIsZ28iTEiUNbFQcmBEdPkp5aspqxVm1aaJ6cGItH4dcyQ==&amp;recipient={0}</href>", p.WinlinkRecipient);
                    kml += @"			<refreshMode>onInterval</refreshMode>
			<refreshInterval>180</refreshInterval>
		</Link>
	</NetworkLink>";
                }

                if (p.Netlogger)
                {
                    // Go get netlogger check-ins
                    kml += "<NetworkLink><name>" + p.NetloggerNetName + @"</name>
	<open>0</open><description></description>
	<Link>";
                    kml += String.Format("<href>{0}/map/checkins.ashx?p={1}</href>", thisServer, strP);
                    kml += @"			<refreshMode>onInterval</refreshMode>
			<refreshInterval>300</refreshInterval>
		</Link>
	</NetworkLink>";
                }

                if (p.Weather)
                {
                    string url = string.Format("https://api.weather.gov/alerts/active?area={0}", p.WeatherState);
                }

                kml += "</Document></kml>";

                context.Response.ContentType = "application/vnd.google-earth.kml+xml";
                context.Response.Write(kml);
                
            }
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