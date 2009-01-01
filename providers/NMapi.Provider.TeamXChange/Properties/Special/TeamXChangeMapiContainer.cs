//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMapiContainer.cs
//
// Copyright 2008 VipCOM GmbH, Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
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
	using NMapi.Flags;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;
	using NMapi.Table;

	public abstract class TeamXChangeMapiContainer : TeamXChangeMapiProp, IMapiContainer
	{
		internal TeamXChangeMapiContainer (long obj, 
			TeamXChangeSession session) : base (obj, session)
		{
		}

		public IMapiTable GetContentsTable (int flags)
		{
			var prms = new MAPIContainer_GetContentsTable_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;

			var res = MakeCall<MAPIContainer_GetContentsTable_res, 
				MAPIContainer_GetContentsTable_arg> (
					clnt.MAPIContainer_GetContentsTable_1, prms);

			return (IMapiTable) session.CreateObject (this, 
				res.obj.Value.Value, res.ulObjType, null);
		}

		public IMapiTableReader GetHierarchyTable(int flags)
		{
			var prms = new MAPIContainer_GetHierarchyTable_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;

			var res = MakeCall<MAPIContainer_GetHierarchyTable_res, 
				MAPIContainer_GetHierarchyTable_arg> (
					clnt.MAPIContainer_GetHierarchyTable_1, prms);

			return (IMapiTableReader)session.CreateObject (this, 
				res.obj.Value.Value, res.ulObjType, null);
		}

		public OpenEntryResult OpenEntry (byte [] entryID)
		{
			return OpenEntry (entryID, null, 0);
		}

		public OpenEntryResult OpenEntry (
			byte [] entryID, NMapiGuid interFace,int flags)
		{
			var prms = new MAPIContainer_OpenEntry_arg ();
			prms.obj = new HObject (obj);
			prms.eid = new SBinary (entryID);
			prms.lpInterface = new LPGuid (interFace);
			prms.ulFlags = flags;

			var res = MakeCall<MAPIContainer_OpenEntry_res, 
				MAPIContainer_OpenEntry_arg> (
					clnt.MAPIContainer_OpenEntry_1, prms);

			OpenEntryResult ret = new OpenEntryResult();
			ret.ObjType = res.ulObjType;
			ret.Unk = session.CreateObject (this, 
				res.obj.Value.Value, res.ulObjType, interFace);
			return ret;
		}

		public void SetSearchCriteria (SRestriction restriction,
			EntryList containerList, int searchFlags)
		{
			var prms = new MAPIContainer_SetSearchCriteria_arg ();
			prms.obj = new HObject (obj);
			prms.lpRestriction = new LPSRestriction (restriction);
			prms.lpContainerList = new LPEntryList (containerList);
			prms.ulSearchFlags = searchFlags;

			var res = MakeCall<MAPIContainer_SetSearchCriteria_res, 
				MAPIContainer_SetSearchCriteria_arg> (
					clnt.MAPIContainer_SetSearchCriteria_1, prms);
		}

		public GetSearchCriteriaResult GetSearchCriteria (int flags)
		{
			var prms = new MAPIContainer_GetSearchCriteria_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;

			var res = MakeCall<MAPIContainer_GetSearchCriteria_res, 
				MAPIContainer_GetSearchCriteria_arg> (
					clnt.MAPIContainer_GetSearchCriteria_1, prms);

			GetSearchCriteriaResult ret = new GetSearchCriteriaResult();
			ret.Restriction = res.lpRestriction.Value;
			ret.ContainerList = res.lpContainerList.Value;
			ret.SearchState = res.ulSearchState;
			return ret;
		}
	}
}
