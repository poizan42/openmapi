//
// openmapi.org - NMapi C# Mapi API - IndigoMapiTable.cs
//
// Copyright 2008 Topalis AG
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

namespace NMapi.Provider.Indigo.Table {

	using System;
	using System.IO;
	using System.ServiceModel;

	using NMapi.Flags;
	using NMapi.Events;
	using NMapi.Table;
	using NMapi.Properties;
	using NMapi.Provider.Indigo.Properties.Special;

	public class IndigoMapiTable : IndigoBase, IMapiTable
	{	
		private ObjectEventSet eventSet;
	
		internal IndigoMapiTable (IndigoMapiObjRef obj, IndigoMapiFolder folder) : 
			base (obj, folder.session)
		{
		}

		public int Advise (byte[] ignored, NotificationEventType eventMask, IMapiAdviseSink adviseSink)
		{
			try {
				return session.Advise (this, obj, ignored, eventMask, adviseSink);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
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
			try {
				return session.Advise (this, obj, null, eventMask, adviseSink);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void Unadvise (int connection)
		{
			try {
				session.Unadvise (obj, connection);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public MapiError GetLastError (int hresult, int flags)
		{
			try {
				return session.Proxy.IMapiTable_GetLastError (obj, hresult, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public GetStatusResult Status
		{
			get {
				try {
					return session.Proxy.IMapiTable_GetStatus (obj);
				} catch (FaultException<MapiIndigoFault> e) {
					throw new MapiException (e.Detail.Message, e.Detail.HResult);
				}
			}
		}

		public void SetColumns (SPropTagArray propTagArray, int flags)
		{
			try {
				session.Proxy.IMapiTable_SetColumns (obj, propTagArray, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public SPropTagArray QueryColumns (int flags)
		{
			try {
				return session.Proxy.IMapiTable_QueryColumns (obj, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public int GetRowCount (int flags)
		{
			try {
				return session.Proxy.IMapiTable_GetRowCount (obj, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public int SeekRow (int bkOrigin, int rowCount)
		{
			try {
				return session.Proxy.IMapiTable_SeekRow (obj, bkOrigin, rowCount);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void SeekRowApprox (int numerator, int denominator)
		{
			try {
				session.Proxy.IMapiTable_SeekRowApprox (obj, numerator, denominator);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public QueryPositionResult QueryPosition ()
		{
			try {
				return session.Proxy.IMapiTable_QueryPosition (obj);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void FindRow (SRestriction restriction, int origin, int flags)
		{
			try {
				session.Proxy.IMapiTable_FindRow (obj, restriction, origin, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void Restrict (SRestriction restriction, int flags)
		{
			try {
				session.Proxy.IMapiTable_Restrict (obj, restriction, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public int CreateBookmark ()
		{
			try {
				return session.Proxy.IMapiTable_CreateBookmark (obj);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void FreeBookmark (int position)
		{
			try {
				session.Proxy.IMapiTable_FreeBookmark (obj, position);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void SortTable (SSortOrderSet sortCriteria, int flags)
		{
			try {
				session.Proxy.IMapiTable_SortTable (obj, sortCriteria, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public SSortOrderSet QuerySortOrder ()
		{
			try {
				return session.Proxy.IMapiTable_QuerySortOrder (obj);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public SRowSet QueryRows (int rowCount, int flags)
		{
			try {
				return session.Proxy.IMapiTable_QueryRows (obj, rowCount, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void Abort ()
		{
			try {
				session.Proxy.IMapiTable_Abort (obj);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public ExpandRowResult ExpandRow (byte [] instanceKey, int rowCount, int flags)
		{
			try {
				return session.Proxy.IMapiTable_ExpandRow (obj, instanceKey, rowCount, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public int CollapseRow (byte [] instanceKey, int flags)
		{
			try {
				return session.Proxy.IMapiTable_CollapseRow (obj, instanceKey, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public int WaitForCompletion (int flags, int timeout)
		{
			try {
				return session.Proxy.IMapiTable_WaitForCompletion (obj, flags, timeout);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public byte [] GetCollapseState (int flags, byte [] instanceKey)
		{
			try {
				return session.Proxy.IMapiTable_GetCollapseState (obj, flags, instanceKey);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public int SetCollapseState (int flags, byte [] collapseState)
		{
			try {
				return session.Proxy.IMapiTable_SetCollapseState (obj, flags, collapseState);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
	
	}

}
