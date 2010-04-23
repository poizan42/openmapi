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
using System.Diagnostics;

namespace NMapi.Provider.Styx.Interop {

    public class MemContext : IDisposable {

        private bool disposed = false;
        private List<IntPtr> slots = new List<IntPtr> ();
        
        public IntPtr Alloc<T> (int count) {
            int size = count * Marshal.SizeOf (typeof (T));
            IntPtr memory = Marshal.AllocHGlobal (size);
            slots.Add (memory);
            return memory;
        }

        public IntPtr Alloc (int size) {
            IntPtr memory = Marshal.AllocHGlobal (size);
            slots.Add (memory);
            return memory;
        }

        public IntPtr StringDup (string str, bool AsAnsi) {
            IntPtr memory;
            
            if (AsAnsi) {
                memory = Marshal.StringToHGlobalAnsi (str);
            } else {
                memory = Marshal.StringToHGlobalUni (str);
            }

            slots.Add (memory);
            return memory;
        }

        public void Dispose (bool disposing) {

            if (disposed)
                return;
            
            Trace.WriteLine (String.Format ("Freeing Memory {0}", this));
            
            foreach (IntPtr pointer in slots) {
                Marshal.FreeHGlobal (pointer);
            }

            disposed = true;

            if (disposing) {
                slots = null;
            }
        }

        public void Dispose () {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        ~MemContext () {
            Dispose (false);
        }
    }
}
