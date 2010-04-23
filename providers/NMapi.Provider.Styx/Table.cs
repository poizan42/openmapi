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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Provider.Styx.Interop;


#pragma warning disable 0169

namespace NMapi.Provider.Styx {

    public class Table : Unknown, IMapiTable {

        private ObjectEventSet eventSet;

        public Table (IntPtr obj) : base (obj) {

        }

        #region IMapiTable Membsers

        public void Abort () {
            int hr = CMapi_Table_Abort (cobj);
            Transmogrify.CheckHResult (hr);
        }

        public EventConnection Advise (Flags.NotificationEventType eventMask, Events.IMapiAdviseSink sink) {
            return Advise (null, eventMask, sink);
        }

        public EventConnection Advise (byte[] ignored, Flags.NotificationEventType eventMask, Events.IMapiAdviseSink sink) {
            uint Connection;
            AdviseBridge bridge = new AdviseBridge (sink);
            int hr = CMapi_Table_Advise (cobj, (uint) eventMask, bridge.SinkPtr, out Connection);

            /* XXX hack to reduce flags to supported one(s) on error */
            if((uint)hr == 0x80070057) {
                NotificationEventType newEvMask = eventMask & NotificationEventType.TableModified;
                hr = CMapi_Table_Advise (cobj, (uint) newEvMask, bridge.SinkPtr, out Connection);
            }
            /* XXX end of hack */

            Transmogrify.CheckHResult (hr);
            return new EventConnection ((int) Connection);
        }

        public int CollapseRow (byte[] instanceKey, int flags) {
            throw new NotImplementedException ();
        }

        public int CreateBookmark () {
            throw new NotImplementedException ();
        }

        public Events.ObjectEventSet Events {
            get {
                if (eventSet == null)
                    eventSet = new ObjectEventSet (this, null);
                return eventSet;
            }
        }

        public ExpandRowResult ExpandRow (byte[] instanceKey, int rowCount, int flags) {
            throw new NotImplementedException ();
        }

        public void FindRow (Restriction restriction, int numerator, int denominator) {
            using (MemContext MemCtx = new MemContext ()) {

                IntPtr ResHandle = Transmogrify.RestrictionToPointer (restriction, MemCtx);
                Console.WriteLine ("XX: {0} {1}", numerator, denominator);
                int hr = CMapi_Table_FindRow (cobj, ResHandle, (uint) numerator, (uint) denominator);
                Transmogrify.CheckHResult (hr);
            }
        }

        public void FreeBookmark (int position) {
            throw new NotImplementedException ();
        }

        public byte[] GetCollapseState (int flags, byte[] instanceKey) {
            throw new NotImplementedException ();
        }

        public MapiError GetLastError (int hresult, int flags) {
            IntPtr ErrorHandle;
            int hr = CMapi_Table_GetLastError (cobj, hresult, (uint) flags, out ErrorHandle);
            Transmogrify.CheckHResult (hr);
            MapiError error = Transmogrify.PtrToMapiError (ErrorHandle);
            CMapi.FreeBuffer(ErrorHandle);
            return error;
        }

        public int GetRowCount (int flags) {
            uint count;
            int hr = CMapi_Table_GetRowCount (cobj, (uint) flags, out count);
            Transmogrify.CheckHResult (hr);
            return (int) count;
        }

        public PropertyTag[] QueryColumns (int flags) {

            IntPtr TagArrayHandle;
            PropertyTag[] ret;

            int hr = CMapi_Table_QueryColumns (cobj, (uint) flags, out TagArrayHandle);
            Transmogrify.CheckHResult (hr);

            ret = Transmogrify.PtrToTagArray (TagArrayHandle);
            CMapi.FreeBuffer(TagArrayHandle);
            return ret;
        }

        public QueryPositionResult QueryPosition () {
            uint RowPos, Numerator, Denominator;

            int hr = CMapi_Table_QueryPosition (cobj, out RowPos, out Numerator, out Denominator);
            Transmogrify.CheckHResult (hr);

            QueryPositionResult res = new QueryPositionResult ();
            res.Denominator = (int) Denominator;
            res.Numerator = (int) Numerator;
            res.Row = (int) RowPos;

            return res;
        }

        public RowSet QueryRows (int rowCount, int flags) {
            IntPtr RowHandle;
            RowSet ret;

            int hr = CMapi_Table_QueryRows (cobj, rowCount, (uint) flags, out RowHandle);
            Transmogrify.CheckHResult (hr);

            ret = Transmogrify.PtrToRowSet (RowHandle);
            CMapi.FreeProws(RowHandle);
            return ret;
        }

        public SortOrderSet QuerySortOrder () {
            throw new NotImplementedException ();
        }

        public void Restrict (Restriction restriction, int flags) {
            using (MemContext MemCtx = new MemContext ()) {

                IntPtr ResHandle = Transmogrify.RestrictionToPointer (restriction, MemCtx);
                int hr = CMapi_Table_Restrict (cobj, ResHandle, (uint) flags);
                Transmogrify.CheckHResult (hr);
            }
        }

        public int SeekRow (int bkOrigin, int rowCount) {
            int RowsSought;
            int hr = CMapi_Table_SeekRow (cobj, (uint) bkOrigin, rowCount, out RowsSought);
            Transmogrify.CheckHResult (hr);
            return RowsSought;
        }

        public void SeekRowApprox (int numerator, int denominator) {
            int hr = CMapi_Table_SeekRowApprox (cobj, (uint) numerator, (uint) denominator);
            Transmogrify.CheckHResult (hr);
        }

        public int SetCollapseState (int flags, byte[] collapseState) {
            throw new NotImplementedException ();
        }

        public void SetColumns (PropertyTag[] propTagArray, int flags) {

            using (MemContext MemCtx = new MemContext ()) {
                IntPtr TagArrayHandle = Transmogrify.TagArrayToPtr (propTagArray, MemCtx);

                int hr = CMapi_Table_SetColumns (cobj, TagArrayHandle, (uint) flags);
                Transmogrify.CheckHResult (hr);
            }
        }

        public void SortTable (SortOrderSet Criteria, int flags) {

            using (MemContext MemCtx = new MemContext ()) {

                IntPtr SortCriteria = Transmogrify.SortOrderSetToPtr (Criteria, MemCtx);
                int hr = CMapi_Table_SortTable (cobj, SortCriteria, (uint) flags);
                Transmogrify.CheckHResult (hr);

            }
        }

        public GetStatusResult Status {
            get { throw new NotImplementedException (); }
        }

        public void Unadvise (EventConnection txcOutlookHackConnection) {
            uint connection = (uint) txcOutlookHackConnection.Connection;
            int hr = CMapi_Table_Unadvise (cobj, connection);
            Transmogrify.CheckHResult (hr);
            AdviseBridge.RemoveBridge (connection);
            //XXX FIXME leaking the advise Bridge
        }

        public int WaitForCompletion (int flags, int timeout) {
            throw new NotImplementedException ();
        }

        #endregion


        #region C-Glue
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_GetLastError (IntPtr table,
															int hResult,
															uint ulFlags,
															out IntPtr lppMAPIError);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_Advise (IntPtr table,
													  uint ulEventMask,
													  IntPtr lpAdviseSink,
													  out uint lpulConnection);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_Unadvise (IntPtr table,
														uint ulConnection);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_GetStatus (IntPtr table,
														 out uint lpulTableStatus,
														 out uint lpulTableType);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_SetColumns (IntPtr table,
														  IntPtr lpPropTagArray,
														  uint ulFlags);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_QueryColumns (IntPtr table,
															uint ulFlags,
															out IntPtr lpPropTagArray);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_GetRowCount (IntPtr table,
														   uint ulFlags,
														   out uint lpulCount);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_SeekRow (IntPtr table,
													   uint bkOrigin,
													   int lRowCount,
													   out int lplRowsSought);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_SeekRowApprox (IntPtr table,
															 uint ulNumerator,
															 uint ulDenominator);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_QueryPosition (IntPtr table,
															 out uint lpulRow,
															 out uint lpulNumerator,
															 out uint lpulDenominator);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_FindRow (IntPtr table,
													   IntPtr lpRestriction,
													   uint bkOrigin,
													   uint ulFlags);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_Restrict (IntPtr table,
														IntPtr lpRestriction,
														uint ulFlags);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_CreateBookmark (IntPtr table,
															  out uint lpbkPosition);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_FreeBookmark (IntPtr table,
															uint bkPosition);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_SortTable (IntPtr table,
														 IntPtr lpSortCriteria,
														 uint ulFlags);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_QuerySortOrder (IntPtr table,
															  out IntPtr lppSortCriteria);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_QueryRows (IntPtr table,
														 int lRowCount,
														 uint ulFlags,
														 out IntPtr lppRows);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_Abort (IntPtr table);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_ExpandRow (IntPtr table,
														 uint cbInstanceKey,
														 byte[] pbInstanceKey,
														 uint ulRowCount,
														 uint ulFlags,
														 IntPtr lppRows,
														 out uint lpulMoreRows);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_CollapseRow (IntPtr table,
														   uint cbInstanceKey,
														   byte[] pbInstanceKey,
														   uint ulFlags,
														   out uint lpulRowCount);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_WaitForCompletion (IntPtr table,
																 uint ulFlags,
																 uint ulTimeout,
																 out uint lpulTableStatus);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_GetCollapseState (IntPtr table,
																uint ulFlags,
																uint cbInstanceKey,
																byte[] lpbInstanceKey,
																out uint lpcbCollapseState,
																IntPtr lppbCollapseState);
        [DllImport ("libcmapi")]
        private static extern int CMapi_Table_SetCollapseState (IntPtr table,
																uint ulFlags,
																uint cbCollapseState,
																byte[] pbCollapseState,
																out uint lpbkLocation);
        #endregion


    }
}
