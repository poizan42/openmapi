//
// openmapi.org - NMapi C# Mapi API - OncEventDispatcher.cs
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
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

using NMapi;
using NMapi.Events;
using NMapi.Flags;

using NMapi.Interop.MapiRPC;

namespace NMapi.Server {

	public class OncEventDispatcher : IEventDispatcher
	{
		private OncProxySession session;
		private Dictionary<int, VirtualAdviseSink> sinks;
		private object callBackMonitor = new object ();

		// TODO: Threading!

		public class VirtualAdviseSink : IMapiAdviseSink
		{
			private OncEventDispatcher parent;
			private int backendConnection;
			private int clientConnection;

			public int ClientConnection {
				get { return clientConnection; }
				set { clientConnection = value; }
			}
			
			public int BackendConnection {
				get { return backendConnection; }
				set { backendConnection = value; }
			}

			public VirtualAdviseSink (OncEventDispatcher parent)
			{
				this.parent = parent;
			}

			public void OnNotify (Notification [] notifications)
			{
				Trace.WriteLine ("dispatching! (Thread: " + 
					Thread.CurrentThread.GetHashCode () + ").");
				try {
					lock (parent.callBackMonitor) {
						Trace.WriteLine ("EVENT for connection (sink) '" + 
							ClientConnection + "' received!");
						ClEvMapi realEvent = new ClEvMapi ();
						realEvent.ulConn = ClientConnection;
						realEvent.notif = notifications;
						parent.session.ReverseEventConnectionServer.PushEvent (realEvent);
					}
				} catch (Exception e) {
					Trace.WriteLine (e.Message);
				}
			}
		}

		public OncEventDispatcher (OncProxySession session)
		{
			this.session = session;
			this.sinks = new Dictionary<int, VirtualAdviseSink> ();
		}

		public int Register (IAdvisor targetAdvisor, byte[] entryID, 
			NotificationEventType eventMask, int txcOutlookHack)
		{
			var sink = new VirtualAdviseSink (this);
			int backendConnection = targetAdvisor.Advise (entryID, eventMask, sink);
			sink.BackendConnection = backendConnection;
			sink.ClientConnection = txcOutlookHack;
			sinks [txcOutlookHack] = sink;
			Trace.WriteLine ("registered SINK '" + txcOutlookHack + "' on server!");
			return txcOutlookHack;
		}

		public void Unregister (IAdvisor targetAdvisor, int txcOutlookHackConnection)
		{
			int backendConnection = sinks [txcOutlookHackConnection].BackendConnection;
			targetAdvisor.Unadvise (backendConnection);
			sinks.Remove (txcOutlookHackConnection);
			Trace.WriteLine ("unregistered '" + txcOutlookHackConnection + "' sink.");
		}
	}

}
