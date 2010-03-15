//
// openmapi.org - NMapi C# Mapi API - GroupwiseUserStatus.cs
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
	public enum GroupwiseUserStatus
	{
		/// <summary>The item was accepted.</summary>
		Accepted = 0x00000001,
		
		/// <summary>An alarm is set for the item.</summary>
		Alarm = 0x00000002,

		/// <summary>The item was completed.</summary>
		Completed = 0x00000008,

		/// <summary>The item was delegated.</summary>
		Delegated = 0x00000010,

		/// <summary>The item was deleted.</summary>
		Deleted = 0x00000020,

		/// <summary>The item was downloaded.</summary>
		Downloaded = 0x00000100,

		/// <summary>The item was forwarded.</summary>
		Forwarded = 0x00000200,

		/// <summary>The item was hidden.</summary>
	 	Hidden = 0x00000400,

		/// <summary>The item was incomplete.</summary>
		Incomplete  = 0x00000800,

		/// <summary>The item was moved.</summary>
	 	Moved = 0x00001000,

		/// <summary>The item was opened.</summary>
		Opened = 0x00004000,

		/// <summary>The item was purged.</summary>
		Purged = 0x00008000,

		/// <summary>The item was read .</summary>
		Read = 0x00010000,

		/// <summary>The item was replied to.</summary>
		Replied = 0x00020000,

		/// <summary>The item was retracted.</summary>
		Retracted = 0x00040000,

		/// <summary>A retraction has been requested for the item.</summary>
		RetractReq = 0x00080000,

		/// <summary>The item has been started.</summary>
		Started = 0x00200000,

		/// <summary>The item was unaccepted.</summary>
		Unaccepted = 0x01000000,

		/// <summary>The item was undeleted.</summary>
		Undeleted = 0x04000000,

		/// <summary>The item was unread.</summary>
		Unread = 0x10000000,

		/// <summary>The item was unstarted.</summary>
		Unstarted = 0x20000000
	}
	
}
