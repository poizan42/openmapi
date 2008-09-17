//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMapiContainer.cs
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
	using RemoteTea.OncRpc;
	using NMapi.Flags;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;
	using NMapi.Table;

	public abstract class TeamXChangeMapiContainer : TeamXChangeMapiProp, IMapiContainer
	{
		internal TeamXChangeMapiContainer (long obj, TeamXChangeSession session) :
			base (obj, session)
		{
		}

		public IMapiTable GetContentsTable (int flags)
		{
			MAPIContainer_GetContentsTable_arg arg = new MAPIContainer_GetContentsTable_arg();
			MAPIContainer_GetContentsTable_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIContainer_GetContentsTable_1(arg);
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
			return (IMapiTable) session.CreateObject(this, res.obj.Value.Value, null);
		}

		public IMapiTableReader GetHierarchyTable(int flags)
		{
			var arg = new MAPIContainer_GetHierarchyTable_arg ();
			MAPIContainer_GetHierarchyTable_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIContainer_GetHierarchyTable_1(arg);
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
			return (IMapiTableReader)session.CreateObject(this, res.obj.Value.Value, null);
		}

		public OpenEntryResult OpenEntry (byte [] entryID)
		{
			return OpenEntry (entryID, null, 0);
		}

		public OpenEntryResult OpenEntry (
			byte [] entryID, NMapiGuid interFace,int flags)
		{
			var arg = new MAPIContainer_OpenEntry_arg ();
			MAPIContainer_OpenEntry_res res;
			OpenEntryResult ret = new OpenEntryResult();
		
			arg.obj = new HObject (new LongLong (obj));
			arg.eid = new SBinary (entryID);
			arg.lpInterface = new LPGuid (interFace);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIContainer_OpenEntry_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed  (res.hr))
				throw new MapiException(res.hr);

			ret.ObjType = session.GetObjType(res.obj.Value.Value);
			ret.Unk = session.CreateObject(this, res.obj.Value.Value, interFace);
			return ret;
		}

		public void SetSearchCriteria (SRestriction restriction,
			EntryList containerList, int searchFlags)
		{
			var arg = new MAPIContainer_SetSearchCriteria_arg();
			MAPIContainer_SetSearchCriteria_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpRestriction = new LPSRestriction (restriction);
			arg.lpContainerList = new LPEntryList (containerList);
			arg.ulSearchFlags = searchFlags;
			try {
				res = clnt.MAPIContainer_SetSearchCriteria_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed  (res.hr))
				throw new MapiException (res.hr);
		}

		public GetSearchCriteriaResult GetSearchCriteria (int flags)
		{
			var arg = new MAPIContainer_GetSearchCriteria_arg ();
			MAPIContainer_GetSearchCriteria_res res;
			GetSearchCriteriaResult ret = new GetSearchCriteriaResult();
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			try {
				res = clnt.MAPIContainer_GetSearchCriteria_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			ret.Restriction = res.lpRestriction.Value;
			ret.ContainerList = res.lpContainerList.value;
			ret.SearchState = res.ulSearchState;
			return ret;
		}
	}
}
