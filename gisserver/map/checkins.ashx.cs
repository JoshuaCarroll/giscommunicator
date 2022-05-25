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
    /// Summary description for checkins
    /// </summary>
    public class checkins : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            string kml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"" xmlns:kml=""http://www.opengis.net/kml/2.2"" xmlns:atom=""http://www.w3.org/2005/Atom""><Document>
<Style id=""checkinHidelabel"">
    <IconStyle>
        <hotSpot x=""15"" y=""0"" xunits=""pixels"" yunits=""pixels""/>
        <Icon>
            <href>http://aa5jc.com/map/icons/person.png</href>
        </Icon>
        <color>ffffffff</color>
    </IconStyle>
    <LabelStyle>
        <color>00ffffff</color>
    </LabelStyle>
    <LineStyle>
        <color>ffffffff</color>
        <width>0</width>
    </LineStyle>
</Style>
<Style id=""checkinShowlabel"">
    <IconStyle>
        <hotSpot x=""15"" y=""0"" xunits=""pixels"" yunits=""pixels""/>
        <color>FFffffff</color>
        <Icon>
            <href>http://aa5jc.com/map/icons/person.png</href>
        </Icon>
    </IconStyle>
    <LabelStyle>
        <color>FFffffff</color>
    </LabelStyle>
    <LineStyle>
        <color>ffffffff</color>
        <width>0</width>
    </LineStyle>
</Style>
<StyleMap id=""checkinIconStyle"">
    <Pair>
        <key>normal</key>
        <styleUrl>#checkinHidelabel</styleUrl>
    </Pair>
    <Pair>
        <key>highlight</key>
        <styleUrl>#checkinShowlabel</styleUrl>
    </Pair>
</StyleMap>";

            string strP = context.Request.QueryString["p"];
            if (strP != null && strP != "")
            {
                Parameters p = Parameters.FromString(strP);

                p.NetloggerUrl = p.NetloggerUrl.Replace("http://", "https://");

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
                        string name = checkin.Element("Callsign").Value;
                        string address = checkin.Element("Street").Value + ", " + checkin.Element("CityCountry").Value + ", " + checkin.Element("State").Value + " " + checkin.Element("Zip").Value;

                        kml += string.Format(@"
                                <Placemark>
                                    <name>{0}</name>
                                    <styleUrl>#checkinIconStyle</styleUrl>
                                    <address>{1}</address>   
                                    <description>{2}</description>
                                </Placemark>", name, address, checkin.Element("FirstName").Value);
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