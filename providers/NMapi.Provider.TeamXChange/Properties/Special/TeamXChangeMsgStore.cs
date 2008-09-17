//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMsgStore.cs
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

namespace NMapi.Properties.Special {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using RemoteTea.OncRpc;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;

	using NMapi.Flags;
	using NMapi.Events;
	using NMapi.Table;

	public class TeamXChangeMsgStore : TeamXChangeMapiProp, IMsgStore
	{
		private ObjectEventProxy events;

		/// <summary>
		///  Provides simple access to events.
		/// </summary>
		public ObjectEventProxy Events {
			get {
				if (events == null)
					events = new ObjectEventProxy (this);
				return events;
			}
		}

		public TeamXChangeMsgStore (long obj, TeamXChangeSession session) : base (obj, session)
		{
		}

		/// <summary>
		///  Dispose is disabled for Message-Stores.
		/// </summary>
		public override void Dispose ()
		{
			//DO NOTHING
		}

		/// <summary>
		///  Close is disabled for Message-Stores.
		/// </summary>
		public override void Close ()
		{
			//DO NOTHING
		}
		
		/// <summary>
		///  This is called to actually close the message store.
		/// </summary>
		internal void Close2 ()
		{
			base.Dispose ();
		}

		public int Advise (byte [] entryID, 
			NotificationEventType eventMask, IMapiAdviseSink adviseSink)
		{
			return session.Advise (adviseSink, OrigEID, entryID, eventMask);
		}
		public void Unadvise (int connection)
		{
			session.Unadvise (connection);
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		internal byte [] OrigEID
		{
			get {
				var arg = new MsgStore_GetOrigEID_arg ();
				MsgStore_GetOrigEID_res res;
		
				arg.obj = new HObject (new LongLong (obj));
				try {
					res = clnt.MsgStore_GetOrigEID_1 (arg);
				}
				catch (IOException e) {
					throw new MapiException (e);
				}
				catch (OncRpcException e) {
					throw new MapiException (e);
				}
				return res.eid.lpb;
			}
		}

		public int CompareEntryIDs (byte [] entryID1, byte [] entryID2, int flags)
		{
			var arg = new MsgStore_CompareEntryIDs_arg ();
			MsgStore_CompareEntryIDs_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.eid1 = new SBinary (entryID1);
			arg.eid2 = new SBinary (entryID2);
			arg.ulFlags = flags;
			try {
				res = clnt.MsgStore_CompareEntryIDs_1(arg);
			}
			catch (IOException e) {
				throw new MapiException (e);
			}
			catch (OncRpcException e) {
				throw new MapiException (e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.ulResult;
		}

		public OpenEntryResult OpenEntry (byte [] entryID)
		{
			return OpenEntry (entryID, null, 0);
		}

		public OpenEntryResult Root {
			get { return OpenEntry (null, null, 0); }
		}

		public OpenEntryResult OpenEntry (
			byte [] entryID, NMapiGuid interFace, int flags)
		{
			var arg = new MsgStore_OpenEntry_arg();
			MsgStore_OpenEntry_res res;
			OpenEntryResult ret = new OpenEntryResult();
		
			arg.obj = new HObject (new LongLong (obj));
			arg.eid = new SBinary (entryID);
			arg.lpInterface = new LPGuid (interFace);
			arg.ulFlags = flags;
			try {
				res = clnt.MsgStore_OpenEntry_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);

			ret.ObjType = session.GetObjType (res.obj.Value.Value);
			ret.Unk = session.CreateObject (this, res.obj.Value.Value, interFace);
			return ret;
		}

		public void SetReceiveFolder (
			string messageClass, byte [] entryID, int flags)
		{
			var arg = new MsgStore_SetReceiveFolder_arg();
			MsgStore_SetReceiveFolder_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.eid = new SBinary(entryID);
			arg.ulFlags = flags;
			if ((flags & Mapi.Unicode) != 0)
				arg.lpszMessageClassW = new LPWStr (messageClass);
			else
				arg.lpszMessageClassA = new LPStr (messageClass);
			try {
				res = clnt.MsgStore_SetReceiveFolder_1 (arg);
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

		public GetReceiveFolderResult GetReceiveFolder (string messageClass, int flags)
		{
			var arg = new MsgStore_GetReceiveFolder_arg();
			MsgStore_GetReceiveFolder_res res;
			GetReceiveFolderResult ret = new GetReceiveFolderResult();
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			if ((flags & Mapi.Unicode) != 0) {
				arg.lpszMessageClassW = new LPWStr (messageClass);
				arg.lpszMessageClassA = new LPStr ();
			} else {
				arg.lpszMessageClassA = new LPStr (messageClass);
				arg.lpszMessageClassW = new LPWStr ();
			}
			try {
				res = clnt.MsgStore_GetReceiveFolder_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);

			ret.EntryID = res.eid.lpb;
			if ((flags & Mapi.Unicode) != 0)
				ret.ExplicitClass = res.lpszExplicitClassW.value;
			else
				ret.ExplicitClass = res.lpszExplicitClassA.value;
			return ret;
		}
	
		// NOT IMPLEMENTED: GetReceiveFolderTable()

		public void StoreLogoff (int flags)
		{
			var arg = new MsgStore_StoreLogoff_arg ();
			MsgStore_StoreLogoff_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			try {
				res = clnt.MsgStore_StoreLogoff_1(arg);
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
		}

		public void AbortSubmit (byte [] entryID, int flags)
		{
			var arg = new MsgStore_AbortSubmit_arg ();
			MsgStore_AbortSubmit_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.eid = new SBinary (entryID);
			arg.ulFlags = flags;
			try {
				res = clnt.MsgStore_AbortSubmit_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException (e);
			}
			catch (OncRpcException e) {
				throw new MapiException (e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
		}
		
		// NOT IMPLEMENTED: GetOutgoingQueue() 
		// NOT IMPLEMENTED: SetLockState()
		// NOT IMPLEMENTED: FinishedMsg()
		// NOT IMPLEMENTED: NotifyNewMail()

		public IMapiFolder HrOpenIPMFolder (string path, int flags)
		{
			byte [] eidroot;
			string [] paths;
			IMapiFolder folder = null;
			bool found = false;
		
			eidroot = HrGetOneProp (Property.IpmSubtreeEntryId).Value.bin.lpb;
			folder  = (IMapiFolder) OpenEntry (eidroot, null, flags).Unk;
			if (path == "/")
				return folder;
		
			paths = path.Split ('/');		
			try {
				for (int i = 1; i < paths.Length; i++) {
					IMapiTableReader tableReader = null;
					bool first = true;
					int idx_name = -1, idx_eid = -1;
					string  match = paths[i];
				
					try {
						found = false;
						tableReader = folder.GetHierarchyTable (Mapi.Unicode);
						while (!found) {
							SRowSet rows = tableReader.GetRows(10);
							if (rows.ARow.Length == 0)
								break;
						
							for (int idx = 0; idx < rows.ARow.Length; idx++) {
								SPropValue [] prps = rows.ARow [idx].lpProps;

								if (first) {
									first = false;
									idx_name = SPropValue.GetArrayIndex (
										prps, Property.DisplayNameW);

									idx_eid  = SPropValue.GetArrayIndex (
										prps, Property.EntryId);
								}
							
								SPropValue name = SPropValue.GetArrayProp (prps, idx_name);
								SPropValue eid  = SPropValue.GetArrayProp (prps, idx_eid);
							
								if (name != null && name.Value.Unicode == match) {
									folder = (IMapiFolder) OpenEntry (
											eid.Value.Binary.lpb, 
											null, flags).Unk;
									found = true;
									break;
								}
							}
						}
						if (!found)
							throw new MapiException (Error.NotFound);
					}
					finally {
						if (tableReader != null)
							tableReader.Close ();
					}
				}
			}
			finally {
				if (!found && folder != null)
					folder.Close ();
			}
			return folder;
		}
	}

}
