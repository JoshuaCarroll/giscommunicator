<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="gisserver.map._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AA5JC</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="crossorigin" />
    <link href="https://fonts.googleapis.com/css2?family=Antonio:wght@400;700&display=swap" rel="stylesheet" />
    <style type="text/css">
        body {
            background-color: #000000;
            color: #ffffff;
            font-family: Antonio;
        }
        a, a:active, a:visited {
            color: #04f7ff;
        }

        .screenshot {
            height: 275px;
            margin: 10px;
            border: 1px solid #bfbfbf;
        }

        #btnUrl {
            margin-left: 40px;
        }
    </style>
	<script type="text/javascript">

        var copyToClipboard_originalText;
        var copyToClipboard_originalColor;
        var copyToClipboard_caller;

        function copyToClipboard(copyText, obj) {
            navigator.clipboard.writeText(copyText);

            copyToClipboard_originalText = obj.value;
            copyToClipboard_caller = obj;

            obj.value = "URL copied to your clipboard!";
            obj.style.backgroundColor = "#eff404";
            
            setTimeout(copyToClipboard_done, 2000);
        }

        function copyToClipboard_done() {
            copyToClipboard_caller.value = copyToClipboard_originalText;
            copyToClipboard_caller.style.backgroundColor = "";
            copyToClipboard_caller = null;
            copyToClipboard_originalText = null;
        }
    </script>
</head>
<body>
    <h1>AA5JC SitRep Map</h1>
    <form id="form1" runat="server">
        <div>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" Font-Bold="True" ForeColor="#FF3300" />
            <p>
                <img alt="Screenshot of the sit rep map." src="../screenshot.jpg" align="right" class="screenshot" />The primary purpose of this tool is to provide government and agency leaders with actionable infomation they need to make decisions.&nbsp; This situational awareness map will show relevant icons representing the item plotted, ensuring that leaders can glance at the map and know what it happening in their area of concern.&nbsp; By following the steps below, you will be able to view the data selected on a map.</p>
            <p>
                Your map will automatically refresh its data at set intervals.&nbsp; Once you download your KML file, you won&#39;t need to download a new one ever again unless you need to change the settings selected below.</p>
            <hr />
            <ol>
                <li>Install Google Earth, available at <a target="_blank" href="https://www.google.com/earth/about/versions/?gl=US&hl=en#download-pro">https://www.google.com/earth/</a></li>
                <li>
                    <asp:Label ID="lblInstructions" runat="server" Text="Select the options below, then submit the form to generate your map:"></asp:Label>
                    <ul>
                        <li><asp:CheckBox ID="chkWinlink" runat="server" Text="Winlink forms" OnCheckedChanged="chkWinlink_CheckedChanged" AutoPostBack="True" />
                                <asp:CustomValidator ID="validatorSelectOne" runat="server" ErrorMessage="You must select at least one data set." OnServerValidate="validatorSelectOne_ServerValidate" ValidateEmptyText="True" ForeColor="#CC3300">*</asp:CustomValidator>
                            <asp:Panel ID="pnlWinlink" runat="server" Visible="false">
                                <label>Report recipient's callsign:</label><asp:TextBox ID="txtRecipient" runat="server"></asp:TextBox>
                                <asp:CustomValidator ID="validatorRecipientCallsign" runat="server" ErrorMessage="You checked the box to include Winlink data, but you didn't specifiy a recipient whose reports should be displayed." OnServerValidate="validatorRecipientCallsign_ServerValidate" ValidateEmptyText="True" ForeColor="#CC3300">*</asp:CustomValidator>
                                <br />
                            </asp:Panel>
                        </li>
                        <li><asp:CheckBox ID="chkNetlogger" runat="server" Text="Netlogger check-ins" AutoPostBack="True" OnCheckedChanged="chkNetlogger_CheckedChanged" />
                            <asp:Panel ID="pnlNetlogger" runat="server" Visible="false">
                                <label>Select the net from this list of active nets on Netlogger:</label><asp:DropDownList ID="ddlActiveNets" runat="server"></asp:DropDownList>
                                <asp:CustomValidator ID="validatorNetlogger" runat="server" ErrorMessage="You checked the box to include NetLogger check-ins, but didn't specify a net.  If your net isn't listed, open your net in NetLogger then refresh this page.  This is because we must verify the net name and NetLogger server that your net uses." ForeColor="#CC3300" OnServerValidate="validatorNetlogger_ServerValidate" ValidateEmptyText="True">*</asp:CustomValidator>
                                <br />
                            </asp:Panel>
                        </li>
                        <li><asp:CheckBox ID="chkWeather" runat="server" Text="Severe weather polygons" AutoPostBack="False" /></li>
                    </ul>
                </li>
                <li><asp:Button ID="btnGenerate" runat="server" Text="Generate KML and link" OnClick="btnGenerate_Click" /></li>
                <li><asp:Button ID="btnDownload" runat="server" Enabled="False" OnClick="btnDownload_Click" Text="Download KML file" /></li>
                <li>Open the <abbr title="Keyhole Markup Language: A standard file format used to display geographic data in an Earth browser such as Google Earth.">KML</abbr> file.&nbsp; This should open Google Earth and display the data. (If nothing displays, it&#39;s possible that there is no data available yet.)</li>
            </ol>
            <asp:Panel ID="pnlActions" runat="server" Visible="false">
                <br />
                <asp:Button ID="btnUrl" runat="server" Text="Click here to copy a link you can share with others who want to view this same data set" UseSubmitBehavior="False" />
            </asp:Panel>
        </div>
    </form>
</body>
</html>
