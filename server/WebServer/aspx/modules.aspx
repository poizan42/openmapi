<%@ Page language="C#" MasterPageFile="~/Master.master" 
	CodeFile="modules.aspx.cs" Inherits="NMapi.Server.UI.Pages.ModulesPage" %>
<%@ MasterType VirtualPath="~/Master.master" %>
<%@ Import Namespace="NMapi.Server.ICalls" %>
<script runat="server"></script>
<asp:content id="MainContent" contentplaceholderid="Main" runat="server">
	<h1>Modules</h1>

	<asp:table id="moduleTable" BorderWidth="1" GridLines="both" runat="server" />

</asp:content>
