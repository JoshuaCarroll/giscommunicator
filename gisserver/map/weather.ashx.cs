using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace gisserver.map
{
    /// <summary>
    /// Summary description for weather
    /// </summary>
    public class weather : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            string kml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"" xmlns:kml=""http://www.opengis.net/kml/2.2"" xmlns:atom=""http://www.w3.org/2005/Atom""><Document>
<Style id=""examplePolyStyle"">
    <PolyStyle>
        <color>ff0000cc</color>
        <colorMode>random</colorMode>
    </PolyStyle>
</Style>";

            string strP = context.Request.QueryString["p"];
            if (strP != null && strP != "")
            {
                Parameters p = Parameters.FromString(strP);

                string strJson = "";
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "AA5JC SitRep, joshcarroll21@gmail.com)");
                    strJson = wc.DownloadString(string.Format("https://api.weather.gov/alerts/active?area={0}", p.WeatherState));
                }

                JObject wx = JObject.Parse(strJson);
                JArray features = (JArray)wx["features"];

                for (int i = 0; i < features.Count; i++)
                {
                    try
                    {
                        string name = (string)features[i]["properties"]["event"];
                        string description = (string)features[i]["properties"]["description"];
                        string id = (string)features[i]["properties"]["id"];

                        JArray arrCoords = (JArray)features[i]["geometry"]["coordinates"];

                        for (int j = 0; j < arrCoords.Count; j++)
                        {
                            JArray arrPoints = (JArray)arrCoords[j];

                            kml += @"
                            <Placemark>
                                <name>" + name + @"</name>
                                <styleUrl>#examplePolyStyle</styleUrl>
                                <description><![CDATA[
                                    " + description + @"
                                ]]></description>
                                <Polygon id=""" + id + @""">
                                  <extrude>0</extrude>
                                  <tessellate>0</tessellate>
                                  <altitudeMode>clampToGround</altitudeMode>
                                  <outerBoundaryIs>
                                    <LinearRing>
                                        <coordinates>";

                            for (int k = 0; k < arrPoints.Count; k++)
                            {
                                string lat = arrPoints[k][1].Value<string>();
                                string lon = arrPoints[k][0].Value<string>();
                                kml += string.Format("{0},{1},0", lon, lat) + Environment.NewLine;
                            }

                                kml += @"
                                        </coordinates>
                                    </LinearRing>
                                  </outerBoundaryIs>
                                </Polygon>
                            </Placemark>";
                        }
                    }
                    catch { }
                }
                
            }

            kml += "</Document></kml>";

            context.Response.ContentType = "text/plain";
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