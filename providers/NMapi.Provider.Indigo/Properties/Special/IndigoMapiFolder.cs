//
// openmapi.org - NMapi C# Mapi API - IndigoMapiFolder.cs
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

	using NMapi.Flags;
	using NMapi.Table;
	using NMapi.Properties.Special;

	public class IndigoMapiFolder : IndigoMapiContainer, IMapiFolder
	{
		private IndigoMsgStore store;
	
		internal IndigoMapiFolder (IndigoMapiObjRef obj, IndigoMsgStore store) :
			base (obj, store.session)
		{
			this.store = store;
		}

		internal IndigoMsgStore Store {
			get { return store; }
		}

		public IMessage CreateMessage (NMapiGuid interFace, int flags)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMapiFolder_CreateMessage (obj, interFace, flags);
				return (IMessage) session.CreateObject (this, objRef, interFace);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void CopyMessages (EntryList msgList,
			NMapiGuid interFace, IMapiFolder destFolder,
			IMapiProgress progress, int flags)
		{
			try {
				session.Proxy.IMapiFolder_CopyMessages (obj, msgList, interFace, destFolder, progress, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void DeleteMessages (
			EntryList msgList, IMapiProgress progress, int flags)
		{
			try {
				session.Proxy.IMapiFolder_DeleteMessages (obj, msgList, progress, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public IMapiFolder CreateFolder (int folderType, string folderName, 
			string folderComment, NMapiGuid interFace, int flags)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMapiFolder_CreateFolder (obj, folderType, folderName, folderComment, interFace, flags);
				return (IMapiFolder) session.CreateObject (this, objRef, interFace);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void CopyFolder (byte [] entryID, NMapiGuid interFace, 
			IMapiFolder destFolder, string newFolderName,
			IMapiProgress progress, int flags)
		{
			try {
				session.Proxy.IMapiFolder_CopyFolder (obj, entryID, interFace, destFolder, newFolderName, progress, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void DeleteFolder (byte [] entryID, IMapiProgress progress, int flags)
		{
			try {
				session.Proxy.IMapiFolder_DeleteFolder (obj, entryID, progress, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void SetReadFlags (EntryList msgList, IMapiProgress progress, int flags)
		{
			try {
				session.Proxy.IMapiFolder_SetReadFlags (obj, msgList, progress, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public int GetMessageStatus (byte [] entryID, int flags)
		{
			try {
				return session.Proxy.IMapiFolder_GetMessageStatus (obj, entryID, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public int SetMessageStatus (byte [] entryID, int newStatus, int newStatusMask)
		{
			try {
				return session.Proxy.IMapiFolder_SetMessageStatus (obj, entryID, newStatus, newStatusMask);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void SaveContentsSort (SSortOrderSet sortOrder, int flags)
		{
			try {
				session.Proxy.IMapiFolder_SaveContentsSort (obj, sortOrder, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public void EmptyFolder (IMapiProgress progress, int flags)
		{
			try {
				session.Proxy.IMapiFolder_EmptyFolder (obj, progress, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		public long AssignIMAP4UID (byte [] entryID, int flags)
		{
			try {
				return session.Proxy.IMapiFolder_AssignIMAP4UID (obj, entryID, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
	
	}

}
