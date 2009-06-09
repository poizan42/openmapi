//
// openmapi.org - NMapi C# Mapi API - PropertyTag.cs
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

namespace NMapi {

	using System;
	using NMapi.Flags;
	using NMapi.Properties;

	/// <summary>
	///  Wraps the property tag integer for stronger typing
	//   and easier access to Id and Type of the tag.
	/// </summary>
	public struct PropertyTag
	{
		private int propTag;

		/// <summary>
		///  The property type of the property tag.
		/// </summary>
		public PropertyType Type {
			get {
				return PropertyTypeHelper.PROP_TYPE (propTag);
			}
		}

		/// <summary>
		///  The id of the property.
		/// </summary>
		public int Id {
			get {
				return PropertyTypeHelper.PROP_ID (propTag);
			}
		}

		/// <summary>
		///  Returns the value of the full property tag, that is the concatenation 
		///  of the property type and property id in a single 32-bit integer.
		/// </summary>
		public int Tag {
			get { return propTag; }
		}
		
		/// <summary>
		///  Returns the property ID range that this property is part of.
		/// </summary>
		public PropertyRange ContainedInPropertyRange {
			get {
				if (Id >= 0x4000 && Id < 0x5800) return PropertyRange.CustomTransportEnvelopeTransmitted;
				if (Id >= 0x5800 && Id < 0x6000) return PropertyRange.RecipientAssociatedNotTransmitted;
				if (Id >= 0x6000 && Id < 0x6600) return PropertyRange.CustomNotTransmitted;
				if (Id >= 0x6600 && Id < 0x6800) return PropertyRange.ServiceProviderNotTransmitted;
				if (Id >= 0x6800 && Id < 0x7C00) return PropertyRange.MessageClassTransmitted;
				if (Id >= 0x7C00 && Id < 0x8000) return PropertyRange.MessageClassNotTransmitted;
				if (Id >= 0x8000 && Id < 0xFFFE) return PropertyRange.NamedTransmitted;
				return PropertyRange.Unknown;
			}
		}
		
		/// <summary>
		///  If true, the property tag is in the range of properties that must be 
		///  transmitted when an object is sent by a transport provider.
		/// </summary>
		public bool IsTransmitted {
			get {
				var range = ContainedInPropertyRange;
				return (range == PropertyRange.CustomTransportEnvelopeTransmitted || 
						range == PropertyRange.MessageClassTransmitted || 
						range == PropertyRange.NamedTransmitted);
			}
		}
		
		/// <summary>
		///  True if the property tag is in the range of named properties. 
		///  That means that the value of the ID has only a certain meaning 
		///  for the objects in the same store with the same mapping signature.
		///  The permanent name can be resolved using the named-property methods
		///  on the IMapiProp interface.
		/// </summary>
		public bool IsNamedProperty {
			get {
				return (ContainedInPropertyRange == PropertyRange.NamedTransmitted);
			}
		}

		/// <summary>
		///  Converts the tag to a different type.
		/// </summary>
		public PropertyTag AsType (PropertyType type)
		{
			return new PropertyTag (PropertyTypeHelper.CHANGE_PROP_TYPE (Tag, type));
		}
		
		/// <summary>
		///  Convenience method to create a property value (using "data")
		///  from the current property tag. 
		/// </summary>
		public PropertyValue CreateValue (object data)
		{
			return PropertyValue.Make (Tag, data);
		}

		/// <summary>
		///  Creates a new property tag object from a 32-bit property tag integer.
		/// </summary>
		public PropertyTag (int pt)
		{
			propTag = pt;
		}
		
		/// <summary>
		///  Creates an array of property tag objects from a 32-bit 
		///  integer-array of property tags.
		/// </summary>
		public static PropertyTag[] ArrayFromIntegers (params int[] intTags)
		{
			PropertyTag[] result = new PropertyTag [intTags.Length];
			int i = 0;
			foreach (int tag in intTags)
				result [i++] = new PropertyTag (tag);
			return result;
		}

	}

}
