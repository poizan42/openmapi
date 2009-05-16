//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMessageSynchronizer.cs
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
	public class TeamXChangeMessageImporter2 : TeamXChangeBase
	{
		private TeamXChangeMessageSynchronizer sync;
	
		public TeamXChangeMessageImporter2 (long obj, TeamXChangeMessageSynchronizer sync)
		 	: base (obj, sync.folder.session)
		{
			this.sync = sync;
		}
	
		public IMessage MessageCreated (byte[] msgKey, int ulFlags)
		{
			var prms = new MsgImp_MessageCreated_arg ();		
			prms.obj = new HObject (obj);
			prms.key = new SBinary (msgKey);
			prms.ulFlags = ulFlags;

			var res = MakeCall<MsgImp_MessageCreated_res, MsgImp_MessageCreated_arg> (
				clnt.MsgImp_MessageCreated_1, prms);

			return (IMessage) sync.folder.session.CreateObject 
				(sync.folder, res.obj.Value.Value, res.ulObjType, null);
		}
	
		public IMessage MessageChanged (byte [] msgKey, int ulFlags)
		{
			var prms = new MsgImp_MessageChanged_arg ();
			prms.obj = new HObject (obj);
			prms.key = new SBinary (msgKey);
			prms.ulFlags = ulFlags;

			var res = MakeCall<MsgImp_MessageChanged_res, MsgImp_MessageChanged_arg> (
					clnt.MsgImp_MessageChanged_1, prms);

			return (IMessage) sync.folder.session.CreateObject(sync.folder, res.obj.Value.Value, res.ulObjType, null);
		}
	
		public void ReadStateChanged (byte [] msgKey, int ulReadState)
		{
			var prms = new MsgImp_ReadStateChanged_arg ();
			prms.obj = new HObject (obj);
			prms.key = new SBinary(msgKey);
			prms.ulReadState = ulReadState;

			var res = MakeCall<MsgImp_ReadStateChanged_res, MsgImp_ReadStateChanged_arg> (
					clnt.MsgImp_ReadStateChanged_1, prms);
		}

		public void MessageDeleted (byte [] msgKey)
		{
			var prms = new MsgImp_MessageDeleted_arg ();
			prms.obj = new HObject (obj);
			prms.key = new SBinary(msgKey);

			var res = MakeCall<MsgImp_MessageDeleted_res, MsgImp_MessageDeleted_arg> (
					clnt.MsgImp_MessageDeleted_1, prms);
		}
	
		public void MessageMovedFrom (byte [] msgKey)
		{
			var prms = new MsgImp_MessageMovedFrom_arg ();
			prms.obj = new HObject (obj);
			prms.key = new SBinary(msgKey);
			prms.llChangeKey = new LongLong (0);
			prms.ulReadState = 0;

			var res = MakeCall<MsgImp_MessageMovedFrom_res, MsgImp_MessageMovedFrom_arg> (
					clnt.MsgImp_MessageMovedFrom_1, prms);
		}
	
		public void MessageMovedTo (byte [] msgKey, byte [] folderKey)
		{	
			var prms = new MsgImp_MessageMovedTo_arg ();
			prms.obj = new HObject (obj);
			prms.key = new SBinary(msgKey);
			prms.parentkey = new SBinary(folderKey);

			var res = MakeCall<MsgImp_MessageMovedTo_res, MsgImp_MessageMovedTo_arg> (
					clnt.MsgImp_MessageMovedTo_1, prms);
		}
	}
}