//
// openmapi.org - NMapi C# Mapi API - GroupwiseItemType.cs
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
	public enum GroupwiseItemType
	{
		
		/// <summary>Mail</summary>
		Mail = 0x00000001,
		
		/// <summary>Note</summary>
		Note = 0x00000002,
		
		/// <summary>Task</summary>
		Todo = 0x00000004,
		
		/// <summary>Appointment</summary>
		Appointment = 0x00000008,
		
		/// <summary>Phone message</summary>
		Phone = 0x00000010,
		
		/// <summary>Busy search</summary>
		Search = 0x00000020,
		
		/// <summary>Profile</summary>
		Profile = 0x00002000,
		
		/// <summary>ODMA reference</summary>
		OdmaReference = 0x00004000,
		
		/// <summary>Independent Service Vendor object.</summary>
		ISVObject = 0x00010000,
		
		/// <summary>Workflow</summary>
		WorkFlow = 0x00020000

	}

}
