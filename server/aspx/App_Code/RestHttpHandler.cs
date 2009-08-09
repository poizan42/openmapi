//
// openmapi.org - NMapi C# Mapi API - RestHttpHandler.cs
//
// Copyright 2009 Topalis AG
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
using System.Web.Routing;

namespace NMapi.Server {

	/// <summary>
	///  HTTP Handler to process request coming in through the REST API.
	/// </summary>
	public class RestHttpHandler : IHttpHandler, IRequiresSessionState
	{
		private RequestContext requestContext;
		
		public RequestContext RequestContext {
			get { return requestContext; }
			set { requestContext = value; }
		}

		public void ProcessRequest (HttpContext httpContext)
		{
			httpContext.Response.ContentType = "text/plain";
			httpContext.Response.ContentEncoding = Encoding.UTF8;
			httpContext.Response.Cache.SetCacheability (HttpCacheability.NoCache);
			
			httpContext.Response.Write ("testing 1, 2, 3, ...");
		}
		
		
		/*
		
		TODO: map to methods (on serializable interface)
			[ unpack RequestContexts + call ...]
			
			This code should be generated from some xml;
			The routing in global.asax should be as well.
				
		
		*/

		public bool IsReusable {
			get { return true; }
		}
	

	}

}
