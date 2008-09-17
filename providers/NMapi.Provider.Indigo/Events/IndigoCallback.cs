//
// openmapi.org - NMapi C# Mapi API - IndigoCallback.cs
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

using NMapi.Flags;
using NMapi.Events;
using NMapi.Provider.Indigo.Properties.Special;
using NMapi.Provider.Indigo.Table;

// TODO: SBinary as key is weird!
// TODO: Threading

namespace NMapi.Provider.Indigo {

	[CallbackBehavior (ConcurrencyMode = ConcurrencyMode.Reentrant, 
		UseSynchronizationContext = false)]
	public class IndigoCallback : IMapiIndigoCallback
	{
		private VirtualEvents virtualEvents;

		public IndigoCallback (VirtualEvents virtualEvents)
		{
			this.virtualEvents = virtualEvents;
		}

		public void OnNotify (int connection, Notification [] notifications)
		{

			Console.WriteLine ("Received event! (Thread: " + Thread.CurrentThread.GetHashCode () + ").");

			List<IMapiAdviseSink> sinks = virtualEvents.GetByRemoteConnection (connection);

			foreach (IMapiAdviseSink sink in sinks) {
				// TODO: clone notifications ?

				sink.OnNotify (notifications);
			}

		}
	}

	public sealed class VirtualEvents 
	{
		private IndigoMapiSession session;
		private Random random;

		private Dictionary<int, IMapiAdviseSink> virtualConnections = new Dictionary<int, IMapiAdviseSink> ();
		private Dictionary<SBinary, List<int>> filterToVirtual = new Dictionary<SBinary, List<int>> ();
		private Dictionary<int, SBinary> virtualToFilter = new Dictionary<int, SBinary> ();
		private Dictionary<SBinary, int> filterToRemote = new Dictionary<SBinary, int> ();
		private Dictionary<int, SBinary> remoteToFilter = new Dictionary<int, SBinary> ();

		public VirtualEvents (IndigoMapiSession session)
		{
			this.session = session;
			this.random = new Random ();
		}

		private int GetNewConnection ()
		{
			int connection = 0;
			do
				connection = random.Next ();
			while (virtualConnections.ContainsKey (connection));
			return connection;			
		}

		private void AddVirtualConnection (int connection, SBinary filter, IMapiAdviseSink sink)
		{
			virtualConnections [connection] = sink;
			virtualToFilter [connection] = filter;
			if (!filterToVirtual.ContainsKey (filter))
				filterToVirtual [filter] = new List<int> ();
			filterToVirtual [filter].Add (connection);
		}

		private void RemoveVirtualConnection (int connection)
		{
			virtualToFilter.Remove (connection);
			virtualConnections.Remove (connection);
		}

		private void CreateRemoteConnection (object sender, IndigoMapiObjRef obj, 
			byte[] entryID, NotificationEventType eventMask, SBinary filter)
		{
			Console.WriteLine ("Register Event! (Thread: " + Thread.CurrentThread.GetHashCode () + ").");

			int remoteConnection = 0;
			try {
				// Bad.

				if (sender is IndigoMapiTable)
					remoteConnection = session.Proxy.IMapiTable_Advise_2 (obj, entryID, eventMask);
				else if (sender is IndigoMsgStore)
					remoteConnection = session.Proxy.IMsgStore_Advise (obj, entryID, eventMask);
				else
					throw new MapiException ("Indigo only supports " + 
						"Advise () on IMapiTable and IMsgStore.");

			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}

			filterToRemote [filter] = remoteConnection;
			remoteToFilter [remoteConnection] = filter;
		}

		private void RemoveRemoteConnection (SBinary filter)
		{
			int remoteConnection = filterToRemote [filter];

			try {
			// TODO: UNREGISTER remoteConnection
	
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}

			remoteToFilter.Remove (remoteConnection);
			filterToRemote.Remove (filter);

		}


		public List<IMapiAdviseSink> GetByRemoteConnection (int connection)
		{
			var filter = remoteToFilter [connection];
			return GetByFilter (filter);
		}			

		private List<IMapiAdviseSink> GetByFilter (SBinary filter)
		{
			List<IMapiAdviseSink> result = new List <IMapiAdviseSink> ();
			List<int> list = filterToVirtual [filter];
			foreach (int id in list) {
				IMapiAdviseSink sink = virtualConnections [id];
				result.Add (sink);
			}
			return result;
		}

		internal int Advise (object sender, IndigoMapiObjRef obj, byte[] entryID, 
			NotificationEventType eventMask, IMapiAdviseSink sink)
		{
			byte[] mask = BitConverter.GetBytes ( (int) eventMask);

			byte [] filterBytes = new byte [4];
			Array.Copy (mask, 0, filterBytes, 0, 4);

			if (entryID != null) {
				filterBytes = new byte [entryID.Length + 4]; //TODO
				Array.Copy (entryID, 0, filterBytes, 0, entryID.Length);
				Array.Copy (mask, 0, filterBytes, entryID.Length, 4);
			}

			SBinary filter = new SBinary (filterBytes);

			int connection = GetNewConnection ();
			AddVirtualConnection (connection, filter, sink);

			if (!filterToRemote.ContainsKey (filter))
				CreateRemoteConnection (sender, obj, entryID, eventMask, filter);

			return connection;
		}
	

		internal void Unadvise (int connection)
		{
			if (virtualConnections.ContainsKey (connection)) {
				IMapiAdviseSink sink = virtualConnections [connection];				

				SBinary filter = virtualToFilter [connection];
				List<int> similiarConnections = filterToVirtual [filter];

				if (similiarConnections.Count == 1)
					RemoveRemoteConnection (filter);

				similiarConnections.Remove (connection);
				RemoveVirtualConnection (connection);				
			}
		}


	}


}
