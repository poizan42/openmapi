//
// openmapi.org - NMapi C# Mapi API - PropertyMetaData.cs
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

namespace NMapi {

	// TODO!

	/// <summary>
	///  An object that contains meta-data about the property, such as the 
	///  class of objects that it usually appears on or information about the 
	///  expected use in restrictions, etc.
	///  The purpose of this is to enable components to perform some 
	///  optimizations, based on the expected use of the property.
	///  The meta-data is attached to property definitions.
	/// </summary>
	public abstract class PropertyMetaData
	{
		internal PropertyMetaData ()
		{
		}
		
		/// <summary>
		///  Creates a meta-data object from a property tag (of a tagged property).
		/// </summary>
		public static TaggedPropertyMetaData FromDefinition (PropertyTag tag, 
			MapiPropDefAttribute attribute)
		{
			throw new NotImplementedException ("Not yet implemented!");
		}
		
		/// <summary>
		///  Creates a meta-data object from an named property definition.
		/// </summary>
		public static NamedPropertyMetaData FromDefinition (NamedPropertyDef namedPropDef)
		{
			throw new NotImplementedException ("Not yet implemented!");
		}
		
	}
	
	/// <summary>
	///  
	/// </summary>
	public sealed class NamedPropertyMetaData : PropertyMetaData
	{
		
		// TODO
		
	}
	
	/// <summary>
	///  
	/// </summary>
	public sealed class TaggedPropertyMetaData : PropertyMetaData
	{	
		
		// TODO
		
	}

}
