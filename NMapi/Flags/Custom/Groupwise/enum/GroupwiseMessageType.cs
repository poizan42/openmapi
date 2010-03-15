//
// openmapi.org - NMapi C# Mapi API - GroupwiseMessageType.cs
//
// Copyright 2009 Topalis AG
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

namespace NMapi.Flags.Groupwise {

	/// <summary></summary>
	[Flags]
	public enum GroupwiseMessageType
	{
		/// <summary>No body; files only</summary>
		FilesOnly = 0x00000001,

		/// <summary>Forwarded message</summary>
		Forward = 0x00000002,

		/// <summary>Reply to message</summary>
		Reply = 0x00000004,

		/// <summary>Encapsulated message</summary>
		Encapsulated = 0x00000008

	}
	
}
