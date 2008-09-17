//
// openmapi.org - NMapi C# Mapi API - IndigoMapiContainer.cs
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

namespace NMapi.Provider.Indigo.Properties.Special {

	using System;
	using System.IO;
	using System.ServiceModel;

	using NMapi.Flags;
	using NMapi.Properties.Special;
	using NMapi.Table;

	public abstract class IndigoMapiContainer : IndigoMapiProp, IMapiContainer
	{	
		internal IndigoMapiContainer (IndigoMapiObjRef obj, IndigoMapiSession session) :
			base (obj, session)
		{
		}

		public IMapiTable GetContentsTable (int flags)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMapiContainer_GetContentsTable (obj, flags);
				return (IMapiTable) session.CreateObject (this, objRef, null);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public IMapiTableReader GetHierarchyTable (int flags)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMapiContainer_GetHierarchyTable (obj, flags);
				return (IMapiTableReader) session.CreateObject (this, objRef, null);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public OpenEntryResult OpenEntry (byte [] entryID)
		{
			try {
				OpenEntryResult ret = new OpenEntryResult ();
				IndigoMapiObjRef objRef = session.Proxy.IMapiContainer_OpenEntry (obj, entryID);
				ret.ObjType = objRef.Type;
				ret.Unk = session.CreateObject (this, objRef, null);
				return ret;
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}


		public OpenEntryResult OpenEntry (
			byte [] entryID, NMapiGuid interFace, int flags)
		{
			try {
				OpenEntryResult ret = new OpenEntryResult ();
				IndigoMapiObjRef objRef = session.Proxy.IMapiContainer_OpenEntry_3 (obj, entryID, interFace, flags);
				ret.ObjType = objRef.Type;
				ret.Unk = session.CreateObject (this, objRef, null);
				return ret;
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void SetSearchCriteria (SRestriction restriction,
			EntryList containerList, int searchFlags)
		{
			try {
				session.Proxy.IMapiContainer_SetSearchCriteria (obj, restriction, containerList, searchFlags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}


		public GetSearchCriteriaResult GetSearchCriteria (int flags)
		{
			try {
				return session.Proxy.IMapiContainer_GetSearchCriteria (obj, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
	}
}
