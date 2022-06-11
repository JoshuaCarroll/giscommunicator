using System;
using System.Xml;

namespace gisreporter_
{
	public class MapItem
	{
        public string Name { get; set; }
        public string Description { get; set; }
        public string LocationLatitude { get; set; }
        public string LocationLongitude { get; set; }
        public string LocationDescription { get; set; }
        public string Icon { get; set; }
        public string ReportedDateTime { get; set; }
        public string UniqueID { get; set; }
        public string Recipient { get; set; }
        public string FormData { get; set; }

        public MapItem(string name, string description, string locationLatitude, string locationLongitude, string locationDescription, string icon, string reportedDateTime, string uniqueid, string recipient, string formData)
        {
            Name = name;
            Description = description;
            LocationLatitude = locationLatitude;
            LocationLongitude = locationLongitude;
            LocationDescription = locationDescription;
            ReportedDateTime = reportedDateTime;
            UniqueID = uniqueid;
            Recipient = recipient;
            FormData = formData;
        }

        public void GetIconFromXml(string formData)
        {
            #region Determine the icon to use
            XmlDocument document = new XmlDocument();
            document.LoadXml(formData);
            XmlNamespaceManager mgr = new XmlNamespaceManager(document.NameTable);
            mgr.AddNamespace("ns", "http://www.aa5jc.com/namespace");

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
}

