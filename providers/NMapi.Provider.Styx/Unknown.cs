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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NMapi.Provider.Styx
{
	
	public class Unknown : IDisposable, IBase {
		protected IntPtr cobj;

        public IntPtr nativeObject {
            get { return cobj; }
        }
		
        private bool disposed = false;

		public Unknown (IntPtr obj) {
            Wrap (obj);
		}

        protected Unknown () {
            cobj = IntPtr.Zero;
        }

        protected void Wrap (IntPtr obj) {
            this.cobj = obj;
            int foo = (int) obj;
            Trace.WriteLine (String.Format ("Wrapping Object: {0:x} {1}", foo, this));
        }

        //
        public uint AddRef () {

            if (cobj == IntPtr.Zero) {
                throw new NullReferenceException ("No underlying wrapped object");
            }

            return CMapi_Unknown_AddRef (cobj);
        }
        
        public uint Release () {

            if (cobj == IntPtr.Zero) {
                throw new NullReferenceException ("No underlying wrapped object");
            }

            return CMapi_Unknown_Release (cobj);
        }
        
        // IDisposable Implementation
        public void Dispose () {
            Dispose (true);
            GC.SuppressFinalize (this);
        }
        
        private void Dispose (bool disposing) {
  
            if (this.disposed == false) {
                Trace.WriteLine (String.Format ("Unwrapping Object: {0:x} {1}", (int) cobj, this));

                if (cobj != IntPtr.Zero) {
                    CMapi_Unknown_Release (cobj);
                    cobj = IntPtr.Zero;
                }

                disposed = true;
            }
        }

        #region IBase Members

        public void Close () {
            Dispose ();
        }

        #endregion
        
        ~Unknown () {
            Dispose (false);
        }
        
        [DllImport ("libcmapi")]
        private static extern uint CMapi_Unknown_AddRef (IntPtr obj);

        [DllImport ("libcmapi")]
        private static extern uint CMapi_Unknown_Release (IntPtr obj);
    }
}
