//
// openmapi.org - NMapi C# Mapi API - IInternalCalls.cs
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
using NMapi.Admin;
using NMapi.Tools.Shell;

namespace NMapi.Server.ICalls {

	/// <summary>
	///  This interface is used by the ASP.NET web interface to talk 
	///  back to the proxy session (that started it).
	/// </summary>
	public interface IInternalCalls
	{
		/// <summary>
		///  Returns true if the correct password is supplied.
		/// </summary>
		bool AuthenticateAdmin (string password);

		/// <summary>
		///  Call when the user logged in successfully.
		/// </summary>
		void RegisterLogin ();

		/// <summary>
		///  Returns a reference to the session manager.
		/// </summary>
		ISessionManager SessionManager { get; }

		/// <summary>
		///  
		/// </summary>
		IMapiShell CreateNewShell ();

		/// <summary>
		///  
		/// </summary>
		string FlushShellOutputBuffer (IMapiShell shell);

		/// <summary>
		///  
		/// </summary>
		IMapiAdmin GetMapiAdmin (int backendId);

		/// <summary>
		///  Information on the executing proxy process.
		/// </summary>
		ProxyInformation ProxyInformation { get; }


		/// <summary>
		///  The Version of the proxy.
		/// </summary>
		string Version { get; }

		/// <summary>
		///
		/// </summary>
		string[] ModuleNames { get; }

		/// <summary>
		///  Restarts the server.
		/// </summary>
		void Restart ();
	}
}

