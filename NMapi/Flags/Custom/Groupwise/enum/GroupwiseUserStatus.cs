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
using System.IO;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags.Groupwise {

		/// <summary>
		///   
		/// </summary>
		[Flags]
		public enum GroupwiseUserStatus
		{
			// The item was accepted 
			Accepted = 0x00000001,
			
			//  An alarm is set for the item
			Alarm = 0x00000002,

			// The item was completed 
			Completed = 0x00000008,

			// The item was delegated 
			Delegated = 0x00000010,

			// The item was deleted
			Deleted = 0x00000020,

			// The item was downloaded 
			Downloaded = 0x00000100,

			// The item was forwarded 
			Forwarded = 0x00000200,

			// The item was hidden 
		 	Hidden = 0x00000400,

			// The item was incomplete 
			Incomplete  = 0x00000800,

			// The item was moved 
		 	Moved = 0x00001000,

			// The item was opened 
			Opened = 0x00004000,

			// The item was purged 
			Purged = 0x00008000,

			//The item was read 
			Read = 0x00010000,

			// The item was replied to
			Replied = 0x00020000,

			// The item was retracted 
			Retracted = 0x00040000,

			// A retraction has been requested for the item
			RetractReq = 0x00080000,

			// The item has been started
			Started = 0x00200000,

			// The item was unaccepted
			Unaccepted = 0x01000000,

			// The item was undeleted
			Undeleted = 0x04000000,

			// The item was unread
			Unread = 0x10000000,

			// The item was unstarted
			Unstarted = 0x20000000
		}
}
