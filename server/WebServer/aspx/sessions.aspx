<%@ Page language="C#" MasterPageFile="~/Master.master" 
	CodeFile="sessions.aspx.cs" Inherits="NMapi.Server.UI.Pages.SessionsPage" %>
<%@ MasterType VirtualPath="~/Master.master" %>

<script runat="server"></script>
<asp:content contentplaceholderid="Header" runat="server">
	<script type="text/javascript">
		$(document).ready (function() {
			$("#" + serverSideInfo.sessionTableId).tablesorter (); 
		});
	</script>
</asp:content>
<asp:content id="MainContent" contentplaceholderid="Main" runat="server">
	<h1>Session Monitor</h1>

	<asp:table id="sessionTable" class="vtable" runat="server">
		<asp:tableheaderrow TableSection="TableHeader" class="vtableHead">
			<asp:tableheadercell>Session ID</asp:tableheadercell>
			<asp:tableheadercell>Init Date</asp:tableheadercell>
			<asp:tableheadercell>Protocol</asp:tableheadercell>
			<asp:tableheadercell>Source</asp:tableheadercell>
			<asp:tableheadercell>AllowShellAttachment</asp:tableheadercell>
			<asp:tableheadercell>IsSecure</asp:tableheadercell>
			<asp:tableheadercell>IsPersistent</asp:tableheadercell>
			<asp:tableheadercell>RequiresSessionKey</asp:tableheadercell>
		</asp:tableheaderrow>
	</asp:table>
</asp:content>
<asp:content contentplaceholderid="SidebarContent" runat="server">
	<h3>Summary</h3>
	<p>
		Active Sessions: <b><asp:label id="sessionCount" runat="server" /></b>
	</p>
	<h3>About Session Monitor</h3>

	<p>
		The session monitor displays a list of Sessions that are 
		currently connected.
	</p>
</asp:content>

