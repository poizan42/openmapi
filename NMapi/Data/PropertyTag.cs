//
// openmapi.org - NMapi C# Mapi API - PropertyTag.cs
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

namespace NMapi {

	using System;
	using System.Text;
	using NMapi.Flags;
	using NMapi.Properties;

	/// <summary>
	///  Wraps the property tag integer for stronger typing
	///  and easier access to Id and Type of the tag.
	/// </summary>
	/// <remarks>
	///  <para>
	///   TODO: Describe what property tags are.
	///  </para>
	///  <para>
	///   TODO: Describe the relationship to the int-value.
	///  </para>
	///  <para>
	///   TODO: Describe the relationship to property-values.
	///  </para>
	///  <para>
	///   This type is immutable.
	///  </para>
	/// </remarks>
	public abstract partial class PropertyTag : ICloneable
	{
		private readonly int propTag;

		/// <summary>The property type of the property tag.</summary>		
		/// <remarks></remarks>
		/// <returns></returns>
		public PropertyType Type {
			get {
				return PropertyTypeHelper.PROP_TYPE (propTag);
			}
		}

		/// <summary>The id of the property.</summary>
		/// <remarks></remarks>
		/// <value></value>
		public int Id {
			get {
				return PropertyTypeHelper.PROP_ID (propTag);
			}
		}

		/// <summary>
		///  Returns the value of the full property tag, that is the concatenation 
		///  of the property type and property id in a single 32-bit integer.
		/// </summary>
		/// <remarks></remarks>
		/// <value></value>
		public int Tag {
			get { return propTag; }
		}
		
		/// <summary>Gets the range that this property is part of.</summary>
		/// <remarks></remarks>
		/// <returns>The value of the range that this Property-Id is part of.</returns>
		public PropertyRange ContainedInPropertyRange {
			get {
				if (Id >= 0x0001 && Id <= 0x0C00) return PropertyRange.Core_Envelope;
				if (Id >= 0x0C00 && Id <= 0x0E00) return PropertyRange.Core_PerRecipient;
				if (Id >= 0x0E00 && Id <= 0x1000) return PropertyRange.Core_NonTransmittable;
				if (Id >= 0x1000 && Id <= 0x3000) return PropertyRange.Core_MessageContent;

				if (Id >= 0x3000 && Id < 0x3400) return PropertyRange.Core_System_CommonProperties;
				if (Id >= 0x3400 && Id < 0x3600) return PropertyRange.Core_System_MessageStoreObject;
				if (Id >= 0x3600 && Id < 0x3800) return PropertyRange.Core_System_FolderOrAbContainer;
				if (Id >= 0x3700 && Id < 0x3900) return PropertyRange.Core_System_Attachment;
				if (Id >= 0x3900 && Id < 0x3A00) return PropertyRange.Core_System_AddressBookObject;
				if (Id >= 0x3A00 && Id < 0x3C00) return PropertyRange.Core_System_MailUser;
				if (Id >= 0x3C00 && Id < 0x3D00) return PropertyRange.Core_System_DistributionList;
				if (Id >= 0x3D00 && Id < 0x3E00) return PropertyRange.Core_System_ProfileSection;
				if (Id >= 0x3E00 && Id < 0x4000) return PropertyRange.Core_System_StatusObject;

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
		/// <remarks></remarks>
		///  <returns>True if the property must be transmitted.</returns>
		public bool IsTransmitted {
			get {
				var range = ContainedInPropertyRange;
				return (range == PropertyRange.CustomTransportEnvelopeTransmitted || 
						range == PropertyRange.MessageClassTransmitted || 
						range == PropertyRange.NamedTransmitted);
			}
		}
		
		/// <summary>Checks if the property tag refers to a multi-value property.</summary>
		/// <remarks>Multi-value-properties are properties that (can) store an array of values.</remarks>
		/// <returns>True if the PropertyTyper of this tag is a multi-value property-type.</returns>
		public bool IsMultiValue {
			get {
				return ((int) Type & NMAPI.MV_FLAG) != 0;
			}
		}
		
		/// <summary>Checks if a property tag refers to a named property.</summary>
		/// <remarks>
		///  That means that the value of the ID has only a certain meaning 
		///  for the objects in the same store with the same mapping signature.
		///  The permanent name can be resolved using the named-property methods
		///  on the IMapiProp interface.
		/// </remarks>
		/// <param name="intTags"></param>
		/// <returns>True if the property tag is in the range of named properties. </returns>
		public bool IsNamedProperty {
			get {
				return (ContainedInPropertyRange == PropertyRange.NamedTransmitted);
			}
		}

		/// <summary>Converts the tag to a different property type.</summary>
		/// <remarks></remarks>
		/// <param name="type"></param>
		/// <returns></returns>		
		public PropertyTag AsType (PropertyType type)
		{
			return PropertyTag.CreatePropertyTag (PropertyTypeHelper.CHANGE_PROP_TYPE (Tag, type));
		}
		
		
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <returns></returns>
		public PropertyTag AsMultiValue ()
		{
			throw new NotImplementedException ("Not yet implemented."); // TODO
		}
		
		
		
		
		/*
		/// <summary>
		///  Convenience method to create a property value (using "data")
		///  from the current property tag. 
		/// </summary>
		public PropertyValue CreateValue (object data)
		{
			return PropertyValue.Make (Tag, data);
		}
		*/

		/// <summary>
		///  Creates a new property tag object from a 32-bit property tag integer.
		/// </summary>
		/// <param name="intTags"></param>		
		protected PropertyTag (int pt)
		{
			propTag = pt;
		}
		
		/// <summary>
		///  Creates an array of property tag objects from a 32-bit 
		///  integer-array of property tags.
		/// </summary>
		/// <remarks></remarks>
		/// <param name="intTags"></param>
		/// <returns></returns>
		public static PropertyTag[] ArrayFromIntegers (params int[] intTags)
		{
			PropertyTag[] result = new PropertyTag [intTags.Length];
			int i = 0;
			foreach (int tag in intTags)
				result [i++] = PropertyTag.CreatePropertyTag (tag);
			return result;
		}

		/// <summary>Returns a human-readable representation of the property tag.</summary>
		public override string ToString ()
		{
			return new StringBuilder ()
				.Append (base.ToString ())
				.Append ("{ Type: ")
				.Append (Type)
				.Append (", Id: ")
				.Append (Id.ToString ("X"))
				.Append (", Range: ")
				.Append (ContainedInPropertyRange)
				.Append (" }").ToString ();
		}

		#region operators
		
		/*
		
		TODO: this currently would break VMAPI.
		
		public static bool operator == (PropertyTag a, PropertyTag b)
		{
			// can't be null.
			return (a.propTag == b.propTag);
		}

		public static bool operator != (PropertyTag a, PropertyTag b)
		{
			return !(a == b);
		} 
		
		public bool Equals (PropertyTag obj)
		{
			return (this == obj);
		}
		
		public override bool Equals (object o)
		{
			if (o == null || !(o is PropertyTag))
				return false;
			return this.Equals ((PropertyTag) o);
		}
		
		public override int GetHashCode ()
		{
			return propTag.GetHashCode ();
		}
		*/
		
		#endregion operators
		
		/// <summary></summary>
		/// <returns></returns>
		public object Clone ()
		{
			// shallow-copy is sufficient, because we have only an int-memeber.
			return this.MemberwiseClone ();
		}
		
		
		/// <summary></summary>
		/// <returns></returns>
		public bool IdEquals (PropertyTag tag2)
		{
			if (tag2 == null)
				return false;
			return (Id == tag2.Id);
		}
		
		// TODO: clone? deep copy?
		private static PropertyTag[] EnforceType (PropertyTag[] tags, 
			PropertyType sourceType, PropertyType targetType)
		{
			if (tags == null)
				return null;
			PropertyTag[] newTags = new PropertyTag [tags.Length];
			for (int i=0; i<tags.Length; i++)
				if (tags [i].Type == sourceType)
					newTags [i] = tags [i].AsType (targetType);
				else
					newTags [i] = tags [i];
			return newTags;
		}
		
		/// <summary>
		///  Ensures that there are not String8 property tags in the array.
		///  If any tags are found they are replaced by Unicode property tags.
		///  The same is true for MvString8-Tags.
		/// </summary>
		/// <remarks></remarks>
		/// <param name="tags"></param>
		/// <returns></returns>
		public static PropertyTag[] EnforceUnicodeTags (PropertyTag[] tags)
		{
			PropertyTag[] tags2 = EnforceType (tags, PropertyType.String8, PropertyType.Unicode);
			return EnforceType (tags2, PropertyType.MvString8, PropertyType.MvUnicode);
		}
		
		/// <summary>
		///  Ensures that there are no Unicode property tags in the array.
		///  If any tags are found they are replaced by String8 property tags.
		///  The same is true for MvUnicode-Tags.
		/// </summary>
		/// <param name="tags"></param>
		/// <returns></returns>
		public static PropertyTag[] EnforceString8Tags (PropertyTag[] tags)
		{
			PropertyTag[] tags2 = EnforceType (tags, PropertyType.Unicode, PropertyType.String8);
			return EnforceType (tags2, PropertyType.MvUnicode, PropertyType.MvString8);
		}
		
	}


}
