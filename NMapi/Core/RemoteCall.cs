//
// openmapi.org - NMapi C# Mapi API - RemoteCall.cs
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

	public enum RemoteCall
	{
		OpenSession,
		CloseSession,
		Handshake,
		IBase_Dispose,
		IBase_Close,
		IMapiProp_GetLastError,
		IMapiProp_SaveChanges,
		IMapiProp_GetProps,
		IMapiProp_GetPropList,
		IMapiProp_OpenProperty_4,
		IMapiProp_OpenProperty,
		IMapiProp_SetProps,
		IMapiProp_DeleteProps,
		IMapiProp_GetNamesFromIDs,
		IMapiProp_GetIDsFromNames,
		IMapiContainer_GetContentsTable,
		IMapiContainer_GetHierarchyTable,
		IMapiContainer_OpenEntry,
		IMapiContainer_OpenEntry_3,
		IMapiContainer_SetSearchCriteria,
		IMapiContainer_GetSearchCriteria,
		IMapiFolder_CreateMessage,
		IMapiFolder_CopyMessages,
		IMapiFolder_DeleteMessages,
		IMapiFolder_CreateFolder,
		IMapiFolder_CopyFolder,
		IMapiFolder_DeleteFolder,
		IMapiFolder_SetReadFlags,
		IMapiFolder_GetMessageStatus,
		IMapiFolder_SetMessageStatus,
		IMapiFolder_SaveContentsSort,
		IMapiFolder_EmptyFolder,
		IMapiFolder_AssignIMAP4UID,
		IMessage_GetAttachmentTable,
		IMessage_OpenAttach,
		IMessage_CreateAttach,
		IMessage_DeleteAttach,
		IMessage_GetRecipientTable,
		IMessage_ModifyRecipients,
		IMessage_SubmitMessage,
		IMessage_SetReadFlag,
		IMsgStore_Advise,
		IMsgStore_Unadvise,
		IMsgStore_CompareEntryIDs,
		IMsgStore_OpenEntry,
		IMsgStore_GetRoot,
		IMsgStore_OpenEntry_3,
		IMsgStore_SetReceiveFolder,
		IMsgStore_GetReceiveFolder,
		IMsgStore_StoreLogoff,
		IMsgStore_AbortSubmit,
		IMsgStore_HrOpenIPMFolder,
		IMapiSession_OpenStore,
		IMapiSession_GetPrivateStore,
		IMapiSession_GetPublicStore,
		IMapiSession_Logon,
		IMapiSession_GetIdentity,
		IMapiSession_GetConfig,
		IMapiSession_GetConfigNull,
		IMapiTableReader_GetTags,
		IMapiTableReader_GetRows,
		IMapiTable_Advise_2,
		IMapiTable_Advise,
		IMapiTable_Unadvise,
		IMapiTable_GetLastError,
		IMapiTable_GetStatus,
		IMapiTable_SetColumns,
		IMapiTable_QueryColumns,
		IMapiTable_GetRowCount,
		IMapiTable_SeekRow,
		IMapiTable_SeekRowApprox,
		IMapiTable_QueryPosition,
		IMapiTable_FindRow,
		IMapiTable_Restrict,
		IMapiTable_CreateBookmark,
		IMapiTable_FreeBookmark,
		IMapiTable_SortTable,
		IMapiTable_QuerySortOrder,
		IMapiTable_QueryRows,
		IMapiTable_Abort,
		IMapiTable_ExpandRow,
		IMapiTable_CollapseRow,
		IMapiTable_WaitForCompletion,
		IMapiTable_GetCollapseState,
		IMapiTable_SetCollapseState,
		IStream_GetData,
		IStream_PutData
	}

}
