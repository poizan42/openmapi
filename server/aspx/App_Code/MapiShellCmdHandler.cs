//
// openmapi.org - NMapi C# Mapi API - MapiShellCmdHandler.cs
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
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;

using NMapi.Server.ICalls;
using NMapi.Tools.Shell;

namespace NMapi.Server {

	public class MapiShellCmdHandler : IHttpHandler, IRequiresSessionState
	{
		public void ProcessRequest (HttpContext context)
		{
			if (context.Session ["loggedIn"] != "yes" || !context.Request.IsLocal)
				throw new Exception ("Not logged in or remote connection; Only local connections are allowed.");

			IInternalCalls calls = InternalCallsClient.GetNewClient ();

			context.Response.ContentType = "text/plain";
			context.Response.ContentEncoding = System.Text.Encoding.UTF8;
			context.Response.Cache.SetCacheability (HttpCacheability.NoCache);
			
			string result = String.Empty;
			string cmd = context.Request.QueryString ["cmd"];

			if (cmd == "!INIT!IRB!")
				context.Session ["mapishell"] = null;
			
			IMapiShell shell = (IMapiShell) context.Session ["mapishell"];
			if (shell == null) {
				shell = calls.CreateNewShell ();
				context.Session ["mapishell"] = shell;
			} else {
				shell.PutInputAndWait (cmd);
				context.Response.Write (shell.CurrentPrefix);
				context.Response.Write ("\n");
				context.Response.Write (calls.FlushShellOutputBuffer (shell));
			}
			
			if (shell.IsClosed) {
				context.Session ["mapishell"] = null;
				// TODO: rpc call: remove!
			}
		}

		public bool IsReusable {
			get { return true; }
		}

	}

}
