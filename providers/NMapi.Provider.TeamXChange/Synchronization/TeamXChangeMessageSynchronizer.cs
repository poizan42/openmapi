//
// openmapi.org - NMapi C# Mapi API - IMessageSynchronizer.cs
//
// Copyright 2009 VipCom GmbH, Topalis AG
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

namespace NMapi.Synchronization {

	using System;
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop.MapiRPC;

	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Properties.Special;
	using NMapi.Table;
	
	/// <summary>
	/// 
	/// </summary>
	public class TeamXChangeMessageSynchronizer : TeamXChangeBase
	{
		internal TeamXChangeMapiFolder folder;
		private TeamXChangeMessageImporter1 importer;
	
		public TeamXChangeMessageSynchronizer (long obj, TeamXChangeMapiFolder folder)
			: base (obj, folder.session)
		{
			this.folder = folder;
		}

		public void Configure (byte[] syncKey, int ulFlags)
		{
			var prms = new MsgSync_Configure_arg ();
			prms.obj = new HObject (obj);
			prms.synckey = new SBinary (syncKey);
			prms.ulFlags = ulFlags;

			var res = MakeCall<MsgSync_Configure_res, MsgSync_Configure_arg> (
				clnt.MsgSync_Configure_1, prms);
		}
	
		public void BeginExport (TeamXChangeMessageImporter1 importer)
		{
			this.importer = importer;
		
			var prms = new MsgSync_BeginExport_arg ();
			prms.obj = new HObject (obj);
		
			var res = MakeCall<MsgSync_BeginExport_res, MsgSync_BeginExport_arg> (
					clnt.MsgSync_BeginExport_1, prms);
		}
	
		private void ConfirmLastExport ()
		{
			var prms = new MsgSync_ConfirmLastExport_arg ();
			prms.obj = new HObject (obj);
		
			var res = MakeCall<MsgSync_ConfirmLastExport_res, MsgSync_ConfirmLastExport_arg> (
				clnt.MsgSync_ConfirmLastExport_1, prms);
		}
	
		public bool ExportNextMessage ()
		{
			var prms = new MsgSync_ExportNextMessage_arg ();
			prms.obj = new HObject (obj);

			MsgSync_ExportNextMessage_res res;
			try {
				res = MakeCall<MsgSync_ExportNextMessage_res, MsgSync_ExportNextMessage_arg> (
					clnt.MsgSync_ExportNextMessage_1, prms);
			} catch (MapiException e) {
				if (e.HResult == Error.NotFound)
					return true;
				throw;
			}
			IMessage msg = null;
			switch (res.item.action) {
				case Common.MessageSyncAction.Create:
					msg = (IMessage)session.CreateObject (folder, res.item.obj.Value.Value, res.item.ulObjType, null);
					importer.MessageCreated (res.item.messagekey.lpb, res.item.ulFlags, msg);
					msg.Close ();
				break;
				case Common.MessageSyncAction.Delete:
					importer.MessageDeleted (res.item.messagekey.lpb);
				break;
				case Common.MessageSyncAction.Modify:
					msg = (IMessage)session.CreateObject (folder, res.item.obj.Value.Value, res.item.ulObjType, null);
					importer.MessageChanged (res.item.messagekey.lpb, 0, msg);
					msg.Close ();
				break;
				case Common.MessageSyncAction.MoveFrom:
					importer.MessageMovedFrom (res.item.messagekey.lpb, 
					                          res.item.llChangeKey.Value,
					                          res.item.ulReadState);
				break;
				case Common.MessageSyncAction.MoveTo:
					importer.MessageMovedTo (res.item.messagekey.lpb, res.item.folderkey.lpb);
				break;
				case Common.MessageSyncAction.ReadState:
					importer.ReadStateChanged (res.item.messagekey.lpb, res.item.ulReadState);
				break;
			}
			ConfirmLastExport ();
			return false;
		}
	
		public byte [] EndExport ()
		{
			var prms = new MsgSync_EndExport_arg ();
			prms.obj = new HObject (obj);
			var res = MakeCall<MsgSync_EndExport_res, MsgSync_EndExport_arg> (
				clnt.MsgSync_EndExport_1, prms);
			return res.synckey.lpb;
		}
	
		public TeamXChangeMessageImporter2 BeginImport ()
		{
			var prms = new MsgSync_BeginImport_arg ();
			prms.obj = new HObject (obj);

			var res = MakeCall<MsgSync_BeginImport_res, MsgSync_BeginImport_arg> (
				clnt.MsgSync_BeginImport_1, prms);

			return (TeamXChangeMessageImporter2) folder.session.CreateObject (
				this, res.obj.Value.Value, res.ulObjType, null);
		}
	
		public void EndImport ()
		{
			var prms = new MsgSync_EndImport_arg ();
			prms.obj = new HObject (obj);

			var res = MakeCall<MsgSync_EndImport_res, MsgSync_EndImport_arg> (
				clnt.MsgSync_EndImport_1, prms);
		}
	
		public IMessage OpenMessage (byte[] messageKey, int ulFlags)
		{
			var prms = new MsgSync_OpenMessage_arg ();
			prms.obj = new HObject (obj);
			prms.key = new SBinary (messageKey);
			prms.ulFlags = ulFlags;

			var res = MakeCall<MsgSync_OpenMessage_res, MsgSync_OpenMessage_arg> (
				clnt.MsgSync_OpenMessage_1, prms);

			return (IMessage) folder.session.CreateObject (folder, res.obj.Value.Value, res.ulObjType, null);
		}

	}	
}