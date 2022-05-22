using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gisserver
{
    /// <summary>
    /// Summary description for _default
    /// </summary>
    public class _default : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string kml2 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"" xmlns:kml=""http://www.opengis.net/kml/2.2"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<NetworkLink>
	<name>SitRep Loader</name>
	<open>1</open>
	<description>Amateur radio messages and operators</description>
	<LookAt>
		<longitude>-92.10432730888137</longitude>
		<latitude>34.82677287952281</latitude>
		<altitude>0</altitude>
		<heading>0.2481151053895833</heading>
		<tilt>0</tilt>
		<range>622153.3130889114</range>
		<gx:altitudeMode>relativeToSeaFloor</gx:altitudeMode>
	</LookAt>
	<Link>
		<href>https://giscommunicator.azurewebsites.net/api/gisreceiver?code=xNjuM3fBnIsZ28iTEiUNbFQcmBEdPkp5aspqxVm1aaJ6cGItH4dcyQ==&amp;recipient=ALL</href>
		<refreshMode>onInterval</refreshMode>
		<refreshInterval>180</refreshInterval>
	</Link>
</NetworkLink>
</kml> ";

			string kml1 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"" xmlns:kml=""http://www.opengis.net/kml/2.2"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<NetworkLink>
	<name>AA5JC SitRep</name>
	<open>1</open>
	<description>Amateur radio messages and operators</description>
	<Link>
		<href>https://aa5jc.com/&amp;recipient=ALL</href>
		<refreshMode>onInterval</refreshMode>
		<refreshInterval>3600</refreshInterval>
	</Link>
</NetworkLink>
</kml> ";

			context.Response.ContentType = "application/vnd.google-earth.kml+xml";
            context.Response.Write(kml2);
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