using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace gisreporter_
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


        public WinlinkMessage(string messageBody, string filepath)
        {
            string fileExtension = Path.GetExtension(filepath).ToLower();
            string filename = Path.GetFileNameWithoutExtension(filepath);
            MessageBody = messageBody;

            string[] msgArr;
            // Each of these needs to populate the MessageXML property
            switch (fileExtension)
            {
                case ".b2f":
                    B2F b2F = new B2F(MessageBody);
                    MessageBody = b2F.Body;
                    DateTimeSent = b2F.Date.ToString();
                    MessageSubject = b2F.Subject;
                    SendersCallsign = b2F.From;

                    for (int i = 0; i < b2F.Files.Count; i++)
                    {
                        string ext = Path.GetExtension(b2F.Files[i].Filename).ToLower();
                        if (ext == ".xml")
                        {
                            MessageXML = b2F.Files[i].Contents;
                        }
                        else if (ext == ".txt")
                        {
                            MessageString = b2F.Files[i].Contents;
                        }
                    }
                    break;
                case ".mime":
                default:
                    try
                    {
                        string pattern = @"boundary=""?(?<boundary>[^"";\r\n]+)""?;?";
                        RegexOptions options = RegexOptions.Singleline;
                        Match BoundaryMatch = Regex.Match(MessageBody, pattern, options);

                        if (BoundaryMatch != null)
                        {
                            string baseBoundary = "--" + BoundaryMatch.Groups["boundary"].Value;

                            UniqueID = filename;

                            msgArr = MessageBody.Split(baseBoundary);

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
                                MessageString = msgArr[1];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("** Exception while trying to find XML data in message: " + ex);
                    }

                    break;
            }

            // Now let's parse the XML
            try
            {
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

                int posLastDot = MessageSubject.LastIndexOf('.');
                if (posLastDot > 0)
                {
                    MessageSubject = MessageSubject.Substring(0, posLastDot);
                }

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

                MessageString = MessageString.Replace("\r\nContent-Type: text/plain; charset=\"iso-8859-1\"\r\nContent-Transfer-Encoding: quoted-printable\r\n\r\n\r\n", "");
                MessageString = MessageString.Replace("=20", " ").Replace("=A0", " ").Replace("=B0", "°");

                int indexOfMessageTerminator = MessageString.IndexOf("\r\n\r\n------------------------------------\r\nExpress Sending Station: ");
                if (indexOfMessageTerminator > -1)
                {
                    MessageString = MessageString.Substring(0, indexOfMessageTerminator);
                }

                SendersCallsign = document.SelectSingleNode("RMS_Express_Form/form_parameters/senders_callsign", mgr).InnerText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("** Exeception thrown while parsing XML data: " + ex);
            }
        }

        public MapItem ToMapItem()
        {
            MapItem mapItem = new MapItem(MessageSubject, MessageString, Latitude, Longitude, "", "", DateTimeSent, UniqueID, Recipient, MessageXML);
            return mapItem;
        }
    }
}


