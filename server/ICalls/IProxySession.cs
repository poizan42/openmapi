//
// openmapi.org - NMapi C# Mapi API - IProxySession.cs
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

namespace NMapi.Server {

	/// <summary>
	/// 
	/// </summary>
	public interface IProxySession
	{
		/// <summary>
		///  The ID of the connection. This is globally unique.
		/// </summary>
		string Id { get; set; }

		/// <summary>
		///  The DateTime at the time the the Session was created.
		/// </summary>
		DateTime InitDate { get; }

		/// <summary>
		///  The name of the protcol. Usually the form is "openmapi/somename".
		/// </summary>
		string Protocol { get; }

		/// <summary>
		///  A string describing the source of the connection, e.g. 
		///  a hostname or an ip address (or a jabber jid, etc.).
		/// </summary>
		string Source { get; }

		/// <summary>
		///  
		/// </summary>
		string ClientName { get; }

		/// <summary>
		///  
		/// </summary>
		bool IsAuthenticated { get; }

		/// <summary>
		///  
		/// </summary>
		bool IsLocal { get; }

		/// <summary>
		///  True if the proxy should be able to attach a MapiShell
		///  to the running connection.
		/// </summary>
		bool AllowShellAttachment { get; }

		/// <summary>
		///  True if the connection is considered to be secure.
		/// </summary>
		bool IsSecure { get; }

		/// <summary>
		///  True if the connection is persistent; Tcp would be an 
		///  example of this. HTTP is a counter-example.
		/// </summary>
		bool IsPersistent { get; }

		/// <summary>
		///  True if the session keeps track of a session key itself to 
		///  map calls on the same instance to the same server-side object.
		/// </summary>
		bool RequiresSessionKey { get; }

	}

}
