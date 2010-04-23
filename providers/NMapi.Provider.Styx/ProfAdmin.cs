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
using NMapi.Events;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Provider.Styx.Interop;

namespace NMapi.Provider.Styx {
    public class ProfAdmin : Unknown {

        internal ProfAdmin (IntPtr cobj) : base (cobj) {

        }

        public IMapiTable GetProfileTable () {

            IntPtr tableHandle;
            int hr = CMapi_ProfAdmin_GetProfileTable (cobj, 0, out tableHandle);
            Transmogrify.CheckHResult (hr);
            return new Table (tableHandle);
        }

        public void CreateProfile (string Name, string password, uint UiFlags, uint Flags) {
            int hr = CMapi_ProfAdmin_CreateProfile (cobj, Name, password, UiFlags, Flags);
            Transmogrify.CheckHResult (hr);
        }

        public void DeleteProfile (string Name, uint Flags) {
            int hr = CMapi_ProfAdmin_DeleteProfile (cobj, Name, Flags);
            Transmogrify.CheckHResult (hr);
        }

        [DllImport ("libcmapi")]
        private static extern int CMapi_ProfAdmin_GetLastError (IntPtr table,
                                                                int hResult,
                                                                uint ulFlags,
                                                                out IntPtr lppMAPIError);

        [DllImport ("libcmapi")]
        private static extern int CMapi_ProfAdmin_GetProfileTable (IntPtr admin, uint ulFlags, out IntPtr table);

        [DllImport ("libcmapi", CharSet=CharSet.Auto)]
        private static extern int CMapi_ProfAdmin_CreateProfile (IntPtr admin,
                                                                 string lpszProfileName,
                                                                 string lpszPassword,
                                                                 uint ulUIParam,
                                                                 uint ulFlags);

        [DllImport ("libcmapi", CharSet=CharSet.Auto)]
        private static extern int CMapi_ProfAdmin_DeleteProfile (IntPtr admin,
                                                                 string lpszProfileName,
                                                                 uint ulFlags);
    }
}
