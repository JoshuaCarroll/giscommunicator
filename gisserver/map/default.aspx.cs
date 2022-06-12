using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace gisserver.map
{
    public partial class _default : System.Web.UI.Page
    {
        string thisServer;
        Parameters parameters;

        protected void Page_Load(object sender, EventArgs e)
        {
            thisServer = HttpContext.Current.Request.Url.AbsoluteUri;
            thisServer = thisServer.Substring(0, thisServer.LastIndexOf(HttpContext.Current.Request.Url.AbsolutePath));

            if (!this.IsPostBack)
            {
                string strP = Request.QueryString["p"];

                if (strP != null && strP != "")
                {
                    parameters = Parameters.FromString(strP);

                    chkNetlogger.Checked = parameters.Netlogger;
                    pnlNetlogger.Visible = parameters.Netlogger;
                    if (parameters.Netlogger)
                    {
                        ddlActiveNets.Items.Clear();
                        ddlActiveNets.Items.Add(parameters.NetloggerNetName);
                    }

                    chkWeather.Checked = parameters.Weather;

                    chkWinlink.Checked = parameters.Winlink;
                    pnlWinlink.Visible = parameters.Winlink;
                    if (parameters.Winlink)
                    {
                        txtRecipient.Text = parameters.WinlinkRecipient;
                    }
                    string url = thisServer + "/map/?p=" + parameters.ToString();
                    btnUrl.OnClientClick = "javascript:copyToClipboard('" + url + "', this);return false;";

                    lblInstructions.Text = "Review the options below that were already selected for you:";

                    chkNetlogger.Enabled = false;
                    chkWeather.Enabled = false;
                    chkWinlink.Enabled = false;
                    txtRecipient.Enabled = false;
                    ddlActiveNets.Enabled = false;
                    btnGenerate.Enabled = false;
                    btnDownload.Enabled = true;
                    pnlActions.Visible = true;
                }
                else
                {
                    LoadActiveNets();
                }
            }
        }

        protected void LoadActiveNets()
        {
            //ddlActiveNets.Items.Clear();
            ddlActiveNets.Items.Add(new ListItem("* SELECT ONE *", ""));

            XDocument xml = XDocument.Load("http://www.netlogger.org/api/GetActiveNets.php");
            foreach (XElement server in xml.Descendants("Server"))
            {
                foreach (XElement net in server.Descendants("Net"))
                {
                    string url = string.Format("http://www.netlogger.org/api/GetCheckins.php?ServerName={0}&NetName={1}", server.Element("ServerName").Value, net.Element("NetName").Value);
                    ddlActiveNets.Items.Add(new ListItem(net.Element("NetName").Value, url));
                }
            }
        }
        protected void chkWinlink_CheckedChanged(object sender, EventArgs e)
        {
            pnlWinlink.Visible = chkWinlink.Checked;
        }
        protected void chkNetlogger_CheckedChanged(object sender, EventArgs e)
        {
            pnlNetlogger.Visible = chkNetlogger.Checked;
        }

        protected void validatorRecipientCallsign_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (chkWinlink.Checked)
            {
                if (txtRecipient.Text.Trim() != string.Empty)
                {
                    args.IsValid = true;
                }
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void validatorNetlogger_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (chkNetlogger.Checked)
            {
                if (ddlActiveNets.SelectedValue != string.Empty)
                {
                    args.IsValid = true;
                }
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                Parameters p = new Parameters();
                p.Winlink = chkWinlink.Checked;
                p.Weather = chkWeather.Checked;
                p.Netlogger = chkNetlogger.Checked;

                if (p.Winlink)
                    p.WinlinkRecipient = txtRecipient.Text;

                if (p.Netlogger)
                {
                    p.NetloggerNetName = ddlActiveNets.SelectedItem.Text;
                    p.NetloggerUrl = ddlActiveNets.SelectedItem.Value;
                    int start = p.NetloggerUrl.IndexOf("=") + 1;
                    int length = p.NetloggerUrl.IndexOf("&") - start;
                    p.NetloggerServer = p.NetloggerUrl.Substring(start, length);
                }

                Response.Redirect("/map/?p=" + p.ToString());
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            Parameters p = Parameters.FromString(Request.QueryString["p"]);

            string filename = "";
            if (p.Winlink)
                filename += p.WinlinkRecipient.Trim().Replace(" ", "").Replace(',', '_') + "-";
            if (p.Weather)
                filename += "Wx-";
            if (p.Netlogger)
            {
                if (p.NetloggerNetName.Length > 15)
                {
                    filename += p.NetloggerNetName.Trim().Substring(0, 15);
                }
                else
                {
                    filename += p.NetloggerNetName.Trim();
                }
            }

            string kml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"" xmlns:kml=""http://www.opengis.net/kml/2.2"" xmlns:atom=""http://www.w3.org/2005/Atom"">
<Document>
    <name>" + filename + @"</name>
    <open>1</open>
    <NetworkLink>
	    <name>AA5JC SitRep</name>
	    <open>1</open>
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
		    <href>" + thisServer + "/map/map.ashx?p=" + p.ToString() + @"</href>
		    <refreshMode>onInterval</refreshMode>
		    <refreshInterval>3600</refreshInterval>
	    </Link>
    </NetworkLink>
</Document>
</kml> ";

            Response.Clear();
            Response.ContentType = "application/vnd.google-earth.kml+xml";
            Response.AddHeader("content-disposition", "attachment;    filename=" + filename + ".kml");
            Response.Write(kml);
            Response.End();
        }

        protected void validatorSelectOne_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!chkNetlogger.Checked && !chkWeather.Checked && !chkWinlink.Checked)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
    }
}