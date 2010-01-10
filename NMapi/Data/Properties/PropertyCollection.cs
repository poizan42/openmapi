//
// openmapi.org - NMapi C# Mapi API - PropertyCollection.cs
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

	/// <summary>
	///  
	/// </summary>
	public sealed class PropertyCollection
	{	
		private PropertyValue[] props;		
		
		/// <summary>
		///  
		/// </summary>
		/// <param name="props"></param>
		public PropertyCollection (PropertyValue[] props)
		{

		}
		
		
		
		/// <summary>Returns a the index of the property matching the specified property or -1, if not found.</summary>
		/// <remarks></remarks>
		/// <param name="val"></param>
		public int IndexByValue (PropertyValue val)
		{
			if (val == null)
				return -1;
			if (props == null || props.Length == 0)
				return -1;
			for (int i=0; i<props.Length; i++) {
				var prop = props [i];
				if (prop.PropTag == val.PropTag)
					if (prop.CompareTo (val) == 0)
						return i;
			}
			return -1;
		}
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="tag"></param>
		public int IndexByTag (PropertyTag tag)
		{
			if (tag == null)
				return -1;
			if (props == null || props.Length == 0)
				return -1;
			for (int i=0; i<props.Length; i++)
				if (props [i].PropTag == tag.Tag)
					return i;
			return -1;
		}
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="tag"></param>
		public PropertyValue FindByTagOrNull (PropertyTag tag)
		{
			int index = IndexByTag (tag);
			if (index < 0)
				return null;
			return props [index];
		}
		
		// TODO: FindById
		// TODO: FindByIdOrNull
		
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="props"></param>
		/// <returns></returns>
		public static PropertyValue[] EnforceUnicodeProperties (PropertyValue [] props)
		{
			if (props == null)
				return null;
			
			PropertyValue[] result = new PropertyValue [props.Length];
			for (int i=0; i < props.Length; i++) {
				if (props [i] is String8Property)
					result [i] = ((String8Property) props [i]).ToUnicodeProperty ();
				else if (props [i] is String8ArrayProperty)
					result [i] = ((String8ArrayProperty) props [i]).ToUnicodeArrayProperty ();
				else
					result [i] = props [i];				
			}
			return result;
		}
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="props"></param>
		/// <returns></returns>
		public static PropertyValue[] EnforceString8Properties (PropertyValue [] props)
		{
			if (props == null)
				return null;
				
			PropertyValue[] result = new PropertyValue [props.Length];
			for (int i=0; i < props.Length; i++) {
				if (props [i] is UnicodeProperty)
					result [i] = ((UnicodeProperty) props [i]).ToString8Property ();
				else if (props [i] is UnicodeArrayProperty)
					result [i] = ((UnicodeArrayProperty) props [i]).ToString8ArrayProperty ();
				else
					result [i] = props [i];				
			}
			return result;
		}
		
	}
	
