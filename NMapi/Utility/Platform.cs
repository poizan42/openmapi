//
// openmapi.org - NMapi C# Mapi API - Platform.cs
//
// Copyright 2010 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NMapi.Utility {

	/// <summary>Platform detection.</summary>
	internal static class Platform
	{
				
		[DllImport("libc")]
		private static extern int uname (IntPtr buf);
		private static OS? cached = null;
		private static object osLock = new object ();

		/// <summary></summary>
		internal enum OS { Windows, Mac, Unix, unknown };
		
		/// <summary></summary>
		internal static OS Current {
			get {
				lock (osLock) {
					if (cached == null)
						cached = FindOS ();
					return (OS) cached;
				}
			}
		}
		
		private static OS FindOS ()
		{
			if (System.IO.Path.DirectorySeparatorChar == '\\')
				return OS.Windows;
			if (IsRunningOnMac ())
				return OS.Mac;
			if (System.Environment.OSVersion.Platform == PlatformID.Unix)
				return OS.Unix;
			return OS.unknown;
		}
		
		//From Managed.Windows.Forms/XplatUI
		private static bool IsRunningOnMac ()
		{
			IntPtr buf = IntPtr.Zero;
			try{
				buf = Marshal.AllocHGlobal(8192);
				// This is a hacktastic way of getting sysname from uname ()
				if (uname(buf) == 0) {
					string os = Marshal.PtrToStringAnsi (buf);
					if (os == "Darwin")
						return true;
				}
			} catch {
				// do nothing.
			} finally{
				if (buf != IntPtr.Zero)
					Marshal.FreeHGlobal (buf);
			}
			return false;
		}
		
	}
	
}
