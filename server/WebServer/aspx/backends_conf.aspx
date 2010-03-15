<%@ Page language="C#" MasterPageFile="~/Master.master" 
	CodeFile="backends_conf.aspx.cs" Inherits="NMapi.Server.UI.Pages.BackendsConfPage" %>
<%@ MasterType VirtualPath="~/Master.master" %>
<script runat="server"></script>
<asp:content contentplaceholderid="Header" runat="server">
	<script type="text/javascript">
		$(document).ready (function() {
			$("#" + serverSideInfo.userTableId).tablesorter (); 

			$("#tabs > ul").tabs ({ fx: { height: "toggle", opacity: "toggle" } });

		});
	</script>
</asp:content>
<asp:content id="MainContent" contentplaceholderid="Main" runat="server">
	<h1>Configure Backend: <asp:label id="backendTitle" runat="server" /></h1>


	<form runat="server">
		<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

        <div id="tabs" class="widget">

            <ul class="tabnav">
                <li><a href="#users">Users</a></li>
                <li><a href="#groups">Groups</a></li>
                <li><a href="#permissions">Permissions</a></li>
            </ul>

            <div id="users" class="tabdiv">

		<asp:UpdatePanel id="userSelectorPanel" runat="server">
			<ContentTemplate>
				<table align="center" border="0">
				<tr>
					<td>Select User:</td>
					<td>
						<asp:dropdownlist id="activeUser" runat="server" 
							OnSelectedIndexChanged="UserChanged" 
							AutoPostBack="True" Width="200px">
						</asp:dropdownlist>
					</td>
					<td><b>|</b></td>
					<td>Add User:</td>
					<td><asp:TextBox id="addUserName" runat="server" /></td>
					<td><asp:button Text="Add User" onclick="AddUser" runat="server" /></td>
				</tr>
				</table>
			</ContentTemplate>
		</asp:UpdatePanel>


		<asp:UpdatePanel id="userUpdatePanel" runat="server">
			<ContentTemplate>
				<h2>Edit User: <asp:label id="userId" runat="server" /><!-- careful: label is used in code! //--></h2>
			
				<h3>User Information</h3>

				<table>
					<tr><td>First Name:</td><td><asp:TextBox id="userFirstName" runat="server" /> (opt)</td></tr>
					<tr><td>Last Name:</td><td><asp:TextBox id="userLastName" runat="server" /> (opt)</td></tr>
					<tr><td>Description:</td><td><asp:TextBox id="userComment" runat="server" /> (opt)</td></tr>
				</table>

				<h3>Groups</h3>

				<table>
					<tr><td>[X]</td><td>Group1</td></tr>
					<tr><td>[X]</td><td>Group2</td></tr>
				</table>

				<br /><br />

				<p>
					<asp:button onclick="DeleteCurrentUser" Text="Delete" runat="server" />
				</p>

			</ContentTemplate>
		</asp:UpdatePanel>


            </div>
            
            <div id="groups" class="tabdiv">

			yyy

            </div>
            
            <div id="permissions" class="tabdiv">
		<table cellpadding="20" border="0">
		<tr>
		<td valign="top" style="width:200px;">
			<asp:UpdatePanel id="treeViewPanel" runat="server">
				<ContentTemplate>
					<asp:TreeView id="foldersTreeView"
						EnableClientScript="true" 
						PathSeparator="/"
						OnTreeNodePopulate="TreeNodePopulate"
						OnSelectedNodeChanged="TreeNodeSelected"
						runat="server">

						<Nodes>
							<asp:TreeNode Text="/" Value="ROOT" SelectAction="Expand" PopulateOnDemand="true" />
						</Nodes>
					</asp:TreeView>
				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
		<td valign="top">
			<asp:UpdatePanel id="permissionEditPanel" runat="server">
				<ContentTemplate>
					<h3>Permissions for: '<asp:label id="xxx" runat="server" />'</h3>

					

				</ContentTemplate>
			</asp:UpdatePanel>
		</td>
		</tr>
		</table>

            </div>

        </div>



	</form>

</asp:content>
