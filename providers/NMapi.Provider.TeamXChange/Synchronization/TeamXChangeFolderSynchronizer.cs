//
// openmapi.org - NMapi C# Mapi API - TeamXChangeFolderSynchronizer.cs
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
	public class TeamXChangeFolderSynchronizer : TeamXChangeBase
	{
		internal TeamXChangeMsgStore store;
		internal TeamXChangeFolderImporter1 importer;
	
		public TeamXChangeFolderSynchronizer (long obj, TeamXChangeMsgStore store)
			: base (obj, store.session)
		{
			this.store = store;
		}
	
		public void Configure (byte [] syncKey, int ulFlags)
		{
			var prms = new FldSync_Configure_arg ();
			prms.obj = new HObject (obj);
			prms.synckey = new SBinary (syncKey);
			prms.ulFlags = ulFlags;

			var res = MakeCall<FldSync_Configure_res, FldSync_Configure_arg> (
				clnt.FldSync_Configure_1, prms);
		}

		public void BeginExport (TeamXChangeFolderImporter1 importer)
		{	
			var prms = new FldSync_BeginExport_arg ();
			prms.obj = new HObject (obj);

			var res = MakeCall<FldSync_BeginExport_res, FldSync_BeginExport_arg> (
				clnt.FldSync_BeginExport_1, prms);

			this.importer = importer;
		}
	
		private void ConfirmLastExport ()
		{
			var prms = new FldSync_ConfirmLastExport_arg ();
			prms.obj = new HObject (obj);

			var res = MakeCall<FldSync_ConfirmLastExport_res, FldSync_ConfirmLastExport_arg> (
				clnt.FldSync_ConfirmLastExport_1, prms);
		}
	
		public bool ExportNextFolder ()
		{
			var prms = new FldSync_ExportNextFolder_arg ();
			prms.obj = new HObject (obj);

			FldSync_ExportNextFolder_res res;
			try {
				res = MakeCall<FldSync_ExportNextFolder_res, FldSync_ExportNextFolder_arg> (
						clnt.FldSync_ExportNextFolder_1, prms);
			} catch (MapiException e) {
				if (e.HResult == Error.NotFound)
					return true;
				throw;
			}
			switch (res.item.action)
			{
				case Common.FolderSyncAction.Create:
					IMapiFolder folder = (IMapiFolder) session.CreateObject (store, 
						res.item.obj.Value.Value, res.item.ulObjType, null);
					importer.FolderCreated (res.item.folderkey.lpb,
						folder, res.item.newparentkey.lpb,
						res.item.pwszName.value, res.item.ulFolderType);
					folder.Close ();
					break;
					case Common.FolderSyncAction.Delete:
					importer.FolderDeleted (res.item.folderkey.lpb, res.item.oldparentkey.lpb);
					break;
				case Common.FolderSyncAction.Modify:
					importer.FolderChanged (res.item.folderkey.lpb,
						res.item.pwszName.value, res.item.oldparentkey.lpb,
						res.item.newparentkey.lpb);
					break;
			}
			ConfirmLastExport ();
			return false;
		}
	
		public byte[] EndExport ()
		{
			var prms = new FldSync_EndExport_arg ();
			prms.obj = new HObject (obj);

			var res = MakeCall<FldSync_EndExport_res, FldSync_EndExport_arg> (
				clnt.FldSync_EndExport_1, prms);
			return res.synckey.lpb;
		}
	
		public TeamXChangeFolderImporter2 BeginImport ()
		{
			var prms = new FldSync_BeginImport_arg ();
			prms.obj = new HObject (obj);

			var res = MakeCall<FldSync_BeginImport_res, FldSync_BeginImport_arg> (
				clnt.FldSync_BeginImport_1, prms);
			return (TeamXChangeFolderImporter2) store.session.CreateObject (this, res.obj.Value.Value, res.ulObjType, null);
		}
	
		public void EndImport ()
		{
			var prms = new FldSync_EndImport_arg ();
			prms.obj = new HObject (obj);

			var res = MakeCall<FldSync_EndImport_res, FldSync_EndImport_arg> (
				clnt.FldSync_EndImport_1, prms);
		}
	
		public IMapiFolder OpenFolder (byte [] folderKey, int ulFlags)
		{
			var prms = new FldSync_OpenFolder_arg ();
			prms.obj = new HObject (obj);
			prms.key = new SBinary (folderKey);
			prms.ulFlags = ulFlags;

			var res = MakeCall<FldSync_OpenFolder_res, FldSync_OpenFolder_arg> (
				clnt.FldSync_OpenFolder_1, prms);
			return (IMapiFolder) store.session.CreateObject (store, res.obj.Value.Value, res.ulObjType, null);
		}
	}
}