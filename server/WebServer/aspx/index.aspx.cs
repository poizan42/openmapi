//
// openmapi.org - NMapi C# Mapi API - index.aspx.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;
using System.Web;
using System.Web.UI;

using NMapi.Server;
using NMapi.Server.ICalls;

namespace NMapi.Server.UI.Pages {

	public partial class IndexPage : Page
	{
		protected void Page_Load (object sender, EventArgs ea)
		{
			IInternalCalls calls = InternalCallsClient.GetNewClient ();

			version.Text = calls.Version;
			sessions.Text = String.Empty + calls.SessionManager.CurrentSessions.Length;

			ProxyInformation pinfo = calls.ProxyInformation;

			if (Session ["lastLogin"] != null) {
				DateTime lastLogin = (DateTime) Session ["lastLogin"];
				lastLoginAdmin.Text = lastLogin.ToString ();
			}

			uptime.Text = pinfo.Uptime.ToString ();
			startedAt.Text = pinfo.InitTime.ToString ();
			lastStartTime.Text = pinfo.LastStartTime.ToString ();
			timesRestarted.Text = String.Empty + pinfo.TimesRestarted;

		}
	}

}
