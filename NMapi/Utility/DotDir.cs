//
// openmapi.org - NMapi C# Mapi API - DotDir.cs
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
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NMapi.Utility {

	/// <summary>Common configuration paths, depending on the OS.</summary>
	/// <remarks></remarks>
	public static class DotDir
	{

		// System.Environment.SpecialFolder.CommonApplicationData
		// System.Environment.SpecialFolder.LocalApplicationData

		private static string FolderName {
			get {
				if (Platform.Current == Platform.OS.Mac || Platform.Current == Platform.OS.Windows)
					return "OpenMapi";
				return "openmapi";
			}
		}

		private static string MacLibraryPath {
			get {
				return Path.Combine (
					Environment.GetFolderPath (Environment.SpecialFolder.Personal), 
					"Library");
			}
		}

		/// <summary>The main OpenMapi configuratiom directory.</summary>
		/// <remarks>
		///  <para>
		///   This directory should be used by NMapi, the OpenMapi server, VMapi 
		///   and other tools that are part of OpenMapi to store configuration 
		///   and user data.
		///  </para>
		///  <para>
		///   On Mac OS X this is probably: ~/Library/Application Support/OpenMapi
		///   On Linux you should get: ~/.config/openmapi
		///   and on Windows: ~/AppData/OpenMapi
		///  </para>
		/// </remarks>
		/// <value>The OpenMapi configuration directory.</value>
		public static string LocalSettingsPath {
			get {
				if (Platform.Current == Platform.OS.Mac)
					return Path.Combine (Path.Combine (MacLibraryPath, "Application Support"), FolderName);

				return Path.Combine (Environment.GetFolderPath (
					System.Environment.SpecialFolder.ApplicationData), FolderName);
			}
		}
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <value></value>
		public static string CachePath {
			get {
				if (Platform.Current == Platform.OS.Mac)
					return Path.Combine (Path.Combine (MacLibraryPath, "Caches"), FolderName);
				return Path.Combine (TempPath, FolderName + "Cache");
			}
		}
		
		/// <summary>The system temp path.</summary>
		/// <remarks></remarks>
		/// <value></value>
		public static string TempPath {
			get { return Path.GetTempPath (); }
		}
		
	}
	
}
