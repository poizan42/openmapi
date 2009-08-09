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
			private EventConnection backendConnection;
			private EventConnection clientConnection;
			private SBinary targetEntryID;
			private NotificationEventType targetMask;
			
			public SBinary TargetEntryID {
				get { return targetEntryID; }
				set { targetEntryID = value; }
			}
			
			public NotificationEventType TargetEventMask {
				get { return targetMask; }
				set { targetMask = value; }
			}
			
			public EventConnection ClientConnection {
				get { return clientConnection; }
				set { clientConnection = value; }
			}
			
			public EventConnection BackendConnection {
				get { return backendConnection; }
				set { backendConnection = value; }
			}

			public VirtualAdviseSink (OncEventDispatcher parent)
			{
				this.parent = parent;
			}
			
			public bool MatchMask (NotificationEventType eventType)
			{
				return ((targetMask & eventType) != 0);
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
						realEvent.ulConn = ClientConnection.Connection;
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

		public EventConnection Register (IAdvisor targetAdvisor, byte[] entryID, 
			NotificationEventType eventMask, EventConnection txcOutlookHack)
		{
			var sink = new VirtualAdviseSink (this);
			EventConnection backendConnection = targetAdvisor.Advise (entryID, eventMask, sink);
			sink.BackendConnection = backendConnection;
			sink.ClientConnection = txcOutlookHack;
			sink.TargetEntryID = new SBinary (entryID);
			sink.TargetEventMask = eventMask;
			sinks [txcOutlookHack.Connection] = sink;
			Console.WriteLine ("registered SINK '" + txcOutlookHack + "' on server!");
			if (entryID != null)
				Console.WriteLine ("--> " + new SBinary (entryID).ToHexString () + "");
			return txcOutlookHack;
		}

		public void Unregister (IAdvisor targetAdvisor, EventConnection txcOutlookHackConnection)
		{
			EventConnection backendConnection = sinks [txcOutlookHackConnection.Connection].BackendConnection;
			targetAdvisor.Unadvise (backendConnection);
			sinks.Remove (txcOutlookHackConnection.Connection);
			Trace.WriteLine ("unregistered '" + txcOutlookHackConnection + "' sink.");
		}
		
		// POC
		// TODO: check eventmask...
		public void PushEvents (byte[] entryID, Notification[] notifications)
		{
			
			Console.WriteLine (" --- ATTEMPTING TO PUSH EVENTS!!! --- ");
			//lock (callBackMonitor) {
			/*
				SBinary binEntryId = new SBinary (entryID);
				foreach (var currentPair in sinks) {
					var currentSink = currentPair.Value;

					if (currentSink.TargetEntryID.Equals (binEntryId) 
						&& currentSink.MatchMask (notifications [0].EventType)) { // HACK!
						Console.WriteLine (" ---> FOUND SINK ");
			//			currentSink.OnNotify (notifications);
					}
				}
			//}
			*/
		}
		
	}

}
