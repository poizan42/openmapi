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


namespace NMapi.Provider.Styx
{
    public class Container : Prop, IMapiContainer
    {        
        internal Container (IntPtr cobj) : base (cobj) {
            
        }

        public IMapiTable GetContentsTable (int flags) {
            IntPtr TableHandle;

            int hr = CMapi_Container_GetContentsTable (cobj, (uint) flags, out TableHandle);
            Transmogrify.CheckHResult (hr);
            Table table = new Table (TableHandle);

            return table;
        }

        public IMapiTableReader GetHierarchyTable (int flags) {
            IntPtr TableHandle;
            IMapiTableReader reader;

            int hr;
            if (CMapi.IsNative) {
                hr = CMapi_Container_GetHierarchyTable (cobj, (uint) flags, out TableHandle);
            } else {
                hr = CMapi_Container_UMAPI_GetHierarchyTable (cobj, (uint) flags, out TableHandle);
            }

            Transmogrify.CheckHResult (hr);

            if (CMapi.IsNative) {
                Table table = new Table (TableHandle);
                reader = new TableReaderWrapped (table);
            } else {
                reader = new TableReaderNative (TableHandle);
            }

            return reader;
        }

        public GetSearchCriteriaResult GetSearchCriteria (int flags) {

            IntPtr RestrictionHandle;
            IntPtr ContailerListHandle;
            uint SearchState;

            int hr = CMapi_Container_GetSearchCriteria (cobj, (uint) flags, out RestrictionHandle, out ContailerListHandle, out SearchState);
            Transmogrify.CheckHResult (hr);

            GetSearchCriteriaResult res = new GetSearchCriteriaResult ();
            res.ContainerList = Transmogrify.PtrToEntryList (ContailerListHandle);
            res.SearchState = (int) SearchState;
            res.Restriction = Transmogrify.PtrToRestriction (RestrictionHandle);

            return res;
        }

        public IBase OpenEntry (byte[] entryID, NMapiGuid interFace, int flags) {

            using (MemContext MemCtx = new MemContext ()) {
                IntPtr Handle;
                uint ObjTypeId;

                uint EidSize = entryID == null ? 0 : (uint) entryID.Length;
                int hr = CMapi_Container_OpenEntry (cobj, EidSize, entryID, IntPtr.Zero, (uint) flags, out ObjTypeId, out Handle);
                Transmogrify.CheckHResult (hr);

                IBase entry = CMapi.CreateObjectForType (Handle, ObjTypeId);

                return entry;
            }
        }

        public IBase OpenEntry (byte[] entryID) {
            return OpenEntry (entryID, null, 16 /* XXX MAPI_BEST_ACCESS */);
        }

        public void SetSearchCriteria (Restriction restriction, EntryList containerList, int searchFlags) {
            using (MemContext MemCtx = new MemContext ()) {
                IntPtr lpRestriction;
                IntPtr lpContainerList;

                lpRestriction = Transmogrify.RestrictionToPointer(restriction, MemCtx);
		lpContainerList = Transmogrify.EntryListToPtr(containerList, MemCtx);
                int hr = CMapi_Container_SetSearchCriteria (cobj, lpRestriction, lpContainerList, (uint) searchFlags);
                Transmogrify.CheckHResult (hr);
            }
        }


        #region C-Glue
        [DllImport ("libcmapi")]
        public static extern int CMapi_Container_GetContentsTable (IntPtr      container,
																   uint        ulFlags,
																   out IntPtr  lppTable);

        [DllImport ("libcmapi")]
        public static extern int CMapi_Container_GetHierarchyTable (IntPtr     container,
																	uint       ulFlags,
																	out IntPtr lppTable);

        [DllImport ("libcmapi")]
        public static extern int CMapi_Container_UMAPI_GetHierarchyTable (IntPtr container,
                                                                          uint ulFlags,
                                                                          out IntPtr lppTable);

        [DllImport ("libcmapi")]
        public static extern int CMapi_Container_OpenEntry (IntPtr container,
															uint           cbEntryID,
															byte[]       lpEntryID,
															IntPtr          lpInterface,
															uint           ulFlags,
															out uint          lpulObjType,
															out IntPtr      lppUnk);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Container_SetSearchCriteria (IntPtr container,
																	IntPtr  lpRestriction,
																	IntPtr     lpContainerList,
																	uint           ulSearchFlags);
        [DllImport ("libcmapi")]
        public static extern int CMapi_Container_GetSearchCriteria (IntPtr container,
																	uint           ulFlags,
																	out IntPtr lppRestriction,
																	out IntPtr    lppContainerList,
																	out uint          lpulSearchState);

        #endregion


    }
}
