//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMapiTable.cs
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

namespace NMapi.Table {

	using System;
	using System.IO;
	using RemoteTea.OncRpc;
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

		public int Advise (byte[] ignored, NotificationEventType eventMask, IMapiAdviseSink adviseSink)
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
			MAPITable_GetEventKey_arg arg = new MAPITable_GetEventKey_arg ();
			MAPITable_GetEventKey_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			try {
				res = clnt.MAPITable_GetEventKey_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException (e);
			}
			catch (OncRpcException e) {
				throw new MapiException (e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return session.Advise (adviseSink, folder.Store.OrigEID, 
				res.key.lpb, eventMask);
		}

		public void Unadvise (int connection)
		{
			session.Unadvise (connection);
		}

		public MapiError GetLastError (int hresult, int flags)
		{
			MAPITable_GetLastError_arg arg = new MAPITable_GetLastError_arg();
			MAPITable_GetLastError_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.hResult = hresult;
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_GetLastError_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			if ((flags & Mapi.Unicode) != 0) 
				return res.lpMapiErrorW.Value;
			else
				return res.lpMapiErrorA.Value; 
		}

		public GetStatusResult Status
		{
			get {
				MAPITable_GetStatus_arg arg = new MAPITable_GetStatus_arg();
				MAPITable_GetStatus_res res;
				GetStatusResult ret = new GetStatusResult();
			
				arg.obj = new HObject (new LongLong (obj));
				try {
					res = clnt.MAPITable_GetStatus_1(arg);
				}
				catch (IOException e) {
					throw new MapiException(e);
				}
				catch (OncRpcException e) {
					throw new MapiException(e);
				}
				if (Error.CallHasFailed (res.hr))
					throw new MapiException(res.hr);
				ret.TableStatus = res.ulTableStatus;
				ret.TableType = res.ulTableType;
				return ret;
			}
		}

		public void SetColumns (SPropTagArray propTagArray, int flags)
		{
			MAPITable_SetColumns_arg arg = new MAPITable_SetColumns_arg();
			MAPITable_SetColumns_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.pTags = new LPSPropTagArray (propTagArray);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_SetColumns_1(arg);
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

		public SPropTagArray QueryColumns (int flags)
		{
			MAPITable_QueryColumns_arg arg = new MAPITable_QueryColumns_arg();
			MAPITable_QueryColumns_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_QueryColumns_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return res.pTags.Value;
		}

		public int GetRowCount (int flags)
		{
			MAPITable_GetRowCount_arg arg = new MAPITable_GetRowCount_arg();
			MAPITable_GetRowCount_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_GetRowCount_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return res.ulCount;
		}

		public int SeekRow (int bkOrigin, int rowCount)
		{	
			MAPITable_SeekRow_arg arg = new MAPITable_SeekRow_arg();
			MAPITable_SeekRow_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.bkOrigin = bkOrigin;
			arg.lRowCount = rowCount;
			try {
				res = clnt.MAPITable_SeekRow_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return res.lRowsSought;
		}

		public void SeekRowApprox (int numerator, int denominator)
		{
			MAPITable_SeekRowApprox_arg arg = new MAPITable_SeekRowApprox_arg();
			MAPITable_SeekRowApprox_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulNumerator = numerator;
			arg.ulDenominator = denominator;
			try {
				res = clnt.MAPITable_SeekRowApprox_1(arg);
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

		public QueryPositionResult QueryPosition ()
		{
			MAPITable_QueryPosition_arg arg = new MAPITable_QueryPosition_arg();
			MAPITable_QueryPosition_res res;
			QueryPositionResult ret = new QueryPositionResult();
		
			arg.obj = new HObject (new LongLong (obj));
			try {
				res = clnt.MAPITable_QueryPosition_1 (arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			ret.Row = res.ulRow;
			ret.Numerator = res.ulNumerator;
			ret.Denominator = res.ulDenominator;
			return ret;
		}

		public void FindRow (SRestriction restriction, int origin, int flags)
		{
			MAPITable_FindRow_arg arg = new MAPITable_FindRow_arg();
			MAPITable_FindRow_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpRestriction = new LPSRestriction (restriction);
			arg.bkOrigin = origin;
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_FindRow_1 (arg);
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

		public void Restrict (SRestriction restriction, int flags)
		{
			MAPITable_Restrict_arg arg = new MAPITable_Restrict_arg();
			MAPITable_Restrict_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpRestriction = new LPSRestriction (restriction);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_Restrict_1(arg);
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

		public int CreateBookmark ()
		{
			MAPITable_CreateBookmark_arg arg = new MAPITable_CreateBookmark_arg();
			MAPITable_CreateBookmark_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			try {
				res = clnt.MAPITable_CreateBookmark_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.bkPosition;
		}

		public void FreeBookmark (int position)
		{
			MAPITable_FreeBookmark_arg arg = new MAPITable_FreeBookmark_arg();
			MAPITable_FreeBookmark_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.bkPosition = position;
			try {
				res = clnt.MAPITable_FreeBookmark_1(arg);
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

		public void SortTable (SSortOrderSet sortCriteria, int flags)
		{
			MAPITable_SortTable_arg arg = new MAPITable_SortTable_arg();
			MAPITable_SortTable_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lpSortCriteria = new LPSSortOrderSet (sortCriteria);
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_SortTable_1(arg);
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

		public SSortOrderSet QuerySortOrder ()
		{
			MAPITable_QuerySortOrder_arg arg = new MAPITable_QuerySortOrder_arg();
			MAPITable_QuerySortOrder_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			try {
				res = clnt.MAPITable_QuerySortOrder_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.lpSortCriteria.Value;
		}

		public SRowSet QueryRows (int rowCount, int flags)
		{
			MAPITable_QueryRows_arg arg = new MAPITable_QueryRows_arg();
			MAPITable_QueryRows_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.lRowCount = rowCount;
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_QueryRows_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.lpRows.Value;
		}

		public void Abort ()
		{
			MAPITable_Abort_arg arg = new MAPITable_Abort_arg();
			MAPITable_Abort_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			try {
				res = clnt.MAPITable_Abort_1(arg);
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

		public ExpandRowResult ExpandRow (byte [] instanceKey, int rowCount, int flags)
		{
			MAPITable_ExpandRow_arg arg = new MAPITable_ExpandRow_arg();
			MAPITable_ExpandRow_res res;
			ExpandRowResult ret = new ExpandRowResult();
		
			arg.obj = new HObject (new LongLong (obj));
			arg.instkey = instanceKey;
			arg.ulRowCount = rowCount;
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_ExpandRow_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			ret.Rows = res.lpRows.Value;
			ret.MoreRows = res.ulMoreRows;
			return ret;
		}

		public int CollapseRow (byte [] instanceKey, int flags)
		{
			MAPITable_CollapseRow_arg arg = new MAPITable_CollapseRow_arg();
			MAPITable_CollapseRow_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.instkey = instanceKey;
			arg.ulFlags = flags;
			try {
				res = clnt.MAPITable_CollapseRow_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException (res.hr);
			return res.ulRowCount;
		}

		public int WaitForCompletion (int flags, int timeout)
		{
			MAPITable_WaitForCompletion_arg arg = new MAPITable_WaitForCompletion_arg();
			MAPITable_WaitForCompletion_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			arg.ulTimeout = timeout;
			try {
				res = clnt.MAPITable_WaitForCompletion_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.ulTableStatus;
		}

		public byte [] GetCollapseState(int flags, byte [] instanceKey)
		{
			MAPITable_GetCollapseState_arg arg = new MAPITable_GetCollapseState_arg();
			MAPITable_GetCollapseState_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			arg.instkey = instanceKey;
			try {
				res = clnt.MAPITable_GetCollapseState_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed  (res.hr))
				throw new MapiException(res.hr);
			return res.state;
		}

		public int SetCollapseState (int flags, byte [] collapseState)
		{
			MAPITable_SetCollapseState_arg arg = new MAPITable_SetCollapseState_arg();
			MAPITable_SetCollapseState_res res;
		
			arg.obj = new HObject (new LongLong (obj));
			arg.ulFlags = flags;
			arg.state = collapseState;
			try {
				res = clnt.MAPITable_SetCollapseState_1(arg);
			}
			catch (IOException e) {
				throw new MapiException(e);
			}
			catch (OncRpcException e) {
				throw new MapiException(e);
			}
			if (Error.CallHasFailed (res.hr))
				throw new MapiException(res.hr);
			return res.bkLocation;
		}
	
	}

}
