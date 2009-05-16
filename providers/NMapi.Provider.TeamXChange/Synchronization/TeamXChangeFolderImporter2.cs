//
// openmapi.org - NMapi C# Mapi API - TeamXChangeFolderImporter2.cs
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

	using NMapi.Interop;
	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Properties.Special;
	using NMapi.Table;


	public class TeamXChangeFolderImporter2 : TeamXChangeBase
	{	
		private TeamXChangeFolderSynchronizer sync;

		public TeamXChangeFolderImporter2 (long obj, TeamXChangeFolderSynchronizer sync)
			: base (obj, sync.session)
		{
			this.sync = sync;
		}
	
		public IMapiFolder FolderCreated (byte [] folderKey,
			byte [] parentKey, string name, int ulFolderType)
		{
			var prms = new FldImp_FolderCreated_arg ();
			prms.obj = new HObject (obj);
			prms.key = new SBinary (folderKey);
			prms.parentkey = new SBinary (parentKey);
			prms.pwszName  = new UnicodeAdapter (name);
	
			var res = MakeCall<FldImp_FolderCreated_res, FldImp_FolderCreated_arg> (
				clnt.FldImp_FolderCreated_1, prms);
		
			return (IMapiFolder) sync.store.session.CreateObject (
					sync.store, res.obj.Value.Value, res.ulObjType, null);
		}
	
		public void FolderDeleted (byte [] folderKey, byte [] parentKey)
		{
			var prms = new FldImp_FolderDeleted_arg ();
			prms.obj = new HObject (obj);
			prms.key = new SBinary (folderKey);
			prms.parentkey = new SBinary (parentKey);
	
			var res = MakeCall<FldImp_FolderDeleted_res, FldImp_FolderDeleted_arg> (
				clnt.FldImp_FolderDeleted_1, prms);
		}
	
		public void FolderChanged (byte [] folderKey,
			string  name, byte [] oldParentKey, byte [] newParentKey)
		{	
			var prms = new FldImp_FolderChanged_arg ();
			prms.obj          = new HObject (obj);
			prms.key          = new SBinary(folderKey);
			prms.pwszName     = new UnicodeAdapter (name);
			prms.oldparentkey = new SBinary(oldParentKey);
			prms.newparentkey = new SBinary(newParentKey);

			var res = MakeCall<FldImp_FolderChanged_res, FldImp_FolderChanged_arg> (
				clnt.FldImp_FolderChanged_1, prms);
		}
	
	}

}