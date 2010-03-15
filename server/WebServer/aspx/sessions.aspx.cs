//
// openmapi.org - NMapi C# Mapi API - sessions.aspx.cs
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
using System.Web.UI.WebControls;

using NMapi.Server;
using NMapi.Server.ICalls;

namespace NMapi.Server.UI.Pages {

	public partial class SessionsPage : Page
	{
		protected void Page_Init (object sender, EventArgs ea)
		{
			Master.ActiveCategory = "monitor";

			IInternalCalls calls = InternalCallsClient.GetNewClient ();

			var currentSessions = calls.SessionManager.CurrentSessions;
			sessionCount.Text = String.Empty + currentSessions.Length;

			TableRow row;
			foreach (IProxySession session in currentSessions) {
				row = new TableRow ();
				row.TableSection = TableRowSection.TableBody;

				Utility.AddCell (row, session.Id);
				Utility.AddCell (row, "" + session.InitDate);
				Utility.AddCell (row, session.Protocol);
				Utility.AddCell (row, session.Source);
				Utility.AddCell (row, "" + session.AllowShellAttachment);
				Utility.AddCell (row, "" + session.IsSecure);
				Utility.AddCell (row, "" + session.IsPersistent);
				Utility.AddCell (row, "" + session.RequiresSessionKey);

				sessionTable.Rows.Add (row);
			}
		}

	}

}
