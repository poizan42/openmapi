//
// openmapi.org - NMapi C# Mapi API - Priority.cs
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

	/// <summary>Possible value for the property tag Property.Priority.</summary>
	public enum Priority
	{
		/// <summary>High priority.</summary>
		/// <remarks>The classic MAPI name for this constant is PRIO_URGENT.</remarks>
		Urgent =  1,
			
		/// <summary>The default.</summary>
		/// <remarks>The classic MAPI name for this constant is PRIO_NORMAL.</remarks>
		Normal =  0,
	
		/// <summary>Low priorty.</summary>
		/// <remarks>The classic MAPI name for this constant is PRIO_NON_URGENT.</remarks>
		NonUrgent = -1
	}

}
