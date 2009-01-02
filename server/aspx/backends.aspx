<%@ Page language="C#" MasterPageFile="~/Master.master" 
	CodeFile="backends.aspx.cs" Inherits="NMapi.Server.UI.Pages.BackendsPage" %>
<%@ MasterType VirtualPath="~/Master.master" %>
<asp:content contentplaceholderid="Header" runat="server">
	<script type="text/javascript">
		$(document).ready ( function() {
			$("#backendsTable").tablesorter (); 

			$("#configureDialog").dialog ({
				autoOpen: false,
				modal: true, 
				height: 200,
				width: 450,
				minHeight: 200,
				minWidth: 450,
				overlay: { 
					opacity: 0.5, 
					background: "black" 
				} 
			});

			$(".editButton").click ( function() {
				$("#confTitle").text ($(this).data ("title"));
				$("#confBackendId").attr ("value", $(this).data ("backendId"));
				$("#confBackendTitle").attr ("value", $(this).data ("title"));
				$("#configureDialog").dialog ("open");
			});

			$("#" + serverSideInfo.addButtonId).click ( function() {
				alert ("add!");
			});

			if (serverSideInfo.showAlert != null)
				alert (serverSideInfo.showAlert);

		});


	</script>
</asp:content>
<asp:content id="MainContent" contentplaceholderid="Main" runat="server">
	<h1>Backend Manager</h1>

	<form id="backendsForm" runat="server">
		<script>
			$(document).ready (function() {
				$("#backendsTable_row0").data ("backendId", "0");
				$("#backendsTable_row0").data ("title", "My teamXchange Server");
			});
		</script>
		<table id="backendsTable" class="tablesorter" >
			<thead>
				<tr>
					<th>Enabled</th>
					<th>Name</th>
					<th>Host</th>
					<th>Backend Type</th>
					<th>Status</th>
					<th>Delete</th>
					<th>&nbsp;</th>
				</tr>
			</thead>
			<tbody>
				<tr>
					<td>&nbsp;</td>
					<td>My teamXchange Server</td>
					<td>localhost</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>
						<input type="button" class="editButton" id="backendsTable_row0" value="Configure" />
					</td>
				</tr>
			</tbody>
		</table>
		<asp:button id="addButton" Text="Add Backend" runat="server" />
	</form>

	<div id="configureDialog">
		<form id="configureForm" runat="server">
			<input type="hidden" id="confBackendId" name="confBackendId" />
			<input type="hidden" id="confBackendTitle" name="confBackendTitle" />

			<big>Configure <span id="confTitle"></span></big>
			<p>
				<small>Please enter the admin password of the 
					backend (if any) and click "Continue".</small>
			</p>
			<table align="center">
				<tr>
					<td>Password:</td>
					<td><input type="password" id="confPassword" name="confPassword" /></td>
					<td><asp:button text="Continue" onclick="ConfigureClicked" runat="server" /></td>
				</tr>
			</table>
		</form>
	</div>
</asp:content>

