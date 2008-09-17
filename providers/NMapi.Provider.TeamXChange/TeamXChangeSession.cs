//
// openmapi.org - NMapi C# Mapi API - TeamXChangeSession.cs
//
// Copyright 2008 VipCom AG
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
	using System.Net.Sockets;
	using RemoteTea.OncRpc;
	using RemoteTea.OncRpc.Server;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;
	using NMapi.Flags;
	using NMapi.Events;
	using NMapi.Properties.Special;
	using NMapi.Table;

	/// <summary>
	///  Internal representation of the Session.
	/// </summary>
	public class TeamXChangeSession :  OncRpcDispatchable, IDisposable
	{
		private string host;
		private int port;
		private MAPIRPCClient client;
		private int evuid;
		private TcpClient eventSock;
		private OncRpcTcpConnectionServerTransport eventServ; 
		private Dictionary<int, IEventSubscription> eventSubMap;
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		public TeamXChangeSession () : this (Common.Host)
		{
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		public TeamXChangeSession (string server) : this (Common.GetHostName (server), 
			Common.GetPort (server))
		{
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		private TeamXChangeSession (string host, int port) 
		{
			this.host  = host;
			this.port = port;
			try {
				IPHostEntry hostEntry = Dns.GetHostEntry (host); // TODO: Ensure that call doesn't wait foreever!
				IPAddress ip = null;
				if (hostEntry.AddressList.Length > 0)
					ip = hostEntry.AddressList [0];
				client = new MAPIRPCClient (ip, this.port, 
					OncRpcProtocols.ONCRPC_TCP);
				client.GetClient().SetTimeout (5 * 60 * 1000);
				int version = GetVersion (0);
				if (version == 5)
					version = 6;
				if (version != RpcVersionInf.MAPIRPCVERSION) {
					string msg  = "server version " +  version 
						+ " does not match  client version " 
						+ RpcVersionInf.MAPIRPCVERSION;
					throw new MapiException (msg, Error.Version);
				}
				evuid = 0;
				eventSubMap = new Dictionary<int, IEventSubscription> ();
				eventSock = new TcpClient (host, this.port+1);

				eventServ = new OncRpcTcpConnectionServerTransport(this,
			                                                eventSock,
			                                                1,
									1,
			                                                8192,
			                                                null,
			                                                10);
				eventServ.Listen ();
				byte[] data = GetNativeID (0);
				eventSock.GetStream ().Write (data, 0, data.Length);
			}
			catch (SocketException e) {
				throw new MapiException ("unknown host="+host, e);
			}
			catch (IOException e) {
				throw new MapiException ("host="+host, e);
			
			}
			catch (OncRpcException e) {
				throw new MapiException ("host="+host, e);
			}
		}

		/// <summary>
		///
		/// </summary>
		public void Dispose (bool disposing)
		{
			if (eventSock != null) {
				try {
					eventSock.Close ();
				}
				catch (IOException) {} // Do nothing
			}
			if (client != null) {
				try { 
					client.Close ();
				}
				catch (OncRpcException) {} // Do nothing
			}
			eventSubMap  = null;
			eventSock    = null;
			client    = null;			
		}
	
		/// <summary>
		///
		/// </summary>
		public void Dispose () 
		{
			Dispose (true);
		}
	
		~TeamXChangeSession () 
		{
			Dispose (false);
		}

		/// <summary>
		///
		/// </summary>
		public void Close () 
		{
			Dispose (true);
		}

		/// <summary>
		///   Returns true if the session has not been closed, yet.
		/// </summary>
		public bool IsOpen {
			get {
				return (client != null);
			}
		}

		/// <exception cref="T:NMapi.MapiException">Throws MapiException</exception>
		/// <exception cref="T:OncRpcException">Throws OncRpcException</exception>
		public void DispatchOncRpcCall (OncRpcCallInformation call,
			int program, int version, int procedure)
		{
			switch (procedure) {
				case 0:
					XdrVoid v = new XdrVoid();
					call.RetrieveCall (v);
					call.Reply (v);
				break;
				case 1:
					ClientEvent e = new ClientEvent ();
					call.RetrieveCall (e);
					switch (e.type)
					{
						case ClientEvType.CLEV_MAPI:
							lock (eventSubMap)
							{
								if (eventSubMap.ContainsKey (e.mapi.ulConn)) {
									var sub = eventSubMap [e.mapi.ulConn];
									if (sub != null)
										sub.OnNotify (e.mapi.notif);
								}
							}
						break;
						case ClientEvType.CLEV_PROGRESS:
							// TODO (This is also marked as "todo" in jumapi.
						break;
					}
				break;
				default:
					call.FailProcedureUnavailable ();
				break;
			}
		}

		/// <summary>
		///
		/// </summary>
		public MAPIRPCClient clnt {
			get { return client; }
		}

		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public byte [] GetNativeID (int flags)
		{
			Session_GetNativeID_arg arg = new Session_GetNativeID_arg ();
			Session_GetNativeID_res res;
		
			try {
				res = client.Session_GetNativeID_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}				
			if (Error.CallHasFailed (res.hr)) {
				throw new MapiException(res.hr);
			}
			return res.id.lpb;
		}
	
		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public void Logon2 (string user, string password, 
			int sessionFlags, int codePage, int localeID)
		{
			Session_Logon2_arg arg = new Session_Logon2_arg();
			Session_Logon2_res res;
		
			arg.pszHost = new LPStr(host);
			arg.pwszUser = new LPWStr(user);
			arg.pwszPassword = new LPWStr(password);
			arg.ulSessionFlags = sessionFlags;
			arg.ulCodePage = codePage;
			arg.ulLocaleID = localeID;

			try {
				res = client.Session_Logon2_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
		}

		/// <summary>
		///  Opens a Message-Store.
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public TeamXChangeMsgStore OpenStore (Mdb flags, string user, bool isPublic) 
		{
			Session_OpenStore_arg arg = new Session_OpenStore_arg ();
			Session_OpenStore_res res;

			arg.pwszStoreUser = new LPWStr (user);
			arg.ulFlags = (int) flags;
			arg.bIsPublic = isPublic ? 1 : 0;
			try {
				res = client.Session_OpenStore_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return new TeamXChangeMsgStore (res.obj.Value.Value, this);
		}

		/// <summary>
		///  Register an AdviseSink for Notifications.
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public int Advise (IMapiAdviseSink sink, 
			byte [] storeEID, byte [] objEID, NotificationEventType mask)
		{
			lock (eventSubMap) {
				evuid++;
				long obj = SubscribeEvent (evuid, storeEID, objEID, mask);
				IEventSubscription sub = new TeamXChangeEventSubscription (obj, this, sink);
				eventSubMap [evuid] = sub;
			}
			return evuid;
		}
	
		/// <summary>
		///  Unregister from notifications, using the connection id 
		///  returned by Advise ().
		/// </summary>
		public void Unadvise (int connection)
		{
			lock (eventSubMap) {
				if (eventSubMap.ContainsKey (connection)) {
					IEventSubscription sub = eventSubMap [connection];
					if (sub != null)
						sub.Close ();
				}
				eventSubMap.Remove (connection);
			}
		}
	
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		private long SubscribeEvent(int connection,
			byte [] storeEID, byte [] objEID, NotificationEventType mask)
		{
			Session_SubscribeEvent_arg arg = new Session_SubscribeEvent_arg();
			Session_SubscribeEvent_res res;

			arg.ulConnection = connection;
			arg.eidstore = new SBinary (storeEID);
			arg.eidobj = new SBinary (objEID);
			arg.ulMask = (int) mask;
			try {
				res = client.Session_SubscribeEvent_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.obj.Value.Value;
		}

		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>	
		public string GetConfig (string category, string id, int flags)
		{
			Session_GetConfig_arg arg = new Session_GetConfig_arg();
			Session_GetConfig_res res;
		
			arg.pszCategory = new LPStr (category);
			arg.pszID = new LPStr (id);
			arg.ulFlags = flags;
			try {
				res = client.Session_GetConfig_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			if ((flags & Mapi.Unicode) != 0)
				return res.pValue.value.Value.Unicode;
			else
				return res.pValue.value.Value.String;
		}
	
		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public string GetConfigNull (string category, string id, int flags)
		{
			try {
				return GetConfig (category, id, flags);
			}
			catch (MapiException e) {
				if (e.HResult == Error.NotFound)
					return null;
				throw e;
			}
		}
	
		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public int GetVersion (int flags)
		{
			Session_GetVersion_arg arg = new Session_GetVersion_arg();
			Session_GetVersion_res res;
		
			arg.ulFlags = flags;
			try {
				res = client.Session_GetVersion_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed  (res.hr))
				throw new MapiException(res.hr);
			return res.ulVersion;
		}
	
		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public int GetObjType (long obj)
		{
			Base_GetType_arg arg = new Base_GetType_arg ();
			Base_GetType_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			try {
				res = client.Base_GetType_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			return res.type;
		}

		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public IBase CreateObject (IBase parent, long obj, NMapiGuid iid, int propTag)
		{
			switch (GetObjType (obj)) {
				case Mapi.Folder:
					if (((TeamXChangeBase) parent).Do_GetType () == Mapi.Store)
						return new TeamXChangeMapiFolder (obj, (TeamXChangeMsgStore) parent);
					return new TeamXChangeMapiFolder (obj, ((TeamXChangeMapiFolder) parent).Store);
				case Mapi.Message:
					return new TeamXChangeMessage (obj, (TeamXChangeBase) parent);
				case Mapi.Attach:
					return new TeamXChangeAttach (obj, (TeamXChangeMessage)parent);
				case Mapi.SimpleStream:
					return new TeamXChangeStream (obj, (TeamXChangeBase) parent, propTag);
				case Mapi.Table:
					return new TeamXChangeMapiTable (obj, (TeamXChangeMapiFolder) parent);
				case Mapi.TableReader:
					return new TeamXChangeMapiTableReader (obj, (TeamXChangeBase) parent);
				default:
					Console.Write ("unknown type ");
					Console.Write (GetObjType (obj));
					Console.WriteLine ();
					throw new MapiException (Error.BadValue);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public IBase CreateObject (IBase parent, long obj, NMapiGuid iid)
		{
			return CreateObject (parent, obj, iid, (int) PropertyType.Null);
		}

	}


}
