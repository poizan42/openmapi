//
// openmapi.org - NMapi C# Mapi API - NamedPropertyResolver.cs
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

namespace NMapi {

	using System;
	using System.Text;
	using NMapi.Flags;
	using NMapi.Properties;

	/// <summary>
	///  An instance of this class is returned by the ResolveNamedProperties () 
	///  extension methods. The class basically contains an internal map of 
	///  resolved named properties that can then be used to construct property tags.
	/// </summary>
	public sealed class NamedPropertyResolver
	{
		private MapiNameId[] names;
		private PropertyTag[] tags;
		
		/// <summary></summary>
		/// <param name="names"></param>
		/// <param name="tags"></param>
		public NamedPropertyResolver (MapiNameId[] names, PropertyTag[] tags)
		{
			this.names = names;
			this.tags = tags;
		}
		
		/// <summary>
		///  Looks up the property tag assigned by the OpenMapi provider to 
		///  the named property. This operation is fast, because the lookup table 
		///  is being build before the NamedPropertyResolver is constructed.
		/// </summary>
		/// <remarks>
		///  Please note that this relationship depends on instance of the 
		///  provider as well as (potentially) on the particular MAPI object. 
		///  Only objects of the same store with the same value in the 
		///  Property.MappingSignature property share the same mapping. 
		///  Therefore you must always retrieve a new NamePropertyResolver 
		///  when required.
		/// </remarks>
		/// <param name="namedPropertyDefinition">The definition of the named property to be resolved.</param>
		/// <returns>The PropertyTag or null, if not found.</returns>
		public PropertyTag Lookup (NamedPropertyDef namedPropertyDefinition)
		{
			if (names == null || tags == null)
				return null;
			
			PropertyTag tag = FindPropertyId (namedPropertyDefinition);			
			return tag.AsType (namedPropertyDefinition.Type);
		}
	
		private PropertyTag FindPropertyId (NamedPropertyDef namedPropertyDefinition)
		{			
			for (int i=0; i < names.Length; i++) {
				MapiNameId name = names [i];
				if (namedPropertyDefinition.NameDefinition.LogicallyEquals (name) && tags.Length > i)
					return tags [i];
			}
			return null;
		}
		
	}


}
