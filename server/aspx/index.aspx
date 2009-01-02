<%@ Page language="C#" MasterPageFile="~/Master.master" 
	CodeFile="index.aspx.cs" Inherits="NMapi.Server.UI.Pages.IndexPage" %>
<%@ MasterType VirtualPath="~/Master.master" %>

<asp:content contentplaceholderid="Header" runat="server">
	<script type="text/javascript">
		$(document).ready ( function() {
			$("#overviewTable").tablesorter (); 
		});
	</script>
</asp:content>
<asp:content id="MainContent" contentplaceholderid="Main" runat="server">
	<h1>Overview</h1>

<!--	<div id="mustConfigureBackendNote" class="task" runat="server">
		No backend has been added to the list of backends. 
		You should configure at least one backend.
	</div>
//-->

	<div class="overviewPanel">
		<h3>Summary</h3>

		<table id="overviewTable" class="tablesorter" >
			<thead>
				<tr class="vtableHead"><th>Name</th><th>Value</th></tr>
			</thead>
			<tbody>
				<tr><td>Server started:</td><td><asp:label id="startedAt" runat="server" /></td></tr>
				<tr><td>Last login (admin):</td><td><asp:label id="lastLoginAdmin" runat="server" /></td></tr>
				<tr><td>Uptime:</td><td><asp:label id="uptime" runat="server" /></td></tr>
				<tr><td>Restarts:</td><td><asp:label id="timesRestarted" runat="server" /></td></tr>
				<tr><td>Last Start:</td><td><asp:label id="lastStartTime" runat="server" /></td></tr>
				<tr><td>Sessions:</td><td><asp:label id="sessions" runat="server" /></td></tr>
				<tr><td>Version:</td><td><asp:label id="version" runat="server" /></td></tr>
			</tbody>
		</table>
	</div>
	<div class="overviewPanel">
		<h3>Server Control</h3>
		<form runat="server">
			<asp:button text="Restart Server" runat="server" /><br /><br />
			<asp:button text="Shutdown Server" runat="server" />
		</form>
	</div>

	<div class="overviewPanel">
		<h3>Installed Backends</h3>
		
		...
		
	</div>

	<div class="overviewPanel">
		<h3>Recent Events</h3>
		
		...
	</div>

	<div style="clear:both;"></div>


</asp:content>
<asp:content contentplaceholderid="SidebarContent" runat="server">
	<h3>Links</h3>
	<p>
		<a href="http://www.openmapi.org">http://www.openmapi.org</a>
	</p>
	<h3>About OpenMapi.org Server</h3>

	<p>
		...
	</p>
</asp:content>
