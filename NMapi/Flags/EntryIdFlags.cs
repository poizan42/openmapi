//
// openmapi.org - NMapi C# Mapi API - EntryIdFlags.cs
//
// Copyright 2008-2010 Topalis AG
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

	/// <summary>Flags that can appear in the first byte of the EntryId flags fields.</summary>
	/// <remarks>
	///  <para>
	///   An EntryId consist of an array of bytes of arbitrary length. The format 
	///   is unknown and depends on the provider that generated the EntryId. However, 
	///   all EntryIds have in common that the first 4 bytes are reserved to store 
	///   some flags (defined here) with a known meaning.
	///  </para>
	/// </remarks>
	[Flags]
	public enum EntryIdFlags
	{
		/// <summary>
		///  If set the EntryId is not a long-time entry-id, meaning it may 
		///  only be valid for the current session. In this case, usually 
		///  <see cref="NotRecip" />, <see cref="ThisSession" />, <see cref="Now" /> 
		///  and <see cref="NotReserved" /> also must be set.
		/// </summary>
		ShortTerm = 0x80,
		
		/// <summary>The EntryId is not a valid recipient.</summary>
		/// <remarks>
		///  This is probably true for most EntryIds, in the store. 
		///  In fact, only entry-ids that represent users and One-Off-EntryIds 
		///  could NOT have this flag set. Not sure how this is actually in pratice.
		/// </remarks>
		NotRecip = 0x40,
		
		/// <summary>The EntryId is valid only for this session.</summary>
		ThisSession = 0x20,
		
		/// <summary>The EntryId is just valid for the current operation. (Really?)</summary>
		Now = 0x10,
		
		/// <summary></summary>
		NotReserved = 0x08
	}


}
