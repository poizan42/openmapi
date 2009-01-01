//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMapiTable.cs
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

namespace NMapi.Table {

	using System;
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop;
	using NMapi.Interop.MapiRPC;

	using NMapi.Flags;
	using NMapi.Events;
	using NMapi.Properties;
	using NMapi.Properties.Special;

	public class TeamXChangeMapiTable : TeamXChangeBase, IMapiTable
	{	
		private TeamXChangeMapiFolder folder;
		private ObjectEventSet eventSet;
	
		internal TeamXChangeMapiTable (long obj, TeamXChangeMapiFolder folder) : 
			base (obj, folder.session)
		{
			this.folder = folder;
		}

		public int Advise (byte[] ignored, NotificationEventType eventMask, 
			IMapiAdviseSink adviseSink)
		{
			return Advise (eventMask, adviseSink);
		}

		public ObjectEventSet Events {
			get {
				if (eventSet == null)
					eventSet = new ObjectEventSet (this, null);
				return eventSet;
			}
		}

		public int Advise (NotificationEventType eventMask, IMapiAdviseSink adviseSink)
		{
			return session.EventServer.Advise (this, eventMask, adviseSink);
		}

		public void Unadvise (int connection)
		{
			session.EventServer.Unadvise (connection);
		}

		public MapiError GetLastError (int hresult, int flags)
		{			
			var prms = new MAPITable_GetLastError_arg ();
			prms.obj = new HObject (obj);
			prms.hResult = hresult;
			prms.ulFlags = flags;
			
			var res = MakeCall<MAPITable_GetLastError_res, 
				MAPITable_GetLastError_arg> (clnt.MAPITable_GetLastError_1, prms);

			if ((flags & Mapi.Unicode) != 0) 
				return res.lpMapiErrorW.Value;
			else
				return res.lpMapiErrorA.Value; 
		}

		public GetStatusResult Status
		{
			get {
				var prms = new MAPITable_GetStatus_arg ();
				prms.obj = new HObject (obj);

				var res = MakeCall<MAPITable_GetStatus_res, 
					MAPITable_GetStatus_arg> (clnt.MAPITable_GetStatus_1, prms);
				
				GetStatusResult ret = new GetStatusResult();
				ret.TableStatus = res.ulTableStatus;
				ret.TableType = res.ulTableType;
				return ret;
			}
		}

		public void SetColumns (SPropTagArray propTagArray, int flags)
		{
			var prms = new MAPITable_SetColumns_arg ();
			prms.obj = new HObject (obj);
			prms.pTags = new LPSPropTagArray (propTagArray);
			prms.ulFlags = flags;
			
			var res = MakeCall<MAPITable_SetColumns_res, 
				MAPITable_SetColumns_arg> (clnt.MAPITable_SetColumns_1, prms);
		}

		public SPropTagArray QueryColumns (int flags)
		{
			var prms = new MAPITable_QueryColumns_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;

			var res = MakeCall<MAPITable_QueryColumns_res, 
				MAPITable_QueryColumns_arg> (clnt.MAPITable_QueryColumns_1, prms);
			return res.pTags.Value;
		}

		public int GetRowCount (int flags)
		{
			var prms = new MAPITable_GetRowCount_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;

			var res = MakeCall<MAPITable_GetRowCount_res, 
				MAPITable_GetRowCount_arg> (clnt.MAPITable_GetRowCount_1, prms);
			return res.ulCount;
		}

		public int SeekRow (int bkOrigin, int rowCount)
		{	
			var prms = new MAPITable_SeekRow_arg ();
			prms.obj = new HObject (obj);
			prms.bkOrigin = bkOrigin;
			prms.lRowCount = rowCount;

			var res = MakeCall<MAPITable_SeekRow_res, 
				MAPITable_SeekRow_arg> (clnt.MAPITable_SeekRow_1, prms);
			return res.lRowsSought;
		}

		public void SeekRowApprox (int numerator, int denominator)
		{
			var prms = new MAPITable_SeekRowApprox_arg ();
			prms.obj = new HObject (obj);
			prms.ulNumerator = numerator;
			prms.ulDenominator = denominator;
			
			var res = MakeCall<MAPITable_SeekRowApprox_res, 
				MAPITable_SeekRowApprox_arg> (clnt.MAPITable_SeekRowApprox_1, prms);
		}

		public QueryPositionResult QueryPosition ()
		{
			var prms = new MAPITable_QueryPosition_arg ();
			prms.obj = new HObject (obj);

			var res = MakeCall<MAPITable_QueryPosition_res, 
				MAPITable_QueryPosition_arg> (clnt.MAPITable_QueryPosition_1, prms);

			QueryPositionResult ret = new QueryPositionResult ();
			ret.Row = res.ulRow;
			ret.Numerator = res.ulNumerator;
			ret.Denominator = res.ulDenominator;
			return ret;
		}

		public void FindRow (SRestriction restriction, int origin, int flags)
		{
			var prms = new MAPITable_FindRow_arg ();
			prms.obj = new HObject (obj);
			prms.lpRestriction = new LPSRestriction (restriction);
			prms.bkOrigin = origin;
			prms.ulFlags = flags;

			var res = MakeCall<MAPITable_FindRow_res, 
				MAPITable_FindRow_arg> (clnt.MAPITable_FindRow_1, prms);
		}

		public void Restrict (SRestriction restriction, int flags)
		{
			var prms = new MAPITable_Restrict_arg ();
			prms.obj = new HObject (obj);
			prms.lpRestriction = new LPSRestriction (restriction);
			prms.ulFlags = flags;

			var res = MakeCall<MAPITable_Restrict_res, 
				MAPITable_Restrict_arg> (clnt.MAPITable_Restrict_1, prms);
		}

		public int CreateBookmark ()
		{
			var prms = new MAPITable_CreateBookmark_arg ();
			prms.obj = new HObject (obj);

			var res = MakeCall<MAPITable_CreateBookmark_res, 
				MAPITable_CreateBookmark_arg> (clnt.MAPITable_CreateBookmark_1, prms);
			return res.bkPosition;
		}

		public void FreeBookmark (int position)
		{
			var prms = new MAPITable_FreeBookmark_arg ();
			prms.obj = new HObject (obj);
			prms.bkPosition = position;
			
			var res = MakeCall<MAPITable_FreeBookmark_res, 
				MAPITable_FreeBookmark_arg> (clnt.MAPITable_FreeBookmark_1, prms);
		}

		public void SortTable (SSortOrderSet sortCriteria, int flags)
		{
			var prms = new MAPITable_SortTable_arg ();
			prms.obj = new HObject (obj);
			prms.lpSortCriteria = new LPSSortOrderSet (sortCriteria);
			prms.ulFlags = flags;
			
			var res = MakeCall<MAPITable_SortTable_res, 
				MAPITable_SortTable_arg> (clnt.MAPITable_SortTable_1, prms);
		}

		public SSortOrderSet QuerySortOrder ()
		{
			var prms = new MAPITable_QuerySortOrder_arg ();
			prms.obj = new HObject (obj);
			
			var res = MakeCall<MAPITable_QuerySortOrder_res, 
				MAPITable_QuerySortOrder_arg> (clnt.MAPITable_QuerySortOrder_1, prms);
			return res.lpSortCriteria.Value;
		}

		public SRowSet QueryRows (int rowCount, int flags)
		{
			var prms = new MAPITable_QueryRows_arg ();
			prms.obj = new HObject (obj);
			prms.lRowCount = rowCount;
			prms.ulFlags = flags;
				
			var res = MakeCall<MAPITable_QueryRows_res, 
				MAPITable_QueryRows_arg> (clnt.MAPITable_QueryRows_1, prms);
			return res.lpRows.Value;
		}

		public void Abort ()
		{
			var prms = new MAPITable_Abort_arg ();
			prms.obj = new HObject (obj);
				
			var res = MakeCall<MAPITable_Abort_res, 
				MAPITable_Abort_arg> (clnt.MAPITable_Abort_1, prms);
		}

		public ExpandRowResult ExpandRow (byte [] instanceKey, int rowCount, int flags)
		{
			var prms = new MAPITable_ExpandRow_arg ();
			prms.obj = new HObject (obj);
			prms.instkey = instanceKey;
			prms.ulRowCount = rowCount;
			prms.ulFlags = flags;
			
			var res = MakeCall<MAPITable_ExpandRow_res, 
				MAPITable_ExpandRow_arg> (clnt.MAPITable_ExpandRow_1, prms);

			ExpandRowResult ret = new ExpandRowResult ();
			ret.Rows = res.lpRows.Value;
			ret.MoreRows = res.ulMoreRows;
			return ret;
		}

		public int CollapseRow (byte [] instanceKey, int flags)
		{
			var prms = new MAPITable_CollapseRow_arg ();
			prms.obj = new HObject (obj);
			prms.instkey = instanceKey;
			prms.ulFlags = flags;
			
			var res = MakeCall<MAPITable_CollapseRow_res, 
				MAPITable_CollapseRow_arg> (clnt.MAPITable_CollapseRow_1, prms);
			return res.ulRowCount;
		}

		public int WaitForCompletion (int flags, int timeout)
		{
			var prms = new MAPITable_WaitForCompletion_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;
			prms.ulTimeout = timeout;
			
			var res = MakeCall<MAPITable_WaitForCompletion_res, 
				MAPITable_WaitForCompletion_arg> (clnt.MAPITable_WaitForCompletion_1, prms);
			return res.ulTableStatus;
		}

		public byte [] GetCollapseState(int flags, byte [] instanceKey)
		{
			var prms = new MAPITable_GetCollapseState_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;
			prms.instkey = instanceKey;
			
			var res = MakeCall<MAPITable_GetCollapseState_res, 
				MAPITable_GetCollapseState_arg> (clnt.MAPITable_GetCollapseState_1, prms);
			return res.state;
		}

		public int SetCollapseState (int flags, byte [] collapseState)
		{
			var prms = new MAPITable_SetCollapseState_arg ();
			prms.obj = new HObject (obj);
			prms.ulFlags = flags;
			prms.state = collapseState;
			
			var res = MakeCall<MAPITable_SetCollapseState_res, 
				MAPITable_SetCollapseState_arg> (clnt.MAPITable_SetCollapseState_1, prms);
			return res.bkLocation;
		}
	
	}

}
