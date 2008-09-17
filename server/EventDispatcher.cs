//
// openmapi.org - NMapi C# Mapi API - EventDispatcher.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.ServiceModel;

using System.Threading;

using NMapi;
using NMapi.Events;
using NMapi.Flags;

namespace NMapi.Server {

	public class EventDispatcher
	{
		private Dictionary<int, VirtualAdviseSink> sinks;

		private object callBackMonitor = new object ();

		// TODO: Threading!

		public class VirtualAdviseSink : IMapiAdviseSink
		{
			private EventDispatcher parent;
			private int connection;
			private IMapiIndigoCallback callback;

			public int Connection {
				get { return connection; }
				set { connection = value; }
			}

			public VirtualAdviseSink (EventDispatcher parent, IMapiIndigoCallback callback)
			{
				this.parent = parent;
				this.callback = callback;
			}

			public void OnNotify (Notification [] notifications)
			{

				// TODO: Dispatch on other thread!

				Console.WriteLine ("dispatching! (Thread: " + Thread.CurrentThread.GetHashCode () + ").");
				try {
					lock (parent.callBackMonitor) {
						callback.OnNotify (connection, notifications);
					}
				} catch (Exception e) {
					Console.WriteLine (e.Message);
				}
				Console.WriteLine ("dispatched! (Thread: " + Thread.CurrentThread.GetHashCode () + ").");
			}
		}

		public EventDispatcher ()
		{
			this.sinks = new Dictionary<int, VirtualAdviseSink> ();
		}

		public int Register (IAdvisor targetAdvisor, byte[] entryID, NotificationEventType eventMask)
		{
			IMapiIndigoCallback callback = OperationContext.Current.
					GetCallbackChannel<IMapiIndigoCallback> ();
			var sink = new VirtualAdviseSink (this, callback);
			int connection = targetAdvisor.Advise (entryID, eventMask, sink);
			sink.Connection = connection;
			sinks [connection] = sink;
			Console.WriteLine ("registered SINK on server!");
			return connection;
		}

		public void Unregister (IAdvisor targetAdvisor, int connection)
		{
			targetAdvisor.Unadvise (connection);
			sinks.Remove (connection);
			Console.WriteLine ("unregistered sink.");
		}
	}

}
