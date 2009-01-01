//
// openmapi.org - NMapi C# Mapi API - TeamXChangeEventSubscription.cs
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

using NMapi.Interop.MapiRPC;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.Events {

	class TeamXChangeEventSubscription : IEventSubscription
	{
		private TeamXChangeMsgStore store;
		private TeamXChangeMapiTable table;
		private IMapiAdviseSink sink;
		private HObject obj;

		internal TeamXChangeEventSubscription (IMapiTable table, 
			IMapiAdviseSink sink, HObject obj)
		{
			this.table = (TeamXChangeMapiTable) table;
			this.sink = sink;
			this.obj = obj;
		}
		
		internal TeamXChangeEventSubscription (IMsgStore store, 
			IMapiAdviseSink sink, HObject obj)
		{
			this.store = (TeamXChangeMsgStore) store;
			this.sink = sink;
			this.obj = obj;
		}

		/// <summary>
		///   
		/// </summary>
		internal void Unadvise ()
		{
			if (store != null)
				MakeUnadviseCall (store, store.obj);
			if (table != null)
				MakeUnadviseCall (table, table.obj);
		}
		
		private void MakeUnadviseCall (TeamXChangeBase baseObj, 
			long targetObj)
		{
			var arg = new MsgStore_Unadvise_arg ();
			arg.obj = HObject.FromLong (targetObj);
			arg.connObj = obj;
			TeamXChangeBase.MakeCall<MsgStore_Unadvise_res, 
				MsgStore_Unadvise_arg> (baseObj.clnt.MsgStore_Unadvise_1, arg);
		}
		
		public void OnNotify (Notification [] notifications)
		{
			sink.OnNotify (notifications);
		}
	}

}
