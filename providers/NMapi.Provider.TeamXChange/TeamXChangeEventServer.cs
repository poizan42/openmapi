//
// openmapi.org - NMapi C# Mapi API - TeamXChangeEventServer.cs
//
// Copyright 2008 VipCom AG, Topalis AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
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

namespace NMapi {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.Net.Security;
	using System.Security.Authentication;
	using System.Security.Cryptography.X509Certificates;
	using System.Security.Cryptography;
	using System.Net.Sockets;
	using CompactTeaSharp;
	using CompactTeaSharp.Server;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;
	using NMapi.Flags;
	using NMapi.Events;
	using NMapi.Properties.Special;
	using NMapi.Table;

	/// <summary>
	///  Internal representation of the Session.
	/// </summary>
	public class TeamXChangeEventServer :  IOncRpcDispatchable, IDisposable
	{
		private MAPIRPCClient client;
		private string host;
		private int port;
		private int evuid;
		private int progressId;
		private TcpClient eventSock;
		private OncRpcTcpConnectionServerTransport eventServ; 
		private Dictionary<int, TeamXChangeEventSubscription> eventSubMap;
		private Dictionary<int, IMapiProgress> progressMap;

		/// <exception cref="MapiException">Throws MapiException</exception>
		public TeamXChangeEventServer (MAPIRPCClient client, string host, int port, byte[] objb) 
		{
			this.client = client;
			this.host = host;
			this.port = port;
			
			try {
				eventSubMap = new Dictionary<int, TeamXChangeEventSubscription> ();
				progressMap = new Dictionary<int, IMapiProgress> ();
				eventSock = new TcpClient (this.host, this.port);

                Stream wrappedStream = OncNetworkUtility.GetSslClientStream (eventSock.GetStream(), host);
				eventServ = new OncRpcTcpConnectionServerTransport (
					this, eventSock, 8192, null, 10, wrappedStream);
				eventServ.Listen ();

				wrappedStream.Write (objb, 0, objb.Length);
			} catch (SocketException e) {
				throw MapiException.Make ("unknown host=" + host, e);
			} catch (IOException e) {
				throw MapiException.Make ("host="+host, e);
			} catch (OncRpcException e) {
				throw MapiException.Make ("host="+host, e);
			}
			
		}

		/// <summary>
		///
		/// </summary>
		public void Dispose (bool disposing)
		{
			try {
				if (eventSock != null) {
					try {
						eventSock.Close ();
					}
					catch (IOException) {} // Do nothing
				}
				eventSubMap = null;
				progressMap = null;
				eventSock = null;
			} catch (Exception) {
				// do nothing
			}
		}
		
		public void Dispose ()
		{
			Dispose (true);
		}

		/// <exception cref="T:NMapi.MapiException">Throws MapiException</exception>
		/// <exception cref="T:OncRpcException">Throws OncRpcException</exception>
		public void DispatchOncRpcCall (OncRpcCallInformation call,
			int program, int version, int procedure)
		{
			switch (procedure) {
				case 0: HandlePingCall (call); break;
				case 1: HandleEventCall (call); break;
				default: call.FailProcedureUnavailable (); break;
			}
		}
		
		private void HandlePingCall (OncRpcCallInformation call)
		{
			XdrVoid v = new XdrVoid ();
			call.RetrieveCall (v);
			call.Reply (v);
		}
		
		private void HandleEventCall (OncRpcCallInformation call)
		{
			ClientEvent e = new ClientEvent ();
			call.RetrieveCall (e);
			switch (e.type) {
				case ClientEvType.CLEV_MAPI:  HandleRealEvent (e); break;
				case ClientEvType.CLEV_PROGRESS: HandleProgressEvent (e); break;
				default: throw new MapiBadValueException ("Unknown event type received."); break;
			}			
		}
		
		private void HandleRealEvent (ClientEvent e)
		{
			lock (eventSubMap) {
				if (e.mapi == null)
					return;
				if (eventSubMap.ContainsKey (e.mapi.ulConn)) {
					var sub = eventSubMap [e.mapi.ulConn];
					if (sub != null)
						sub.OnNotify (e.mapi.notif); // TODO: try ?!
				}
			}
		}
		
		private void HandleProgressEvent (ClientEvent e)
		{
			if (e.progress != null) {
				IMapiProgress progress = null;
				lock (progressMap) {
					progress = GetProgressFromMap (e.progress.ulID);
				}
				if (progress != null) {
					int ul1 = e.progress.ul1;
					int ul2 = e.progress.ul2;
					int ul3 = e.progress.ul3;

					switch (e.progress.type) {
						case ProgressType.PROGRESS_SETLIMITS: progress.SetLimits (ul1, ul2, ul3); break;
						case ProgressType.PROGRESS_UPDATE: progress.Progress (ul1, ul2, ul3); break;
						default: throw new MapiBadValueException ("Unknown progress type received."); break;
					}
				}
			}
		}
		
		private IMapiProgress GetProgressFromMap (int progressId)
		{
			lock (progressMap) {
				if (progressMap.ContainsKey (progressId))
					return progressMap [progressId];
			}
			return null;
		}
		
		public int RegisterProgressId (IMapiProgress progress)
		{
			lock (progressMap) {
				progressId++;
				progressMap [progressId] = progress;				
				return progressId;
			}
		}
		
		public void UnregisterProgressId (int id)
		{
			lock (progressMap) {
				if (progressMap.ContainsKey (progressId))
					progressMap.Remove (progressId);
			}
		}
		
		

		public EventConnection Advise (TeamXChangeMsgStore store, byte [] eid, 
			NotificationEventType mask, IMapiAdviseSink sink)
		{		
			lock (eventSubMap) {
				evuid++;
				var res = TeamXChangeBase.MakeCall<MsgStore_Advise_res, MsgStore_Advise_arg> (
						client.MsgStore_Advise_1, 
						new MsgStore_Advise_arg {
							obj = new HObject (new LongLong (store.obj)),
							eid = new SBinary (eid),
							ulEventMask = (int) mask,
							ulClientID = evuid,
						});
				eventSubMap [evuid] = new TeamXChangeEventSubscription (store, sink, res.obj);
			}
			return new EventConnection (evuid);
		}

		public EventConnection Advise (TeamXChangeMapiTable table, 
			NotificationEventType mask, IMapiAdviseSink sink)
		{		
			lock (eventSubMap) {
				evuid++;
				var res = TeamXChangeBase.MakeCall<MAPITable_Advise_res, MAPITable_Advise_arg> (
						client.MAPITable_Advise_1, 
						new MAPITable_Advise_arg {
							obj = new HObject (new LongLong (table.obj)),
							ulEventMask = (int) mask,
							ulClientID = evuid
						});
				eventSubMap [evuid] = new TeamXChangeEventSubscription (table, sink, res.obj);
			}
			return new EventConnection (evuid);
		}

		public void Unadvise (EventConnection connection)
		{
			lock (eventSubMap) {
				TeamXChangeEventSubscription sub = eventSubMap [connection.Connection];
				try {
					if (sub != null)
						sub.Unadvise ();
				} catch (MapiException) {
					// do nothing
				} finally {
					eventSubMap.Remove (connection.Connection);
				}
			}
		}
		
	}


}
