//
// openmapi.org - NMapi C# Mapi API - TeamXChangeFolderImporter1.cs
//
// Copyright 2009 VipCom GmbH, Topalis AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
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

namespace NMapi.Synchronization {

	using System;
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop.MapiRPC;

	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Properties.Special;
	using NMapi.Table;
	
	/// <summary>
	/// 
	/// </summary>
	public interface TeamXChangeFolderImporter1
	{

		/// <summary>
		/// 
		/// </summary>
		void FolderCreated (byte[] folderKey, IMapiFolder folder, 
			byte[] parentKey, string name, int ulFolderType);

		
		/// <summary>
		/// 
		/// </summary>
		void FolderDeleted (byte[] folderKey, byte[] parentKey);
		
		
		/// <summary>
		/// 
		/// </summary>
		void FolderChanged (byte[] folderKey, string  name, 
			byte[] oldParentKey, byte[] newParentKey);
	}

}