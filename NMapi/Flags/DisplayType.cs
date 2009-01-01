//
// openmapi.org - NMapi C# Mapi API - DisplayType.cs
//
// Copyright 2008 Topalis AG
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


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	public sealed class DisplayType
	{ 
		// PR_DISPLAY_TYPEs

		// AB Contents Tables
		public const int MailUser         = 0x00000000;
		public const int DistList         = 0x00000001;
		public const int Forum            = 0x00000002;
		public const int Agent            = 0x00000003;
		public const int Organization     = 0x00000004;
		public const int PrivateDistlist  = 0x00000005;
		public const int RemoteMailuser   = 0x00000006;

		// AB Hierarchy tables
		public const int Modifiable       = 0x00010000;
		public const int Global           = 0x00020000;
		public const int Local            = 0x00030000;
		public const int Wan              = 0x00040000;
		public const int NotSpecific      = 0x00050000;

		//  Folder Fierarchy Tables
		public const int Folder           = 0x01000000;
		public const int FolderLink       = 0x02000000;
		public const int FolderSpecial    = 0x04000000;


	}

}
