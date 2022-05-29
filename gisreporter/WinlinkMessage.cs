using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace gisreporter
{
    public class WinlinkMessage
    {
        public string MessageBody { get; set; }
        public string DateTimeSent { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MessageSubject { get; set; }
        public string MessageXML { get; set; }
        public string MessageString { get; set; }
        public string SendersCallsign { get; set; }
        public string UniqueID { get; set; }
        public string Recipient { get; set; }
        public string Icon { get; set; }


        public WinlinkMessage(string messageBody, string filename)
        {
            MessageBody = messageBody;

            //try
            //{
                string pattern = @"boundary=""?(?<boundary>[^"";\r\n]+)""?;?";
                RegexOptions options = RegexOptions.Singleline;
                Match BoundaryMatch = Regex.Match(MessageBody, pattern, options);

                if (BoundaryMatch != null)
                {
                    string baseBoundary = "--" + BoundaryMatch.Groups["boundary"].Value;

                    UniqueID = filename;

                    string[] msgArr = MessageBody.Split(baseBoundary);

                    string[] header = msgArr[0].Split("\r\n");

                    string recipientPrefix = "To: ";
                    for (int i = 0; i < header.Length; i++)
                    {
                        if (header[i].StartsWith(recipientPrefix))
                        {
                            Recipient = header[i].Substring(recipientPrefix.Length, header[i].IndexOf('@') - recipientPrefix.Length);
                            break;
                        }
                    }

                    string base64XmlData = msgArr[2];

                    int encodedStringStartIndex = base64XmlData.IndexOf("\r\n\r\n");

                    if (base64XmlData.Contains(".xml\"") && encodedStringStartIndex > 0)
                    {
                        string encodedString = base64XmlData.Substring(encodedStringStartIndex);

                        var valueBytes = Convert.FromBase64String(encodedString);
                        MessageXML = Encoding.UTF8.GetString(valueBytes);

                        XmlDocument document = new XmlDocument();
                        document.LoadXml(MessageXML);
                        XmlNamespaceManager mgr = new XmlNamespaceManager(document.NameTable);
                        mgr.AddNamespace("ns", "http://www.aa5jc.com/namespace");

                        try
                        {
                            Latitude = document.SelectSingleNode("RMS_Express_Form/variables/gpslat", mgr).InnerText;
                            Longitude = document.SelectSingleNode("RMS_Express_Form/variables/gpslon", mgr).InnerText;
                        }
                        catch
                        {
                            try
                            {
                                Latitude = document.SelectSingleNode("RMS_Express_Form/variables/maplat", mgr).InnerText;
                                Longitude = document.SelectSingleNode("RMS_Express_Form/variables/maplon", mgr).InnerText;
                            }
                            catch
                            {
                                try
                                {
                                    string[] gps = document.SelectSingleNode("RMS_Express_Form/variables/gps", mgr).InnerText.Split(',');
                                    Latitude = gps[0];
                                    Longitude = gps[1];
                                }
                                catch
                                {
                                    // Unable to find GPS data
                                }
                            }
                        }

                        MessageSubject = document.SelectSingleNode("RMS_Express_Form/form_parameters/display_form", mgr).InnerText.Replace("_", " ");

                        MessageSubject = MessageSubject.Substring(0, MessageSubject.LastIndexOf('.'));
                        if (MessageSubject.ToLower().EndsWith(" viewer"))
                        {
                            MessageSubject = MessageSubject.Substring(0, MessageSubject.ToLower().LastIndexOf(" viewer")).Trim();
                        }

                        try
                        {
                            DateTimeSent = document.SelectSingleNode("RMS_Express_Form/submission_datetime", mgr).InnerText;
                        }
                        catch
                        {
                            try
                            {
                                DateTimeSent = document.SelectSingleNode("RMS_Express_Form/variables/timestamp", mgr).InnerText;
                            }
                            catch
                            {
                                try
                                {
                                    DateTimeSent = document.SelectSingleNode("RMS_Express_Form/variables/datetime", mgr).InnerText;
                                }
                                catch
                                {
                                    DateTimeSent = DateTime.UtcNow.ToString();
                                }
                            }
                        }

                        try
                        {
                            DateTimeSent = DateTime.Parse(DateTimeSent).ToString();
                        }
                        catch
                        {
                            try
                            {
                                int year = int.Parse(DateTimeSent.Substring(0, 4));
                                int month = int.Parse(DateTimeSent.Substring(4, 2));
                                int day = int.Parse(DateTimeSent.Substring(6, 2));
                                int hour = int.Parse(DateTimeSent.Substring(8, 2));
                                int minute = int.Parse(DateTimeSent.Substring(10, 2));
                                int second = int.Parse(DateTimeSent.Substring(12, 2));
                                DateTime sent = new DateTime(year, month, day, hour, minute, second);
                                DateTimeSent = sent.ToString();
                            }
                            catch
                            {
                                DateTimeSent = DateTime.Now.ToString();
                            }
                        }

                        MessageString = msgArr[1];

                        MessageString = MessageString.Replace("\r\nContent-Type: text/plain; charset=\"iso-8859-1\"\r\nContent-Transfer-Encoding: quoted-printable\r\n\r\n\r\n","");
                        MessageString = MessageString.Replace("=20", " ").Replace("=A0", " ").Replace("=B0", "°");

                        int indexOfMessageTerminator = MessageString.IndexOf("\r\n\r\n------------------------------------\r\nExpress Sending Station: ");
                        if (indexOfMessageTerminator > -1)
                        {
                            MessageString = MessageString.Substring(0, indexOfMessageTerminator);
                        }

                        SendersCallsign = document.SelectSingleNode("RMS_Express_Form/form_parameters/senders_callsign", mgr).InnerText;

                        #region Determine the icon to use
                        Icon = "";
                        string display_form = document.SelectSingleNode("RMS_Express_Form/form_parameters/display_form", mgr).InnerText;
                        XmlNode variables = document.SelectSingleNode("RMS_Express_Form/variables", mgr);

                        switch (display_form)
                        {
                            case "Field Situation Report_viewer.html":
                                if (variables.SelectSingleNode("safetyneed").InnerText == "YES")
                                {
                                    Icon = "emergency-2.png";
                                }
                                else if (variables.SelectSingleNode("powerworks").InnerText == "NO")
                                {
                                    Icon = "power.png";
                                }
                                else if (variables.SelectSingleNode("cell").InnerText == "NO")
                                {
                                    Icon = "cellphones.png";
                                }
                                else if (variables.SelectSingleNode("inter").InnerText == "NO")
                                {
                                    Icon = "internet.png";
                                }
                                else if (variables.SelectSingleNode("tvstatus").InnerText == "NO")
                                {
                                    Icon = "tv.png";
                                }
                                else if (variables.SelectSingleNode("waterworks").InnerText == "NO")
                                {
                                    Icon = "water.png";
                                }
                                else if (variables.SelectSingleNode("amfm").InnerText == "NO")
                                {
                                    Icon = "radio.png";
                                }
                                else if (variables.SelectSingleNode("noaa").InnerText == "NO")
                                {
                                    Icon = "noaa-wx-radio.png";
                                }
                                else if (variables.SelectSingleNode("land").InnerText == "NO")
                                {
                                    Icon = "telephone.png";
                                }
                                else
                                {
                                    Icon = "radar.png";
                                }
                                
                                break;
                            case "Severe WX Report viewer.html":
                                if (variables.SelectSingleNode("tornado").InnerText != "NONE")
                                {
                                    Icon = "tornado.png";
                                }
                                else if (variables.SelectSingleNode("winddamage").InnerText != "NONE")
                                {
                                    Icon = "treedown.png";
                                }
                                else if (variables.SelectSingleNode("flood").InnerText != "NONE")
                                {
                                    Icon = "flood.png";
                                }
                                else if (variables.SelectSingleNode("hailsize").InnerText != "NONE")
                                {
                                    Icon = "hail.png";
                                }
                                else if (variables.SelectSingleNode("freezingrain").InnerText != "NONE")
                                {
                                    Icon = "snow.png";
                                }
                                else if (variables.SelectSingleNode("snow").InnerText != "NONE")
                                {
                                    Icon = "snow.png";
                                }
                                else if (variables.SelectSingleNode("windspeed").InnerText != "NONE")
                                {
                                    Icon = "wind.png";
                                }
                                else
                                {
                                    Icon = "storm.png";
                                }

                                break;
                            case "Local Weather Report Viewer.html":
                                Icon = "cloudy.png";
                                break;
                            case "Winlink_Check_In_Viewer.html":
                                Icon = "person.png";
                                break;
                            default:
                                Icon = "default.png";
                                break;
                        }

                        #endregion

                    }
                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("** " + ex);
//            }

        }

        public MapItem ToMapItem()
        {
            MapItem mapItem = new MapItem(MessageSubject, MessageString, Latitude, Longitude, "", "", DateTimeSent, UniqueID, Recipient);
            return mapItem;
        }
    }
}
