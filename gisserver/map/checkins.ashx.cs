using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                ///TODO: Issue #11
                p.NetloggerUrl = p.NetloggerUrl.Replace("https://", "http://");

                XDocument xml = XDocument.Load(p.NetloggerUrl);

                // Check and make sure this net is open
                if (xml.Element("NetLoggerXML").Element("Error") != null && xml.Element("NetLoggerXML").Element("Error").Value.StartsWith("Query Returned an empty result"))
                {
                    // Loop through servers and nets to find a net with this name
                    XDocument xPastNets = XDocument.Load("http://www.netlogger.org/api/GetPastNets.php?Interval=10");
                    
                    XElement xServer = (from Server in xPastNets.Element("NetLoggerXML").Element("ServerList").Descendants("Server")
                                    where (string)Server.Element("ServerName") == p.NetloggerServer
                                    select Server).First();

                    string netID = (from net in xServer.Elements("Net")
                                    where (string)net.Element("NetName") == p.NetloggerNetName
                                    select net.Element("NetID")).First().Value;

                    xml = XDocument.Load(string.Format("http://www.netlogger.org/api/GetPastNetCheckins.php?ServerName={0}&NetName={1}&NetID={2}", p.NetloggerServer, p.NetloggerNetName, netID));
                }
                else
                {
                    Debug.WriteLine(xml.Element("Error"));
                }

                foreach (XElement checkin in xml.Descendants("Checkin"))
                {
                    if (checkin.Element("Callsign").Value.Trim() != string.Empty && checkin.Element("CityCountry").Value.Trim() != string.Empty)
                    {
                        string name = checkin.Element("FirstName").Value + " " + checkin.Element("Callsign").Value;
                        string address = checkin.Element("Street").Value + ", " + checkin.Element("CityCountry").Value + ", " + checkin.Element("State").Value + " " + checkin.Element("Zip").Value;
                        //string description = "<![CDATA[ <h1>" + name + "</h1><p>" + checkin.Element("Street").Value + "<br>" + checkin.Element("CityCountry").Value + ", " + checkin.Element("State").Value + " " + checkin.Element("Zip").Value + "</p> ]]> ";

                        kml += string.Format(@"
                                <Placemark>
                                    <name>{0}</name>
                                    <styleUrl>#iconStyle</styleUrl>
                                    <address>{1}</address>                                    
                                    <Style><IconStyle><Icon><href>http://aa5jc.com/map/icons/person.png</href></Icon><color>0Effffff</color></IconStyle></Style>
                                </Placemark>", name, address);
                    }
                }
            }

            kml += "</Document></kml>";

            //context.Response.ContentType = "application/vnd.google-earth.kml+xml";
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