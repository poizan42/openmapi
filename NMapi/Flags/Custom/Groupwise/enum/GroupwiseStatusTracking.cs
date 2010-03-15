//
// openmapi.org - NMapi C# Mapi API - GroupwiseStatusTracking.cs
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
	public enum GroupwiseStatusTracking
	{

		/// <summary>No tracking.</summary>
		None = 0x00,

		/// <summary>Delivered.</summary>
		Delivered = 0x01,

		/// <summary>Host deleted.</summary>
		HostDeleted = 0x02, 

		/// <summary>Deleted</summary>
		Deleted = 0x04,  

		/// <summary>Opened</summary>
		Opened = 0x08,  

		/// <summary>Full tracking</summary>
		Full = 0xFF,

	}
}
