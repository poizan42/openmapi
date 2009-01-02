//
// openmapi.org - NMapi C# Mapi API - SessionManager.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
 
using System.ServiceModel;
using System.Linq;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

using NMapi.Server.ICalls;

namespace NMapi.Server {

	/// <summary>
	///  Keeps track of all active incoming connections.
	/// </summary>
	public sealed class SessionManager : MarshalByRefObject, ISessionManager
	{
		private object regLock = new object ();
		private Dictionary<string, IProxySession> sessions;
		private Random random;

		public event EventHandler<ProxySessionEventArgs> SessionUnregistering;
		public event EventHandler<ProxySessionEventArgs> SessionUnregistered;
		public event EventHandler<ProxySessionEventArgs> SessionRegistering;
		public event EventHandler<ProxySessionEventArgs> SessionRegistered;

		public SessionManager ()
		{
			this.sessions = new Dictionary<string, IProxySession> ();
			this.random = new Random ();
		}

		public void RegisterSession (IProxySession session)
		{
			if (session.Id == null)
				session.Id = String.Empty + random.Next ();
			var eventArgs = new ProxySessionEventArgs (session);

			if (SessionRegistering != null)
				SessionRegistering (this, eventArgs);
			lock (regLock) {
				if (sessions.ContainsKey (session.Id))
					throw new Exception ("Session with same ID already registered!");
				sessions [session.Id] = session;
			}

			if (SessionRegistered != null)
				SessionRegistered (this, eventArgs);
		}

		private void UnregisterSession (IProxySession session, bool failSilent)
		{
			bool contains = false;
			lock (regLock) {
				contains = sessions.ContainsKey (session.Id);
			}
			if (!contains) {
				if (!failSilent)
					throw new Exception ("ProxySession " + session.Id + 
						" has not been registered.");
				return;
			}
			var eventArgs = new ProxySessionEventArgs (session);
			if (SessionUnregistering != null)
				SessionUnregistering (this, eventArgs);
			lock (regLock) {
				sessions.Remove (session.Id);
			}
			if (SessionUnregistered != null)
				SessionUnregistered (this, eventArgs);
		}

		public void UnregisterSession (IProxySession session)
		{
			UnregisterSession (session, false);
		}

		public void TryUnregisterSession (IProxySession session)
		{
			UnregisterSession (session, true);
		}

		public IProxySession[] CurrentSessions {
			get {
				lock (regLock) {
					return sessions.Values.ToArray ();
				}
			}
		}


	}

}
