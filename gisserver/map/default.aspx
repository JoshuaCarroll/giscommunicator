<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="gisserver.map._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <h1>AA5JC SitRep</h1>
    <form id="form1" runat="server">
        <div>
            <br />
            Select the options below, then submit the form to generate your KML file.<asp:ValidationSummary ID="ValidationSummary1" runat="server" Font-Bold="True" ForeColor="#CC3300" />
            <br />
            <ol>
                <li>Install Google Earth, available at <a target="_blank" href="https://www.google.com/earth/about/versions/?gl=US&hl=en#download-pro">https://www.google.com/earth/</a></li>
                <li>Select the options below:<br />
                    <label>Select the datasets to include:</label><br />
                    <ul>
                        <li><asp:CheckBox ID="chkWinlink" runat="server" Text="Winlink GIS forms" OnCheckedChanged="chkWinlink_CheckedChanged" AutoPostBack="True" />
                            <asp:Panel ID="pnlWinlink" runat="server" Visible="false">
                                <label>Report recipient's callsign:</label><asp:TextBox ID="txtRecipient" runat="server"></asp:TextBox>
                                <asp:CustomValidator ID="validatorRecipientCallsign" runat="server" ErrorMessage="CustomValidator" OnServerValidate="validatorRecipientCallsign_ServerValidate" ValidateEmptyText="True" ForeColor="#CC3300">*</asp:CustomValidator>
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
                        <li><asp:CheckBox ID="chkWeather" runat="server" Text="Severe weather polygons" AutoPostBack="True" OnCheckedChanged="chkWeather_CheckedChanged" />
                            <asp:Panel ID="pnlWeather" runat="server" Visible="false">
                                <label>Load weather data for this state:</label>
								<asp:DropDownList ID="ddlStates" runat="server">
									<asp:ListItem Value="AL">* SELECT ONE *</asp:ListItem>
									<asp:ListItem Value="AL">Alabama</asp:ListItem>
									<asp:ListItem Value="AK">Alaska</asp:ListItem>
									<asp:ListItem Value="AZ">Arizona</asp:ListItem>
									<asp:ListItem Value="AR" Selected="True">Arkansas</asp:ListItem>
									<asp:ListItem Value="CA">California</asp:ListItem>
									<asp:ListItem Value="CO">Colorado</asp:ListItem>
									<asp:ListItem Value="CT">Connecticut</asp:ListItem>
									<asp:ListItem Value="DE">Delaware</asp:ListItem>
									<asp:ListItem Value="DC">District of Columbia</asp:ListItem>
									<asp:ListItem Value="FL">Florida</asp:ListItem>
									<asp:ListItem Value="GA">Georgia</asp:ListItem>
									<asp:ListItem Value="HI">Hawaii</asp:ListItem>
									<asp:ListItem Value="ID">Idaho</asp:ListItem>
									<asp:ListItem Value="IL">Illinois</asp:ListItem>
									<asp:ListItem Value="IN">Indiana</asp:ListItem>
									<asp:ListItem Value="IA">Iowa</asp:ListItem>
									<asp:ListItem Value="KS">Kansas</asp:ListItem>
									<asp:ListItem Value="KY">Kentucky</asp:ListItem>
									<asp:ListItem Value="LA">Louisiana</asp:ListItem>
									<asp:ListItem Value="ME">Maine</asp:ListItem>
									<asp:ListItem Value="MD">Maryland</asp:ListItem>
									<asp:ListItem Value="MA">Massachusetts</asp:ListItem>
									<asp:ListItem Value="MI">Michigan</asp:ListItem>
									<asp:ListItem Value="MN">Minnesota</asp:ListItem>
									<asp:ListItem Value="MS">Mississippi</asp:ListItem>
									<asp:ListItem Value="MO">Missouri</asp:ListItem>
									<asp:ListItem Value="MT">Montana</asp:ListItem>
									<asp:ListItem Value="NE">Nebraska</asp:ListItem>
									<asp:ListItem Value="NV">Nevada</asp:ListItem>
									<asp:ListItem Value="NH">New Hampshire</asp:ListItem>
									<asp:ListItem Value="NJ">New Jersey</asp:ListItem>
									<asp:ListItem Value="NM">New Mexico</asp:ListItem>
									<asp:ListItem Value="NY">New York</asp:ListItem>
									<asp:ListItem Value="NC">North Carolina</asp:ListItem>
									<asp:ListItem Value="ND">North Dakota</asp:ListItem>
									<asp:ListItem Value="OH">Ohio</asp:ListItem>
									<asp:ListItem Value="OK">Oklahoma</asp:ListItem>
									<asp:ListItem Value="OR">Oregon</asp:ListItem>
									<asp:ListItem Value="PA">Pennsylvania</asp:ListItem>
									<asp:ListItem Value="RI">Rhode Island</asp:ListItem>
									<asp:ListItem Value="SC">South Carolina</asp:ListItem>
									<asp:ListItem Value="SD">South Dakota</asp:ListItem>
									<asp:ListItem Value="TN">Tennessee</asp:ListItem>
									<asp:ListItem Value="TX">Texas</asp:ListItem>
									<asp:ListItem Value="UT">Utah</asp:ListItem>
									<asp:ListItem Value="VT">Vermont</asp:ListItem>
									<asp:ListItem Value="VA">Virginia</asp:ListItem>
									<asp:ListItem Value="WA">Washington</asp:ListItem>
									<asp:ListItem Value="WV">West Virginia</asp:ListItem>
									<asp:ListItem Value="WI">Wisconsin</asp:ListItem>
									<asp:ListItem Value="WY">Wyoming</asp:ListItem>
								</asp:DropDownList>
                                <asp:CustomValidator ID="validatorWeather" runat="server" ErrorMessage="You selected the option to include weather data, but you didn't select a state." ForeColor="#CC3300" OnServerValidate="validatorWeather_ServerValidate" ValidateEmptyText="True">*</asp:CustomValidator>
                                <br />
                            </asp:Panel>
                        </li>
                    </ul>
                </li>
                <li><asp:Button ID="btnGenerate" runat="server" Text="Click here to generate and download your KML file" OnClick="btnGenerate_Click" /></li>
                <li>Open the KML file.&nbsp; This should open Google Earth and display the data. You can share this with others who want to view the same data.</li>
            </ol>
        </div>
    </form>
</body>
</html>
