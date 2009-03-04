//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMessage.cs
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
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;

	using NMapi.Flags;
	using NMapi.Table;

	public class TeamXChangeMessage : TeamXChangeMapiProp, IMessage
	{
		private TeamXChangeBase parent;

		internal TeamXChangeMessage (long obj, TeamXChangeBase parent) : base (obj, parent.session)
		{
			this.parent = parent;
		}

		public IMapiTableReader GetAttachmentTable (int flags)
		{
			var prms = new Message_GetAttachmentTable_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;

			var res = MakeCall<Message_GetAttachmentTable_res, 
				Message_GetAttachmentTable_arg> (clnt.Message_GetAttachmentTable_1, prms);
			return (IMapiTableReader) session.CreateObject (this, 
				res.obj.Value.Value, res.ulObjType, null);
		}

		public IAttach OpenAttach (int attachmentNum, NMapiGuid interFace, int flags)
		{
			var prms = new Message_OpenAttach_arg ();
			prms.obj = new HObject (obj);
			prms.ulAttachmentNum = attachmentNum;
			prms.lpInterface = new LPGuid (interFace);
			prms.ulFlags = flags;

			var res = MakeCall<Message_OpenAttach_res, 
				Message_OpenAttach_arg> (clnt.Message_OpenAttach_1, prms);
			
			return (IAttach) session.CreateObject (this, 
				res.obj.Value.Value, res.ulObjType, interFace);
		}

		public CreateAttachResult CreateAttach (NMapiGuid interFace, int flags)
		{
			var prms = new Message_CreateAttach_arg ();
			prms.obj = new HObject (obj);
			prms.lpInterface = new LPGuid (interFace);
			prms.ulFlags = flags;

			var res = MakeCall<Message_CreateAttach_res, 
				Message_CreateAttach_arg> (clnt.Message_CreateAttach_1, prms);

			CreateAttachResult ret = new CreateAttachResult ();
			ret.AttachmentNum = res.ulAttachmentNum;
			ret.Attach = (IAttach) session.CreateObject (this, 
				res.obj.Value.Value, res.ulObjType, interFace);
			return ret;
		}

		public void DeleteAttach (int attachmentNum, IMapiProgress progress, int flags)
		{
			var prms = new Message_DeleteAttach_arg ();
			prms.obj = new HObject (obj);
			prms.ulAttachmentNum = attachmentNum;
			prms.ulFlags = flags;

			var res = MakeCall<Message_DeleteAttach_res, 
				Message_DeleteAttach_arg> (clnt.Message_DeleteAttach_1, prms);
		}

		public IMapiTableReader GetRecipientTable (int flags)
		{
			var prms = new Message_GetRecipientTable_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;

			var res = MakeCall<Message_GetRecipientTable_res, 
				Message_GetRecipientTable_arg> (clnt.Message_GetRecipientTable_1, prms);

			return (IMapiTableReader) session.CreateObject (this, 
				res.obj.Value.Value, res.ulObjType, null);
		}
	
		private SPropValue FindProp (int propTag, SPropValue [] props)
		{
			foreach (var item in props)
				if (item.PropTag == propTag)
					return item;
			return null;
		}

		public void ModifyRecipients (int flags, AdrList mods)
		{
			int i;
			bool bWipe = false;
		
			if (flags == 0) {
				bWipe = true;
				flags = ModRecip.Add;
			}
		
			if ((flags & (ModRecip.Modify|ModRecip.Remove)) != 0) {
				for (i = 0; i <mods.AEntries.Length; i++) 
					if (FindProp (Property.RowId, mods.AEntries[i].PropVals) == null) 
						throw new MapiException (Error.InvalidParameter);
			}

			if (bWipe) {
				var arg = new Message_DeleteRecipient_arg ();
				Message_DeleteRecipient_res res;
			
				arg.obj = new HObject (new LongLong (obj));
				arg.pEntry = new LPAdrEntry ();
				try {
					res = clnt.Message_DeleteRecipient_1(arg);
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
		
			if (flags == ModRecip.Add)
			{
				var arg = new Message_AddRecipient_arg();
				Message_AddRecipient_res res;
				SPropValue[] props;
				IntProperty rowid;
			
				for (i = 0; i < mods.AEntries.Length; i++) {
					arg.obj = new HObject (new LongLong (obj));
					arg.pEntry = new LPAdrEntry (mods.AEntries [i]);
					try {
						res = clnt.Message_AddRecipient_1(arg);
					}
					catch (IOException e) {
						throw new MapiException(e);
					}
					catch (OncRpcException e) {
						throw new MapiException(e);
					}
					if (Error.CallHasFailed (res.hr))
						throw new MapiException (res.hr);
					rowid = (IntProperty) FindProp (Property.RowId, mods.AEntries[i].PropVals);
					if (rowid == null) {
						props = new SPropValue [mods.AEntries[i].PropVals.Length];
						Array.Copy (mods.AEntries[i].PropVals, 0, props, 0, 
								     	 mods.AEntries[i].PropVals.Length);
						rowid = (IntProperty) props [props.Length-1];
					}
					rowid.PropTag = Property.RowId;
					rowid.Value = res.ulRowid;
				}
			} 
			else if (flags == ModRecip.Modify)
			{
				var arg = new Message_ModifyRecipient_arg ();
				Message_ModifyRecipient_res res;
			
				for (i = 0; i < mods.AEntries.Length; i++) 
				{
					arg.obj = new HObject (new LongLong (obj));
					arg.pEntry = new LPAdrEntry (mods.AEntries[i]);
					try {
						res = clnt.Message_ModifyRecipient_1 (arg);
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
			}
			else if (flags == ModRecip.Remove)
			{
				var arg = new Message_DeleteRecipient_arg();
				Message_DeleteRecipient_res res;
			
				for (i = 0; i < mods.AEntries.Length; i++) 
				{
					arg.obj = new HObject (new LongLong (obj));
					arg.pEntry = new LPAdrEntry (mods.AEntries[i]);
					try {
						res = clnt.Message_DeleteRecipient_1 (arg);
					}
					catch (IOException e) {
						throw new MapiException(e);
					}
					catch (OncRpcException e) {
						throw new MapiException(e);
					}
					if (Error.CallHasFailed (res.hr))
						throw new MapiException(res.hr);
				}
			}
		}

		public void SubmitMessage (int flags)
		{
			IMsgStore store;

			if (parent is IMapiFolder)
				store = ((TeamXChangeMapiFolder)parent).Store;
			else if (parent is IMsgStore)
				store = (IMsgStore) parent;
			else
				throw new MapiException (Error.InvalidParameter);
			
			// send it.
			var arg = new Message_SubmitMessage_arg();
			Message_SubmitMessage_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags | Common.SUBMIT_SETFLAGS | Common.SUBMIT_COPYTOQUEUE;
			try {
				res = clnt.Message_SubmitMessage_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);

			// see what to do after submit.		
			TeamXChangeMapiFolder srcfolder = null, dstfolder = null;
		
			try {
				int [] tags = { Property.ParentEntryId,
								Property.EntryId,
								Property.SentMailEntryId, 
								Property.DeleteAfterSubmit };
			
				SPropValue [] props = GetProps (new SPropTagArray(tags), 0);
			
				EntryList msglist = new EntryList (1);
				msglist.Bin [0] = ((BinaryProperty) props[1]).Value;
			
				srcfolder = (TeamXChangeMapiFolder) store.OpenEntry (
					((BinaryProperty) props[0]).Value.lpb, null, Mapi.Modify);
			
				if (props[2].PropTag == Property.SentMailEntryId) {
					dstfolder = (TeamXChangeMapiFolder) store.OpenEntry (
						((BinaryProperty) props[2]).Value.lpb, null, Mapi.Modify);
				
					srcfolder.CopyMessages (msglist, null, 
						dstfolder, null, NMAPI.MESSAGE_MOVE);
				}
				else if (props[3].PropTag == Property.DeleteAfterSubmit 
					&& ((BooleanProperty) props[3]).Value != 0)
				{	
					srcfolder.DeleteMessages (msglist, null, 0);
				}
			}
			finally {
				if (srcfolder != null)
					srcfolder.Close ();
				if (dstfolder != null)
					dstfolder.Close ();
			}
		}

		public void SetReadFlag (int flags)
		{
			var prms = new Message_SetReadFlag_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;

			var res = MakeCall<Message_SetReadFlag_res, 
				Message_SetReadFlag_arg> (clnt.Message_SetReadFlag_1, prms);
		}

	}

}
