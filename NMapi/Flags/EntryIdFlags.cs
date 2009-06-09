//
// openmapi.org - NMapi C# Mapi API - EntryIdFlags.cs
//
// Copyright 2008-2009 Topalis AG
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

	/// <summary>
	///  Flags that can appear in the first byte of the EntryId flags fields.
	/// </summary>
	[Flags]
	public enum EntryIdFlags
	{
		
		/// <summary>
		///  If set the EntryId is not a long-time entry-id, meaning it may 
		///  only be valid for the current session.
		/// </summary>
		ShortTerm = 0x80,
		
		/// <summary>
		///  
		/// </summary>
		NotRecip = 0x40,
		
		/// <summary>
		///  
		/// </summary>
		ThisSession = 0x20,
		
		/// <summary>
		///  
		/// </summary>
		Now = 0x10,
		
		/// <summary>
		///  
		/// </summary>
		NotReserved = 0x08
	}


}
