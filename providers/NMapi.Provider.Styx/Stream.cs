/* 
 *  NMapy.Styx - The Border between C and C#
 *
 *  Copyright (C) Michael Kukat <michael.kukat@topalis.com> 
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
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NMapi.Provider.Styx
{
	
	public class Stream : IDisposable, IStream {
            protected IntPtr cobj;

            private const int BLOCKSIZE = 32768;

        public IntPtr nativeObject {
            get { return cobj; }
        }

        public bool IsText { get { return false; } }

        private bool disposed = false;

		public Stream (IntPtr obj) {
            Wrap (obj);
		}

        protected Stream () {
            cobj = IntPtr.Zero;
        }

        protected void Wrap (IntPtr obj) {
            this.cobj = obj;
            int foo = (int) obj;
            Trace.WriteLine (String.Format ("Wrapping Object: {0:x} {1}", foo, this));
        }

        public void Clone(out Stream newStream) {
            IntPtr ppstm = IntPtr.Zero;
            newStream = null;

            CMapi_Stream_Clone(cobj, out ppstm);

            if(ppstm != IntPtr.Zero) newStream = new Stream(ppstm);
        }

        public void Commit(int grfCommitFlags) {
            CMapi_Stream_Commit(cobj, grfCommitFlags);
        }

        public void CopyTo(Stream pstm, long cb, out long pcbRead, out long  pcbWritten) {
            CMapi_Stream_CopyTo(cobj, pstm.nativeObject, cb, out pcbRead, out pcbWritten);
        }

        public void LockRegion(ulong libOffset, long cb, int dwLockType) {
            CMapi_Stream_LockRegion(cobj, libOffset, cb, dwLockType);
        }

        public byte[] Read(int len) {
            byte[] pv = new byte[len];
            ulong pcbRead;

            CMapi_Stream_Read(cobj, pv, (ulong) len, out pcbRead);

            if(pcbRead <= 0) return null;

            if(pcbRead < (ulong) len) {
                byte[] ret = new byte[pcbRead];
                Buffer.BlockCopy(pv, 0, ret, 0, (int) pcbRead);
                return ret;
            } else return pv;
        }

        public void GetData (System.IO.Stream destination)
        {
            
            byte [] buffer;
            while (true) {
                buffer = Read (BLOCKSIZE);
                if (buffer == null)
                    break;
                destination.Write (buffer, 0, buffer.Length);
            }
            destination.Flush ();
        }

        public void Revert() {
            CMapi_Stream_Revert(cobj);
        }

        public void Seek(long dlibMove, int dwOrigin, out long plibNewPosition) {
            CMapi_Stream_Seek(cobj, dlibMove, dwOrigin, out plibNewPosition);
        }

        public void SetSize(ulong libNewSize) {
            CMapi_Stream_SetSize(cobj, libNewSize);

        }

        public void Stat(out IntPtr pstatstg, int grfStatFlag) {
            /* XXX */
            CMapi_Stream_Stat(cobj, out pstatstg, grfStatFlag);
        }

        public void UnlockRegion(ulong libOffset, long cb, int dwLockType) {
            CMapi_Stream_UnlockRegion(cobj, libOffset, cb, dwLockType);
        }

        public void Write(byte[] pc) {
            ulong pcbWritten;

            CMapi_Stream_Write(cobj, pc, (ulong) pc.Length, out pcbWritten);

            if((ulong) pc.Length >= pcbWritten) throw(MapiException.Make("IStream::Write() didn't write whole buffer"));
        }

        public void PutData (System.IO.Stream source)
        {
            byte [] buffer = new byte [BLOCKSIZE];
            int len;
            while (true) {
                len = source.Read (buffer, 0, buffer.Length);
                if (len <= 0)
                    break;
                Write (buffer);
            }
        }

        //
        public uint AddRef () {

            if (cobj == IntPtr.Zero) {
                throw new NullReferenceException ("No underlying wrapped object");
            }

            return CMapi_Stream_AddRef (cobj);
        }
        
        public uint Release () {

            if (cobj == IntPtr.Zero) {
                throw new NullReferenceException ("No underlying wrapped object");
            }

            return CMapi_Stream_Release (cobj);
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
                    CMapi_Stream_Release (cobj);
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
        
        ~Stream () {
            Dispose (false);
        }
        
        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_Clone(IntPtr stream, out IntPtr ppstm);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_Commit(IntPtr stream, int grfCommitFlags);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_CopyTo(IntPtr stream, IntPtr pstm, long cb, out long pcbRead, out long  pcbWritten);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_LockRegion(IntPtr stream, ulong libOffset, long cb, int dwLockType);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_Read(IntPtr stream, byte[] pv, ulong cb, out ulong pcbRead);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_Revert(IntPtr stream);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_Seek(IntPtr stream, long dlibMove, int dwOrigin, out long plibNewPosition);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_SetSize(IntPtr stream, ulong libNewSize);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_Stat(IntPtr stream, out IntPtr pstatstg, int grfStatFlag);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_UnlockRegion(IntPtr stream, ulong libOffset, long cb, int dwLockType);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_Write(IntPtr stream, byte[] pc, ulong cb, out ulong pcbWritten);

        [DllImport ("libcmapi")]
        private static extern void CMapi_Stream_QueryInterface(IntPtr stream, IntPtr riid, out IntPtr ppvObject);

        [DllImport ("libcmapi")]
        private static extern uint CMapi_Stream_AddRef(IntPtr stream);

        [DllImport ("libcmapi")]
        private static extern uint CMapi_Stream_Release(IntPtr stream);
    }
}
