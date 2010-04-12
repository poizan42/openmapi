//
// openmapi.org - NMapi C# Mapi API - SearchState.cs
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

namespace NMapi.Flags {

	/// <summary></summary>
	/// <remarks></remarks>
	[Flags]
	public enum SearchState
	{

		/// <summary></summary>
		/// <remarks>Classic MAPI name is SEARCH_RUNNING.</remarks>
		Running = 0x00000001,

		/// <summary></summary>
		/// <remarks>Classic MAPI name is SEARCH_REBUILD.</remarks>
		Rebuild = 0x00000002,
		
		/// <summary></summary>
		/// <remarks>Classic MAPI name is SEARCH_RECURSIVE.</remarks>
		Recursive = 0x00000004,
		
		/// <summary></summary>
		/// <remarks>Classic MAPI name is SEARCH_FOREGROUND.</remarks>
		Foreground = 0x00000008,
		
		/// <summary></summary>
		/// <remarks>Classic MAPI name is SEARCH_STATIC.</remarks>
		Static = 0x00010000,
		
		/// <summary></summary>
		/// <remarks>Classic MAPI name is SEARCH_MAYBE_STATIC.</remarks>
		MaybeStatic = 0x00020000,

	}

}
