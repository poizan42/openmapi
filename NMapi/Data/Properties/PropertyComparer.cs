//
// openmapi.org - NMapi C# Mapi API - PropertyComparer.cs
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
using System.Collections;
using System.Runtime.Serialization;

using System.Diagnostics;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;
using NMapi.Interop;

namespace NMapi.Properties {

	/// <summary>Implements an IComparer for PropertyValue objects.</summary>
	/// <remarks> TODO: describe _HOW_ the data is compared exactly. </remarks>
	public sealed class PropertyComparer : IComparer
	{

		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="obj1"></param>
		/// <param name="obj2"></param>
		/// <returns></returns>
		public int Compare (object obj1, object obj2)
		{
			PropertyValue prop1 = obj1 as PropertyValue;
			PropertyValue prop2 = obj2 as PropertyValue;
		
			if (prop1 == null && prop2 == null) // TODO: if one object is NOT a property object we should actually fail!
				return 0;
			if (prop1 == null && prop2 != null)
				return -1;
			if (prop1 != null && prop2 == null)
				return 1;

			// at this point, both are NOT null.
			if (PropertyTag.CreatePropertyTag (prop1.PropTag).Type != PropertyTag.CreatePropertyTag (prop2.PropTag).Type)
				throw new MapiInvalidParameterException (); // TODO: add better error message
			return prop1.CompareTo (prop2);
		}

	}

}