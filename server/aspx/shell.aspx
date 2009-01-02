<%@ Page language="C#" MasterPageFile="~/Master.master" 
	CodeFile="shell.aspx.cs" Inherits="NMapi.Server.UI.Pages.ShellPage" %>

<%@ MasterType VirtualPath="~/Master.master" %>
<script runat="server"></script>
<asp:content contentplaceholderid="Header" runat="server">
	<script type="text/javascript" src="js/mouseapp_2.js"></script>
	<script type="text/javascript" src="js/mouseirb_2.js"></script>
	<script type="text/javascript" src="js/irb.js"></script>
	<style type="text/css">
		@import 'shell.css';
	</style>
</asp:content>
<asp:content id="MainContent" contentplaceholderid="Main" runat="server">
	<h1>Shell</h1>

	<input class="keyboard-selector-input" type="text" id="irb_input" autocomplete="off" />

	<table width="100%" cellpadding="0" cellspacing="0">
	<tr>
		<td valign="top">
			<div id="shellwin">
				<div id="terminal">
				    <div id="irb"></div>
				</div>
			</div>
		</td>
		<td style="width:150px;" valign="top">


			<h2>Create new shell</h2>

			<h2>Attach to Session</h2>

			<ul id="attachList" runat="server">
			</ul>


		</td>
	</tr>
	</table>

</asp:content>
