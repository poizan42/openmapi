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

        #region IMapiFolder UMAPI emulation functions (named properties on folders)

        /* XXX emulate TXC behaviour with named properties on Folders, which is not supported by Exchange */
        public PropertyValue[] GetProps (PropertyTag[] Tags, int flags) {
            PropertyValue[] values = base.GetProps(Tags, flags);
            uint count;

            /* walk over properties to find named props not being found */
            count = 0;
            /* XXX don't do this on a "get all" request */
            if(Tags != null && values != null && values.Length > 0 && Tags.Length == values.Length) {
                foreach(PropertyValue val in values) {
                    Flags.PropertyType type = (Flags.PropertyType) PropertyTypeHelper.PROP_TYPE(val.PropTag);
                    if(type == Flags.PropertyType.Error && PropertyTag.CreatePropertyTag(val.PropTag).IsNamedProperty)
                        count++;
                }
            }
            if(count > 0) {
                using (IMessage msg = this.OpenMapi_GetOrCreateAssociated("FolderNamedProperties", false)) {
                    if(msg != null) {
                        PropertyTag[] newTags = new PropertyTag[count];
                        int iter;
                        count = 0;
                        for(iter = 0; iter < values.Length; iter++) {
                            PropertyValue val = values[iter];
                            Flags.PropertyType type = (Flags.PropertyType) PropertyTypeHelper.PROP_TYPE(val.PropTag);
                            if(type == Flags.PropertyType.Error && PropertyTag.CreatePropertyTag(val.PropTag).IsNamedProperty) {
                                newTags[count] = Tags[iter];
                                count++;
                            }
                        }
                        /* mix array here! */
                        values = msg.GetProps(newTags, flags);
                    }
                }
            }

            return values;
        }

        /* XXX emulate TXC behaviour with named properties on Folders, which is not supported by Exchange */
        public PropertyProblem[] SetProps (PropertyValue[] propArray) {
            PropertyProblem[] problems = base.SetProps(propArray);
            uint count;

            /* walk over problems to find named props with the right errors to set those props on msg */
            count = 0;
            if(problems != null && problems.Length > 0) {
                foreach(PropertyProblem prob in problems) {
                    if(PropertyTag.CreatePropertyTag(prob.PropTag).IsNamedProperty && (
                        prob.SCode == Error.NoAccess ||
                        prob.SCode == Error.NoSupport ||
                        prob.SCode == Error.UnexpectedId)) count++;
                }
            }

            /* if any suitable props have been found, construct a SetProps request with them for the msg object */
            if(count > 0) {
                using (IMessage msg = this.OpenMapi_GetOrCreateAssociated("FolderNamedProperties", true)) {
                    if(msg != null) {
                        PropertyProblem[] newProblems = null;
                        PropertyValue[] newProps = new PropertyValue[count];
                        int[] indexmap = new int[count];
    
                        count = 0;
                        foreach(PropertyProblem prob in problems) {
                            if(PropertyTag.CreatePropertyTag(prob.PropTag).IsNamedProperty && (
                                prob.SCode == Error.NoAccess ||
                                prob.SCode == Error.NoSupport ||
                                prob.SCode == Error.UnexpectedId)) {
                                indexmap[count] = prob.Index;
                                newProps[count] = propArray[prob.Index];
                                count++;
                            }
                        }
                        newProblems = msg.SetProps(newProps);
                        msg.SaveChanges(0);
    
                        /* XXX mix problems */
                        return newProblems;
                    }
                }
            }

            return problems;
        }

        #endregion

        #region IMapiFolder Members

        public void CopyFolder (byte[] entryID, NMapiGuid interFace, IMapiFolder destFolder, string newFolderName, IMapiProgress progress, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                uint EidSize = entryID == null ? 0 : (uint) entryID.Length;
                IntPtr ifHandle = Transmogrify.GuidToPtr (interFace, MemCtx);
                IntPtr nativeObject = ((Unknown)destFolder).nativeObject; /* XXX not really clean */

                /* XXX implement progress */
                flags |= Mapi.Unicode; /* does this work with non-exchange-servers? */
                int hr = CMapi_Folder_CopyFolder (cobj, EidSize, entryID, ifHandle, nativeObject, newFolderName, 0, IntPtr.Zero, (uint) flags);

                Transmogrify.CheckHResult (hr);
            }
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

        public IMapiFolder CreateFolder (NMapi.Flags.FolderType folderType, string folderName, string folderComment, NMapiGuid interFace, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr ifHandle = Transmogrify.GuidToPtr (interFace, MemCtx);
                IntPtr newFolder;

                flags |= Mapi.Unicode; /* does this work with non-exchange-servers? */
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
            /* XXX implement progress */
            int hr = CMapi_Folder_EmptyFolder(cobj, 0, IntPtr.Zero, (uint) flags);

            Transmogrify.CheckHResult (hr);
        }

        public int GetMessageStatus (byte[] entryID, int flags) {
            uint messageStatus;
            uint EidSize = entryID == null ? 0 : (uint) entryID.Length;

            int hr = CMapi_Folder_GetMessageStatus(cobj, EidSize, entryID, (uint) flags, out messageStatus);

            Transmogrify.CheckHResult (hr);
            return (int) messageStatus;
        }

        public void SaveContentsSort (SortOrderSet sortOrder, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr nativeSortOrder = Transmogrify.SortOrderSetToPtr (sortOrder, MemCtx);

                int hr = CMapi_Folder_SaveContentsSort (cobj, nativeSortOrder, (uint) flags);
                Transmogrify.CheckHResult (hr);
            }
        }

        public int SetMessageStatus (byte[] entryID, int newStatus, int newStatusMask) {
            uint oldStatus;
            uint EidSize = entryID == null ? 0 : (uint) entryID.Length;

            int hr = CMapi_Folder_SetMessageStatus(cobj, EidSize, entryID, (uint) newStatus, (uint) newStatusMask, out oldStatus);

            Transmogrify.CheckHResult (hr);
            return (int) oldStatus;
        }

        public void SetReadFlags (EntryList msgList, IMapiProgress progress, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr lpMsgList = Transmogrify.EntryListToPtr(msgList, MemCtx);

                if(lpMsgList == IntPtr.Zero) return;

                /* XXX implement progress */
                int hr = CMapi_Folder_SetReadFlags (cobj, lpMsgList, 0, IntPtr.Zero, (uint) flags);

                Transmogrify.CheckHResult (hr);
            }
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
                                                           [MarshalAs (UnmanagedType.LPWStr)] string lpszNewFolderName,
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
