//
// openmapi.org - NMapi C# Mapi API - OncRpcService.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General public override License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General public override License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//


using System;
using System.IO;

using RemoteTea.OncRpc;
using System.Net;

using NMapi.Interop;
using NMapi.Interop.MapiRPC;
using RemoteTea.OncRpc.Server;

namespace NMapi.Interop.MapiRPC {

	public sealed class OncRpcService : MAPIRPCServerStub
	{
		public OncRpcService() : this (0) 
		{
		}

		public OncRpcService (int port) : this (null, port)
		{
		}

		public OncRpcService (IPAddress bindAddr, int port)
		{
			info = new OncRpcServerTransportRegistrationInfo [] {
				new OncRpcServerTransportRegistrationInfo(MAPIRPC.ASERV_PROGRAM, 1),
			};

			transports = new OncRpcServerTransport [] {
				new OncRpcUdpServerTransport(this, bindAddr, port, info, 32768),
				new OncRpcTcpServerTransport(this, bindAddr, port, info, 32768)
			};
		}

		public override Session_GetNativeID_res Session_GetNativeID_1 (Session_GetNativeID_arg arg1)
		{
			throw new NotImplementedException ("Session_GetNativeID_1");
		}

		public override Session_SubscribeEvent_res Session_SubscribeEvent_1(Session_SubscribeEvent_arg arg1)
		{
			throw new NotImplementedException ("Session_SubscribeEvent_1");
		}

		public override Session_AdmLogon_res Session_AdmLogon_1(Session_AdmLogon_arg arg1)
		{
			throw new NotImplementedException ("Session_AdmLogon_1");
		}

		public override Session_GetConfig_res Session_GetConfig_1(Session_GetConfig_arg arg1)
		{
			throw new NotImplementedException ("Session_GetConfig_1");
		}

		public override Session_GetVersion_res Session_GetVersion_1(Session_GetVersion_arg arg1)
		{
			throw new NotImplementedException ("Session_GetVersion_1");
		}

		public override Session_Logon2_res Session_Logon2_1(Session_Logon2_arg arg1)
		{
			throw new NotImplementedException ("Session_Logon2_1");
		}

		public override Session_GetLoginID_res Session_GetLoginID_1(Session_GetLoginID_arg arg1)
		{
			throw new NotImplementedException ("Session_GetLoginID_1");
		}

		public override Session_OpenStore_res Session_OpenStore_1(Session_OpenStore_arg arg1)
		{
			throw new NotImplementedException ("Session_OpenStore_1");
		}

		public override Session_SetPassword_res Session_SetPassword_1(Session_SetPassword_arg arg1)
		{
			throw new NotImplementedException ("Session_SetPassword_1");
		}

		public override Base_RefAdd_res Base_RefAdd_1(Base_RefAdd_arg arg1)
		{
			throw new NotImplementedException ("Base_RefAdd_1");
		}

		public override Base_RefRel_res Base_RefRel_1(Base_RefRel_arg arg1)
		{
			throw new NotImplementedException ("Base_RefRel_1");
		}

		public override Base_GetType_res Base_GetType_1(Base_GetType_arg arg1)
		{
			throw new NotImplementedException ("Session_GetType_1");
		}

		public override MAPIProp_GetLastError_res MAPIProp_GetLastError_1(MAPIProp_GetLastError_arg arg1)
		{
			throw new NotImplementedException ("MAPIProp_GetLastError_1");
		}

		public override MAPIProp_SaveChanges_res MAPIProp_SaveChanges_1(MAPIProp_SaveChanges_arg arg1)
		{
			throw new NotImplementedException ("MAPIProp_SaveChanges_1");
		}

		public override MAPIProp_GetProps_res MAPIProp_GetProps_1(MAPIProp_GetProps_arg arg1)
		{
			throw new NotImplementedException ("MAPIProp_GetProps_1");
		}

		public override MAPIProp_GetPropList_res MAPIProp_GetPropList_1(MAPIProp_GetPropList_arg arg1)
		{
			throw new NotImplementedException ("MAPIProp_GetPropList_1");
		}

		public override MAPIProp_OpenProperty_res MAPIProp_OpenProperty_1(MAPIProp_OpenProperty_arg arg1)
		{
			throw new NotImplementedException ("MAPIProp_OpenProperty_1");
		}

		public override MAPIProp_SetProps_res MAPIProp_SetProps_1(MAPIProp_SetProps_arg arg1)
		{
			throw new NotImplementedException ("MAPIProp_SetProps_1");
		}

		public override MAPIProp_DeleteProps_res MAPIProp_DeleteProps_1(MAPIProp_DeleteProps_arg arg1)
		{
			throw new NotImplementedException ("MAPIProp_DeleteProps_1");
		}

		public override MAPIProp_GetNamesFromIDs_res MAPIProp_GetNamesFromIDs_1(MAPIProp_GetNamesFromIDs_arg arg1)
		{
			throw new NotImplementedException ("MAPIProp_GetNamesFromIDs_1");
		}

		public override MAPIProp_GetIDsFromNames_res MAPIProp_GetIDsFromNames_1(MAPIProp_GetIDsFromNames_arg arg1)
		{
			throw new NotImplementedException ("MAPIProp_GetIDsFromNames_1");
		}

		public override MsgStore_CompareEntryIDs_res MsgStore_CompareEntryIDs_1(MsgStore_CompareEntryIDs_arg arg1)
		{
			throw new NotImplementedException ("MsgStore_CompareEntryIDs_1");
		}

		public override MsgStore_OpenEntry_res MsgStore_OpenEntry_1(MsgStore_OpenEntry_arg arg1)
		{
			throw new NotImplementedException ("MsgStore_OpenEntry_1");
		}

		public override MsgStore_SetReceiveFolder_res MsgStore_SetReceiveFolder_1(MsgStore_SetReceiveFolder_arg arg1)
		{
			throw new NotImplementedException ("Session_SetReceiveFolder_1");
		}

		public override MsgStore_GetReceiveFolder_res MsgStore_GetReceiveFolder_1(MsgStore_GetReceiveFolder_arg arg1)
		{
			throw new NotImplementedException ("Session_GetReceiveFolder_1");
		}

		public override MsgStore_GetReceiveFolderTable_res MsgStore_GetReceiveFolderTable_1(MsgStore_GetReceiveFolderTable_arg arg1)
		{
			throw new NotImplementedException ("Session_GetReceiveFolderTable_1");
		}

		public override MsgStore_StoreLogoff_res MsgStore_StoreLogoff_1(MsgStore_StoreLogoff_arg arg1)
		{
			throw new NotImplementedException ("Session_StoreLogoff_1");
		}

		public override MsgStore_AbortSubmit_res MsgStore_AbortSubmit_1(MsgStore_AbortSubmit_arg arg1)
		{
			throw new NotImplementedException ("Session_AbortSubmit_1");
		}

		public override MsgStore_GetOutgoingQueue_res MsgStore_GetOutgoingQueue_1(MsgStore_GetOutgoingQueue_arg arg1)
		{
			throw new NotImplementedException ("Session_GetOutgoingQueue_1");
		}

		public override MsgStore_SetLockState_res MsgStore_SetLockState_1(MsgStore_SetLockState_arg arg1)
		{
			throw new NotImplementedException ("MsgStore_SetLockState_1");
		}

		public override MsgStore_FinishedMsg_res MsgStore_FinishedMsg_1(MsgStore_FinishedMsg_arg arg1)
		{
			throw new NotImplementedException ("MsgStore_FinishedMsg_1");
		}

		public override MsgStore_NotifyNewMail_res MsgStore_NotifyNewMail_1(MsgStore_NotifyNewMail_arg arg1)
		{
			throw new NotImplementedException ("MsgStore_NotifyNewMail_1");
		}

		public override MsgStore_GetOrigEID_res MsgStore_GetOrigEID_1(MsgStore_GetOrigEID_arg arg1)
		{
			throw new NotImplementedException ("MsgStore_GetOrigEID_1");
		}

		public override MsgStore_SetWrappedEID_res MsgStore_SetWrappedEID_1(MsgStore_SetWrappedEID_arg arg1)
		{
			throw new NotImplementedException ("MsgStore_SetWrappedEID_1");
		}

		public override MAPIContainer_GetContentsTable_res MAPIContainer_GetContentsTable_1(MAPIContainer_GetContentsTable_arg arg1)
		{
			throw new NotImplementedException ("MAPIContainer_GetContentsTable_1");
		}

		public override MAPIContainer_GetHierarchyTable_res MAPIContainer_GetHierarchyTable_1(MAPIContainer_GetHierarchyTable_arg arg1)
		{
			throw new NotImplementedException ("MAPIContainer_GetHierarchyTable_1");
		}

		public override MAPIContainer_OpenEntry_res MAPIContainer_OpenEntry_1(MAPIContainer_OpenEntry_arg arg1)
		{
			throw new NotImplementedException ("MAPIContainer_OpenEntry_1");
		}

		public override MAPIContainer_SetSearchCriteria_res MAPIContainer_SetSearchCriteria_1(MAPIContainer_SetSearchCriteria_arg arg1)
		{
			throw new NotImplementedException ("MAPIContainer_SetSearchCriteria_1");
		}

		public override MAPIContainer_GetSearchCriteria_res MAPIContainer_GetSearchCriteria_1(MAPIContainer_GetSearchCriteria_arg arg1)
		{
			throw new NotImplementedException ("MAPIContainer_GetSearchCriteria_1");
		}

		public override MAPIFolder_CreateMessage_res MAPIFolder_CreateMessage_1(MAPIFolder_CreateMessage_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_CreateMessage_1");
		}

		public override MAPIFolder_CopyMessages_res MAPIFolder_CopyMessages_1(MAPIFolder_CopyMessages_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_CopyMessages_1");
		}

		public override MAPIFolder_DeleteMessages_res MAPIFolder_DeleteMessages_1(MAPIFolder_DeleteMessages_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_DeleteMessages_1");
		}

		public override MAPIFolder_CreateFolder_res MAPIFolder_CreateFolder_1(MAPIFolder_CreateFolder_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_CreateFolder_1");
		}

		public override MAPIFolder_CopyFolder_res MAPIFolder_CopyFolder_1(MAPIFolder_CopyFolder_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_CopyFolder_1");
		}

		public override MAPIFolder_DeleteFolder_res MAPIFolder_DeleteFolder_1(MAPIFolder_DeleteFolder_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_DeleteFolder_1");
		}

		public override MAPIFolder_SetReadFlags_res MAPIFolder_SetReadFlags_1(MAPIFolder_SetReadFlags_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_SetReadFlags_1");
		}

		public override MAPIFolder_GetMessageStatus_res MAPIFolder_GetMessageStatus_1(MAPIFolder_GetMessageStatus_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_GetMessageStatus_1");
		}

		public override MAPIFolder_SetMessageStatus_res MAPIFolder_SetMessageStatus_1(MAPIFolder_SetMessageStatus_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_SetMessageStatus_1");
		}

		public override MAPIFolder_SaveContentsSort_res MAPIFolder_SaveContentsSort_1(MAPIFolder_SaveContentsSort_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_SaveContentsSort_1");
		}

		public override MAPIFolder_EmptyFolder_res MAPIFolder_EmptyFolder_1(MAPIFolder_EmptyFolder_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_EmptyFolder_1");
		}

		public override MAPIFolder_GetAccessMask_res MAPIFolder_GetAccessMask_1(MAPIFolder_GetAccessMask_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_GetAccessMask_1");
		}

		public override MAPIFolder_AssignIMAP4UID_res MAPIFolder_AssignIMAP4UID_1(MAPIFolder_AssignIMAP4UID_arg arg1)
		{
			throw new NotImplementedException ("MAPIFolder_AssignIMAP4UID_1");
		}

		public override TblData_SetTags_res TblData_SetTags_1(TblData_SetTags_arg arg1)
		{
			throw new NotImplementedException ("TblData_SetTags_1");
		}

		public override TblData_GetTags_res TblData_GetTags_1(TblData_GetTags_arg arg1)
		{
			throw new NotImplementedException ("TblData_GetTags_1");
		}

		public override TblData_GetRows_res TblData_GetRows_1(TblData_GetRows_arg arg1)
		{
			throw new NotImplementedException ("TblData_GetRows_1");
		}

		public override Message_GetAttachmentTable_res Message_GetAttachmentTable_1(Message_GetAttachmentTable_arg arg1)
		{
			throw new NotImplementedException ("Message_GetAttachmentTable_1");
		}

		public override Message_OpenAttach_res Message_OpenAttach_1(Message_OpenAttach_arg arg1)
		{
			throw new NotImplementedException ("Message_OpenAttach_1");
		}

		public override Message_CreateAttach_res Message_CreateAttach_1(Message_CreateAttach_arg arg1)
		{
			throw new NotImplementedException ("Message_CreateAttach_1");
		}

		public override Message_DeleteAttach_res Message_DeleteAttach_1(Message_DeleteAttach_arg arg1)
		{
			throw new NotImplementedException ("Message_DeleteAttach_1");
		}

		public override Message_GetRecipientTable_res Message_GetRecipientTable_1(Message_GetRecipientTable_arg arg1)
		{
			throw new NotImplementedException ("Message_GetRecipientTable_1");
		}

		public override Message_AddRecipient_res Message_AddRecipient_1(Message_AddRecipient_arg arg1)
		{
			throw new NotImplementedException ("Message_AddRecipient_1");
		}

		public override Message_ModifyRecipient_res Message_ModifyRecipient_1(Message_ModifyRecipient_arg arg1)
		{
			throw new NotImplementedException ("Message_ModifyRecipient_1");
		}

		public override Message_DeleteRecipient_res Message_DeleteRecipient_1(Message_DeleteRecipient_arg arg1)
		{
			throw new NotImplementedException ("Message_DeleteRecipient_1");
		}

		public override Message_SubmitMessage_res Message_SubmitMessage_1(Message_SubmitMessage_arg arg1)
		{
			throw new NotImplementedException ("Message_SubmitMessage_1");
		}

		public override Message_SetReadFlag_res Message_SetReadFlag_1(Message_SetReadFlag_arg arg1)
		{
			throw new NotImplementedException ("Message_SetReadFlag_1");
		}

		public override SimpleStream_Read_res SimpleStream_Read_1(SimpleStream_Read_arg arg1)
		{
			throw new NotImplementedException ("SimpleStream_Read_1");
		}

		public override SimpleStream_Write_res SimpleStream_Write_1(SimpleStream_Write_arg arg1)
		{
			throw new NotImplementedException ("SimpleStream_Write_1");
		}

		public override SimpleStream_BeginWrite_res SimpleStream_BeginWrite_1(SimpleStream_BeginWrite_arg arg1)
		{
			throw new NotImplementedException ("SimpleStream_BeginWrite_1");
		}

		public override SimpleStream_EndWrite_res SimpleStream_EndWrite_1(SimpleStream_EndWrite_arg arg1)
		{
			throw new NotImplementedException ("SimpleStream_EndWrite_1");
		}

		public override SimpleStream_BeginRead_res SimpleStream_BeginRead_1(SimpleStream_BeginRead_arg arg1)
		{
			throw new NotImplementedException ("SimpleStream_BeginRead_1");
		}

		public override SimpleStream_EndRead_res SimpleStream_EndRead_1(SimpleStream_EndRead_arg arg1)
		{
			throw new NotImplementedException ("SimpleStream_EndRead_1");
		}

		public override MAPITable_GetLastError_res MAPITable_GetLastError_1(MAPITable_GetLastError_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_GetLastError_1");
		}

		public override MAPITable_GetStatus_res MAPITable_GetStatus_1(MAPITable_GetStatus_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_GetStatus_1");
		}

		public override MAPITable_SetColumns_res MAPITable_SetColumns_1(MAPITable_SetColumns_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_SetColumns_1");
		}

		public override MAPITable_QueryColumns_res MAPITable_QueryColumns_1(MAPITable_QueryColumns_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_QueryColumns_1");
		}

		public override MAPITable_GetRowCount_res MAPITable_GetRowCount_1(MAPITable_GetRowCount_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_GetRowCount_1");
		}

		public override MAPITable_SeekRow_res MAPITable_SeekRow_1(MAPITable_SeekRow_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_SeekRow_1");
		}

		public override MAPITable_SeekRowApprox_res MAPITable_SeekRowApprox_1(MAPITable_SeekRowApprox_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_SeekRowApprox_1");
		}

		public override MAPITable_QueryPosition_res MAPITable_QueryPosition_1(MAPITable_QueryPosition_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_QueryPosition_1");
		}

		public override MAPITable_FindRow_res MAPITable_FindRow_1(MAPITable_FindRow_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_FindRow_1");
		}

		public override MAPITable_Restrict_res MAPITable_Restrict_1(MAPITable_Restrict_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_Restrict_1");
		}

		public override MAPITable_CreateBookmark_res MAPITable_CreateBookmark_1(MAPITable_CreateBookmark_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_CreateBookmark_1");
		}

		public override MAPITable_FreeBookmark_res MAPITable_FreeBookmark_1(MAPITable_FreeBookmark_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_FreeBookmark_1");
		}

		public override MAPITable_SortTable_res MAPITable_SortTable_1(MAPITable_SortTable_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_SortTable_1");
		}

		public override MAPITable_QuerySortOrder_res MAPITable_QuerySortOrder_1(MAPITable_QuerySortOrder_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_QuerySortOrder_1");
		}

		public override MAPITable_QueryRows_res MAPITable_QueryRows_1(MAPITable_QueryRows_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_QueryRows_1");
		}

		public override MAPITable_Abort_res MAPITable_Abort_1(MAPITable_Abort_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_Abort_1");
		}

		public override MAPITable_ExpandRow_res MAPITable_ExpandRow_1(MAPITable_ExpandRow_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_ExpandRow_1");
		}

		public override MAPITable_CollapseRow_res MAPITable_CollapseRow_1(MAPITable_CollapseRow_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_CollapseRow_1");
		}

		public override MAPITable_WaitForCompletion_res MAPITable_WaitForCompletion_1(MAPITable_WaitForCompletion_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_WaitForCompletion_1");
		}

		public override MAPITable_GetCollapseState_res MAPITable_GetCollapseState_1(MAPITable_GetCollapseState_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_GetCollapseState_1");
		}

		public override MAPITable_SetCollapseState_res MAPITable_SetCollapseState_1(MAPITable_SetCollapseState_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_SetCollapseState_1");
		}

		public override MAPITable_GetEventKey_res MAPITable_GetEventKey_1(MAPITable_GetEventKey_arg arg1)
		{
			throw new NotImplementedException ("MAPITable_GetEventKey_1");
		}






		public override Admin_AdmSetPassword_res Admin_AdmSetPassword_1(Admin_AdmSetPassword_arg arg1)
		{
			throw new NotImplementedException ("Admin_AdmSetPassword_1");
		}

		public override Admin_UserCreate_res Admin_UserCreate_1(Admin_UserCreate_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserCreate_1");
		}

		public override Admin_UserDelete_res Admin_UserDelete_1(Admin_UserDelete_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserDelete_1");
		}

		public override Admin_UserGet_res Admin_UserGet_1(Admin_UserGet_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserGet_1");
		}

		public override Admin_UserPut_res Admin_UserPut_1(Admin_UserPut_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserPut_1");
		}

		public override Admin_UserGetFirst_res Admin_UserGetFirst_1(Admin_UserGetFirst_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserGetFirst_1");
		}

		public override Admin_UserGetNext_res Admin_UserGetNext_1(Admin_UserGetNext_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserGetNext_1");
		}

		public override Admin_UserSetPassword_res Admin_UserSetPassword_1(Admin_UserSetPassword_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserSetPassword_1");
		}

		public override Admin_UserAddGroup_res Admin_UserAddGroup_1(Admin_UserAddGroup_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserAddGroup_1");
		}

		public override Admin_UserRemGroup_res Admin_UserRemGroup_1(Admin_UserRemGroup_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserRemGroup_1");
		}

		public override Admin_UserGetGroupsFirst_res Admin_UserGetGroupsFirst_1(Admin_UserGetGroupsFirst_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserGetGroupsFirst_1");
		}

		public override Admin_UserGetGroupsNext_res Admin_UserGetGroupsNext_1(Admin_UserGetGroupsNext_arg arg1)
		{
			throw new NotImplementedException ("Admin_UserGetGroupsNext_1");
		}

		public override Admin_GroupCreate_res Admin_GroupCreate_1(Admin_GroupCreate_arg arg1)
		{
			throw new NotImplementedException ("Admin_GroupCreate_1");
		}

		public override Admin_GroupDelete_res Admin_GroupDelete_1(Admin_GroupDelete_arg arg1)
		{
			throw new NotImplementedException ("Admin_GroupDelete_1");
		}

		public override Admin_GroupGet_res Admin_GroupGet_1(Admin_GroupGet_arg arg1)
		{
			throw new NotImplementedException ("Admin_GroupGet_1");
		}

		public override Admin_GroupPut_res Admin_GroupPut_1(Admin_GroupPut_arg arg1)
		{
			throw new NotImplementedException ("Admin_GroupPut_1");
		}

		public override Admin_GroupGetFirst_res Admin_GroupGetFirst_1(Admin_GroupGetFirst_arg arg1)
		{
			throw new NotImplementedException ("Admin_GroupGetFirst_1");
		}

		public override Admin_GroupGetNext_res Admin_GroupGetNext_1(Admin_GroupGetNext_arg arg1)
		{
			throw new NotImplementedException ("Admin_GroupGetNext_1");
		}

		public override Admin_GroupGetMembersFirst_res Admin_GroupGetMembersFirst_1(Admin_GroupGetMembersFirst_arg arg1)
		{
			throw new NotImplementedException ("Admin_GroupGetMembersFirst_1");
		}

		public override Admin_GroupGetMembersNext_res Admin_GroupGetMembersNext_1(Admin_GroupGetMembersNext_arg arg1)
		{
			throw new NotImplementedException ("Admin_GroupGetMembersNext_1");
		}

		public override Admin_FolderGetFirst_res Admin_FolderGetFirst_1(Admin_FolderGetFirst_arg arg1)
		{
			throw new NotImplementedException ("Admin_FolderGetFirst_1");
		}

		public override Admin_FolderGetNext_res Admin_FolderGetNext_1(Admin_FolderGetNext_arg arg1)
		{
			throw new NotImplementedException ("Admin_FolderGetNext_1");
		}

		public override Admin_FolderGetAcl_res Admin_FolderGetAcl_1(Admin_FolderGetAcl_arg arg1)
		{
			throw new NotImplementedException ("Admin_FolderGetAcl_1");
		}

		public override Admin_FolderPutAcl_res Admin_FolderPutAcl_1(Admin_FolderPutAcl_arg arg1)
		{
			throw new NotImplementedException ("Admin_FolderPutAcl_1");
		}

		public override Admin_FolderGetRights_res Admin_FolderGetRights_1(Admin_FolderGetRights_arg arg1)
		{
			throw new NotImplementedException ("Admin_FolderGetRights_1");
		}

		public override Admin_FolderGet_res Admin_FolderGet_1(Admin_FolderGet_arg arg1)
		{
			throw new NotImplementedException ("Admin_FolderGet_1");
		}

		public override Admin_TraceWrite_res Admin_TraceWrite_1(Admin_TraceWrite_arg arg1)
		{
			throw new NotImplementedException ("Admin_TraceWrite_1");
		}

		public override Admin_TraceSetLevel_res Admin_TraceSetLevel_1(Admin_TraceSetLevel_arg arg1)
		{
			throw new NotImplementedException ("Admin_TraceSetLevel_1");
		}

		public override Admin_TraceSetFlags_res Admin_TraceSetFlags_1(Admin_TraceSetFlags_arg arg1)
		{
			throw new NotImplementedException ("Admin_TraceSetFlags_1");
		}

		public override Admin_ConfigPut_res Admin_ConfigPut_1(Admin_ConfigPut_arg arg1)
		{
			throw new NotImplementedException ("Admin_ConfigPut_1");
		}

		public override Admin_ConfigGet_res Admin_ConfigGet_1(Admin_ConfigGet_arg arg1)
		{
			throw new NotImplementedException ("Admin_ConfigGet_1");
		}

		public override Admin_ConfigDel_res Admin_ConfigDel_1(Admin_ConfigDel_arg arg1)
		{
			throw new NotImplementedException ("Admin_ConfigDel_1");
		}

		public override Admin_ConfigGetCategories_res Admin_ConfigGetCategories_1(Admin_ConfigGetCategories_arg arg1)
		{
			throw new NotImplementedException ("Admin_ConfigGetCategories_1");
		}

		public override Admin_ConfigGetCategoryVars_res Admin_ConfigGetCategoryVars_1(Admin_ConfigGetCategoryVars_arg arg1)
		{
			throw new NotImplementedException ("Admin_ConfigGetCategoryVars_1");
		}

		public override Admin_LicensePut_res Admin_LicensePut_1(Admin_LicensePut_arg arg1)
		{
			throw new NotImplementedException ("Admin_LicensePut_1");
		}






		public override ModifyTable_GetLastError_res ModifyTable_GetLastError_1(ModifyTable_GetLastError_arg arg1)
		{
			throw new NotImplementedException ("ModifyTable_GetLastError_1");
		}

		public override ModifyTable_GetTable_res ModifyTable_GetTable_1(ModifyTable_GetTable_arg arg1)
		{
			throw new NotImplementedException ("ModifyTable_GetTable_1");
		}

		public override ModifyTable_ModifyRow_res ModifyTable_ModifyRow_1(ModifyTable_ModifyRow_arg arg1)
		{
			throw new NotImplementedException ("ModifyTable_ModifyRow_1");
		}
	}

}


