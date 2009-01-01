//
// openmapi.org - NMapi C# Mapi API - IMapiOverIndigo.cs
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

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.ServiceModel;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi {

	[ServiceContract (
		Name              = "OpenMapiProtocol", 
		Namespace         = "http://www.openmapi.org/indigo/0.1/", 
		ConfigurationName = "NMapiServer",
		SessionMode       = SessionMode.Required, 
		CallbackContract  = typeof (IMapiIndigoCallback))]
	public interface IMapiOverIndigo
	{
		//
		// MAPI OVER INDIGO - INTERNAL
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef OpenSession (string connectionString);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void CloseSession ();

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		string Handshake ();


		//
		// IBase interface
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IBase_Dispose (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IBase_Close (IndigoMapiObjRef obj);

		//
		// IMapiProp interface
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		MapiError IMapiProp_GetLastError (IndigoMapiObjRef obj, int hresult, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiProp_SaveChanges (IndigoMapiObjRef obj, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropValue [] IMapiProp_GetProps (IndigoMapiObjRef obj, SPropTagArray propTagArray, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropTagArray IMapiProp_GetPropList (IndigoMapiObjRef obj, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiProp_OpenProperty_4 (IndigoMapiObjRef obj, int propTag, NMapiGuid interFace, int interfaceOptions, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiProp_OpenProperty (IndigoMapiObjRef obj, int propTag);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropProblemArray IMapiProp_SetProps (IndigoMapiObjRef obj, SPropValue[] propArray);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropProblemArray IMapiProp_DeleteProps (IndigoMapiObjRef obj, SPropTagArray propTagArray);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		GetNamesFromIDsResult IMapiProp_GetNamesFromIDs (IndigoMapiObjRef obj, SPropTagArray propTags, NMapiGuid propSetGuid, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropTagArray IMapiProp_GetIDsFromNames (IndigoMapiObjRef obj, MapiNameId [] propNames, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropValue IMapiProp_HrGetOneProp (IndigoMapiObjRef obj, int tag);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropValue IMapiProp_HrGetOnePropNull (IndigoMapiObjRef obj, int tag);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiProp_HrSetOneProp (IndigoMapiObjRef obj, SPropValue tag);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiProp_HrDeleteOneProp (IndigoMapiObjRef obj, int propTag);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropValue IMapiProp_HrGetNamedProp (IndigoMapiObjRef obj, MapiNameId mnid);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropValue IMapiProp_HrGetNamedProp_2_str (IndigoMapiObjRef obj, NMapiGuid guid, string name);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropValue IMapiProp_HrGetNamedProp_2_int (IndigoMapiObjRef obj, NMapiGuid guid, int id);


		//
		// IMapiContainer interface
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiContainer_GetContentsTable (IndigoMapiObjRef obj, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiContainer_GetHierarchyTable (IndigoMapiObjRef obj, int flags);

		// Returns a special type identifier + a handle
		// Note: The identifier is actually an int and will be converted later.
		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiContainer_OpenEntry (IndigoMapiObjRef obj, byte [] entryID);

		// Returns a special type identifier + a handle
		// Note: The identifier is actually an int and will be converted later.
		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiContainer_OpenEntry_3 (IndigoMapiObjRef obj, byte [] entryID, NMapiGuid interFace, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiContainer_SetSearchCriteria (IndigoMapiObjRef obj, SRestriction restriction, EntryList containerList, int searchFlags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		GetSearchCriteriaResult IMapiContainer_GetSearchCriteria (IndigoMapiObjRef obj, int flags);


		//
		// IMapiFolder interface
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiFolder_CreateMessage (IndigoMapiObjRef obj, NMapiGuid interFace, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiFolder_CopyMessages (IndigoMapiObjRef obj, EntryList msgList, NMapiGuid interFace, IMapiFolder destFolder, IMapiProgress progress, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiFolder_DeleteMessages (IndigoMapiObjRef obj, EntryList msgList, IMapiProgress progress, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiFolder_CreateFolder (IndigoMapiObjRef obj, Folder folderType, string folderName, string folderComment, NMapiGuid interFace, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiFolder_CopyFolder (IndigoMapiObjRef obj, byte [] entryID, NMapiGuid interFace, IMapiFolder destFolder, string newFolderName, IMapiProgress progress, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiFolder_DeleteFolder (IndigoMapiObjRef obj, byte [] entryID, IMapiProgress progress, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiFolder_SetReadFlags (IndigoMapiObjRef obj, EntryList msgList, IMapiProgress progress, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiFolder_GetMessageStatus (IndigoMapiObjRef obj, byte [] entryID, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiFolder_SetMessageStatus (IndigoMapiObjRef obj, byte [] entryID, int newStatus, int newStatusMask);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiFolder_SaveContentsSort (IndigoMapiObjRef obj, SSortOrderSet sortOrder, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiFolder_EmptyFolder (IndigoMapiObjRef obj, IMapiProgress progress, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		long IMapiFolder_AssignIMAP4UID (IndigoMapiObjRef obj, byte [] entryID, int flags);


		//
		// IMapiMessage interface
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMessage_GetAttachmentTable (IndigoMapiObjRef obj, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMessage_OpenAttach (IndigoMapiObjRef obj, int attachmentNum, NMapiGuid interFace, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMessage_CreateAttach (IndigoMapiObjRef obj, NMapiGuid interFace, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMessage_DeleteAttach (IndigoMapiObjRef obj, int attachmentNum, IMapiProgress progress, int flags); // TODO: MapiProgress

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMessage_GetRecipientTable (IndigoMapiObjRef obj, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMessage_ModifyRecipients (IndigoMapiObjRef obj, int flags, AdrList mods);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void  IMessage_SubmitMessage (IndigoMapiObjRef obj, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void  IMessage_SetReadFlag (IndigoMapiObjRef obj, int flags);

		//
		// IMsgStore interface
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMsgStore_Advise (IndigoMapiObjRef obj, byte [] entryID, NotificationEventType eventMask);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMsgStore_Unadvise (IndigoMapiObjRef obj, int connection);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMsgStore_CompareEntryIDs (IndigoMapiObjRef obj, byte [] entryID1, byte [] entryID2, int flags);

		// Returns a special type identifier + a handle
		// Note: The identifier is actually an int and will be converted later.
		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMsgStore_OpenEntry (IndigoMapiObjRef obj, byte [] entryID);

		// Returns a special type identifier + a handle
		// Note: The identifier is actually an int and will be converted later.
		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMsgStore_GetRoot (IndigoMapiObjRef obj);

		// Returns a special type identifier + a handle
		// Note: The identifier is actually an int and will be converted later.
		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMsgStore_OpenEntry_3 (IndigoMapiObjRef obj, byte [] entryID, NMapiGuid interFace, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMsgStore_SetReceiveFolder (IndigoMapiObjRef obj, string messageClass, byte [] entryID, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		GetReceiveFolderResult IMsgStore_GetReceiveFolder (IndigoMapiObjRef obj, string messageClass, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMsgStore_StoreLogoff (IndigoMapiObjRef obj, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMsgStore_AbortSubmit (IndigoMapiObjRef obj, byte [] entryID, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMsgStore_HrOpenIPMFolder (IndigoMapiObjRef obj, string path, int flags);


		//
		// IMapiSession interface
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiSession_GetPrivateStore (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		IndigoMapiObjRef IMapiSession_GetPublicStore (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiSession_Logon (IndigoMapiObjRef obj, string host, string user, string password);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		byte[] IMapiSession_GetIdentity (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		string IMapiSession_GetConfig (IndigoMapiObjRef obj, string category, string id, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		string IMapiSession_GetConfigNull (IndigoMapiObjRef obj, string category, string id, int flags);


		//
		// IMapiTableReader interface
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropTagArray IMapiTableReader_GetTags (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SRowSet IMapiTableReader_GetRows (IndigoMapiObjRef obj, int cRows);


		//
		// IMapiTable interface
		//

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiTable_Advise_2 (IndigoMapiObjRef obj, byte[] ignored, NotificationEventType eventMask); // TODO

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiTable_Advise (IndigoMapiObjRef obj, NotificationEventType eventMask); // TODO

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiTable_Unadvise (IndigoMapiObjRef obj, int connection); // TODO

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		MapiError IMapiTable_GetLastError (IndigoMapiObjRef obj, int hresult, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		GetStatusResult IMapiTable_GetStatus (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiTable_SetColumns (IndigoMapiObjRef obj, SPropTagArray propTagArray, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SPropTagArray IMapiTable_QueryColumns (IndigoMapiObjRef obj, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiTable_GetRowCount (IndigoMapiObjRef obj, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiTable_SeekRow (IndigoMapiObjRef obj, int bkOrigin, int rowCount);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiTable_SeekRowApprox (IndigoMapiObjRef obj, int numerator, int denominator);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		QueryPositionResult IMapiTable_QueryPosition (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiTable_FindRow (IndigoMapiObjRef obj, SRestriction restriction, int origin, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiTable_Restrict (IndigoMapiObjRef obj, SRestriction restriction, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiTable_CreateBookmark (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiTable_FreeBookmark (IndigoMapiObjRef obj, int position);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiTable_SortTable (IndigoMapiObjRef obj, SSortOrderSet sortCriteria, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SSortOrderSet IMapiTable_QuerySortOrder (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		SRowSet IMapiTable_QueryRows (IndigoMapiObjRef obj, int rowCount, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		void IMapiTable_Abort (IndigoMapiObjRef obj);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		ExpandRowResult IMapiTable_ExpandRow (IndigoMapiObjRef obj, byte [] instanceKey, int rowCount, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiTable_CollapseRow (IndigoMapiObjRef obj, byte [] instanceKey, int flags);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiTable_WaitForCompletion (IndigoMapiObjRef obj, int flags, int timeout);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		byte[] IMapiTable_GetCollapseState (IndigoMapiObjRef obj, int flags, byte [] instanceKey);

		[OperationContract]
		[FaultContract (typeof (MapiIndigoFault))]
		int IMapiTable_SetCollapseState (IndigoMapiObjRef obj, int flags, byte [] collapseState);


	}

}
