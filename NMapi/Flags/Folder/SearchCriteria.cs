//
// openmapi.org - NMapi C# Mapi API - SearchCriteria.cs
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

	/// <summary>Possible flags for the SetSearchCriteria call.</summary>
	/// <remarks>Note: The unicode flag is possible as well!</remarks>
	[Flags]
	public enum SearchCriteria
	{
		
		/// <summary></summary>
		/// <remarks>Classic MAPI name is STOP_SEARCH.</remarks>
		Stop = 0x00000001,
		
		/// <summary></summary>
		/// <remarks>Classic MAPI name is RESTART_SEARCH.</remarks>
		Restart = 0x00000002,

		/// <summary>SubFolders should be searched as well.</summary>
		/// <remarks>Classic MAPI name is RECURSIVE_SEARCH.</remarks>
 		Recursive = 0x00000004,

		/// <summary>SubFolders should not be searched.</summary>
		/// <remarks>Classic MAPI name is SHALLOW_SEARCH.</remarks>
 		Shallow = 0x00000008,

		/// <summary></summary>
		/// <remarks>Classic MAPI name is FOREGROUND_SEARCH.</remarks>
		Foreground = 0x00000010,
		
		/// <summary></summary>
		/// <remarks>Classic MAPI name is BACKGROUND_SEARCH.</remarks>
		Background = 0x00000020,

		/// <summary>The search is performed using the FullText-Index, ignoring everything else.</summary>
		/// <remarks>Classic MAPI name is NON_CONTENT_INDEXED_SEARCH.</remarks>
		ContentIndexed = 0x00010000,

		/// <summary>The search should be performed without using the FullText-Index.</summary>
		/// <remarks>Classic MAPI name is NON_CONTENT_INDEXED_SEARCH.</remarks>
		NonContentIndexed = 0x00020000,
		
		/// <summary></summary>
		/// <remarks>Classic MAPI name is STATIC_SEARCH.</remarks>
		Static = 0x00040000

	}

}
