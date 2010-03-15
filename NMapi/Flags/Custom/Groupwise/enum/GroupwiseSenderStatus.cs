//
// openmapi.org - NMapi C# Mapi API - GroupwiseSenderStatus.cs
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
	public enum GroupwiseSenderStatus
	{

		/// <summary>Item has been deleted.</summary>
		Deleted = 0x00000020,

		/// <summary>Item has been downloaded.</summary>
		Downloaded = 0x00000200,

		/// <summary>Item has been purged.</summary>
		Purged = 0x00008000,
		
		/// <summary>Item has been undeleted.</summary>
		Undeleted = 0x04000000,
	}

}
