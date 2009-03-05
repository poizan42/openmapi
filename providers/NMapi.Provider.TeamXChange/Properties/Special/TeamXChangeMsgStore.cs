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
	using CompactTeaSharp;
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
			return session.EventServer.Advise (this, entryID, eventMask, adviseSink);
		}

		public void Unadvise (int connection)
		{
			session.EventServer.Unadvise (connection);
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		public byte [] OrigEID														// HACK: This should be private or internal.
		{
			get {
				var prms = new MsgStore_GetOrigEID_arg ();
				prms.obj = new HObject (obj);

				var res = MakeCall<MsgStore_GetOrigEID_res, 
					MsgStore_GetOrigEID_arg> (clnt.MsgStore_GetOrigEID_1, prms);
				return res.eid.lpb;
			}
		}

		public int CompareEntryIDs (byte [] entryID1, byte [] entryID2, int flags)
		{		
			var prms = new MsgStore_CompareEntryIDs_arg ();
			prms.obj = new HObject (obj);
			prms.eid1 = new SBinary (entryID1);
			prms.eid2 = new SBinary (entryID2);
			prms.ulFlags = flags;
			
			var res = MakeCall<MsgStore_CompareEntryIDs_res, 
				MsgStore_CompareEntryIDs_arg> (
					clnt.MsgStore_CompareEntryIDs_1, prms);
			return res.ulResult;
		}

		public IBase OpenEntry (byte [] entryID)
		{
			return OpenEntry (entryID, null, 0);
		}

		public IBase Root {
			get { return OpenEntry (null, null, 0); }
		}

		public IBase OpenEntry (
			byte [] entryID, NMapiGuid interFace, int flags)
		{
			var prms = new MsgStore_OpenEntry_arg ();
			prms.obj = new HObject (obj);
			prms.eid = new SBinary (entryID);
			prms.lpInterface = new LPGuid (interFace);
			prms.ulFlags = flags;
			
			var res = MakeCall<MsgStore_OpenEntry_res, 
				MsgStore_OpenEntry_arg> (
					clnt.MsgStore_OpenEntry_1, prms);
			
			return session.CreateObject (this, res.obj.Value.Value, res.ulObjType, interFace);
		}

		public void SetReceiveFolder (
			string messageClass, byte [] entryID, int flags)
		{
			var prms = new MsgStore_SetReceiveFolder_arg ();
			prms.obj = new HObject (obj);
			prms.eid = new SBinary(entryID);
			prms.ulFlags = flags;
			if ((flags & Mapi.Unicode) != 0)
				prms.lpszMessageClassW = new UnicodeAdapter (messageClass);
			else
				prms.lpszMessageClassA = new StringAdapter (messageClass);
			
			var res = MakeCall<MsgStore_SetReceiveFolder_res, 
				MsgStore_SetReceiveFolder_arg> (
					clnt.MsgStore_SetReceiveFolder_1, prms);
		}

		public GetReceiveFolderResult GetReceiveFolder (string messageClass, int flags)
		{
			var prms = new MsgStore_GetReceiveFolder_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;
			if ((flags & Mapi.Unicode) != 0) {
				prms.lpszMessageClassW = new UnicodeAdapter (messageClass);
				prms.lpszMessageClassA = new StringAdapter ();
			} else {
				prms.lpszMessageClassA = new StringAdapter (messageClass);
				prms.lpszMessageClassW = new UnicodeAdapter ();
			}
						
			var res = MakeCall<MsgStore_GetReceiveFolder_res, 
				MsgStore_GetReceiveFolder_arg> (
					clnt.MsgStore_GetReceiveFolder_1, prms);

			GetReceiveFolderResult ret = new GetReceiveFolderResult();
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
			var prms = new MsgStore_StoreLogoff_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;
						
			var res = MakeCall<MsgStore_StoreLogoff_res, 
				MsgStore_StoreLogoff_arg> (
					clnt.MsgStore_StoreLogoff_1, prms);
		}

		public void AbortSubmit (byte [] entryID, int flags)
		{
			var prms = new MsgStore_AbortSubmit_arg ();
			prms.obj = new HObject (obj);
			prms.eid = new SBinary (entryID);
			prms.ulFlags = flags;
						
			var res = MakeCall<MsgStore_AbortSubmit_res, 
				MsgStore_AbortSubmit_arg> (
					clnt.MsgStore_AbortSubmit_1, prms);
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

			var binProp = (BinaryProperty) new MapiPropHelper (this).HrGetOneProp (Property.IpmSubtreeEntryId);
			eidroot = binProp.Value.lpb;
			folder  = (IMapiFolder) OpenEntry (eidroot, null, flags);
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
							RowSet rows = tableReader.GetRows(10);
							if (rows.ARow.Length == 0)
								break;

							for (int idx = 0; idx < rows.ARow.Length; idx++) {
								PropertyValue [] prps = rows.ARow [idx].lpProps;

								if (first) {
									first = false;
									idx_name = PropertyValue.GetArrayIndex (
										prps, Property.DisplayNameW);

									idx_eid  = PropertyValue.GetArrayIndex (
										prps, Property.EntryId);
								}
							
								PropertyValue name = PropertyValue.GetArrayProp (prps, idx_name);
								BinaryProperty eid = (BinaryProperty) PropertyValue.GetArrayProp (prps, idx_eid);
								if (name != null && ((UnicodeProperty) name).Value == match) {
									folder = (IMapiFolder) OpenEntry (eid.Value.lpb, null, flags);
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
				if (!found && folder != null) {
					folder.Close ();
					folder = null;
				}
			}
			return folder;
		}
	}

}
