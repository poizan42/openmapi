//
// openmapi.org - NMapi C# Mapi API - OncRpcService.cs
//
// Copyright 2009 Topalis AG
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
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

using System.Net;
using System.Net.Sockets;
using NMapi.Interop;
using NMapi.Interop.MapiRPC;
using CompactTeaSharp;
using CompactTeaSharp.Server;

namespace NMapi.Server {

	public sealed partial class OncRpcService : BaseOncRpcService
	{
		public OncRpcService (CommonRpcService service, 
			SessionManager sman, IPAddress ip, int port) : base (service, sman, ip, port)
		{
		}
		
		public override Session_InitSession_res Session_InitSession_1 (
			OncRpcCallInformation call, Session_InitSession_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL Session_InitSession");
			string key = GetSessionKey (call.PeerAddress.ToString (), call.PeerPort);
			if (!sessions.ContainsKey (key)) {
				var newSession = new OncProxySession (call.PeerAddress, 
					call.PeerPort, commonRpcService);
				sessions [key] = newSession;
				sessionManager.RegisterSession (newSession);
			}

 			// TODO: incorrect RemoteCall enum value!
			var request = new Request (sessions [key], RemoteCall.IMapiSession_Logon, 0);
			long id = RegisterMapiSession (request, arg1.pwszArgs.Value);
			
			var res = new Session_InitSession_res ();
			res.obj = new HObject (new LongLong (id));
			Trace.WriteLine (" ==> END CALL Session_InitSession");
			return res;
		}
		
		public override Session_GetVersion_res Session_GetVersion_1 (
			OncRpcCallInformation call, Session_GetVersion_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL Session_GetVersion");
			var response = new Session_GetVersion_res ();
			response.ulVersion = (int) RpcVersionInf.MAPIRPCVERSION;
			Trace.WriteLine (" ==> END CALL Session_GetVersion");
			return response;
		}

		public override Session_GetLoginName_res Session_GetLoginName_1 (
			OncRpcCallInformation call, Session_GetLoginName_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL Session_GetLoginName");
			var session = GetProxySessionForConnection (call);
			var response = new Session_GetLoginName_res ();
			response.pwszLoginName = new UnicodeAdapter (session.LoginName);
			Trace.WriteLine (" ==> END CALL Session_GetLoginName");
			return response;
		}

		public override Base_Close_res Base_Close_1 (
			OncRpcCallInformation call, Base_Close_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL Base_Close");
			try {
			
				var session = GetProxySessionForConnection (call);
				object obj = session.ObjectStore.GetObject (arg1.obj.value.Value);
				IDisposable disposableObj = obj as IDisposable;
				if (disposableObj != null) {
					Trace.WriteLine ("RELEASING " + arg1.obj.value.Value);
					disposableObj.Dispose ();
				}
			} catch (Exception e) {
				Console.WriteLine (e);
			}
			var result = new Base_Close_res ();
			result.refCount = 0; // TODO: check - correct?
			Trace.WriteLine (" ==> END CALL Base_RefClose");
			return result;	
		}

		public override MsgStore_GetOrigEID_res MsgStore_GetOrigEID_1 (
			OncRpcCallInformation call, MsgStore_GetOrigEID_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL MsgStore_GetOrigEID");
			
			var session = GetProxySessionForConnection (call);
			IMsgStore obj = session.ObjectStore.GetIMsgStore (arg1.obj.value.Value);
			var result = new MsgStore_GetOrigEID_res ();
			
			// TODO: TXC-only!
			result.eid = new SBinary ( ((TeamXChangeMsgStore) obj).OrigEID);
			
			Trace.WriteLine (" ==> END CALL MsgStore_GetOrigEID");	
			return result;
		}

		public override SimpleStream_Read_res SimpleStream_Read_1 (
			OncRpcCallInformation call, SimpleStream_Read_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL SimpleStream_Read");
			var session = GetProxySessionForConnection (call);
			var result = session.StreamHelper.Read (arg1);
			Trace.WriteLine (" ==> END CALL SimpleStream_Read");
			return result;
		}

		public override SimpleStream_Write_res SimpleStream_Write_1 (
			OncRpcCallInformation call, SimpleStream_Write_arg arg1)
		{			
			Trace.WriteLine (" ==> START CALL SimpleStream_Read");		
			var session = GetProxySessionForConnection (call);
			var result = session.StreamHelper.Write (arg1);
			Trace.WriteLine (" ==> END CALL SimpleStream_Read");
			return result;
		}
		
		public override SimpleStream_EndWrite_res SimpleStream_EndWrite_1 (
			OncRpcCallInformation call, SimpleStream_EndWrite_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL SimpleStream_EndWrite");		
			var session = GetProxySessionForConnection (call);
			session.StreamHelper.EndWrite (arg1);
			var result = new SimpleStream_EndWrite_res ();
			Trace.WriteLine (" ==> END CALL SimpleStream_EndWrite");
			return result;
		}

		// TODO: stubbed !!!!!!!!!!!
		public override Session_ABGetUserData_res Session_ABGetUserData_1 (
			OncRpcCallInformation call, Session_ABGetUserData_arg arg1)
		{
			Trace.WriteLine (" ==> Session_ABGetUserData_1 - NOT IMPLEMENTED!");
			var res = new Session_ABGetUserData_res (); // STUB
			var ud = new ABUSERDATA ();
			ud.pwszId = new UnicodeAdapter ("");
			ud.pwszDisplay = new UnicodeAdapter ("");
			ud.pwszAdrType = new UnicodeAdapter ("");
			ud.pwszSmtpAdr = new UnicodeAdapter ("");
			ud.pwszIntAdr = new UnicodeAdapter ("");
			ud.eid = new SBinary (new byte [] {0});
			ud.searchKey = new SBinary (new byte [] {0});
			res.pData = ud;
			return res;
		}
		
	}

}
