//
// openmapi.org - NMapi C# Mapi API - RestHttpRouteHandler.cs
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
using System.Web;
using System.Web.Routing;

namespace NMapi.Server {

	/// <summary>
	///  Routes REST API requests to the RestHttpHandler handler ...
	/// </summary>
	public class RestHttpRouteHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler (RequestContext requestContext)
		{
			RestHttpHandler handler = new RestHttpHandler ();
			handler.RequestContext = requestContext;
			return handler;
		}
	}

}
