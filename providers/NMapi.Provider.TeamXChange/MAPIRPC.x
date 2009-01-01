




enum RpcVersionInf { MAPIRPCVERSION = 9 };








typedef LONGLONG HOBJECT;





struct PROGRESSBAR
{
        u_long ulID;
        u_long ulMin;
        u_long ulMax;
        u_long ulFlags;
};

typedef PROGRESSBAR *LPPROGRESSBAR;

enum ClientEvType { CLEV_MAPI, CLEV_PROGRESS };

struct CLEVMAPI
{
        u_long ulConn;
        NOTIFICATION notif<>;
};

enum ProgressType { PROGRESS_SETLIMITS, PROGRESS_UPDATE };

struct CLEVPROGRESS
{
        ProgressType type;
        u_long ulID;
        u_long ul1;
        u_long ul2;
        u_long ul3;
};

union ClientEvent switch(ClientEvType type)
{
case CLEV_MAPI:
        CLEVMAPI mapi;
case CLEV_PROGRESS:
        CLEVPROGRESS progress;
};

struct ABUSERDATA
{
        LPWSTR pwszId;
        LPWSTR pwszDisplay;
        LPWSTR pwszAdrType;
        LPWSTR pwszSmtpAdr;
        LPWSTR pwszIntAdr;
        SBinary eid;
        SBinary searchKey;
};

typedef ABUSERDATA *LPABUSERDATA;

struct ABUSERLIST
{
        ABUSERDATA *pData;
        ABUSERLIST *pNext;
};

typedef ABUSERLIST *LPABUSERLIST;





struct Session_GetVersion_arg { u_long ulFlags; };



struct Session_GetVersion_res { u_long hr; u_long ulVersion; };




struct Session_InitSession_arg { LPWSTR pwszArgs; };



struct Session_InitSession_res { u_long hr; HOBJECT obj; };




struct Session_Logon2_arg { HOBJECT obj; LPSTR pszHost; LPWSTR pwszUser; LPWSTR pwszPassword; u_long ulSessionFlags; u_long ulCodePage; u_long ulLocaleID; };
struct Session_Logon2_res { u_long hr; };



struct Session_OpenStore_arg { HOBJECT obj; u_long ulFlags; LPWSTR pwszStoreUser; int bIsPublic; };






struct Session_OpenStore_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct Session_GetLoginName_arg { HOBJECT obj; u_long ulFlags; };




struct Session_GetLoginName_res { u_long hr; LPWSTR pwszLoginName; };




struct Session_AdmLogon_arg { HOBJECT obj; LPSTR pszPassword; u_long ulCodePage; u_long ulLocaleID; };






struct Session_AdmLogon_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct Session_GetConfig_arg { HOBJECT obj; LPSTR pszCategory; LPSTR pszID; u_long ulFlags; };






struct Session_GetConfig_res { u_long hr; LPSPropValue pValue; };




struct Session_SetPassword_arg { HOBJECT obj; LPWSTR pwszPassword; };




struct Session_SetPassword_res { u_long hr; };



struct Session_ABGetUserData_arg { HOBJECT obj; SBinary eid; };




struct Session_ABGetUserData_res { u_long hr; struct ABUSERDATA *pData; };




struct Session_ABGetUserDataBySmtpAddress_arg { HOBJECT obj; LPWSTR pwszSmtpAddress; };




struct Session_ABGetUserDataBySmtpAddress_res { u_long hr; struct ABUSERDATA *pData; };




struct Session_ABGetUserDataByInternalAddress_arg { HOBJECT obj; LPWSTR pwszInternalAddress; };




struct Session_ABGetUserDataByInternalAddress_res { u_long hr; struct ABUSERDATA *pData; };




struct Session_ABGetChangeTime_arg { HOBJECT obj; u_long ulFlags; };




struct Session_ABGetChangeTime_res { u_long hr; FILETIME ft; };




struct Session_ABGetUserList_arg { HOBJECT obj; u_long ulFlags; };




struct Session_ABGetUserList_res { u_long hr; struct ABUSERLIST *pList; };
struct Base_Close_arg { HOBJECT obj; };



struct Base_Close_res { u_long refCount; };







struct MAPIProp_GetLastError_arg { HOBJECT obj; u_long hResult; u_long ulFlags; };





struct MAPIProp_GetLastError_res { u_long hr; LPMAPIERROR_A lpMapiErrorA; LPMAPIERROR_W lpMapiErrorW; };





struct MAPIProp_SaveChanges_arg { HOBJECT obj; u_long ulFlags; };




struct MAPIProp_SaveChanges_res { u_long hr; };



struct MAPIProp_GetProps_arg { HOBJECT obj; LPSPropTagArray lpPropTagArray; u_long ulFlags; };





struct MAPIProp_GetProps_res { u_long hr; SPropValue props<>; };




struct MAPIProp_GetPropList_arg { HOBJECT obj; u_long ulFlags; };




struct MAPIProp_GetPropList_res { u_long hr; LPSPropTagArray lpPropTagArray; };




struct MAPIProp_OpenProperty_arg { HOBJECT obj; u_long ulPropTag; LPGUID lpiid; u_long ulInterfaceOptions; u_long ulFlags; };







struct MAPIProp_OpenProperty_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct MAPIProp_SetProps_arg { HOBJECT obj; SPropValue props<>; };




struct MAPIProp_SetProps_res { u_long hr; LPSPropProblemArray lpProblems; };




struct MAPIProp_DeleteProps_arg { HOBJECT obj; LPSPropTagArray lpPropTagArray; };




struct MAPIProp_DeleteProps_res { u_long hr; LPSPropProblemArray lpProblems; };




struct MAPIProp_GetNamesFromIDs_arg { HOBJECT obj; LPSPropTagArray lpPropTags; LPGUID lpPropSetGuid; u_long ulFlags; };






struct MAPIProp_GetNamesFromIDs_res { u_long hr; LPSPropTagArray lpPropTags; LPMAPINAMEID names<>; };





struct MAPIProp_GetIDsFromNames_arg { HOBJECT obj; LPMAPINAMEID names<>; u_long ulFlags; };





struct MAPIProp_GetIDsFromNames_res { u_long hr; LPSPropTagArray lpPropTags; };
struct MsgStore_CompareEntryIDs_arg { HOBJECT obj; SBinary eid1; SBinary eid2; u_long ulFlags; };






struct MsgStore_CompareEntryIDs_res { u_long hr; u_long ulResult; };




struct MsgStore_OpenEntry_arg { HOBJECT obj; SBinary eid; LPGUID lpInterface; u_long ulFlags; };






struct MsgStore_OpenEntry_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct MsgStore_SetReceiveFolder_arg { HOBJECT obj; LPSTR lpszMessageClassA; LPWSTR lpszMessageClassW; u_long ulFlags; SBinary eid; };







struct MsgStore_SetReceiveFolder_res { u_long hr; };



struct MsgStore_GetReceiveFolder_arg { HOBJECT obj; LPSTR lpszMessageClassA; LPWSTR lpszMessageClassW; u_long ulFlags; };






struct MsgStore_GetReceiveFolder_res { u_long hr; SBinary eid; LPSTR lpszExplicitClassA; LPWSTR lpszExplicitClassW; };






struct MsgStore_GetReceiveFolderTable_arg { HOBJECT obj; u_long ulFlags; };




struct MsgStore_GetReceiveFolderTable_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct MsgStore_StoreLogoff_arg { HOBJECT obj; u_long ulFlags; };




struct MsgStore_StoreLogoff_res { u_long hr; u_long ulFlags; };




struct MsgStore_AbortSubmit_arg { HOBJECT obj; SBinary eid; u_long ulFlags; };





struct MsgStore_AbortSubmit_res { u_long hr; };



struct MsgStore_GetOutgoingQueue_arg { HOBJECT obj; u_long ulFlags; };




struct MsgStore_GetOutgoingQueue_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct MsgStore_SetLockState_arg { HOBJECT obj; SBinary eid; u_long ulLockState; };





struct MsgStore_SetLockState_res { u_long hr; };



struct MsgStore_FinishedMsg_arg { HOBJECT obj; u_long ulFlags; SBinary eid; };





struct MsgStore_FinishedMsg_res { u_long hr; };



struct MsgStore_NotifyNewMail_arg { HOBJECT obj; LPNOTIFICATION lpNotification; };




struct MsgStore_NotifyNewMail_res { u_long hr; };



struct MsgStore_GetOrigEID_arg { HOBJECT obj; };



struct MsgStore_GetOrigEID_res { u_long hr; SBinary eid; };




struct MsgStore_SetWrappedEID_arg { HOBJECT obj; SBinary eid; };




struct MsgStore_SetWrappedEID_res { u_long hr; };



struct MsgStore_Advise_arg { HOBJECT obj; SBinary eid; u_long ulEventMask; u_long ulClientID; };






struct MsgStore_Advise_res { u_long hr; HOBJECT obj; };




struct MsgStore_Unadvise_arg { HOBJECT obj; HOBJECT connObj; };




struct MsgStore_Unadvise_res { u_long hr; };







struct MAPIContainer_GetContentsTable_arg { HOBJECT obj; u_long ulFlags; };




struct MAPIContainer_GetContentsTable_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct MAPIContainer_GetHierarchyTable_arg { HOBJECT obj; u_long ulFlags; };




struct MAPIContainer_GetHierarchyTable_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct MAPIContainer_OpenEntry_arg { HOBJECT obj; SBinary eid; LPGUID lpInterface; u_long ulFlags; };






struct MAPIContainer_OpenEntry_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct MAPIContainer_SetSearchCriteria_arg { HOBJECT obj; LPSRestriction lpRestriction; LPENTRYLIST lpContainerList; u_long ulSearchFlags; };






struct MAPIContainer_SetSearchCriteria_res { u_long hr; };



struct MAPIContainer_GetSearchCriteria_arg { HOBJECT obj; u_long ulFlags; };




struct MAPIContainer_GetSearchCriteria_res { u_long hr; LPSRestriction lpRestriction; LPENTRYLIST lpContainerList; u_long ulSearchState; };
struct MAPIFolder_CreateMessage_arg { HOBJECT obj; LPGUID lpInterface; u_long ulFlags; };





struct MAPIFolder_CreateMessage_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct MAPIFolder_CopyFolder_arg { HOBJECT obj; SBinary srceid; LPGUID lpInterface; SBinary dsteid; LPSTR pszNewNameA; LPWSTR pszNewNameW; LPPROGRESSBAR pBar; u_long ulFlags; };
struct MAPIFolder_CopyFolder_res { u_long hr; };



struct MAPIFolder_CopyMessages_arg { HOBJECT obj; LPENTRYLIST lpMsgList; LPGUID lpInterface; SBinary dsteid; LPPROGRESSBAR pBar; u_long ulFlags; };
struct MAPIFolder_CopyMessages_res { u_long hr; };




struct MAPIFolder_DeleteMessages_arg { HOBJECT obj; LPENTRYLIST lpMsgList; LPPROGRESSBAR pBar; u_long ulFlags; };






struct MAPIFolder_DeleteMessages_res { u_long hr; };



struct MAPIFolder_CreateFolder_arg { HOBJECT obj; u_long ulFolderType; LPSTR lpszFolderNameA; LPWSTR lpwszFolderNameW; LPSTR lpszFolderCommentA; LPWSTR lpwszFolderCommentW; LPGUID lpInterface; u_long ulFlags; };
struct MAPIFolder_CreateFolder_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct MAPIFolder_DeleteFolder_arg { HOBJECT obj; SBinary eid; LPPROGRESSBAR pBar; u_long ulFlags; };






struct MAPIFolder_DeleteFolder_res { u_long hr; };



struct MAPIFolder_SetReadFlags_arg { HOBJECT obj; LPENTRYLIST lpMsgList; LPPROGRESSBAR pBar; u_long ulFlags; };






struct MAPIFolder_SetReadFlags_res { u_long hr; };



struct MAPIFolder_GetMessageStatus_arg { HOBJECT obj; SBinary eid; u_long ulFlags; };





struct MAPIFolder_GetMessageStatus_res { u_long hr; u_long ulMessageStatus; };




struct MAPIFolder_SetMessageStatus_arg { HOBJECT obj; SBinary eid; u_long ulNewStatus; u_long ulNewStatusMask; };






struct MAPIFolder_SetMessageStatus_res { u_long hr; u_long ulOldStatus; };




struct MAPIFolder_SaveContentsSort_arg { HOBJECT obj; LPSSortOrderSet lpSort; u_long ulFlags; };





struct MAPIFolder_SaveContentsSort_res { u_long hr; };



struct MAPIFolder_EmptyFolder_arg { HOBJECT obj; LPPROGRESSBAR pBar; u_long ulFlags; };





struct MAPIFolder_EmptyFolder_res { u_long hr; };





struct MAPIFolder_AssignIMAP4UID_arg { HOBJECT obj; SBinary msgeid; u_long ulFlags; };





struct MAPIFolder_AssignIMAP4UID_res { u_long hr; LONGLONG msguid; };
struct TblData_SetTags_arg { HOBJECT obj; LPSPropTagArray pTags; };




struct TblData_SetTags_res { u_long hr; };



struct TblData_GetTags_arg { HOBJECT obj; };



struct TblData_GetTags_res { u_long hr; LPSPropTagArray pTags; };




struct TblData_GetRows_arg { HOBJECT obj; u_long cRows; };




struct TblData_GetRows_res { u_long hr; LPSRowSet pRows; };
struct Message_GetAttachmentTable_arg { HOBJECT obj; u_long ulFlags; };




struct Message_GetAttachmentTable_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct Message_OpenAttach_arg { HOBJECT obj; u_long ulAttachmentNum; LPGUID lpInterface; u_long ulFlags; };






struct Message_OpenAttach_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct Message_CreateAttach_arg { HOBJECT obj; LPGUID lpInterface; u_long ulFlags; };





struct Message_CreateAttach_res { u_long hr; u_long ulAttachmentNum; u_long ulObjType; HOBJECT obj; };






struct Message_DeleteAttach_arg { HOBJECT obj; u_long ulAttachmentNum; u_long ulFlags; };





struct Message_DeleteAttach_res { u_long hr; };



struct Message_GetRecipientTable_arg { HOBJECT obj; u_long ulFlags; };




struct Message_GetRecipientTable_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct Message_AddRecipient_arg { HOBJECT obj; LPADRENTRY pEntry; };




struct Message_AddRecipient_res { u_long hr; u_long ulRowid; };




struct Message_ModifyRecipient_arg { HOBJECT obj; LPADRENTRY pEntry; };




struct Message_ModifyRecipient_res { u_long hr; };



struct Message_DeleteRecipient_arg { HOBJECT obj; LPADRENTRY pEntry; };




struct Message_DeleteRecipient_res { u_long hr; };



struct Message_SubmitMessage_arg { HOBJECT obj; u_long ulFlags; };




struct Message_SubmitMessage_res { u_long hr; };



struct Message_SetReadFlag_arg { HOBJECT obj; u_long ulFlags; };




struct Message_SetReadFlag_res { u_long hr; };







struct SimpleStream_Read_arg { HOBJECT obj; u_long count; };




struct SimpleStream_Read_res { u_long hr; opaque data<>; };




struct SimpleStream_Write_arg { HOBJECT obj; opaque data<>; };




struct SimpleStream_Write_res { u_long hr; u_long written; };




struct SimpleStream_BeginWrite_arg { HOBJECT obj; };



struct SimpleStream_BeginWrite_res { u_long hr; };



struct SimpleStream_EndWrite_arg { HOBJECT obj; };



struct SimpleStream_EndWrite_res { u_long hr; };



struct SimpleStream_BeginRead_arg { HOBJECT obj; };



struct SimpleStream_BeginRead_res { u_long hr; };



struct SimpleStream_EndRead_arg { HOBJECT obj; };



struct SimpleStream_EndRead_res { u_long hr; };







struct MAPITable_GetLastError_arg { HOBJECT obj; u_long hResult; u_long ulFlags; };





struct MAPITable_GetLastError_res { u_long hr; LPMAPIERROR_A lpMapiErrorA; LPMAPIERROR_W lpMapiErrorW; };





struct MAPITable_GetStatus_arg { HOBJECT obj; };



struct MAPITable_GetStatus_res { u_long hr; u_long ulTableStatus; u_long ulTableType; };





struct MAPITable_SetColumns_arg { HOBJECT obj; LPSPropTagArray pTags; u_long ulFlags; };





struct MAPITable_SetColumns_res { u_long hr; };



struct MAPITable_QueryColumns_arg { HOBJECT obj; u_long ulFlags; };




struct MAPITable_QueryColumns_res { u_long hr; LPSPropTagArray pTags; };




struct MAPITable_GetRowCount_arg { HOBJECT obj; u_long ulFlags; };




struct MAPITable_GetRowCount_res { u_long hr; u_long ulCount; };




struct MAPITable_SeekRow_arg { HOBJECT obj; u_long bkOrigin; long lRowCount; };





struct MAPITable_SeekRow_res { u_long hr; long lRowsSought; };




struct MAPITable_SeekRowApprox_arg { HOBJECT obj; u_long ulNumerator; u_long ulDenominator; };





struct MAPITable_SeekRowApprox_res { u_long hr; };



struct MAPITable_QueryPosition_arg { HOBJECT obj; };



struct MAPITable_QueryPosition_res { u_long hr; u_long ulRow; u_long ulNumerator; u_long ulDenominator; };






struct MAPITable_FindRow_arg { HOBJECT obj; LPSRestriction lpRestriction; u_long bkOrigin; u_long ulFlags; };






struct MAPITable_FindRow_res { u_long hr; };



struct MAPITable_Restrict_arg { HOBJECT obj; LPSRestriction lpRestriction; u_long ulFlags; };





struct MAPITable_Restrict_res { u_long hr; };



struct MAPITable_CreateBookmark_arg { HOBJECT obj; };



struct MAPITable_CreateBookmark_res { u_long hr; u_long bkPosition; };




struct MAPITable_FreeBookmark_arg { HOBJECT obj; u_long bkPosition; };




struct MAPITable_FreeBookmark_res { u_long hr; };



struct MAPITable_SortTable_arg { HOBJECT obj; LPSSortOrderSet lpSortCriteria; u_long ulFlags; };





struct MAPITable_SortTable_res { u_long hr; };



struct MAPITable_QuerySortOrder_arg { HOBJECT obj; };



struct MAPITable_QuerySortOrder_res { u_long hr; LPSSortOrderSet lpSortCriteria; };




struct MAPITable_QueryRows_arg { HOBJECT obj; long lRowCount; u_long ulFlags; };





struct MAPITable_QueryRows_res { u_long hr; LPSRowSet lpRows; };




struct MAPITable_Abort_arg { HOBJECT obj; };



struct MAPITable_Abort_res { u_long hr; };



struct MAPITable_ExpandRow_arg { HOBJECT obj; opaque instkey<>; u_long ulRowCount; u_long ulFlags; };






struct MAPITable_ExpandRow_res { u_long hr; LPSRowSet lpRows; u_long ulMoreRows; };





struct MAPITable_CollapseRow_arg { HOBJECT obj; opaque instkey<>; u_long ulFlags; };





struct MAPITable_CollapseRow_res { u_long hr; u_long ulRowCount; };




struct MAPITable_WaitForCompletion_arg { HOBJECT obj; u_long ulFlags; u_long ulTimeout; };





struct MAPITable_WaitForCompletion_res { u_long hr; u_long ulTableStatus; };




struct MAPITable_GetCollapseState_arg { HOBJECT obj; u_long ulFlags; opaque instkey<>; };





struct MAPITable_GetCollapseState_res { u_long hr; opaque state<>; };




struct MAPITable_SetCollapseState_arg { HOBJECT obj; u_long ulFlags; opaque state<>; };





struct MAPITable_SetCollapseState_res { u_long hr; u_long bkLocation; };




struct MAPITable_Advise_arg { HOBJECT obj; u_long ulEventMask; u_long ulClientID; };





struct MAPITable_Advise_res { u_long hr; HOBJECT obj; };




struct MAPITable_Unadvise_arg { HOBJECT obj; HOBJECT connObj; };




struct MAPITable_Unadvise_res { u_long hr; };
struct Admin_AdmSetPassword_arg { HOBJECT obj; LPSTR pszPassword; };




struct Admin_AdmSetPassword_res { int ec; };





struct Admin_UserCreate_arg { HOBJECT obj; LPSTR pszId; };




struct Admin_UserCreate_res { int ec; };



struct Admin_UserDelete_arg { HOBJECT obj; LPSTR pszId; };




struct Admin_UserDelete_res { int ec; };



struct Admin_UserGet_arg { HOBJECT obj; LPSTR pszId; };




struct Admin_UserGet_res { int ec; LPSTR pszId; LPSTR pszComment; };





struct Admin_UserPut_arg { HOBJECT obj; LPSTR pszId; LPSTR pszComment; };





struct Admin_UserPut_res { int ec; };



struct Admin_UserGetFirst_arg { HOBJECT obj; };



struct Admin_UserGetFirst_res { int ec; LPSTR pszId; LPSTR pszComment; };





struct Admin_UserGetNext_arg { HOBJECT obj; };



struct Admin_UserGetNext_res { int ec; LPSTR pszId; LPSTR pszComment; };





struct Admin_UserSetPassword_arg { HOBJECT obj; LPSTR pszId; LPSTR pszPassword; };





struct Admin_UserSetPassword_res { int ec; };



struct Admin_UserAddGroup_arg { HOBJECT obj; LPSTR pszUserId; LPSTR pszGroupId; };





struct Admin_UserAddGroup_res { int ec; };



struct Admin_UserRemGroup_arg { HOBJECT obj; LPSTR pszUserId; LPSTR pszGroupId; };





struct Admin_UserRemGroup_res { int ec; };



struct Admin_UserGetGroupsFirst_arg { HOBJECT obj; LPSTR pszUserId; };




struct Admin_UserGetGroupsFirst_res { int ec; LPSTR pszGroupId; };




struct Admin_UserGetGroupsNext_arg { HOBJECT obj; };



struct Admin_UserGetGroupsNext_res { int ec; LPSTR pszGroupId; };






struct Admin_GroupCreate_arg { HOBJECT obj; LPSTR pszId; };




struct Admin_GroupCreate_res { int ec; };



struct Admin_GroupDelete_arg { HOBJECT obj; LPSTR pszId; };




struct Admin_GroupDelete_res { int ec; };



struct Admin_GroupGet_arg { HOBJECT obj; LPSTR pszId; };




struct Admin_GroupGet_res { int ec; LPSTR pszId; LPSTR pszComment; };





struct Admin_GroupPut_arg { HOBJECT obj; LPSTR pszId; LPSTR pszComment; };





struct Admin_GroupPut_res { int ec; };



struct Admin_GroupGetFirst_arg { HOBJECT obj; };



struct Admin_GroupGetFirst_res { int ec; LPSTR pszId; LPSTR pszComment; };





struct Admin_GroupGetNext_arg { HOBJECT obj; };



struct Admin_GroupGetNext_res { int ec; LPSTR pszId; LPSTR pszComment; };





struct Admin_GroupGetMembersFirst_arg { HOBJECT obj; LPSTR pszId; };




struct Admin_GroupGetMembersFirst_res { int ec; LPSTR pszUserId; LPSTR pszUserComment; };





struct Admin_GroupGetMembersNext_arg { HOBJECT obj; };



struct Admin_GroupGetMembersNext_res { int ec; LPSTR pszUserId; LPSTR pszUserComment; };







struct MYACL
{
        int type;
        u_long mask;
        LPSTR pszId;

        struct MYACL *next;
};

typedef MYACL *LPMYACL;

struct Admin_FolderGetFirst_arg { HOBJECT obj; LPSTR pszEID; };




struct Admin_FolderGetFirst_res { int ec; LPSTR pszEID; LPSTR pszName; int subfolder; };






struct Admin_FolderGetNext_arg { HOBJECT obj; };



struct Admin_FolderGetNext_res { int ec; LPSTR pszEID; LPSTR pszName; int subfolder; };






struct Admin_FolderGetAcl_arg { HOBJECT obj; LPSTR pszEID; };




struct Admin_FolderGetAcl_res { int ec; LPSTR pszEID; LPMYACL first; };






struct Admin_FolderPutAcl_arg { HOBJECT obj; LPSTR pszEID; LPMYACL first; };






struct Admin_FolderPutAcl_res { int ec; };



struct Admin_FolderGetRights_arg { HOBJECT obj; LPSTR pszEID; LPSTR pszUserId; };





struct Admin_FolderGetRights_res { int ec; u_long mask; };




struct Admin_FolderGet_arg { HOBJECT obj; LPSTR pszEID; };




struct Admin_FolderGet_res { int ec; LPSTR pszName; LPSTR pszComment; };
struct Admin_TraceWrite_arg { HOBJECT obj; LPSTR pszMessage; };




struct Admin_TraceWrite_res { int ec; };



struct Admin_TraceSetLevel_arg { HOBJECT obj; int level; };




struct Admin_TraceSetLevel_res { int ec; };



struct Admin_TraceSetFlags_arg { HOBJECT obj; u_long flags; };




struct Admin_TraceSetFlags_res { int ec; };







struct Admin_ConfigPut_arg { HOBJECT obj; LPSTR pszCategory; LPSTR pszID; LPSTR pszValue; };






struct Admin_ConfigPut_res { int ec; };



struct Admin_ConfigGet_arg { HOBJECT obj; LPSTR pszCategory; LPSTR pszID; };





struct Admin_ConfigGet_res { int ec; LPSTR pszValue; };




struct Admin_ConfigDel_arg { HOBJECT obj; LPSTR pszCategory; LPSTR pszID; };





struct Admin_ConfigDel_res { int ec; };



struct Admin_ConfigGetCategoriesFirst_arg { HOBJECT obj; };



struct Admin_ConfigGetCategoriesFirst_res { int ec; LPSTR pszCategory; };




struct CATEGORYLL
{
        LPSTR pszCategory;
        LPSTR pszID;
        LPSTR pszValue;
        struct CATEGORYLL *next;
};

typedef CATEGORYLL *LPCATEGORYLL;

struct Admin_ConfigGetCategories_arg { HOBJECT obj; };



struct Admin_ConfigGetCategories_res { int ec; LPCATEGORYLL list; };




struct Admin_ConfigGetCategoryVars_arg { HOBJECT obj; LPSTR pszCategory; };




struct Admin_ConfigGetCategoryVars_res { int ec; LPCATEGORYLL list; };
struct Admin_LicensePut_arg { HOBJECT obj; LPSTR pszLicense; };




struct Admin_LicensePut_res { int ec; };







struct ModifyTable_GetLastError_arg { HOBJECT obj; u_long hResult; u_long ulFlags; };





struct ModifyTable_GetLastError_res { u_long hr; LPMAPIERROR_A lpMapiErrorA; LPMAPIERROR_W lpMapiErrorW; };





struct ModifyTable_GetTable_arg { HOBJECT obj; u_long ulFlags; };




struct ModifyTable_GetTable_res { u_long hr; u_long ulObjType; HOBJECT obj; };





struct ModifyTable_ModifyRow_arg { HOBJECT obj; LPROWENTRY pRowEntry; };




struct ModifyTable_ModifyRow_res { u_long hr; LARGE_INTEGER liRuleId; };





program ASERV_PROGRAM
{

        version ASERV_VERSION
        {







                Session_GetVersion_res Session_GetVersion(Session_GetVersion_arg) = 106;


                Session_InitSession_res Session_InitSession(Session_InitSession_arg) = 107;
                Session_AdmLogon_res Session_AdmLogon(Session_AdmLogon_arg) = 108;
                Session_GetConfig_res Session_GetConfig(Session_GetConfig_arg) = 109;
                Session_Logon2_res Session_Logon2(Session_Logon2_arg) = 110;
                Session_GetLoginName_res Session_GetLoginName(Session_GetLoginName_arg) = 111;
                Session_OpenStore_res Session_OpenStore(Session_OpenStore_arg) = 112;
                Session_SetPassword_res Session_SetPassword(Session_SetPassword_arg) = 113;
                Session_ABGetUserData_res Session_ABGetUserData(Session_ABGetUserData_arg) = 114;
                Session_ABGetUserDataBySmtpAddress_res Session_ABGetUserDataBySmtpAddress(Session_ABGetUserDataBySmtpAddress_arg) = 115;
                Session_ABGetUserDataByInternalAddress_res Session_ABGetUserDataByInternalAddress(Session_ABGetUserDataByInternalAddress_arg) = 116;
                Session_ABGetChangeTime_res Session_ABGetChangeTime(Session_ABGetChangeTime_arg) = 117;
                Session_ABGetUserList_res Session_ABGetUserList(Session_ABGetUserList_arg) = 118;



                Base_Close_res Base_Close(Base_Close_arg) = 200;



                MAPIProp_GetLastError_res MAPIProp_GetLastError(MAPIProp_GetLastError_arg) = 300;
                MAPIProp_SaveChanges_res MAPIProp_SaveChanges(MAPIProp_SaveChanges_arg) = 301;
                MAPIProp_GetProps_res MAPIProp_GetProps(MAPIProp_GetProps_arg) = 302;
                MAPIProp_GetPropList_res MAPIProp_GetPropList(MAPIProp_GetPropList_arg) = 303;
                MAPIProp_OpenProperty_res MAPIProp_OpenProperty(MAPIProp_OpenProperty_arg) = 304;
                MAPIProp_SetProps_res MAPIProp_SetProps(MAPIProp_SetProps_arg) = 305;
                MAPIProp_DeleteProps_res MAPIProp_DeleteProps(MAPIProp_DeleteProps_arg) = 306;
                MAPIProp_GetNamesFromIDs_res MAPIProp_GetNamesFromIDs(MAPIProp_GetNamesFromIDs_arg) = 307;
                MAPIProp_GetIDsFromNames_res MAPIProp_GetIDsFromNames(MAPIProp_GetIDsFromNames_arg) = 308;



                MsgStore_CompareEntryIDs_res MsgStore_CompareEntryIDs(MsgStore_CompareEntryIDs_arg) = 400;
                MsgStore_OpenEntry_res MsgStore_OpenEntry(MsgStore_OpenEntry_arg) = 401;
                MsgStore_SetReceiveFolder_res MsgStore_SetReceiveFolder(MsgStore_SetReceiveFolder_arg) = 402;
                MsgStore_GetReceiveFolder_res MsgStore_GetReceiveFolder(MsgStore_GetReceiveFolder_arg) = 403;
                MsgStore_GetReceiveFolderTable_res MsgStore_GetReceiveFolderTable(MsgStore_GetReceiveFolderTable_arg) = 404;
                MsgStore_StoreLogoff_res MsgStore_StoreLogoff(MsgStore_StoreLogoff_arg) = 405;
                MsgStore_AbortSubmit_res MsgStore_AbortSubmit(MsgStore_AbortSubmit_arg) = 406;
                MsgStore_GetOutgoingQueue_res MsgStore_GetOutgoingQueue(MsgStore_GetOutgoingQueue_arg) = 407;
                MsgStore_SetLockState_res MsgStore_SetLockState(MsgStore_SetLockState_arg) = 408;
                MsgStore_FinishedMsg_res MsgStore_FinishedMsg(MsgStore_FinishedMsg_arg) = 409;
                MsgStore_NotifyNewMail_res MsgStore_NotifyNewMail(MsgStore_NotifyNewMail_arg) = 410;
                MsgStore_GetOrigEID_res MsgStore_GetOrigEID(MsgStore_GetOrigEID_arg) = 411;
                MsgStore_SetWrappedEID_res MsgStore_SetWrappedEID(MsgStore_SetWrappedEID_arg) = 412;
                MsgStore_Advise_res MsgStore_Advise(MsgStore_Advise_arg) = 413;
                MsgStore_Unadvise_res MsgStore_Unadvise(MsgStore_Unadvise_arg) = 414;



                MAPIContainer_GetContentsTable_res MAPIContainer_GetContentsTable(MAPIContainer_GetContentsTable_arg) = 500;
                MAPIContainer_GetHierarchyTable_res MAPIContainer_GetHierarchyTable(MAPIContainer_GetHierarchyTable_arg) = 501;
                MAPIContainer_OpenEntry_res MAPIContainer_OpenEntry(MAPIContainer_OpenEntry_arg) = 502;
                MAPIContainer_SetSearchCriteria_res MAPIContainer_SetSearchCriteria(MAPIContainer_SetSearchCriteria_arg) = 503;
                MAPIContainer_GetSearchCriteria_res MAPIContainer_GetSearchCriteria(MAPIContainer_GetSearchCriteria_arg) = 504;



                MAPIFolder_CreateMessage_res MAPIFolder_CreateMessage(MAPIFolder_CreateMessage_arg) = 600;
                MAPIFolder_CopyMessages_res MAPIFolder_CopyMessages(MAPIFolder_CopyMessages_arg) = 601;
                MAPIFolder_DeleteMessages_res MAPIFolder_DeleteMessages(MAPIFolder_DeleteMessages_arg) = 602;
                MAPIFolder_CreateFolder_res MAPIFolder_CreateFolder(MAPIFolder_CreateFolder_arg) = 603;
                MAPIFolder_CopyFolder_res MAPIFolder_CopyFolder(MAPIFolder_CopyFolder_arg) = 604;
                MAPIFolder_DeleteFolder_res MAPIFolder_DeleteFolder(MAPIFolder_DeleteFolder_arg) = 605;
                MAPIFolder_SetReadFlags_res MAPIFolder_SetReadFlags(MAPIFolder_SetReadFlags_arg) = 606;
                MAPIFolder_GetMessageStatus_res MAPIFolder_GetMessageStatus(MAPIFolder_GetMessageStatus_arg) = 607;
                MAPIFolder_SetMessageStatus_res MAPIFolder_SetMessageStatus(MAPIFolder_SetMessageStatus_arg) = 608;
                MAPIFolder_SaveContentsSort_res MAPIFolder_SaveContentsSort(MAPIFolder_SaveContentsSort_arg) = 609;
                MAPIFolder_EmptyFolder_res MAPIFolder_EmptyFolder(MAPIFolder_EmptyFolder_arg) = 610;

                MAPIFolder_AssignIMAP4UID_res MAPIFolder_AssignIMAP4UID(MAPIFolder_AssignIMAP4UID_arg) = 690;



                TblData_SetTags_res TblData_SetTags(TblData_SetTags_arg) = 700;
                TblData_GetTags_res TblData_GetTags(TblData_GetTags_arg) = 701;
                TblData_GetRows_res TblData_GetRows(TblData_GetRows_arg) = 702;



                Message_GetAttachmentTable_res Message_GetAttachmentTable(Message_GetAttachmentTable_arg) = 800;
                Message_OpenAttach_res Message_OpenAttach(Message_OpenAttach_arg) = 801;
                Message_CreateAttach_res Message_CreateAttach(Message_CreateAttach_arg) = 802;
                Message_DeleteAttach_res Message_DeleteAttach(Message_DeleteAttach_arg) = 803;
                Message_GetRecipientTable_res Message_GetRecipientTable(Message_GetRecipientTable_arg) = 804;
                Message_AddRecipient_res Message_AddRecipient(Message_AddRecipient_arg) = 805;
                Message_ModifyRecipient_res Message_ModifyRecipient(Message_ModifyRecipient_arg) = 806;
                Message_DeleteRecipient_res Message_DeleteRecipient(Message_DeleteRecipient_arg) = 807;
                Message_SubmitMessage_res Message_SubmitMessage(Message_SubmitMessage_arg) = 808;
                Message_SetReadFlag_res Message_SetReadFlag(Message_SetReadFlag_arg) = 809;



                SimpleStream_Read_res SimpleStream_Read(SimpleStream_Read_arg) = 900;
                SimpleStream_Write_res SimpleStream_Write(SimpleStream_Write_arg) = 901;
                SimpleStream_BeginWrite_res SimpleStream_BeginWrite(SimpleStream_BeginWrite_arg) = 902;
                SimpleStream_EndWrite_res SimpleStream_EndWrite(SimpleStream_EndWrite_arg) = 903;
                SimpleStream_BeginRead_res SimpleStream_BeginRead(SimpleStream_BeginRead_arg) = 904;
                SimpleStream_EndRead_res SimpleStream_EndRead(SimpleStream_EndRead_arg) = 905;



                MAPITable_GetLastError_res MAPITable_GetLastError(MAPITable_GetLastError_arg) = 1000;
                MAPITable_GetStatus_res MAPITable_GetStatus(MAPITable_GetStatus_arg) = 1001;
                MAPITable_SetColumns_res MAPITable_SetColumns(MAPITable_SetColumns_arg) = 1002;
                MAPITable_QueryColumns_res MAPITable_QueryColumns(MAPITable_QueryColumns_arg) = 1003;
                MAPITable_GetRowCount_res MAPITable_GetRowCount(MAPITable_GetRowCount_arg) = 1004;
                MAPITable_SeekRow_res MAPITable_SeekRow(MAPITable_SeekRow_arg) = 1005;
                MAPITable_SeekRowApprox_res MAPITable_SeekRowApprox(MAPITable_SeekRowApprox_arg) = 1006;
                MAPITable_QueryPosition_res MAPITable_QueryPosition(MAPITable_QueryPosition_arg) = 1007;
                MAPITable_FindRow_res MAPITable_FindRow(MAPITable_FindRow_arg) = 1008;
                MAPITable_Restrict_res MAPITable_Restrict(MAPITable_Restrict_arg) = 1009;
                MAPITable_CreateBookmark_res MAPITable_CreateBookmark(MAPITable_CreateBookmark_arg) = 1010;
                MAPITable_FreeBookmark_res MAPITable_FreeBookmark(MAPITable_FreeBookmark_arg) = 1011;
                MAPITable_SortTable_res MAPITable_SortTable(MAPITable_SortTable_arg) = 1012;
                MAPITable_QuerySortOrder_res MAPITable_QuerySortOrder(MAPITable_QuerySortOrder_arg) = 1013;
                MAPITable_QueryRows_res MAPITable_QueryRows(MAPITable_QueryRows_arg) = 1014;
                MAPITable_Abort_res MAPITable_Abort(MAPITable_Abort_arg) = 1015;
                MAPITable_ExpandRow_res MAPITable_ExpandRow(MAPITable_ExpandRow_arg) = 1016;
                MAPITable_CollapseRow_res MAPITable_CollapseRow(MAPITable_CollapseRow_arg) = 1017;
                MAPITable_WaitForCompletion_res MAPITable_WaitForCompletion(MAPITable_WaitForCompletion_arg) = 1018;
                MAPITable_GetCollapseState_res MAPITable_GetCollapseState(MAPITable_GetCollapseState_arg) = 1019;
                MAPITable_SetCollapseState_res MAPITable_SetCollapseState(MAPITable_SetCollapseState_arg) = 1020;
                MAPITable_Advise_res MAPITable_Advise(MAPITable_Advise_arg) = 1021;
                MAPITable_Unadvise_res MAPITable_Unadvise(MAPITable_Unadvise_arg) = 1022;




                Admin_AdmSetPassword_res Admin_AdmSetPassword(Admin_AdmSetPassword_arg) = 1100;

                Admin_UserCreate_res Admin_UserCreate(Admin_UserCreate_arg) = 1200;
                Admin_UserDelete_res Admin_UserDelete(Admin_UserDelete_arg) = 1201;
                Admin_UserGet_res Admin_UserGet(Admin_UserGet_arg) = 1202;
                Admin_UserPut_res Admin_UserPut(Admin_UserPut_arg) = 1203;
                Admin_UserGetFirst_res Admin_UserGetFirst(Admin_UserGetFirst_arg) = 1204;
                Admin_UserGetNext_res Admin_UserGetNext(Admin_UserGetNext_arg) = 1205;
                Admin_UserSetPassword_res Admin_UserSetPassword(Admin_UserSetPassword_arg) = 1206;
                Admin_UserAddGroup_res Admin_UserAddGroup(Admin_UserAddGroup_arg) = 1207;
                Admin_UserRemGroup_res Admin_UserRemGroup(Admin_UserRemGroup_arg) = 1208;
                Admin_UserGetGroupsFirst_res Admin_UserGetGroupsFirst(Admin_UserGetGroupsFirst_arg) = 1209;
                Admin_UserGetGroupsNext_res Admin_UserGetGroupsNext(Admin_UserGetGroupsNext_arg) = 1210;

                Admin_GroupCreate_res Admin_GroupCreate(Admin_GroupCreate_arg) = 1300;
                Admin_GroupDelete_res Admin_GroupDelete(Admin_GroupDelete_arg) = 1301;
                Admin_GroupGet_res Admin_GroupGet(Admin_GroupGet_arg) = 1302;
                Admin_GroupPut_res Admin_GroupPut(Admin_GroupPut_arg) = 1303;
                Admin_GroupGetFirst_res Admin_GroupGetFirst(Admin_GroupGetFirst_arg) = 1304;
                Admin_GroupGetNext_res Admin_GroupGetNext(Admin_GroupGetNext_arg) = 1305;
                Admin_GroupGetMembersFirst_res Admin_GroupGetMembersFirst(Admin_GroupGetMembersFirst_arg) = 1306;
                Admin_GroupGetMembersNext_res Admin_GroupGetMembersNext(Admin_GroupGetMembersNext_arg) = 1307;

                Admin_FolderGetFirst_res Admin_FolderGetFirst(Admin_FolderGetFirst_arg) = 1400;
                Admin_FolderGetNext_res Admin_FolderGetNext(Admin_FolderGetNext_arg) = 1401;
                Admin_FolderGetAcl_res Admin_FolderGetAcl(Admin_FolderGetAcl_arg) = 1402;
                Admin_FolderPutAcl_res Admin_FolderPutAcl(Admin_FolderPutAcl_arg) = 1403;
                Admin_FolderGetRights_res Admin_FolderGetRights(Admin_FolderGetRights_arg) = 1404;
                Admin_FolderGet_res Admin_FolderGet(Admin_FolderGet_arg) = 1405;

                Admin_TraceWrite_res Admin_TraceWrite(Admin_TraceWrite_arg) = 1600;
                Admin_TraceSetLevel_res Admin_TraceSetLevel(Admin_TraceSetLevel_arg) = 1601;
                Admin_TraceSetFlags_res Admin_TraceSetFlags(Admin_TraceSetFlags_arg) = 1602;

                Admin_ConfigPut_res Admin_ConfigPut(Admin_ConfigPut_arg) = 1700;
                Admin_ConfigGet_res Admin_ConfigGet(Admin_ConfigGet_arg) = 1701;
                Admin_ConfigDel_res Admin_ConfigDel(Admin_ConfigDel_arg) = 1702;
                Admin_ConfigGetCategories_res Admin_ConfigGetCategories(Admin_ConfigGetCategories_arg) = 1703;
                Admin_ConfigGetCategoryVars_res Admin_ConfigGetCategoryVars(Admin_ConfigGetCategoryVars_arg) = 1704;

                Admin_LicensePut_res Admin_LicensePut(Admin_LicensePut_arg) = 1800;



                ModifyTable_GetLastError_res ModifyTable_GetLastError(ModifyTable_GetLastError_arg) = 2000;
                ModifyTable_GetTable_res ModifyTable_GetTable(ModifyTable_GetTable_arg) = 2001;
                ModifyTable_ModifyRow_res ModifyTable_ModifyRow(ModifyTable_ModifyRow_arg) = 2002;
        } = 1;

} = 1;
