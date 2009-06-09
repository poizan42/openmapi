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

	public sealed class PropertyLookup
	{
		private TypeResolver resolver;
		private Dictionary<int, string> map = new Dictionary<int, string> ();

		public PropertyLookup (TypeResolver resolver)
		{
			this.resolver = resolver;
		}

		public void RegisterClass (string typeName)
		{
			TypeDefinition typeDef = resolver.ResolveDefinition (typeName);
			if (typeDef == null)
				throw CantResolveType (typeName);

			foreach (FieldDefinition field in typeDef.Fields)
				if (IsValidPropertyField (field) && !map.ContainsKey ((int) field.Constant))
					map [(int) field.Constant] = typeName + "." + field.Name;
		}

		public bool IsValidPropertyField (FieldDefinition field)
		{
			if (!field.HasConstant || ! (field.Constant is Int32))
				return false;
			foreach (CustomAttribute attribute in field.CustomAttributes)
				if (attribute.Constructor.DeclaringType.Name == typeof (MapiPropDefAttribute).Name)
					return true;
			return false;
		}

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

