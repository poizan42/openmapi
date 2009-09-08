//
// openmapi.org - NMapi C# Mapi API - PropertyCollection.cs
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
using System.Collections;
using System.Runtime.Serialization;

using System.Diagnostics;
using CompactTeaSharp;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;
using NMapi.Interop;

namespace NMapi.Properties {


	// TODO: implement this -- and use it everywhere instead of plain arrays.
				// (as a fixed size collection.)
	
	/// <summary>
	///  
	/// </summary>
	public sealed class PropertyCollection
	{
		
		
		
		
		/// <summary>
		///  
		/// </summary>
		public PropertyCollection (PropertyValue[] props)
		{

		}
		
		
		
		
		/// <summary>
		///  
		/// </summary>
		public static PropertyValue[] EnforceUnicodeProperties (PropertyValue [] props)
		{
			if (props == null)
				return null;
			PropertyValue[] result = new PropertyValue [props.Length];
			for (int i=0; i < props.Length; i++) {
				if (props [i] is String8Property)
					result [i] = ((String8Property) props [i]).ToUnicodeProperty ();
				else
					result [i] = props [i];				
			}
			return result;
		}
		
		/// <summary>
		///  
		/// </summary>
		public static PropertyValue[] EnforceString8Properties (PropertyValue [] props)
		{
			if (props == null)
				return null;
			PropertyValue[] result = new PropertyValue [props.Length];
			for (int i=0; i < props.Length; i++) {
				if (props [i] is UnicodeProperty)
					result [i] = ((UnicodeProperty) props [i]).ToString8Property ();
				else
					result [i] = props [i];				
			}
			return result;
		}
		
	}
	

}
