//
// openmapi.org - NMapi C# Mapi API - StoreSupport.cs
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

	[Flags]
	public enum StoreSupport
	{
		// Bits for PR_STORE_SUPPORT_MASK 

		EntryidUnique    = 0x00000001,
		ReadOnly         = 0x00000002,
		SearchOk         = 0x00000004,
		ModifyOk         = 0x00000008,
		CreateOk         = 0x00000010,
		AttachOk         = 0x00000020,
		OleOk            = 0x00000040,
		SubmitOk         = 0x00000080,
		NotifyOk         = 0x00000100,
		MvPropsOk        = 0x00000200,
		CategorizeOk     = 0x00000400,
		RtfOk            = 0x00000800,
		RestrictionOk    = 0x00001000,
		SortOk           = 0x00002000,
		PublicFolders    = 0x00004000,
		UncompressedRtf  = 0x00008000

	}

}
