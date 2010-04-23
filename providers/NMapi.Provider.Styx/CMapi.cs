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

using NMapi.Provider.Styx.Interop;

namespace NMapi.Provider.Styx {
    public class CMapi {

        [Flags]
        public enum InitFlags : uint {
            MultithreadNotifications = 1,
            NoCoInit                 = 8,
            NTService                = 16
        }

        [Flags]
        public enum LogonFlags: uint {

            UI              = 0x00000001,  /* Display logon UI                 */
            NewSession      = 0x00000002,  /* Don't use shared session         */
            AllowOthers     = 0x00000008,  /* Make this a shared session       */
            ExplicitProfile = 0x00000010,  /* Don't use default profile        */
            Extended        = 0x00000020,  /* Extended MAPI Logon              */
            ForceDownload   = 0x00001000,  /* Get new mail before return       */
            ServiceUIAlways = 0x00002000,  /* Do logon UI in all providers     */
            NoMail          = 0x00008000,  /* Do not activate transports       */
            NTService       = 0x00010000,  /*Allow logon from an NT service  */

            PasswordUI      = 0x00020000,  /* Display password UI only         */
            TimeoutShort    = 0x00100000,  /* Minimal wait for logon resources */
            Unicode         = 0x80000000
        }

        public enum Flavour {
            Native = 1,
            UMapi  = 2
        }


        static IntPtr obj = new IntPtr (0);
        static object mutex = new Object ();
        static int refcount = 0;

        public struct DllInfo {
            public bool IsOutlook;
            public string path;
        }

        public static List<DllInfo> FindDlls () {
            OperatingSystem os = Environment.OSVersion;
            List<DllInfo> dlls;

            switch (os.Platform) {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    dlls = FindDllsWindows ();
                    break;

                case PlatformID.Unix:
// FIXME: this ID doesn't exist under .NET 3.5
//                case PlatformID.MacOSX:
                    dlls = FindDllsUnix ();
                    break;

                default:
                    throw new NotSupportedException ("Platform not support");
            }

            return dlls;
        }

        public static List<DllInfo> FindDllsUnix () {
            List<DllInfo> dlls = new List<DllInfo> ();

            //XXX Obviously
            DllInfo dll = new DllInfo ();
            dll.path = "/Users/gicmo/Coding/ROOT/lib/libumapi.0.dylib";
            dlls.Add (dll);

            return dlls;
        }

        public static List<DllInfo> FindDllsWindows () {
            Microsoft.Win32.RegistryKey key;

            key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey ("SOFTWARE\\Clients\\Mail", false);
            List<DllInfo> dlls = new List<DllInfo> ();
            if (key == null) {
                return dlls;
            }

            string[] subkeys = key.GetSubKeyNames ();
            key.Close ();

            foreach (string subkey in subkeys) {
                DllInfo CurrentDll;
                CurrentDll.IsOutlook = subkey == "Microsoft Outlook";

                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey ("SOFTWARE\\Clients\\Mail\\" + subkey, false);

                if (key == null) {
                    continue;
                }

                object val = key.GetValue ("DLLPathEx");
                key.Close ();

                if (val != null && val is string) {
                    CurrentDll.path = (string) val;

                    if (CurrentDll.IsOutlook) {
                        dlls.Insert (0, CurrentDll);
                    } else {
                        dlls.Add (CurrentDll);
                    }
                }
            }

            return dlls;
        }

        public static void Initialize (InitFlags flags) {
            lock (mutex) {

                refcount++;

                if (obj != IntPtr.Zero) {
                    return;
                }

                List<DllInfo> dlls = FindDlls ();

                int hr = -1;
                foreach (DllInfo dll in dlls) {
                    hr = CMapi_Initialize (out obj, dll.path, flags);
                    if (hr == 0)
                        break;
                }

                if (hr != 0) {
                    Transmogrify.CheckHResult (hr);
                }
            }
        }

        public static void Uninitialize () {

            lock (mutex) {

                if (--refcount > 0) {
                    return;
                }

                if (obj == IntPtr.Zero) {
                    return;
                }

                System.Console.WriteLine ("Uninitializing");
                CMapi_Uninitialize (obj);
                System.Console.WriteLine ("Done.");

            }
        }

        public static IntPtr LogonEx (string ProfileName, string Password, LogonFlags flags) {
            IntPtr sessionHandle;

            int hr = CMapi_LogonEx (obj, 0, ProfileName, Password, (uint) flags, out sessionHandle);
            Transmogrify.CheckHResult (hr);
            return sessionHandle;
        }

        public static IntPtr LogonU (string Host, string User, string Password, LogonFlags flags) {

            int hr;
            IntPtr SessionHandle;

            if ((flags & ~LogonFlags.Unicode) != 0) {
                throw MapiException.Make (80040106);
            }

            if ((flags & LogonFlags.Unicode) == LogonFlags.Unicode) {
                hr = CMapi_ULogonW (obj, Host, User, Password, out SessionHandle);
            } else {
                hr = CMapi_ULogonA (obj, Host, User, Password, out SessionHandle);
            }

            Transmogrify.CheckHResult (hr);

            return SessionHandle;
        }



        internal static IBase CreateObjectForType (IntPtr Handle, uint ObjTypeId) {
            return CreateObjectForType (Handle, (ObjectTypes) ObjTypeId);
        }

        internal static IBase CreateObjectForType (IntPtr Handle, ObjectTypes ObjTypeId) {

            IBase entry = null;
            switch (ObjTypeId) {
                case ObjectTypes.Folder:
                    entry = new Folder (Handle);
                    break;

                case ObjectTypes.Message:
                    entry = new Message (Handle);
                    break;

                case ObjectTypes.Session:
                    entry = new Session (Handle);
                    break;

                case ObjectTypes.Store:
                    entry = new MsgStore (Handle);
                    break;

                case ObjectTypes.Attachment:
                    entry = new Attachment (Handle);
                    break;

                default:
                    System.Console.WriteLine ("Warning: Unkown Obj Type {0}", ObjTypeId);
                    entry = new Unknown (Handle);
                    break;
            }
            return entry;
        }


        public ProfAdmin AdminProfiles () {
            int hr;
            IntPtr ProfAdminHandle;

            hr = CMapi_AdminProfiles (obj, 0, out ProfAdminHandle);

            Transmogrify.CheckHResult (hr);

            return new ProfAdmin (ProfAdminHandle);
        }

        public static bool IsNative {
            get {
                return CMapi_IsNative (obj) != 0;
            }
        }

        public static void FreeBuffer (IntPtr buffer) {
            int scode = CMapi_FreeBuffer (obj, buffer);
            Transmogrify.CheckHResult (scode);
        }

        public static void FreeProws (IntPtr lpRows) {
            int scode = CMapi_FreeProws (obj, lpRows);
            Transmogrify.CheckHResult (scode);
        }

        public static IntPtr AllocBuffer (uint size) {
            IntPtr buffer;
            int scode = CMapi_AllocateBuffer (obj, size, out buffer);
            Transmogrify.CheckHResult (scode);
            return buffer;
        }

        public static IntPtr AllocMore (IntPtr buffer, uint size) {
            IntPtr NewBuffer;
            int scode = CMapi_AllocateMore (obj, size, buffer, out NewBuffer);
            Transmogrify.CheckHResult (scode);
            return NewBuffer;
        }

        #region C-Glue

        [DllImport ("libcmapi", CharSet = CharSet.Auto)]
        public static extern int CMapi_Initialize (out IntPtr obj, [MarshalAs (UnmanagedType.LPStr)] string dll, InitFlags flags);

        [DllImport ("libcmapi")]
        private static extern int CMapi_Uninitialize (IntPtr obj);

        [DllImport ("libcmapi")]
        private static extern uint CMapi_IsNative (IntPtr obj);

        [DllImport ("libcmapi")]
        private static extern int CMapi_AdminProfiles (IntPtr obj, uint flags, out IntPtr ProfAdmin);

        [DllImport ("libcmapi")]
        private static extern int CMapi_LogonEx (IntPtr obj,
                                                 System.UInt32 UIParam,
                                                 [MarshalAs (UnmanagedType.LPStr)] string ProfileName,
                                                 [MarshalAs (UnmanagedType.LPStr)] string Password,
                                                 System.UInt32 flags,
                                                 out IntPtr handle);

        [DllImport ("libcmapi")]
        private static extern int CMapi_ULogonW (IntPtr obj,
                                                 [MarshalAs (UnmanagedType.LPStr)] string Host,
                                                 [MarshalAs (UnmanagedType.LPWStr)] string User,
                                                 [MarshalAs (UnmanagedType.LPWStr)] string Password,
                                                 out IntPtr handle);

        [DllImport ("libcmapi")]
        private static extern int CMapi_ULogonA (IntPtr obj,
                                                 [MarshalAs (UnmanagedType.LPStr)] string Host,
                                                 [MarshalAs (UnmanagedType.LPStr)] string User,
                                                 [MarshalAs (UnmanagedType.LPStr)] string Password,
                                                 out IntPtr handle);

        [DllImport ("libcmapi")]
        private static extern int CMapi_AllocateBuffer (IntPtr obj, uint size, out IntPtr buffer);

        [DllImport ("libcmapi")]
        private static extern int CMapi_AllocateMore (IntPtr obj, uint size, IntPtr oldBuffer, out IntPtr buffer);

        [DllImport ("libcmapi")]
        private static extern int CMapi_FreeBuffer (IntPtr obj, IntPtr buffer);

        [DllImport ("libcmapi")]
        private static extern int CMapi_FreeProws (IntPtr obj, IntPtr lpRows);

        #endregion
    }
}
