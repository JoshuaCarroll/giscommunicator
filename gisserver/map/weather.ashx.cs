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
</Style>
<Style id=""TSUNAMIWARNING""><PolyStyle><color>CC4763FD</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TORNADOWARNING""><PolyStyle><color>CC0000FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""EXTREMEWINDWARNING""><PolyStyle><color>CC008CFF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SEVERETHUNDERSTORMWARNING""><PolyStyle><color>CC00A5FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FLASHFLOODWARNING""><PolyStyle><color>CC00008B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FLASHFLOODSTATEMENT""><PolyStyle><color>CC00008B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SEVEREWEATHERSTATEMENT""><PolyStyle><color>CCFFFF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SHELTERINPLACEWARNING""><PolyStyle><color>CC7280FA</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""EVACUATIONIMMEDIATE""><PolyStyle><color>CC00FF7F</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""CIVILDANGERWARNING""><PolyStyle><color>CCC1B6FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""NUCLEARPOWERPLANTWARNING""><PolyStyle><color>CC82004B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""RADIOLOGICALHAZARDWARNING""><PolyStyle><color>CC82004B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HAZARDOUSMATERIALSWARNING""><PolyStyle><color>CC82004B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FIREWARNING""><PolyStyle><color>CC2D52A0</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""CIVILEMERGENCYMESSAGE""><PolyStyle><color>CCC1B6FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LAWENFORCEMENTWARNING""><PolyStyle><color>CCC0C0C0</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""STORMSURGEWARNING""><PolyStyle><color>CCF724B5</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HURRICANEFORCEWINDWARNING""><PolyStyle><color>CC5C5CCD</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HURRICANEWARNING""><PolyStyle><color>CC3C14DC</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TYPHOONWARNING""><PolyStyle><color>CC3C14DC</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SPECIALMARINEWARNING""><PolyStyle><color>CC00A5FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""BLIZZARDWARNING""><PolyStyle><color>CC0045FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SNOWSQUALLWARNING""><PolyStyle><color>CC8515C7</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""ICESTORMWARNING""><PolyStyle><color>CC8B008B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""WINTERSTORMWARNING""><PolyStyle><color>CCB469FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HIGHWINDWARNING""><PolyStyle><color>CC20A5DA</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TROPICALSTORMWARNING""><PolyStyle><color>CC2222B2</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""STORMWARNING""><PolyStyle><color>CCD30094</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TSUNAMIADVISORY""><PolyStyle><color>CC1E69D2</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TSUNAMIWATCH""><PolyStyle><color>CCFF00FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""AVALANCHEWARNING""><PolyStyle><color>CCFF901E</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""EARTHQUAKEWARNING""><PolyStyle><color>CC13458B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""VOLCANOWARNING""><PolyStyle><color>CC4F4F2F</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""ASHFALLWARNING""><PolyStyle><color>CCA9A9A9</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""COASTALFLOODWARNING""><PolyStyle><color>CC228B22</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LAKESHOREFLOODWARNING""><PolyStyle><color>CC228B22</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FLOODWARNING""><PolyStyle><color>CC00FF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HIGHSURFWARNING""><PolyStyle><color>CC228B22</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""DUSTSTORMWARNING""><PolyStyle><color>CCC4E4FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""BLOWINGDUSTWARNING""><PolyStyle><color>CCC4E4FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LAKEEFFECTSNOWWARNING""><PolyStyle><color>CC8B8B00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""EXCESSIVEHEATWARNING""><PolyStyle><color>CC8515C7</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TORNADOWATCH""><PolyStyle><color>CC00FFFF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SEVERETHUNDERSTORMWATCH""><PolyStyle><color>CC9370DB</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FLASHFLOODWATCH""><PolyStyle><color>CC578B2E</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""GALEWARNING""><PolyStyle><color>CCDDA0DD</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FLOODSTATEMENT""><PolyStyle><color>CC00FF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""WINDCHILLWARNING""><PolyStyle><color>CCDEC4B0</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""EXTREMECOLDWARNING""><PolyStyle><color>CCFF0000</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HARDFREEZEWARNING""><PolyStyle><color>CCD30094</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FREEZEWARNING""><PolyStyle><color>CC8B3D48</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""REDFLAGWARNING""><PolyStyle><color>CC9314FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""STORMSURGEWATCH""><PolyStyle><color>CCF77FDB</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HURRICANEWATCH""><PolyStyle><color>CCFF00FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HURRICANEFORCEWINDWATCH""><PolyStyle><color>CCCC3299</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TYPHOONWATCH""><PolyStyle><color>CCFF00FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TROPICALSTORMWATCH""><PolyStyle><color>CC8080F0</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""STORMWATCH""><PolyStyle><color>CCB5E4FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HURRICANELOCALSTATEMENT""><PolyStyle><color>CCB5E4FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TYPHOONLOCALSTATEMENT""><PolyStyle><color>CCB5E4FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TROPICALSTORMLOCALSTATEMENT""><PolyStyle><color>CCB5E4FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TROPICALDEPRESSIONLOCALSTATEMENT""><PolyStyle><color>CCB5E4FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""AVALANCHEADVISORY""><PolyStyle><color>CC3F85CD</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""WINTERWEATHERADVISORY""><PolyStyle><color>CCEE687B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""WINDCHILLADVISORY""><PolyStyle><color>CCEEEEAF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HEATADVISORY""><PolyStyle><color>CC507FFF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""URBANANDSMALLSTREAMFLOODADVISORY""><PolyStyle><color>CC7FFF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SMALLSTREAMFLOODADVISORY""><PolyStyle><color>CC7FFF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""ARROYOANDSMALLSTREAMFLOODADVISORY""><PolyStyle><color>CC7FFF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FLOODADVISORY""><PolyStyle><color>CC7FFF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HYDROLOGICADVISORY""><PolyStyle><color>CC7FFF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LAKESHOREFLOODADVISORY""><PolyStyle><color>CC00FC7C</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""COASTALFLOODADVISORY""><PolyStyle><color>CC00FC7C</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HIGHSURFADVISORY""><PolyStyle><color>CCD355BA</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HEAVYFREEZINGSPRAYWARNING""><PolyStyle><color>CCFFBF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""DENSEFOGADVISORY""><PolyStyle><color>CC908070</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""DENSESMOKEADVISORY""><PolyStyle><color>CC8CE6F0</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SMALLCRAFTADVISORYFORHAZARDOUSSEAS""><PolyStyle><color>CCD8BFD8</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SMALLCRAFTADVISORYFORROUGHBAR""><PolyStyle><color>CCD8BFD8</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SMALLCRAFTADVISORYFORWINDS""><PolyStyle><color>CCD8BFD8</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SMALLCRAFTADVISORY""><PolyStyle><color>CCD8BFD8</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""BRISKWINDADVISORY""><PolyStyle><color>CCD8BFD8</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HAZARDOUSSEASWARNING""><PolyStyle><color>CCD8BFD8</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""DUSTADVISORY""><PolyStyle><color>CC6BB7BD</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""BLOWINGDUSTADVISORY""><PolyStyle><color>CC6BB7BD</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LAKEWINDADVISORY""><PolyStyle><color>CC8CB4D2</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""WINDADVISORY""><PolyStyle><color>CC8CB4D2</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FROSTADVISORY""><PolyStyle><color>CCED9564</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""ASHFALLADVISORY""><PolyStyle><color>CC696969</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FREEZINGFOGADVISORY""><PolyStyle><color>CC808000</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FREEZINGSPRAYADVISORY""><PolyStyle><color>CCFFBF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LOWWATERADVISORY""><PolyStyle><color>CC2A2AA5</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LOCALAREAEMERGENCY""><PolyStyle><color>CCC0C0C0</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""AVALANCHEWATCH""><PolyStyle><color>CC60A4F4</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""BLIZZARDWATCH""><PolyStyle><color>CC2FFFAD</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""RIPCURRENTSTATEMENT""><PolyStyle><color>CCD0E040</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""BEACHHAZARDSSTATEMENT""><PolyStyle><color>CCD0E040</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""GALEWATCH""><PolyStyle><color>CCCBC0FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""WINTERSTORMWATCH""><PolyStyle><color>CCB48246</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HAZARDOUSSEASWATCH""><PolyStyle><color>CC8B3D48</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HEAVYFREEZINGSPRAYWATCH""><PolyStyle><color>CC8F8FBC</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""COASTALFLOODWATCH""><PolyStyle><color>CCAACD66</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LAKESHOREFLOODWATCH""><PolyStyle><color>CCAACD66</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FLOODWATCH""><PolyStyle><color>CC578B2E</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HIGHWINDWATCH""><PolyStyle><color>CC0B86B8</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""EXCESSIVEHEATWATCH""><PolyStyle><color>CC000080</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""EXTREMECOLDWATCH""><PolyStyle><color>CCFF0000</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""WINDCHILLWATCH""><PolyStyle><color>CCA09E5F</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LAKEEFFECTSNOWWATCH""><PolyStyle><color>CCFACE87</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HARDFREEZEWATCH""><PolyStyle><color>CCE16941</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FREEZEWATCH""><PolyStyle><color>CCFFFF00</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""FIREWEATHERWATCH""><PolyStyle><color>CCADDEFF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""EXTREMEFIREDANGER""><PolyStyle><color>CC7A96E9</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""911TELEPHONEOUTAGE""><PolyStyle><color>CCC0C0C0</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""COASTALFLOODSTATEMENT""><PolyStyle><color>CC238E6B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""LAKESHOREFLOODSTATEMENT""><PolyStyle><color>CC238E6B</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SPECIALWEATHERSTATEMENT""><PolyStyle><color>CCB5E4FF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""MARINEWEATHERSTATEMENT""><PolyStyle><color>CCB9DAFF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""AIRQUALITYALERT""><PolyStyle><color>CC808080</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""AIRSTAGNATIONADVISORY""><PolyStyle><color>CC808080</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HAZARDOUSWEATHEROUTLOOK""><PolyStyle><color>CCAAE8EE</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""HYDROLOGICOUTLOOK""><PolyStyle><color>CC90EE90</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""SHORTTERMFORECAST""><PolyStyle><color>CC98FB98</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""ADMINISTRATIVEMESSAGE""><PolyStyle><color>CCC0C0C0</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""TEST""><PolyStyle><color>CCFFFFF0</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""CHILDABDUCTIONEMERGENCY""><PolyStyle><color>00FFFFFF</color><colorMode>normal</colorMode></PolyStyle></Style>
<Style id=""BLUEALERT""><PolyStyle><color>00FFFFFF</color><colorMode>normal</colorMode></PolyStyle></Style>";

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