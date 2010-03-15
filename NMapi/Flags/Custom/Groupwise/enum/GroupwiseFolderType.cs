//
// openmapi.org - NMapi C# Mapi API - GroupwiseFolderType.cs
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
	public enum GroupwiseFolderType
	{
		/// <summary>Normal folder.</summary>
	 	Normal = 0,

		/// <summary>Query folder.</summary>
	 	Query = 4,

		/// <summary>User folder (system-created).</summary>
 		User = 6,

		/// <summary>Mailbox (system-created).</summary>
	 	Universal = 7,

		/// <summary>Trash (system-created).</summary>
	 	Trash = 9,

		/// <summary>Calendar (system-created).</summary>
		Calendar = 10,

		/// <summary>Cabinet (system-created).</summary>
	 	Cabinet = 12,

		/// <summary>Current work (system-created).</summary>
	 	WorkInProgress = 13
	}

}
