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


        public WinlinkMessage(string messageBody, string filename)
        {
            MessageBody = messageBody;

            try
            {
                string pattern = @"boundary=""?(?<boundary>[^"";\r\n]+)""?;?";
                RegexOptions options = RegexOptions.Singleline;
                Match BoundaryMatch = Regex.Match(MessageBody, pattern, options);

                if (BoundaryMatch != null)
                {
                    string baseBoundary = "--" + BoundaryMatch.Groups["boundary"].Value;

                    UniqueID = filename;

                    string[] msgArr = MessageBody.Split(baseBoundary);
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
                        if (MessageSubject.EndsWith(" Viewer"))
                        {
                            MessageSubject = MessageSubject.Substring(0, MessageSubject.LastIndexOf(" Viewer", StringComparison.CurrentCultureIgnoreCase)).Trim();
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
                                catch (Exception ex)
                                {
                                    DateTimeSent = DateTime.Now.ToString();
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

                        SendersCallsign = document.SelectSingleNode("RMS_Express_Form/form_parameters/senders_callsign", mgr).InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("** " + ex);
            }

        }

        public MapItem ToMapItem()
        {
            MapItem mapItem = new MapItem(MessageSubject, MessageString, Latitude, Longitude, "", "", DateTimeSent, UniqueID);
            return mapItem;
        }
    }
}
