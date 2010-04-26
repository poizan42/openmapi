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
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Provider.Styx.Interop;

namespace NMapi.Provider.Styx
{
	public class Message : Prop, IMessage  {
        
        internal Message (IntPtr cobj) : base (cobj) {
            
        }

        #region IMessage Members

        public CreateAttachResult CreateAttach (NMapiGuid interFace, int flags) {

            using (MemContext MemCtx = new MemContext ()) {

                IntPtr AttachHandle;
                IntPtr GuidHandle = Transmogrify.GuidToPtr (interFace, MemCtx);
                uint count;
                CMapi_Message_CreateAttach (cobj, GuidHandle, (uint) flags, out count , out AttachHandle);

                CreateAttachResult res = new CreateAttachResult ();
                res.Attach = (IAttach) CMapi.CreateObjectForType (AttachHandle, ObjectTypes.Attachment);
                res.AttachmentNum = (int) count;

                return res;
            }
        }

        public void DeleteAttach (int attachmentNum, IMapiProgress progress, int flags) {
            /* XXX implement progress */
            int hr = CMapi_Message_DeleteAttach(cobj, (uint) attachmentNum, 0, IntPtr.Zero, (uint) flags);
            Transmogrify.CheckHResult (hr);
        }

        public IMapiTableReader GetAttachmentTable (int flags) {
            IntPtr TableHandle;
            int hr = CMapi_Message_GetAttachmentTable (cobj, (uint) flags, out TableHandle);
            Transmogrify.CheckHResult (hr);
            IMapiTableReader reader;
            if (CMapi.IsNative) {
                Table table = new Table (TableHandle);
                reader = new TableReaderWrapped (table);
            } else {
                reader = new TableReaderNative (TableHandle);
            }
            return reader;
        }

        public IMapiTableReader GetRecipientTable (int flags) {
            IntPtr TableHandle;
            int hr = CMapi_Message_GetRecipientTable (cobj, (uint) flags, out TableHandle);
            Transmogrify.CheckHResult (hr);
            IMapiTableReader reader;
            if (CMapi.IsNative) {
                Table table = new Table (TableHandle);
                reader = new TableReaderWrapped (table);
            } else {
                reader = new TableReaderNative (TableHandle);
            }
            return reader;
        }

        public void ModifyRecipients (int flags, AdrList mods) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr ModsHandle = Transmogrify.AdrListToPointer (mods, MemCtx);
                CMapi_Message_ModifyRecipients (cobj, (uint) flags, ModsHandle);
            }
        }

        public IAttach OpenAttach (int attachmentNum, NMapiGuid interFace, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr AttachHandle;
                IntPtr GuidHandle = Transmogrify.GuidToPtr (interFace, MemCtx);
                CMapi_Message_OpenAttach (cobj, (uint) attachmentNum, GuidHandle, (uint) flags, out AttachHandle);
                return (IAttach) CMapi.CreateObjectForType (AttachHandle, ObjectTypes.Attachment);
            }
        }

        public void SetReadFlag (int flags) {
            int hr = CMapi_Message_SetReadFlag (cobj, (uint) flags);
            Transmogrify.CheckHResult (hr);
        }

        public void SubmitMessage (int flags) {
            int hr = CMapi_Message_SubmitMessage (cobj, (uint) flags);
            Transmogrify.CheckHResult (hr);
        }

        #endregion

        [DllImport ("libcmapi")]
        private static extern int CMapi_Message_GetAttachmentTable (IntPtr msg,
                                          uint ulFlags,
                                          out IntPtr lppTable);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Message_OpenAttach (IntPtr msg,
                                                  uint ulAttachmentNum,
                                                  IntPtr lpInterface,
                                                  uint ulFlags,
                                                   out IntPtr lppAttach);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Message_CreateAttach (IntPtr msg,
                                                  IntPtr lpInterface,
                                                  uint ulFlags,
                                                  out uint lpulAttachmentNum,
                                                  out IntPtr lppAttach);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Message_DeleteAttach (IntPtr msg,
                                                  uint ulAttachmentNum,
                                                  uint ulUIParam,
                                                  IntPtr lpProgress,
                                                  uint ulFlags);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Message_GetRecipientTable (IntPtr msg,
                                                  uint ulFlags,
                                                  out IntPtr lppTable);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Message_ModifyRecipients (IntPtr msg,
                                                  uint ulFlags,
                                                  IntPtr lpMods);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Message_SubmitMessage (IntPtr msg,
                                                  uint ulFlags);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Message_SetReadFlag (IntPtr msg,
                                                  uint ulFlags);

    }
}
