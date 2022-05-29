using System;
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

        public MapItem(string name, string description, string locationLatitude, string locationLongitude, string locationDescription, string icon, string reportedDateTime, string uniqueid, string recipient)
        {
            Name = name;
            Description = description;
            LocationLatitude = locationLatitude;
            LocationLongitude = locationLongitude;
            LocationDescription = locationDescription;
            Icon = icon;
            ReportedDateTime = reportedDateTime;
            UniqueID = uniqueid;
            Recipient = recipient;
        }
    }
}

