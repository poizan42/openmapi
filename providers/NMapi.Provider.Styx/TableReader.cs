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
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Provider.Styx.Interop;

namespace NMapi.Provider.Styx {
    
    public class TableReaderWrapped : IMapiTableReader {
        Table table;

        public TableReaderWrapped (Table table) {
            this.table = table;
        }

        #region IMapiTableReader Members

        public RowSet GetRows (int cRows) {
            return table.QueryRows (cRows, 0);
        }

        public PropertyTag[] GetTags () {
            return table.QueryColumns (0);
        }

        #endregion

        #region IBase Members

        public void Close () {
            table.Close ();
        }

        public void Dispose () {
            table.Dispose ();
        }

        #endregion
    }

    public class TableReaderNative: Unknown, IMapiTableReader {

        public TableReaderNative (IntPtr TableReaderHandle)
            : base (TableReaderHandle) {

        }

        #region IMapiTableReader Members

        public RowSet GetRows (int cRows) {
            IntPtr RowSetHandle;
            int hr = CMapi_TableReader_GetRows (cobj, (uint) cRows, out RowSetHandle);

            if (hr != 0x40380)
                Transmogrify.CheckHResult (hr);

            RowSet rows = Transmogrify.PtrToRowSet (RowSetHandle);
            CMapi.FreeBuffer(RowSetHandle);
            return rows;
        }

        public PropertyTag[] GetTags () {
            IntPtr TagArrayHandle;
            int hr = CMapi_TableReader_GetTags (cobj, out TagArrayHandle);
            Transmogrify.CheckHResult (hr);
            PropertyTag[] tags = Transmogrify.PtrToTagArray (TagArrayHandle);
            CMapi.FreeBuffer(TagArrayHandle);
            return tags;
        }

        [DllImport ("libcmapi")]
        public static extern int CMapi_TableReader_GetTags (IntPtr TableReaderHandle,
                                                            out IntPtr TagArrayHandle);

        [DllImport ("libcmapi")]
        public static extern int CMapi_TableReader_GetRows (IntPtr TableReaderHandle,
                                                            uint Count,
                                                            out IntPtr RowSetHandle);
        #endregion
    }

}
