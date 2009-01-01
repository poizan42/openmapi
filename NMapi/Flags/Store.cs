//
// openmapi.org - NMapi C# Mapi API - Store.cs
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

	public struct Store
	{
		// Bits for PR_STORE_SUPPORT_MASK 
	
		public const int EntryidUnique    = 0x00000001;
		public const int ReadOnly         = 0x00000002;
		public const int SearchOk         = 0x00000004;
		public const int ModifyOk         = 0x00000008;
		public const int CreateOk         = 0x00000010;
		public const int AttachOk         = 0x00000020;
		public const int OleOk            = 0x00000040;
		public const int SubmitOk         = 0x00000080;
		public const int NotifyOk         = 0x00000100;
		public const int MvPropsOk        = 0x00000200;
		public const int CategorizeOk     = 0x00000400;
		public const int RtfOk            = 0x00000800;
		public const int RestrictionOk    = 0x00001000;
		public const int SortOk           = 0x00002000;
		public const int PublicFolders    = 0x00004000;
		public const int UncompressedRtf  = 0x00008000;

		// Bits for PR_STORE_STATE

		public const int HasSearches     = 0x01000000;
	}

}
