//
// openmapi.org - NMapi C# Mapi API - MapiPropertyAttribute.cs
//
// Copyright 2008 Topalis AG
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
using NMapi.Flags;

namespace NMapi.Linq {

	/// <summary>
	///   A MapiPropertyAttribute is used to mark all C#-Properties in 
	///   a class implenting the IMapiEntity-Interface as Mapi-Properties 
	///   that should be retrieved when querying data.
	/// </summary>
	[AttributeUsage (AttributeTargets.Property, AllowMultiple=false)]
	public class MapiPropertyAttribute : Attribute
	{
		/// <summary>
		///   The Mapi-Property; If this is a "named" property, the 
		///   Kind-Value is stored instead.
		/// </summary>
		public int PropertyOrKind {
			get;
			set;
		}

/*

		public int GetProperty (IMapiProp obj)
		{
			if (Guid == null)
				return PropertyOrKind;

			// a named property ....

			names = obj.GetIDsFromNames (...); // TODO: get all props ion one go + share for same object!
			...

			
			
		}

*/

		/// <summary>
		///  This values is only used with "named" properties.
		/// </summary>
		public byte[] Guid {
			get;
			set;
		}

		/// <summary>
		///
		/// </summary>
		public NamedProperty NamedProperty {
			get;
			set;
		}

		/// <summary>
		///   The PropertyType of the Mapi-Property.
		/// </summary>
		public PropertyType Type {
			get;
			set;
		}

		/// <summary>
		///   Defines how and when the property is loaded.
		/// </summary>
		public LoadMode LoadMode {
			get;
			set;
		}

		/// <summary>
		///   Defines if the property should be included
		///   when data-binding the object to a control.
		/// </summary>
		public bool AllowDataBinding {
			get;
			set;
		}


		public MapiPropertyAttribute (byte[] guid, int prop, PropertyType pType)
			:this (NamedProperty.Yes, guid, prop, pType, LoadMode.PreFetch, true)
		{
		}

		public MapiPropertyAttribute (int prop, PropertyType pType)
			:this (NamedProperty.No, null, prop, pType, LoadMode.PreFetch, true)
		{
		}

		public MapiPropertyAttribute (NamedProperty named, byte[] guid, int prop, 
			PropertyType pType, LoadMode lMode, bool allowDataBinding)
		{
			this.NamedProperty = named;
			this.PropertyOrKind = prop;
			this.Type = pType;
			this.LoadMode = lMode;
			this.Guid = guid;
			this.AllowDataBinding = allowDataBinding;
		}

	}
}
