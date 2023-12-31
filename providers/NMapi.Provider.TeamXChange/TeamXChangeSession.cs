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
	using CompactTeaSharp;
	using CompactTeaSharp.Server;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;
	using NMapi.Flags;
	using NMapi.Events;
	using NMapi.Properties;
	using NMapi.Properties.Special;
	using NMapi.Table;
	using NMapi.Synchronization;

	/// <summary>
	///  Internal representation of the Session.
	/// </summary>
	public class TeamXChangeSession :  IDisposable
	{
		private string host;
		private int port;
		private HObject obj;
		private MAPIRPCClient client;
		private TeamXChangeEventServer eventServer;
		
		/// <summary>
		///
		/// </summary>
		public MAPIRPCClient clnt {
			get { return client; }
		}
		
		/// <summary>
		///
		/// </summary>
		public TeamXChangeEventServer EventServer {
			get { return eventServer; }
		}
		
		/// <summary>
		///   Returns true if the session has not been closed, yet.
		/// </summary>
		public bool IsOpen {
			get { return (client != null); }
		}
		
		
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
		public TeamXChangeSession (string host, int port) 
		{
			this.host = host;
			this.port = port;
			
			try {				
				IPAddress[] ipList = Dns.GetHostAddresses (host); // TODO: Ensure that call doesn't wait forever!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				if (ipList.Length < 1)
					throw new Exception ("Can't determine ip adress of local host.");
				
				client = new MAPIRPCClient (ipList [0], this.port, OncRpcProtocols.SslTcp);
				client.Client.SetTimeout (5 * 60 * 1000);
				
				CheckVersion ();
				obj = MakeInitSessionCall (String.Empty);
				byte [] objb = DoTheByteShiftingThing (obj.value.Value);
				this.eventServer = new TeamXChangeEventServer (client, host, port+1, objb);
			}
			catch (SocketException e) {
				throw MapiException.Make ("unknown host="+host, e);
			}
			catch (IOException e) {
				throw MapiException.Make ("host="+host, e);
			
			}
			catch (OncRpcException e) {
				throw MapiException.Make ("host="+host, e);
			}
		}
		
		private HObject MakeInitSessionCall (string initStr)
		{
			var res = TeamXChangeBase.MakeCall<Session_InitSession_res, Session_InitSession_arg> (
				client.Session_InitSession_1, 
				new Session_InitSession_arg { pwszArgs = new UnicodeAdapter (initStr) });
			return res.obj;		
		}
		
		private void CheckVersion ()
		{			
			int version = GetVersion (0);
			if (version != (int) RpcVersionInf.MAPIRPCVERSION) {
				string msg  = "server version " +  version 
					+ " does not match  client version " 
					+ RpcVersionInf.MAPIRPCVERSION;
				throw new MapiVersionException (msg);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public int GetVersion (int flags)
		{
			var arg = new Session_GetVersion_arg ();
			arg.ulFlags = flags;
			var res = TeamXChangeBase.MakeCall<Session_GetVersion_res, Session_GetVersion_arg> (
					client.Session_GetVersion_1, arg);
			return res.ulVersion;
		}
		
		private byte[] DoTheByteShiftingThing (long objv)
		{
			byte [] objb = new byte [8];
			objb [0] = (byte) ((objv >> 56) & 0xff);
			objb [1] = (byte) ((objv >> 48) & 0xff);
			objb [2] = (byte) ((objv >> 40) & 0xff);
			objb [3] = (byte) ((objv >> 32) & 0xff);
			objb [4] = (byte) ((objv >> 24) & 0xff);
			objb [5] = (byte) ((objv >> 16) & 0xff);
			objb [6] = (byte) ((objv >>  8) & 0xff);
			objb [7] = (byte) ((objv) & 0xff);
			return objb;
		}
		

		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public IBase CreateObject (IBase parent, long obj, int objType, NMapiGuid interFace, int propTag)
		{
			switch (objType) {
				case MapiObjectType.Folder:
					if (parent is IMsgStore)
						return new TeamXChangeMapiFolder (obj, (TeamXChangeMsgStore) parent);
					return new TeamXChangeMapiFolder (obj, ((TeamXChangeMapiFolder) parent).Store);
				case MapiObjectType.Message:
					return new TeamXChangeMessage (obj, (TeamXChangeBase) parent);
				case MapiObjectType.Attach:
					return new TeamXChangeAttach (obj, (TeamXChangeMessage) parent);
				case MapiObjectType.SimpleStream:
					return new TeamXChangeStream (obj, (TeamXChangeBase) parent, propTag);
				case MapiObjectType.Table:
					return new TeamXChangeMapiTable (obj, (TeamXChangeMapiFolder) parent);
				case MapiObjectType.TableReader:
					return new TeamXChangeMapiTableReader (obj, (TeamXChangeBase) parent);
//				case MapiObjectType.ModifyTable:																		// TODO!!!!!!
//					return new IModifyTable (obj, (IMapiFolder) parent);
				case MapiObjectType.MsgSync:
					return new TeamXChangeMessageSynchronizer (obj, (TeamXChangeMapiFolder) parent);
				case MapiObjectType.FldSync:
					return new TeamXChangeFolderSynchronizer (obj, (TeamXChangeMsgStore) parent);
				case MapiObjectType.MsgImp:
					return new TeamXChangeMessageImporter2 (obj, (TeamXChangeMessageSynchronizer) parent);
				case MapiObjectType.FldImp:
					return new TeamXChangeFolderImporter2 (obj, (TeamXChangeFolderSynchronizer) parent);
				default:
					throw new MapiBadValueException ("unknown type " + objType);
			}
		}
		
		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public IBase CreateObject (IBase parent, long obj, int objType, NMapiGuid interFace) 
		{
			return CreateObject (parent, obj, objType, interFace, Property.Null);
		}

		public void RegClientCert (string password)
		{
			var prms = new Session_RegClientCert_arg ();
			// TXC TODO: We need an obj field here for the OpenMapi.org server.
			prms.pszPassword = new StringAdapter (password);
			prms.usage = 0;
			
			var res = TeamXChangeBase.MakeCall<Session_RegClientCert_res, Session_RegClientCert_arg> (
				client.Session_RegClientCert_1, prms);
		}

		public void RegisterSyncClientID (byte [] id)
		{
			var prms = new Session_RegisterSyncClientID_arg ();
			prms.obj = new HObject (0);
			prms.id = new SBinary (id);
			
			var res = TeamXChangeBase.MakeCall<Session_RegisterSyncClientID_res, Session_RegisterSyncClientID_arg> (
				client.Session_RegisterSyncClientID_1, prms);
		}
		
		/// <summary>
		///
		/// </summary>
		public void Dispose (bool disposing)
		{
			try {
				if (client != null) {
					try { 
						client.Close ();
					}
					catch (OncRpcException) {} // Do nothing
				}
				client = null;
			} catch (Exception) {
				// do nothing
			}
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
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public void Logon2 (string user, string password, 
			int sessionFlags, int codePage, int localeID)
		{
			Session_Logon2_arg arg = new Session_Logon2_arg();
			Session_Logon2_res res;
			
			arg.obj = obj;
			arg.pszHost = new StringAdapter (host);
			arg.pwszUser = new UnicodeAdapter (user);
			arg.pwszPassword = new UnicodeAdapter (password);
			arg.ulSessionFlags = sessionFlags;
			arg.ulCodePage = codePage;
			arg.ulLocaleID = localeID;

			try {
				res = client.Session_Logon2_1(arg);
			}
			catch (IOException e) {
				throw MapiException.Make(e);
			}
			catch (OncRpcException e) {
				throw MapiException.Make(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw MapiException.Make (res.hr);
		}

		/// <summary>
		///  Opens a Message-Store.
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>
		public TeamXChangeMsgStore OpenStore (OpenStoreFlags flags, string user, bool isPublic) 
		{
			Session_OpenStore_arg arg = new Session_OpenStore_arg ();
			Session_OpenStore_res res;

			arg.obj = obj;
			arg.pwszStoreUser = new UnicodeAdapter (user);
			arg.ulFlags = (int) flags;
			arg.bIsPublic = isPublic ? 1 : 0;
			try {
				res = client.Session_OpenStore_1 (arg);
			}
			catch (IOException e) {
				throw MapiException.Make(e);
			}
			catch (OncRpcException e) {
				throw MapiException.Make(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw MapiException.Make (res.hr);
			return new TeamXChangeMsgStore (res.obj.Value.Value, this);
		}

		/// <summary>
		///
		/// </summary>
		/// <exception cref="T:NMapi.MapiException">MapiException</exception>	
		public string GetConfig (string category, string id, int flags)
		{
			Session_GetConfig_arg arg = new Session_GetConfig_arg();
			Session_GetConfig_res res;

			arg.obj = obj;
			arg.pszCategory = new StringAdapter (category);
			arg.pszID = new StringAdapter (id);
			arg.ulFlags = flags;
			try {
				res = client.Session_GetConfig_1(arg);
			}
			catch (IOException e) {
				throw MapiException.Make(e);
			}
			catch (OncRpcException e) {
				throw MapiException.Make(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw MapiException.Make(res.hr);
			if ((flags & Mapi.Unicode) != 0)
				return ((UnicodeProperty) res.pValue.Value).Value;
			else
				return ((String8Property) res.pValue.Value).Value;
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
				throw;
			}
		}
		
		public Address ResolveEntryID (byte [] eid)
		{
			throw new NotImplementedException ("Not implemented!");
		}

		public Address ResolveSmtpAddress (string smtpaddress, string displayname)
		{
			throw new NotImplementedException ("Not implemented!");
		}
		
		public LPProgressBar CreateProgressBar (IMapiProgress progress)
		{
			LPProgressBar result = new LPProgressBar ();
			if (progress != null) {
				result.Value = new ProgressBar ();
				result.Value.ulID = eventServer.RegisterProgressId (progress);
				result.Value.ulMin = progress.Min;
				result.Value.ulMax = progress.Max;
				result.Value.ulFlags = progress.Flags;
			}
			return result;
		}
		
		public void UnregisterProgressBar (LPProgressBar progressBar)
		{
			if (progressBar.Value != null)
				eventServer.UnregisterProgressId (progressBar.Value.ulID);
		}
		
		
		// debugging ... -  do not check in!
		public void ServerTrace (string message)
		{
			Admin_TraceWrite_arg arg = new Admin_TraceWrite_arg ();
			arg.obj = obj;
			arg.pszMessage = new StringAdapter (message);
			client.Admin_TraceWrite_1 (arg);
		}
		
	}


}
