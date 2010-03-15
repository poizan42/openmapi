//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMapiFolder.cs
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

namespace NMapi.Properties.Special {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using CompactTeaSharp;

	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;
	using NMapi.Flags;
	using NMapi.Table;

	public class TeamXChangeMapiFolder : TeamXChangeMapiContainer, IMapiFolder
	{
		private TeamXChangeMsgStore store;
	
		internal TeamXChangeMapiFolder (long obj, TeamXChangeMsgStore store) :
			base (obj, store.session)
		{
			this.store = store;
		}
	
		internal TeamXChangeMsgStore Store {
			get { return store; }
		}

		public IMessage CreateMessage (NMapiGuid interFace, int flags)
		{
			var prms = new MAPIFolder_CreateMessage_arg ();
			prms.obj = new HObject (obj);
			prms.lpInterface = new LPGuid (interFace);
			prms.ulFlags = flags;
			
			var res = MakeCall<MAPIFolder_CreateMessage_res, 
				MAPIFolder_CreateMessage_arg> (clnt.MAPIFolder_CreateMessage_1, prms);
			return (IMessage) session.CreateObject (this, 
				res.obj.Value.Value, res.ulObjType, interFace);
		}

		private SBinary MyCopyMessage (byte [] eid, IMapiFolder destFolder)
		{
			IMessage msgSource = null, msgDest = null;
			try {
				msgSource = (IMessage) OpenEntry (eid, null, 0);
				msgDest = destFolder.CreateMessage (null, 0);
				MessageCopyHelper.MyCopyMessage (msgSource, msgDest);
				var prop = (BinaryProperty) new MapiPropHelper (msgDest).HrGetOneProp (Property.EntryId);
				return prop.Value;
			}
			finally {
				if (msgSource != null)
					msgSource.Close ();
				if (msgDest != null)
					msgDest.Close ();
			}
		}

		private EntryList CopyMessages2 (EntryList msgList,
			NMapiGuid interFace, IMapiFolder destFolder,
			IMapiProgress progress, int flags)
		{
			EntryList ret = null;
			List<SBinary> entryList = new List<SBinary> ();
			for (int i = 0; i < msgList.Bin.Length; i++) {
				try {
					SBinary eid = MyCopyMessage (msgList.Bin[i].lpb, destFolder);
					entryList.Add (eid);
					if ((flags & NMAPI.MESSAGE_MOVE) != 0) {
						EntryList entries = new EntryList ();
						entries.Bin = new SBinary [1];
						entries.Bin [0] = msgList.Bin [i];
						DeleteMessages (entries, null, 0);
					}
				}
				catch (MapiException) {
					entryList.Add (null);
				}
			}
			ret = new EntryList ();
			ret.Bin = entryList.ToArray ();
			return ret;
		}
			            
		public void CopyMessages (EntryList msgList,
			NMapiGuid interFace, IMapiFolder destFolder,
			IMapiProgress progress, int flags)
		{
			var arg = new MAPIFolder_CopyMessages_arg ();
			MAPIFolder_CopyMessages_res res;
			var binProp = (BinaryProperty) new MapiPropHelper (destFolder).
					HrGetOneProp (Property.EntryId);
			byte [] entryID = binProp.Value.lpb;
			arg.obj = new HObject (new LongLong (obj));
			arg.lpMsgList = new LPEntryList (msgList);
			arg.lpInterface = new LPGuid (interFace);
			arg.dsteid = new SBinary (entryID);
			arg.ulFlags = flags;			
			arg.pBar = session.CreateProgressBar (progress);
			try {
				res = clnt.MAPIFolder_CopyMessages_1 (arg);
			} catch (IOException e) {
				throw MapiException.Make (e);
			} catch (OncRpcException e) {
				throw MapiException.Make (e);
			} finally {
				session.UnregisterProgressBar (arg.pBar);
			}
			if (Error.CallHasFailed (res.hr)) {
				if (res.hr == Error.NoSupport) {
					if ((flags & NMAPI.MAPI_DECLINE_OK) != 0)
						throw new MapiDeclineCopyException ();
					else {
						CopyMessages2 (msgList,
							      interFace,
							      destFolder,
							      progress,
							      flags);
					}
				}
				else
					throw MapiException.Make (res.hr);
			}			
		}

		public void DeleteMessages (
			EntryList msgList, IMapiProgress progress, int flags)
		{
			var prms = new MAPIFolder_DeleteMessages_arg ();
			prms.obj = new HObject (obj);
			prms.lpMsgList = new LPEntryList (msgList);
			prms.ulFlags = flags;
			prms.pBar = session.CreateProgressBar (progress);
			try {
				var res = MakeCall<MAPIFolder_DeleteMessages_res, 
					MAPIFolder_DeleteMessages_arg> (clnt.MAPIFolder_DeleteMessages_1, prms);
			} finally {
				session.UnregisterProgressBar (prms.pBar);
			}
		}

		public IMapiFolder CreateFolder (FolderType folderType, string folderName, 
			string folderComment, NMapiGuid interFace, int flags)
		{
			var prms = new MAPIFolder_CreateFolder_arg ();
			prms.obj = new HObject (obj);
			prms.ulFolderType = (int) folderType;
			prms.lpInterface = new LPGuid (interFace);
			prms.ulFlags = flags;
			if ((flags & Mapi.Unicode) != 0) {
				prms.lpwszFolderNameW = new UnicodeAdapter (folderName);
				prms.lpwszFolderCommentW = new UnicodeAdapter (folderComment);
				prms.lpszFolderNameA = new StringAdapter ();
				prms.lpszFolderCommentA = new StringAdapter ();
			} else {
				prms.lpszFolderNameA = new StringAdapter (folderName);
				prms.lpszFolderCommentA = new StringAdapter (folderComment);
				prms.lpwszFolderNameW = new UnicodeAdapter ();
				prms.lpwszFolderCommentW = new UnicodeAdapter ();
			}
			var res = MakeCall<MAPIFolder_CreateFolder_res, 
				MAPIFolder_CreateFolder_arg> (clnt.MAPIFolder_CreateFolder_1, prms);
			
			return (IMapiFolder) session.CreateObject (this, 
				res.obj.Value.Value, res.ulObjType, interFace);
		}

		public void CopyFolder (byte [] entryID, NMapiGuid interFace, 
			IMapiFolder destFolder, string newFolderName,
			IMapiProgress progress, int flags)
		{
			var prms = new MAPIFolder_CopyFolder_arg ();
			prms.obj = new HObject (obj);

			var binProp = (BinaryProperty) new MapiPropHelper (destFolder).
					HrGetOneProp (Property.EntryId);
			byte [] destEntryID = binProp.Value.lpb;

			prms.srceid = new SBinary (entryID);
			prms.dsteid = new SBinary (destEntryID);
			prms.lpInterface = new LPGuid (interFace);
			prms.ulFlags = flags;
			prms.pBar = session.CreateProgressBar (progress);
			try {
				if ((prms.ulFlags & Mapi.Unicode) != 0) {
					prms.pszNewNameW = new UnicodeAdapter (newFolderName);
					prms.pszNewNameA = new StringAdapter ();
				} else {
					prms.pszNewNameA = new StringAdapter (newFolderName);
					prms.pszNewNameW = new UnicodeAdapter ();
				}

				var res = MakeCall<MAPIFolder_CopyFolder_res, 
					MAPIFolder_CopyFolder_arg> (clnt.MAPIFolder_CopyFolder_1, prms);
			} finally {
				session.UnregisterProgressBar (prms.pBar);
			}
		}

		public void DeleteFolder (byte [] entryID, IMapiProgress progress, int flags)
		{
			var prms = new MAPIFolder_DeleteFolder_arg ();
			prms.obj = new HObject (obj);
			prms.eid = new SBinary (entryID);
			prms.ulFlags = flags;
			prms.pBar = session.CreateProgressBar (progress);
			try {
				var res = MakeCall<MAPIFolder_DeleteFolder_res, 
					MAPIFolder_DeleteFolder_arg> (clnt.MAPIFolder_DeleteFolder_1, prms);
			} finally {
				session.UnregisterProgressBar (prms.pBar);
			}
		}

		public void SetReadFlags (EntryList msgList, IMapiProgress progress, int flags)
		{
			var prms = new MAPIFolder_SetReadFlags_arg ();
			prms.obj = new HObject (obj);
			prms.lpMsgList = new LPEntryList (msgList);
			prms.ulFlags = flags;
			prms.pBar = session.CreateProgressBar (progress);
			try {
				var res = MakeCall<MAPIFolder_SetReadFlags_res, 
					MAPIFolder_SetReadFlags_arg> (clnt.MAPIFolder_SetReadFlags_1, prms);
			} finally {
				session.UnregisterProgressBar (prms.pBar);
			}
		}

		public int GetMessageStatus (byte [] entryID, int flags)
		{
			var prms = new MAPIFolder_GetMessageStatus_arg ();
			prms.obj = new HObject (obj);
			prms.eid = new SBinary (entryID);
			prms.ulFlags = flags;
			
			var res = MakeCall<MAPIFolder_GetMessageStatus_res, 
				MAPIFolder_GetMessageStatus_arg> (clnt.MAPIFolder_GetMessageStatus_1, prms);
			return res.ulMessageStatus;
		}

		public int SetMessageStatus (byte [] entryID, int newStatus, int newStatusMask)
		{
			var prms = new MAPIFolder_SetMessageStatus_arg ();
			prms.obj = new HObject (obj);
			prms.eid = new SBinary (entryID);
			prms.ulNewStatus = newStatus;
			prms.ulNewStatusMask = newStatusMask;
			
			var res = MakeCall<MAPIFolder_SetMessageStatus_res, 
				MAPIFolder_SetMessageStatus_arg> (clnt.MAPIFolder_SetMessageStatus_1, prms);
			return res.ulOldStatus;
		}

		public void SaveContentsSort (SortOrderSet sortOrder, int flags)
		{
			var prms = new MAPIFolder_SaveContentsSort_arg ();
			prms.obj = new HObject (obj);
			prms.lpSort = new SortOrderSetPtrAdapter (sortOrder);
			prms.ulFlags = flags;
			
			var res = MakeCall<MAPIFolder_SaveContentsSort_res, 
				MAPIFolder_SaveContentsSort_arg> (clnt.MAPIFolder_SaveContentsSort_1, prms);
		}

		public void EmptyFolder (IMapiProgress progress, int flags)
		{
			var prms = new MAPIFolder_EmptyFolder_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;
			prms.pBar = session.CreateProgressBar (progress);
			try {
				var res = MakeCall<MAPIFolder_EmptyFolder_res, 
					MAPIFolder_EmptyFolder_arg> (clnt.MAPIFolder_EmptyFolder_1, prms);
			} finally {
				session.UnregisterProgressBar (prms.pBar);
			}
		}

		public long AssignIMAP4UID (byte [] entryID, int flags)
		{
			var prms = new MAPIFolder_AssignIMAP4UID_arg ();
			prms.obj = new HObject (obj);
			prms.msgeid = new SBinary (entryID);
			prms.ulFlags = flags;
			
			var res = MakeCall<MAPIFolder_AssignIMAP4UID_res, 
				MAPIFolder_AssignIMAP4UID_arg> (clnt.MAPIFolder_AssignIMAP4UID_1, prms);

			return res.msguid.Value;
		}
	
	}

}
