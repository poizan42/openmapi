//
// openmapi.org - NMapi C# Mapi API - ISessionManager.cs
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

namespace NMapi.Server.ICalls {

	/// <summary>
	///  
	/// </summary>
	public interface ISessionManager
	{
		/// <summary>
		///  Called when a session is about to unregister.
		/// </summary>
		event EventHandler<ProxySessionEventArgs> SessionUnregistering;

		/// <summary>
		///  Called when a session has been unregistered.
		/// </summary>
		event EventHandler<ProxySessionEventArgs> SessionUnregistered;

		/// <summary>
		///  Called when a session is about to register.
		/// </summary>
		event EventHandler<ProxySessionEventArgs> SessionRegistering;

		/// <summary>
		///  Called when a session has been registered.
		/// </summary>
		event EventHandler<ProxySessionEventArgs> SessionRegistered;

		/// <summary>
		///
		/// </summary>
		void RegisterSession (IProxySession session);

		/// <summary>
		///  Unregisters a session.
		/// </summary>
		void UnregisterSession (IProxySession session);

		/// <summary>
		///  Registers a session.
		/// </summary>
		void TryUnregisterSession (IProxySession session);

		/// <summary>
		///  Gets a read-only list of the current proxy Sessions.
		/// </summary>
		IProxySession[] CurrentSessions { get; }

	}
}

