<%@ Page language="C#" CodeFile="login.aspx.cs" 
	Inherits="NMapi.Server.UI.Pages.LoginPage" %>

<script runat="server"></script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
<head>
	<title>OpenMapi.org Proxy Server</title>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<style type="text/css" media="all">@import "/style.css";</style>
	<style>

		#slidePanel {
			width:500px;
			padding:0px;
		}

		.loginPanel {
			margin:0px;
			width:500px;
			border:1px #c0c0c0 solid;
		}

		.loginPanel td {
			background-color:#ffffff;
		}

		.failed {
			clear:both;
			padding:10px;
			margin:20px;
			color:#ffffff;
			background-color:#b92121;
		}
	</style>
	<script type="text/javascript" src="/js/jquery-1.2.6.min.js"></script>
	<script type="text/javascript" src="/js/jquery.corner.js"></script>
	<script type="text/javascript"><%=ssi.JavaScriptDefinition%></script>
	<script type="text/javascript">
		$(document).ready (function () {
			$("#failed").hide ();
			if (!serverSideInfo.isPostBack)
				$("#slidePanel").hide ();
			$("#password").focus ();
		});

		$(window).load (function () {
			if (!serverSideInfo.isPostBack) {
				setTimeout(function() {
					$("#slidePanel").fadeIn ('slow');
					$("#password").focus ();
				}, 300);
			}
			if (serverSideInfo.showFailed)
				$("#failed").corner ("round").fadeIn ('slow');

		});

	</script>
</head>
<body>
	<center>
	<form id="loginForm" runat="server">

		<div style="height:400px;">

		<table class="loginPanel" cellpadding="5" cellspacing="5" align="center">
			<tr style="height:80px;">
				<td colspan="2" style="background-color:#efefef;">&nbsp;</td>
			</tr>
			<tr style="height:5px;">
				<td colspan="2" style="background-color:#016ce5;"></td>
			</tr>
			<tr style="background-color:#ffffff;">
				<td colspan="2" style="height:120px;text-align:right;" valign="top">
					<img src="logo.png" alt="openmapi.org" border="0" /><br />
					<big>Server - Administration</big>
				</td>
			</tr>
		</table>
		<div id="slidePanel">
		<table class="loginPanel" cellpadding="5" cellspacing="5" align="center">
			<tr>
				<td>Password:</td>
				<td>
					<asp:Textbox id="password" Textmode="password" 
						style="width:100%;" runat="server" />
				</td>
			</tr>
			<tr>
				<td>&nbsp;</td>
				<td style="height:60px;">
					<div id="failed" class="failed" runat="server">
						Password incorrect. Please try again.
					</div>
					<asp:Button onclick="LoginClicked" text="Login" 
						style="float: right;" runat="server" />
				</td>
			</tr>
		</table>
		</div>

		</div>

	</form>
	</center>
	<div id="footer">
		<b>OpenMapi.org</b> Proxy. - <% Response.Write (DateTime.Now); %>
	</div>

</body>
</html>
