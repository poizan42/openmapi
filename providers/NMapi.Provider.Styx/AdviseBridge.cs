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
using NMapi.Events;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Provider.Styx.Interop;

using System.Collections.Generic;

namespace NMapi.Provider.Styx
{
    internal delegate int NotifyCallBack (IntPtr Context, uint count, IntPtr Notifications);
    
    [StructLayout(LayoutKind.Explicit)]
    internal struct ErrorNotification {
        
    };
    
	internal class AdviseBridge {

        static Dictionary<uint, AdviseBridge> SinkMap = new Dictionary<uint, AdviseBridge> ();

        IMapiAdviseSink sink;
        NotifyCallBack cb = new NotifyCallBack (OnNotify);
        IntPtr SinkHandle = new IntPtr ();
        GCHandle gch;

        public AdviseBridge (IMapiAdviseSink sink) {
            IntPtr CallbackPtr = Marshal.GetFunctionPointerForDelegate (cb);
            //NB: Objects in the managed heap can be moved around in memory at any time
            // by the garbage collector, so their physical addresses may change without notice.
            // P/Invoke automatically pins down managed objects passed by reference for
            // the duration of each method call. This means pointers passed to unmanaged 
            // code will be valid for that one call. Bear in mind that there is no guarantee 
            // that the object will not be moved to a different memory address on subsequent calls.
            gch = GCHandle.Alloc (this);
            CMapi_AdviseSink_Alloc (CallbackPtr, GCHandle.ToIntPtr (gch), out SinkHandle);
            this.sink = sink;
        }

	/* XXX a desctructor might be needed to delete the native C++ class using CMapi_AdviseSink_Free().
         * Maybe the GCHandle also needs some handling. Needs to be checked! */
        
        public static int OnNotify (IntPtr Context, uint count, IntPtr Notifications) {
            GCHandle gch = GCHandle.FromIntPtr (Context);
            AdviseBridge bridge = (AdviseBridge) gch.Target;
            
            if(Notifications != IntPtr.Zero) {
                Notification[] events = Transmogrify.PtrToNotification (Notifications, count);
                //System.Console.WriteLine ("*** Got {0} notifications", events.Length);
                bridge.sink.OnNotify (events);
            } else {
                System.Console.WriteLine("WARNING! OnNotify() with NULL pointer for notifications");
            }
            return 0;
        }

        public static void AddBrige (AdviseBridge NewSink, uint id) {
            lock (SinkMap) {
                SinkMap.Add (id, NewSink);
            }
        }

        public static AdviseBridge BridgeById (uint id) {
            lock (SinkMap) {
                return SinkMap[id];
            }
        }

        public static void RemoveBridge (uint id) {
            lock (SinkMap) {
                SinkMap.Remove (id);
            }
        }
        
        public IntPtr Handle {
            get {
                return (IntPtr) gch;
            }
        }
        
        public IntPtr SinkPtr {
            get {
                return SinkHandle;
            }
        }
        
        [DllImport ("libcmapi")]
        private static extern int CMapi_AdviseSink_Alloc (IntPtr callback, IntPtr handle, out IntPtr SinkHandle);
        
        [DllImport ("libcmapi")]
        private static extern int CMapi_AdviseSink_Free (IntPtr SinkHandle);
	}
}
