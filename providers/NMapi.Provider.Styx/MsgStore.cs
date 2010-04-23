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
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Provider.Styx.Interop;

#pragma warning disable 0169

namespace NMapi.Provider.Styx
{
    public enum ObjectTypes : uint {
        Store       = 0x00000001,
        AddressBook = 0x00000002,
        Folder      = 0x00000003,
        ABContainer = 0x00000004,
        Message     = 0x00000005,
        MailUser    = 0x00000006,
        Attachment  = 0x00000007,
        DistList    = 0x00000008,
        ProfileSect = 0x00000009,
        Status      = 0x0000000A,
        Session     = 0x0000000B,
        FormInfo    = 0x0000000C
    }

    public class MsgStore : Prop, IMsgStore
    {
        private ObjectEventProxy events;

        internal MsgStore (IntPtr cobj) : base (cobj) {
        
        }

        #region IMsgStore Members

        public void AbortSubmit (byte[] entryID, int flags) {
            uint EidSize = entryID == null ? 0 : (uint) entryID.Length;
            int hr = CMapi_MsgStore_AbortSubmit (cobj, EidSize, entryID, (uint) flags);
            Transmogrify.CheckHResult (hr);
        }

        public EventConnection Advise (byte[] entryID, Flags.NotificationEventType eventMask, IMapiAdviseSink sink) {
            uint Connection;
            AdviseBridge bridge = new AdviseBridge (sink);
            int EidSize = entryID == null ? 0 : entryID.Length;
            int hr = CMapi_MsgStore_Advise (cobj, (uint) EidSize, entryID, (uint) eventMask, bridge.SinkPtr, out Connection);

            /* XXX hack because some flags produce MAPI_E_UNKNOWN_FLAGS */
            if((uint)hr == 0x80040106) {
                /* valid flags for this, according to MAPI reference */
                Flags.NotificationEventType newEvMask = eventMask & (
                    NotificationEventType.CriticalError | NotificationEventType.Extended |
                    NotificationEventType.NewMail | NotificationEventType.ObjectCreated |
                    NotificationEventType.ObjectCopied | NotificationEventType.ObjectDeleted |
                    NotificationEventType.ObjectModified | NotificationEventType.ObjectMoved |
                    NotificationEventType.SearchComplete);
                hr = CMapi_MsgStore_Advise (cobj, (uint) EidSize, entryID, (uint) newEvMask, bridge.SinkPtr, out Connection);
            }
            /* XXX end of hack */
           
            AdviseBridge.AddBrige (bridge, Connection);
            Transmogrify.CheckHResult (hr);
            return new EventConnection ((int) Connection);
        }

        public int CompareEntryIDs (byte[] entryID1, byte[] entryID2, int flags) {
            uint EidSize1 = entryID1 == null ? 0 : (uint) entryID1.Length;
            uint EidSize2 = entryID2 == null ? 0 : (uint) entryID2.Length;

            uint Result;
            int hr = CMapi_MsgStore_CompareEntryIDs (cobj, EidSize1, entryID1, EidSize2, entryID2, (uint) flags, out Result);
            Transmogrify.CheckHResult (hr);
            return (int) Result;
        }

        public NMapi.Events.ObjectEventProxy Events {
            get {
                if (events == null)
                    events = new ObjectEventProxy (this);
                return events;
            }
        }

        public GetReceiveFolderResult GetReceiveFolder (string messageClass, int flags) {
            IntPtr ExplicitClass;
            IntPtr Eid;
            uint EidLength;
            int hr = CMapi_MsgStore_GetReceiveFolder (cobj, messageClass, (uint) flags, out EidLength, out Eid, out ExplicitClass);
            Transmogrify.CheckHResult (hr);

            GetReceiveFolderResult res = new GetReceiveFolderResult ();
            res.ExplicitClass = Marshal.PtrToStringAuto (ExplicitClass);
            res.EntryID = new byte[(int) EidLength];
            Marshal.Copy (Eid, res.EntryID, 0, (int) EidLength);
            return res;
        }

        public IMapiFolder HrOpenIPMFolder (string path, int flags) {
            throw new NotImplementedException ();
        }

        public IBase OpenEntry (byte[] entryID, NMapiGuid interFace, int flags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr Handle;
                uint ObjTypeId;

                uint EidSize = entryID == null ? 0 : (uint) entryID.Length;
                int hr = CMapi_MsgStore_OpenEntry (cobj, EidSize, entryID, IntPtr.Zero, (uint) flags, out ObjTypeId, out Handle);
                Transmogrify.CheckHResult (hr);

                IBase entry = CMapi.CreateObjectForType (Handle, ObjTypeId);
                
                return entry;
            }
        }

        public IBase OpenEntry (byte[] entryID) {
            return OpenEntry (entryID, null, 16 /* XXX MAPI_BEST_ACCESS */);
        }

        public IBase Root {
            get { return OpenEntry (null, null, 1 /* XXX MAPI_BEST_ACCESS */); }
        }

        public void SetReceiveFolder (string messageClass, byte[] entryID, int flags) {
            uint EidSize = entryID == null ? 0 : (uint) entryID.Length;
            int hr = CMapi_MsgStore_SetReceiveFolder (cobj, messageClass, (uint) flags, EidSize, entryID);
            Transmogrify.CheckHResult (hr);
        }

        public void StoreLogoff (int flags) {
            CMapi_MsgStore_StoreLogoff (cobj, (uint) flags);
        }

        public void Unadvise (EventConnection txcOutlookHackConnection) {
            int connection = txcOutlookHackConnection.Connection;
            /* XXX check why we got -1 here (mapishell->quit) */
            if(connection != -1) {
                AdviseBridge.BridgeById ((uint)connection);
                int hr = CMapi_MsgStore_Unadvise (cobj, (uint)connection);
                Transmogrify.CheckHResult (hr);
                AdviseBridge.RemoveBridge ((uint)connection);
            } else
                System.Console.WriteLine("WARNING: Unadvise with Connection #-1");
        }

        public IMapiTableReader GetReceiveFolderTable (int arg) {
            throw new NotImplementedException ();
        }

        public IMapiTableReader GetOutgoingQueue (int arg) {
            throw new NotImplementedException ();
        }

        public byte[] OrigEID {
            get {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region C-Glue
        [DllImport ("libcmapi")]
        private static extern int CMapi_MsgStore_Advise (IntPtr store,
														 uint cbEntryID,
														 byte[] bytes,
														 uint EventMask,
														 IntPtr lpAdviseSink,
														 out uint Connection);

        [DllImport ("libcmapi")]
        private static extern int CMapi_MsgStore_Unadvise (IntPtr store, uint Connection);

        [DllImport ("libcmapi")]
        private static extern int CMapi_MsgStore_CompareEntryIDs (IntPtr store,
																  uint cbEntryID1,
																  byte[] lpEntryID1,
																  uint cbEntryID2,
																  byte[] lpEntryID2,
																  uint ulFlags,
																  out uint lpulResult);

        [DllImport ("libcmapi")]
        public static extern int
		CMapi_MsgStore_OpenEntry (IntPtr store,
								  uint cbEntryID,
								  byte[] bytes,
								  IntPtr lpInterface,
								  uint ulFlags,
								  out uint lpulObjType,
								  out IntPtr lppUnk);

        [DllImport ("libcmapi")]
        public static extern int
		CMapi_MsgStore_GetReceiveFolder (IntPtr store,
										 string MessageClass,
										 uint ulFlags,
										 out uint EntryIDLength,
										 out IntPtr EntryID,
										 out IntPtr ExplicitClass);

        [DllImport ("libcmapi")]
		public static extern int CMapi_MsgStore_SetReceiveFolder (IntPtr store,
																  string lpszMessageClass,
																  uint ulFlags,
																  uint cbEntryID,
																  byte[] lpEntryID);

        [DllImport ("libcmapi")]
		public static extern int CMapi_MsgStore_GetReceiveFolderTable (IntPtr store,
																	   uint ulFlags,
																	   out IntPtr lppTable);
        [DllImport ("libcmapi")]
		public static extern int CMapi_MsgStore_StoreLogoff (IntPtr store,
															 uint lpulFlags);
        [DllImport ("libcmapi")]
		public static extern int CMapi_MsgStore_AbortSubmit (IntPtr store,
															 uint cbEntryID,
															 byte[] lpEntryID,
															 uint ulFlags);
        [DllImport ("libcmapi")]
		public static extern int CMapi_MsgStore_GetOutgoingQueue (IntPtr store,
																  uint ulFlags,
																  out IntPtr lppTable);
        [DllImport ("libcmapi")]
		public static extern int CMapi_MsgStore_SetLockState (IntPtr store,
															  IntPtr lpMessage,
															  uint ulLockState);
        [DllImport ("libcmapi")]
		public static extern int CMapi_MsgStore_FinishedMsg (IntPtr store,
															 uint ulFlags,
															 uint cbEntryID,
															 byte[] lpEntryID);
        [DllImport ("libcmapi")]
		public static extern int CMapi_MsgStore_NotifyNewMail (IntPtr store,
															   IntPtr lpNotification);
        #endregion

    }
}
