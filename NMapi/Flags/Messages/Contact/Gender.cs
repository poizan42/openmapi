//
// openmapi.org - NMapi C# Mapi API - Gender.cs
//
// Copyright 2009-2010 Topalis AG
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
	
	/// <summary>Possible values of the property tag Property.Gender.</summary>
	/// <remarks>
	///  Note: This file is based on the documentation published by Microsoft
	///       [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public enum Gender
	{
		/// <summary></summary>
		Unspecified = 0,

		/// <summary>The person described by the contact object is female.</summary>
		Female,

		/// <summary>The person described by the contact object is male.</summary>
		Male
	}
	
}
