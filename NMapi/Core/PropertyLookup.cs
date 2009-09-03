//
// openmapi.org - NMapi C# Mapi API - PropertyLookup.cs
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
using System.Collections.Generic;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NMapi {

	/// <summary>
	///  Property tags are usually embedded as constants in the NMapi 
	///  assemblies or (if custom properties are used) in other assemblies.
	///  This class provides a way to find the names of these constants 
	///  at runtime or turn a fully qualified name of the constant field into 
	///  the constant value (without loading the assembly).
	/// </summary>
	/// <remarks>
	///  Mostly used for debugging purposes as well as in some tools like the 
	///  mapishell.
	/// </remarks>
	public sealed class PropertyLookup
	{
		private TypeResolver resolver;
		private Dictionary<int, string> map = new Dictionary<int, string> ();

		/// <summary>
		///  Creates a new lookup object using the specified type resolver.
		/// </summary>
		public PropertyLookup (TypeResolver resolver)
		{
			this.resolver = resolver;
		}

		/// <summary>
		///  Registers a new class containing Mapi-Property-Tags. After the class 
		///  is registered the names will be available for lookup. Since the 
		///  names are resolved by reflecting over the class when registering it, 
		///  it should not be called when high performance is required.
		/// </summary>
		public void RegisterClass (string typeName)
		{
			TypeDefinition typeDef = resolver.ResolveDefinition (typeName);
			if (typeDef == null)
				throw CantResolveType (typeName);

			foreach (FieldDefinition field in typeDef.Fields)
				if (IsValidPropertyField (field) && !map.ContainsKey ((int) field.Constant))
					map [(int) field.Constant] = typeName + "." + field.Name;
		}

		/// <summary>
		///  If a field is a valid property tag field returns true.
		/// </summary>
		public bool IsValidPropertyField (FieldDefinition field)
		{
			if (!field.HasConstant || ! (field.Constant is Int32))
				return false;
			foreach (CustomAttribute attribute in field.CustomAttributes)
				if (attribute.Constructor.DeclaringType.Name == typeof (MapiPropDefAttribute).Name)
					return true;
			return false;
		}

		/// <summary>
		///  If the property tag exists as a constant field of a class that 
		///  has previously been registred, returns the name (ClassName + FieldName) 
		///  for the property tag. Otherwise null is returned.
		/// </summary>
		public string GetName (int propTag)
		{
			if (map.ContainsKey (propTag))
				return map [propTag];
			return null;
		}

		private Exception CantResolveType (string typeName)
		{
			return new ArgumentException ("Can't resolve type '" + typeName + "'.");
		}

		private Exception CantResolveField (string typeName, string fieldName)
		{
			return new ArgumentException ("Type '" + typeName + "' does not contain a field '" + fieldName + "'.");
		}
		
		/// <summary>
		///  Attempts to resolve the constant value of the property tag.
		/// </summary>
		/// <returns>
		///  If the field exists in a class that has been registered before and 
		///  the field is a valid property tag field, the PropertyTag that is
		///  contained in the constant field is returned. If the field exists 
		///  but is not a valid property tag field, -1 is returned.
		/// </returns>
		/// <exception ctype="System.ArgumentException">
      /// Thrown if the type of field can't be resolved.
      /// </exception>
		public int GetValue (string typeName, string fieldName)
		{
			int propTag = -1;
			TypeDefinition typeDef = resolver.ResolveDefinition (typeName);
			if (typeDef == null)
				throw CantResolveType (typeName);
			FieldDefinition field = typeDef.Fields.GetField (fieldName);
			if (field == null)
				throw CantResolveField (typeName, fieldName);
			if (IsValidPropertyField (field))
				propTag = (int) field.Constant;
			return propTag;
		}
	}

}

