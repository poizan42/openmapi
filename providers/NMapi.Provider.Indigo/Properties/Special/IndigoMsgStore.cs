//
// openmapi.org - NMapi C# Mapi API - IndigoMsgStore.cs
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
	using System.Collections.Generic;

	using NMapi.Properties.Special;
	using NMapi.Flags;
	using NMapi.Events;
	using NMapi.Table;

	public class IndigoMsgStore : IndigoMapiProp, IMsgStore
	{
		private ObjectEventProxy events;

		public ObjectEventProxy Events {
			get {
				if (events == null)
					events = new ObjectEventProxy (this);
				return events;
			}
		}

		public IndigoMsgStore (IndigoMapiObjRef obj, IndigoMapiSession session) : base (obj, session)
		{
		}

		public int Advise (byte [] entryID, 
			NotificationEventType eventMask, IMapiAdviseSink adviseSink)
		{
			try {
				return session.Advise (this, obj, entryID, eventMask, adviseSink);
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

		public int CompareEntryIDs (byte [] entryID1, byte [] entryID2, int flags)
		{
			try {
				return session.Proxy.IMsgStore_CompareEntryIDs (obj, entryID1, entryID2, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public OpenEntryResult OpenEntry (byte [] entryID)
		{
			try {
				OpenEntryResult ret = new OpenEntryResult ();
				IndigoMapiObjRef objRef = session.Proxy.IMsgStore_OpenEntry (obj, entryID);
				ret.ObjType = objRef.Type;
				ret.Unk = session.CreateObject (this, objRef, null);
				return ret;
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public OpenEntryResult Root {
			get {
				try {
					OpenEntryResult ret = new OpenEntryResult ();
					IndigoMapiObjRef objRef = session.Proxy.IMsgStore_GetRoot (obj);
					ret.ObjType = objRef.Type;
					ret.Unk = session.CreateObject (this, objRef, null);
					return ret;
				} catch (FaultException<MapiIndigoFault> e) {
					throw new MapiException (e.Detail.Message, e.Detail.HResult);
				}
			}
		}

		public OpenEntryResult OpenEntry (
			byte [] entryID, NMapiGuid interFace, int flags)
		{
			try {
				OpenEntryResult ret = new OpenEntryResult ();
				IndigoMapiObjRef objRef = session.Proxy.IMsgStore_OpenEntry_3 (obj, entryID, interFace, flags);
				ret.ObjType = objRef.Type;
				ret.Unk = session.CreateObject (this, objRef, null);
				return ret;
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void SetReceiveFolder (
			string messageClass, byte [] entryID, int flags)
		{
			try {
				session.Proxy.IMsgStore_SetReceiveFolder (obj, messageClass, entryID, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public GetReceiveFolderResult GetReceiveFolder (string messageClass, int flags)
		{
			try {
				return session.Proxy.IMsgStore_GetReceiveFolder (obj, messageClass, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void StoreLogoff (int flags)
		{
			try {
				session.Proxy.IMsgStore_StoreLogoff (obj, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void AbortSubmit (byte [] entryID, int flags)
		{
			try {
				session.Proxy.IMsgStore_AbortSubmit (obj, entryID, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public IMapiFolder HrOpenIPMFolder (string path, int flags)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMsgStore_HrOpenIPMFolder (obj, path, flags);
				return (IMapiFolder) session.CreateObject (this, objRef, null);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
	}

}
