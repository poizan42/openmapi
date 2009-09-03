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
using System.Security.Cryptography.X509Certificates;

using NMapi.Interop;
using NMapi.Interop.MapiRPC;
using CompactTeaSharp;
using CompactTeaSharp.Server;

namespace NMapi.Server {

	public sealed partial class OncRpcService : BaseOncRpcService
	{
		public OncRpcService (CommonRpcService service, 
			SessionManager sman, IPAddress ip, int port, SslStore sslParams) 
				: base (service, sman, ip, port, sslParams)
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
			Trace.WriteLine (" ==> END CALL Base_Close");
			return result;	
		}

		// DEBUGGING ...
		public override MsgStore_SetWrappedEID_res MsgStore_SetWrappedEID_1 (
			OncRpcCallInformation call, MsgStore_SetWrappedEID_arg arg1)
		{
			Console.WriteLine (arg1.eid.ToHexString ());
			return new MsgStore_SetWrappedEID_res ();
		}
		
		
		public override MsgStore_GetOrigEID_res MsgStore_GetOrigEID_1 (
			OncRpcCallInformation call, MsgStore_GetOrigEID_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL MsgStore_GetOrigEID");
			
			var session = GetProxySessionForConnection (call);
			IMsgStore obj = session.ObjectStore.GetIMsgStore (arg1.obj.value.Value);
			var result = new MsgStore_GetOrigEID_res ();
			// TODO: try ... catch ....
			result.eid = new SBinary (obj.OrigEID);
/*				
			if (obj is TeamXChangeMsgStore)
				result.eid = new SBinary ( ((TeamXChangeMsgStore) obj).OrigEID);
			else {
				PropertyTag[] tags = PropertyTag.ArrayFromIntegers (Property.EntryId);
				PropertyValue[] props = obj.GetProps (tags, 0);
				if (props.Length != 1)
					throw new MapiException ("Property.EntryID " + 
						"property not found on MessageStore object.");
				result.eid = (SBinary) props [0];
			}*/
			
			Trace.WriteLine (" ==> END CALL MsgStore_GetOrigEID");	
			return result;
		}

		public override SimpleStream_Read_res SimpleStream_Read_1 (
			OncRpcCallInformation call, SimpleStream_Read_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL SimpleStream_Read");
			SimpleStream_Read_res result = new SimpleStream_Read_res ();
			try {
				var session = GetProxySessionForConnection (call);
				try {
					var result2 = session.StreamHelper.Read (arg1);
					result = result2;
				} catch (MapiException e) {
					result.hr = e.HResult;
				}
			} catch (Exception e) {
				LogException (e);
				result.hr = Error.CallFailed;	
			}
			Trace.WriteLine (" ==> END CALL SimpleStream_Read");
			return result;
		}

		public override SimpleStream_Write_res SimpleStream_Write_1 (
			OncRpcCallInformation call, SimpleStream_Write_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL SimpleStream_Write");
			SimpleStream_Write_res result = new SimpleStream_Write_res ();
			try {
				var session = GetProxySessionForConnection (call);
				try {
					var result2 = session.StreamHelper.Write (arg1);
					result = result2;
				} catch (MapiException e) {
					result.hr = e.HResult;
				}
			} catch (Exception e) {
				LogException (e);
				result.hr = Error.CallFailed;	
			}
			Trace.WriteLine (" ==> END CALL SimpleStream_Write");
			return result;
		}
		
		public override SimpleStream_EndWrite_res SimpleStream_EndWrite_1 (
			OncRpcCallInformation call, SimpleStream_EndWrite_arg arg1)
		{
			Trace.WriteLine (" ==> START CALL SimpleStream_EndWrite");
			var result = new SimpleStream_EndWrite_res ();
			
			try {
				var session = GetProxySessionForConnection (call);
				try {
					session.StreamHelper.EndWrite (arg1);
				} catch (MapiException e) {
					result.hr = e.HResult;
				}
			} catch (Exception e) {
				LogException (e);
				result.hr = Error.CallFailed;	
			}
			
			Trace.WriteLine (" ==> END CALL SimpleStream_EndWrite");
			return result;
		}



		public override Session_ABGetChangeTime_res Session_ABGetChangeTime_1 (
			OncRpcCallInformation call, Session_ABGetChangeTime_arg arg1)
		{
			int flags = arg1.ulFlags;

			Trace.WriteLine (" ==> Session_ABGetChangeTime_1 - NOT IMPLEMENTED!");
			var res = new Session_ABGetChangeTime_res ();
			res.ft = new FileTime (DateTime.Today); // TODO: STUB!
			return res;
		}
		
		
		public override Session_ABGetUserList_res Session_ABGetUserList_1 (
			OncRpcCallInformation call, Session_ABGetUserList_arg arg1)
		{
			int flags = arg1.ulFlags;

			Trace.WriteLine (" ==> Session_ABGetUserList_1 - NOT IMPLEMENTED!");
			var res = new Session_ABGetUserList_res (); // STUB
			
			res.pList = new ABUSERLIST (); // TODO: STUB!
			res.pList.pData = MakeStubData ();
			res.pList.pNext = null;

			return res;
		}
		

		// TODO: stubbed !!!!!!!!!!!
		public override Session_ABGetUserData_res Session_ABGetUserData_1 (
			OncRpcCallInformation call, Session_ABGetUserData_arg arg1)
		{
			Trace.WriteLine (" ==> Session_ABGetUserData_1 - NOT IMPLEMENTED!");
			var res = new Session_ABGetUserData_res (); // STUB
			res.pData = MakeStubData ();
			return res;
		}
		
		// debug
		private ABUSERDATA MakeStubData ()
		{
			return MakeABUserData ("0400000", "STUB", "EX", "STUB@STUB.de", "/o=ORGANIZATION/ou=UNIT/cn=Recipients/cn=STUB", 
								new byte [] {0x00, 0x00, 0x00, 0x00, 0xA1, 0x06, 0xC2, 0xC5, 0x81, 0x96, 0x44, 0x92, 0xA3, 0xCA, 0xC9, 0xDA, 0xF4, 0x73, 0xA2, 0x16, 0x02, 0x6A, 0x00, 
													0x72, 0x00, 0x6F, 0x00, 0x69, 0x00, 0x74, 0x00, 0x68, 0x00, 0x00, 0x00 },

								new byte [] {0x00, 0x00, 0x00, 0x00, 0xA1, 0x06, 0xC2, 0xC5, 0x81, 0x96, 0x44, 0x92, 0xA3, 0xCA, 0xC9, 0xDA, 0xF4, 0x73, 0xA2, 0x16, 0x02, 0x6A, 0x00, 
													0x72, 0x00, 0x6F, 0x00, 0x69, 0x00, 0x74, 0x00, 0x68, 0x00, 0x00, 0x00 } );
		}
		
		private ABUSERDATA MakeABUserData (string id, string display, string adrType, 
			string smtpAdr, string intAdr, byte[] eid, byte[] searchKey)
		{
			var ud = new ABUSERDATA ();
			ud.pwszId = new UnicodeAdapter (id);
			ud.pwszDisplay = new UnicodeAdapter (display);
			ud.pwszAdrType = new UnicodeAdapter (adrType);
			ud.pwszSmtpAdr = new UnicodeAdapter (smtpAdr);
			ud.pwszIntAdr = new UnicodeAdapter (intAdr);
			ud.eid = new SBinary (eid);
			ud.searchKey = new SBinary (searchKey);
			return ud;
		}
		
	}

}
