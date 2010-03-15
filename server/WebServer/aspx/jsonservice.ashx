<% @WebHandler Language="C#" Class="NMapi.Server.Jayrock.NmapiJsonExecutive" %>

//
// openmapi.org - NMapi C# Mapi API - JsonService.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (atif.aziz@skybow.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General public override License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General public override License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//


using System;
using System.IO;
using System.ComponentModel;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;

using Jayrock.Json;
using Jayrock.JsonRpc;
using Jayrock.Services;


namespace NMapi.Server.Jayrock
{
	public sealed class NMapiJsonExecutive : IHttpHandler
	{
		private HttpContext _context;

		public HttpContext Context {
			get { return _context; }
		}

		public HttpApplication ApplicationInstance {
			get { return Context.ApplicationInstance; }
		}

		public HttpApplicationState Application {
			get { return Context.Application; }
		}

		public HttpServerUtility Server {
			get { return Context.Server; }
		}

		public HttpSessionState Session {
			get { return Context.Session; }
		}

		public HttpRequest Request {
			get { return Context.Request; }
		}

		public HttpResponse Response {
			get { return Context.Response; }
		}

		public IPrincipal User {
			get { return Context.User; }
		}

		public virtual void ProcessRequest(HttpContext context)
		{
			_context = context;
			ProcessRequest();
		}
		
		public bool IsReusable {
			get { return false; }
		}

		protected override void ProcessRequest ()
		{
			if (!CaselessString.Equals (Request.RequestType, "POST"))
				throw new JsonRpcException ("HTTP " + Request.RequestType + " is not supported " + 
											"for RPC execution. Use HTTP POST only.", ));

			//
			// Sets the "Cache-Control" header value to "no-cache".
			// NOTE: It does not send the common HTTP 1.0 request directive
			// "Pragma" with the value "no-cache".
			//

			Response.Cache.SetCacheability (HttpCacheability.NoCache);
			Response.ContentType = "text/plain";

			//
			// Delegate rest of the work to JsonRpcServer.
			//

			JsonRpcDispatcher dispatcher = new JsonRpcDispatcher (Service);

			if (Request.IsLocal)
				dispatcher.SetLocalExecution ();

			using (TextReader reader = GetRequestReader ())
				dispatcher.Process (reader, Response.Output);
		}

		private TextReader GetRequestReader ()
		{
			if (CaselessString.Equals(Request.ContentType, "application/x-www-form-urlencoded")) {
				string request = Request.Form.Count == 1 ? Request.Form[0] : Request.Form["JSON-RPC"];
				return new StringReader (request);
			}
			return new StreamReader (Request.InputStream, Request.ContentEncoding);
		}

	}
}
