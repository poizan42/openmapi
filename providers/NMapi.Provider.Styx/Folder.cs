/* 
 *  NMapy.Styx - The Border between C and C#
 *
 *  Copyright (C) Christian Kellner <christian.kellner@topalis.com> 
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Text;
using System.Runtime.InteropServices;

using NMapi;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Flags;

using NMapi.Provider.Styx.Interop;

#pragma warning disable 0169

namespace NMapi.Provider.Styx
{
    public class Folder : Container, IMapiFolder
    {        
        internal Folder (IntPtr cobj) : base (cobj) {

        }

        #region IMapiFolder Members

        public void CopyFolder (byte[] entryID, NMapiGuid interFace, IMapiFolder destFolder, string newFolderName, IMapiProgress progress, int flags) {
            throw new NotImplementedException ();
        }

        public void CopyMessages (EntryList msgList, NMapiGuid interFace, IMapiFolder destFolder, IMapiProgress progress, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr lpMsgList = Transmogrify.EntryListToPtr(msgList, MemCtx);
                IntPtr ifHandle = Transmogrify.GuidToPtr (interFace, MemCtx);
                IntPtr nativeObject = ((Unknown)destFolder).nativeObject; /* XXX not really clean */

                if(lpMsgList == IntPtr.Zero) return;

                /* XXX implement progress */
                int hr = CMapi_Folder_CopyMessages (cobj, lpMsgList, ifHandle, nativeObject, 0, IntPtr.Zero, (uint) flags);

                Transmogrify.CheckHResult (hr);
            }
        }

        /* XXX only works with unicode currently (as set by mapishell) - needs work on marshalling (see below) */
        public IMapiFolder CreateFolder (NMapi.Flags.FolderType folderType, string folderName, string folderComment, NMapiGuid interFace, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr ifHandle = Transmogrify.GuidToPtr (interFace, MemCtx);
                IntPtr newFolder;
                int hr = CMapi_Folder_CreateFolder (cobj, (uint) folderType, folderName, folderComment, ifHandle, (uint) flags, out newFolder);
                Transmogrify.CheckHResult (hr);
                return (IMapiFolder) CMapi.CreateObjectForType(newFolder, ObjectTypes.Folder);
            }
        }

        public IMessage CreateMessage (NMapiGuid interFace, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr ifHandle = Transmogrify.GuidToPtr (interFace, MemCtx);
                IntPtr MsgHandle;
                int hr = CMapi_Folder_CreateMessage (cobj, ifHandle, (uint) flags, out MsgHandle);
                Transmogrify.CheckHResult (hr);

                IMessage msg = (IMessage) CMapi.CreateObjectForType (MsgHandle, ObjectTypes.Message);

                return msg;
            }
        }

        public void DeleteFolder (byte[] entryID, IMapiProgress progress, int flags) {
            uint EidSize = entryID == null ? 0 : (uint) entryID.Length;

            /* XXX implement progress */
            int hr = CMapi_Folder_DeleteFolder (cobj, EidSize, entryID, 0, IntPtr.Zero, (uint) flags);
            Transmogrify.CheckHResult (hr);
        }

        public void DeleteMessages (EntryList msgList, IMapiProgress progress, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr lpMsgList = Transmogrify.EntryListToPtr(msgList, MemCtx);

                if(lpMsgList == IntPtr.Zero) return;

                /* XXX implement progress */
                int hr = CMapi_Folder_DeleteMessages (cobj, lpMsgList, 0, IntPtr.Zero, (uint) flags);

                Transmogrify.CheckHResult (hr);
            }
        }

        public void EmptyFolder (IMapiProgress progress, int flags) {
            throw new NotImplementedException ();
        }

        public int GetMessageStatus (byte[] entryID, int flags) {
            throw new NotImplementedException ();
        }

        public void SaveContentsSort (SortOrderSet sortOrder, int flags) {
            throw new NotImplementedException ();
        }

        public int SetMessageStatus (byte[] entryID, int newStatus, int newStatusMask) {
            throw new NotImplementedException ();
        }

        public void SetReadFlags (EntryList msgList, IMapiProgress progress, int flags) {
            throw new NotImplementedException ();
        }

        #endregion
        
        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_CreateMessage (IntPtr Folder, IntPtr Interface, uint Flags, out IntPtr Message);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_CopyMessages (IntPtr folder,
                                                             IntPtr      lpMsgList,
                                                             IntPtr           lpInterface,
                                                             IntPtr           lpDestFolder,
                                                             uint            ulUIParam,
                                                             IntPtr   lpProgress,
                                                             uint            ulFlags);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_DeleteMessages (IntPtr folder,
                                                               IntPtr      lpMsgList,
                                                               uint            ulUIParam,
                                                               IntPtr   lpProgress,
                                                               uint            ulFlags);
        
        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_CreateFolder (IntPtr folder,
                                                             uint            ulFolderType,
                                                             [MarshalAs (UnmanagedType.LPWStr)] string lpszFolderName,
                                                             [MarshalAs (UnmanagedType.LPWStr)] string lpszFolderComment,
                                                             IntPtr           lpInterface,
                                                             uint            ulFlags,
                                                             out IntPtr    lppFolder);
       
        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_CopyFolder (IntPtr folder,
                                                           uint            cbEntryID,
                                                           byte[]        lpEntryID,
                                                           IntPtr           lpInterface,
                                                           IntPtr           lpDestFolder,
                                                           uint           lpszNewFolderName,
                                                           uint            ulUIParam,
                                                           IntPtr   lpProgress,
                                                           uint            ulFlags);
        
        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_DeleteFolder (IntPtr folder,
                                                             uint            cbEntryID,
                                                             byte[]        lpEntryID,
                                                             uint            ulUIParam,
                                                             IntPtr   lpProgress,
                                                             uint            ulFlags);
        
        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_SetReadFlags (IntPtr folder,
                                                             IntPtr      lpMsgList,
                                                             uint            ulUIParam,
                                                             IntPtr   lpProgress,
                                                             uint            ulFlags);
        
        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_GetMessageStatus (IntPtr folder,
                                                                 uint            cbEntryID,
                                                                 byte[]        lpEntryID,
                                                                 uint            ulFlags,
                                                                 out uint           lpulMessageStatus);
      
        [DllImport ("libcmapi")]
        private static extern int  CMapi_Folder_SetMessageStatus (IntPtr folder,
                                                                  uint            cbEntryID,
                                                                  byte[]                lpEntryID,
                                                                  uint            ulNewStatus,
                                                                  uint            ulNewStatusMask,
                                                                  out uint           lpulOldStatus);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_SaveContentsSort (IntPtr folder,
                                                                 IntPtr  lpSortCriteria,
                                                                 uint            ulFlags);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Folder_EmptyFolder      (IntPtr folder,
                                                                 uint            ulUIParam,
                                                                 IntPtr   lpProgress,
                                                                 uint            ulFlags);


    }
}
