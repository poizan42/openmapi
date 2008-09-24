//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMapiFolder.cs
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
			var arg = new MAPIFolder_CreateMessage_arg();
			MAPIFolder_CreateMessage_res res;

			arg.obj = new HObject (new LongLong (obj));
			arg.lpInterface = new LPGuid (interFace);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIFolder_CreateMessage_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return (IMessage) session.CreateObject (this, res.obj.Value.Value, interFace);
		}

		private SBinary MyCopyMessage (byte [] eid, IMapiFolder destFolder)
		{
			IMessage msgSource = null, msgDest = null;
			try {
				msgSource = (IMessage) OpenEntry (eid, null, 0).Unk;
				msgDest = destFolder.CreateMessage (null, 0);
				MessageCopyHelper.MyCopyMessage (msgSource, msgDest);
				return msgDest.HrGetOneProp (Property.EntryId).Value.bin;
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
			byte [] entryID = destFolder.HrGetOneProp (
					Property.EntryId).Value.bin.lpb;
			arg.obj = new HObject (new LongLong (obj));
			arg.lpMsgList = new LPEntryList (msgList);
			arg.lpInterface = new LPGuid (interFace);
			arg.dsteid = new SBinary (entryID);
			arg.ulFlags = flags;
			arg.pBar = new LPProgressBar();
			try {
				res = clnt.MAPIFolder_CopyMessages_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException (e);
			}
			catch (OncRpcException e) {
				throw new MapiException (e);
			}
			if (Error.CallHasFailed (res.hr)) {
				if (res.hr == Error.NoSupport) {
					if ((flags & NMAPI.MAPI_DECLINE_OK) != 0)
						throw new MapiException (Error.DeclineCopy);
					else {
						CopyMessages2 (msgList,
							      interFace,
							      destFolder,
							      progress,
							      flags);
					}
				}
				else
					throw new MapiException(res.hr);
			}
		}

		public void DeleteMessages (
			EntryList msgList, IMapiProgress progress, int flags)
		{
			var arg = new MAPIFolder_DeleteMessages_arg();
			MAPIFolder_DeleteMessages_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpMsgList = new LPEntryList (msgList);
			arg.ulFlags = flags;
			arg.pBar = new LPProgressBar ();
			try {
				res = clnt.MAPIFolder_DeleteMessages_1(arg);
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

		public IMapiFolder CreateFolder (Folder folderType, string folderName, 
			string folderComment, NMapiGuid interFace, int flags)
		{
			var arg = new MAPIFolder_CreateFolder_arg();
			MAPIFolder_CreateFolder_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFolderType = (int) folderType;
			arg.lpInterface = new LPGuid (interFace);
			arg.ulFlags = flags;
			if ((flags & Mapi.Unicode) != 0) {
				arg.lpwszFolderNameW = new LPWStr (folderName);
				arg.lpwszFolderCommentW = new LPWStr (folderComment);
				arg.lpszFolderNameA = new LPStr ();
				arg.lpszFolderCommentA = new LPStr ();
			} else {
				arg.lpszFolderNameA = new LPStr (folderName);
				arg.lpszFolderCommentA = new LPStr (folderComment);
				arg.lpwszFolderNameW = new LPWStr ();
				arg.lpwszFolderCommentW = new LPWStr ();
			}
			try {
				res = clnt.MAPIFolder_CreateFolder_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return (IMapiFolder) session.CreateObject (this, res.obj.Value.Value, interFace);
		}

		public void CopyFolder (byte [] entryID, NMapiGuid interFace, 
			IMapiFolder destFolder, string newFolderName,
			IMapiProgress progress, int flags)
		{
			var arg = new MAPIFolder_CopyFolder_arg();
			MAPIFolder_CopyFolder_res res;
			byte [] destEntryID = destFolder.HrGetOneProp (
						Property.EntryId).Value.Binary.lpb;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.srceid = new SBinary (entryID);
			arg.dsteid = new SBinary (destEntryID);
			arg.lpInterface = new LPGuid (interFace);
			arg.ulFlags = flags;
			arg.pBar = new LPProgressBar();
			if ((arg.ulFlags & Mapi.Unicode) != 0) {
				arg.pszNewNameW = new LPWStr (newFolderName);
				arg.pszNewNameA = new LPStr ();
			} else {
				arg.pszNewNameA = new LPStr (newFolderName);
				arg.pszNewNameW = new LPWStr ();
			}
			try {
				res = clnt.MAPIFolder_CopyFolder_1(arg);
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

		public void DeleteFolder (byte [] entryID, IMapiProgress progress, int flags)
		{
			MAPIFolder_DeleteFolder_arg arg = new MAPIFolder_DeleteFolder_arg();
			MAPIFolder_DeleteFolder_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.eid = new SBinary (entryID);
			arg.ulFlags = flags;
			arg.pBar = new LPProgressBar ();
			try {
				res = clnt.MAPIFolder_DeleteFolder_1 (arg);
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

		public void SetReadFlags (EntryList msgList, IMapiProgress progress, int flags)
		{
			var arg = new MAPIFolder_SetReadFlags_arg();
			MAPIFolder_SetReadFlags_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpMsgList = new LPEntryList (msgList);
			arg.ulFlags = flags;
			arg.pBar = new LPProgressBar();
			try {
				res = clnt.MAPIFolder_SetReadFlags_1(arg);
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

		public int GetMessageStatus (byte [] entryID, int flags)
		{
			var arg = new MAPIFolder_GetMessageStatus_arg();
			MAPIFolder_GetMessageStatus_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.eid = new SBinary (entryID);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIFolder_GetMessageStatus_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.ulMessageStatus;
		}

		public int SetMessageStatus (byte [] entryID, int newStatus, int newStatusMask)
		{
			var arg = new MAPIFolder_SetMessageStatus_arg();
			MAPIFolder_SetMessageStatus_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.eid = new SBinary (entryID);
			arg.ulNewStatus = newStatus;
			arg.ulNewStatusMask = newStatusMask;
			try {
				res = clnt.MAPIFolder_SetMessageStatus_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e); 
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return res.ulOldStatus;
		}

		public void SaveContentsSort (SSortOrderSet sortOrder, int flags)
		{
			var arg = new MAPIFolder_SaveContentsSort_arg();
			MAPIFolder_SaveContentsSort_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpSort = new LPSSortOrderSet (sortOrder);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIFolder_SaveContentsSort_1(arg);
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

		public void EmptyFolder (IMapiProgress progress, int flags)
		{
			var arg = new MAPIFolder_EmptyFolder_arg();
			MAPIFolder_EmptyFolder_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			arg.pBar = new LPProgressBar ();
			try {
				res = clnt.MAPIFolder_EmptyFolder_1 (arg);
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

		public long AssignIMAP4UID (byte [] entryID, int flags)
		{
			var arg = new MAPIFolder_AssignIMAP4UID_arg();
			MAPIFolder_AssignIMAP4UID_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.msgeid = new SBinary (entryID);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIFolder_AssignIMAP4UID_1 (arg);
			}
			catch (IOException e) { 
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return res.msguid.Value;
		}
	
	}

}
