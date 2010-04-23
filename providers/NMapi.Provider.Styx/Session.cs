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
using System.Collections.Generic;

using NMapi.Provider.Styx.Interop;
using NMapi;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Flags;


namespace NMapi.Provider.Styx
{
    public class Session : Unknown, IMapiSession {

//        bool LoggedOn = false;

        public Session () : base () {
        }

        public Session (IntPtr SessionHandle)
            : base (SessionHandle) {

        }

        public string GetConfig (string category, string id, int flags) {
            throw new NotImplementedException ();
        }

        public string GetConfigNull (string category, string id, int flags) {
            throw new NotImplementedException ();
        }

        public byte[] Identity {
            get {
                throw new NotImplementedException ();
            }
        }

        public void Logon (string host, string user, string password) {
            Logon (host, 0, user, password, 0);
        }

        public void Logon (string host, int sessionFlags, string user, string password, int codePage) {
            System.Diagnostics.Trace.WriteLine (String.Format ("-> CMapi.LogonEx {0:X}", sessionFlags));

            IntPtr handle;

            if (CMapi.IsNative) {
                /* LogonEx needs NULL for password according to the MAPI docs,
                 * which is handled in libcmapi to not break the marshalling stuff here.
                 * host is used as profile name.
                 * Flags are chosen to make sure the provided profile is used for the session,
                 * so it is necessary to create a new session and not use a shared one. */
                handle = CMapi.LogonEx (host, password, CMapi.LogonFlags.ExplicitProfile | CMapi.LogonFlags.Extended | CMapi.LogonFlags.NewSession);
            } else {
                handle = CMapi.LogonU (host, user, password, 0); //xxx unicode?
            }

            System.Diagnostics.Trace.WriteLine ("<- CMapi.LogonEx");
            Wrap (handle);
        }

        private IMsgStore OpenStoreUMapi (bool isPublic) {

            int hr;
            IntPtr StoreHandle;

            if (isPublic) {
                hr = CMapi_Session_GetPublicStore (cobj, out StoreHandle);
            } else {
                hr = CMapi_Session_GetPrivateStore (cobj, out StoreHandle);
            }

            Transmogrify.CheckHResult (hr);

            return new MsgStore (StoreHandle);
        }

        public IMsgStore OpenStore (OpenStoreFlags flags, string user, bool isPublic) {

            //if (user != null) {
                System.Diagnostics.Trace.WriteLine (String.Format (" OpenStore {0} {1}", user, flags));
              //  throw new NotImplementedException ();
           // }

            if (CMapi.IsNative == false) {
                return OpenStoreUMapi (isPublic);
            }

            IntPtr TableHandle;
            int hr = CMapi_Session_GetMsgStoresTable (cobj, 0, out TableHandle);
            Transmogrify.CheckHResult (hr);

            Table table = new Table (TableHandle);
            try {

                PropertyTag[] cols = PropertyTag.ArrayFromIntegers (Property.EntryId,
                                                                    Property.DefaultStore,
                                                                    Property.DisplayName);
                table.SetColumns (cols, 0);

                PropertyRestriction res = new PropertyRestriction ();
                res.PropTag = Property.DefaultStore;
                res.RelOp = RelOp.Equal;
                res.Prop = new BooleanProperty ((short) (isPublic ? 0 : 1));
                res.Prop.PropTag = Property.DefaultStore;

                table.Restrict (res, NMAPI.TBL_BATCH);


                RowSet RowSet = table.QueryRows (10, 0); /* XXX Unicode?! */
                byte[] StoreId = null;

                if (RowSet.Count == 1) {
                    StoreId = (byte[]) RowSet[0].Props[0];
                } else {
                    //TODO: Teh Suck
                    throw new NotImplementedException ("Error opening Message Store: Needs Implementation");
                }

                IntPtr storeHandle;
                hr = CMapi_Session_OpenMsgStore (cobj, 0, (uint) StoreId.Length, StoreId, IntPtr.Zero, (uint) flags, out storeHandle);
                Transmogrify.CheckHResult (hr);
                return new MsgStore (storeHandle);

            } catch (Exception e) {
                throw new NotImplementedException ("Error opening Message Store", e);
            }
        }

        public IMsgStore PrivateStore {
            get {
                /* XXX hack - needs OpenStoreFlags completion for MDB_NO_DIALOG | MAPI_BEST_ACCESS */
                return OpenStore ((NMapi.Flags.OpenStoreFlags)0x11, null, false);
            }
        }

        public IMsgStore PublicStore {
            get {
                /* XXX hack - needs OpenStoreFlags completion for MDB_NO_DIALOG | MAPI_BEST_ACCESS */
                return OpenStore ((NMapi.Flags.OpenStoreFlags)0x11, null, true);
            }
        }

		public Address[] AbGetUserList (int flags)
		{
			throw new MapiNoSupportException (); // TODO!
		}
		
		public Address AbGetUserData (byte[] entryId)
		{
			throw new MapiNoSupportException (); // TODO!
		}
		
		public DateTime AbGetChangeTime (int flags)
		{
			throw new MapiNoSupportException (); // TODO!
		}
		
		public Address AbGetUserDataBySmtpAddress (string smtpAddress)
		{
			throw new MapiNoSupportException (); // TODO!
		}
		
		public Address AbGetUserDataByInternalAddress (string internalAddress)
		{
			throw new MapiNoSupportException (); // TODO!
		}

        #region C-Glue
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_GetPrivateStore (IntPtr session, out IntPtr store);

        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_GetPublicStore (IntPtr session, out IntPtr store);



        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_GetLastError (IntPtr session,
															 int hResult,
															 uint ulFlags,
															 out IntPtr lppMAPIError);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_GetMsgStoresTable (IntPtr session,
																  uint ulFlags,
																  out IntPtr lppTable);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_OpenMsgStore (IntPtr session,
															 uint ulUIParam,
															 uint cbEntryID,
															 byte[] lpEntryID,
															 IntPtr lpInterface,
															 uint ulFlags,
															 out IntPtr lppMDB);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_OpenAddressBook (IntPtr session,
																uint ulUIParam,
																IntPtr lpInterface,
																uint ulFlags,
																out IntPtr lppAdrBook);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_OpenProfileSection (IntPtr session,
																   IntPtr lpUID,
																   IntPtr lpInterface,
																   uint ulFlags,
																   out IntPtr lppProfSect);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_GetStatusTable (IntPtr session,
															   uint ulFlags,
															   out IntPtr lppTable);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_OpenEntry (IntPtr session,
														  uint cbEntryID,
														  byte[] lpEntryID,
														  IntPtr lpInterface,
														  uint ulFlags,
														  out uint lpulObjType,
														  out IntPtr lppUnk);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_CompareEntryIDs (IntPtr session,
																uint cbEntryID1,
																byte[] lpEntryID1,
																uint cbEntryID2,
																byte[] lpEntryID2,
																uint ulFlags,
																out uint lpulResult);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_Advise (IntPtr session,
													   uint cbEntryID,
													   byte[] lpEntryID,
													   uint ulEventMask,
													   IntPtr lpAdviseSink,
													   out uint lpulConnection);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_Unadvise (IntPtr session,
														 uint ulConnection);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_MessageOptions (IntPtr session,
															   uint ulUIParam,
															   uint ulFlags,
															   IntPtr lpszAdrType /* LPTSTR */,
															   IntPtr lpMessage);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_QueryDefaultMessageOpt (IntPtr session,
																	   IntPtr lpszAdrType /* LPTSTR */,
																	   uint ulFlags,
																	   out uint lpcValues,
																	   out IntPtr lppOptions /* LSPropValue */);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_EnumAdrTypes (IntPtr session,
															 uint ulFlags,
															 out uint lpcAdrTypes,
															 out IntPtr lpppszAdrTypes /* LPTSTR** */);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_QueryIdentity (IntPtr session,
															  out uint lpcbEntryID,
															  out IntPtr lppEntryID /* byte[]* */);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_Logoff (IntPtr session,
													   uint ulUIParam,
													   uint ulFlags,
													   uint ulReserved);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_SetDefaultStore (IntPtr session,
																uint ulFlags,
																uint cbEntryID,
																byte[] lpEntryID);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_AdminServices (IntPtr session,
															  uint ulFlags,
															  out IntPtr lppServiceAdmin /* LPSERVICEADMIN* */);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_ShowForm (IntPtr session,
														 uint ulUIParam,
														 IntPtr lpMsgStore,
														 IntPtr lpParentFolder,
														 IntPtr lpInterface,
														 uint ulMessageToken,
														 IntPtr lpMessageSent,
														 uint ulFlags,
														 uint ulMessageStatus,
														 uint ulMessageFlags,
														 uint ulAccess,
														 IntPtr lpszMessageClass /* LPTSTR */);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Session_PrepareForm (IntPtr session,
															IntPtr lpInterface,
															IntPtr lpMessage /* LPMESSAGE */,
															out uint lpulMessageToken);
        #endregion
    }
}
