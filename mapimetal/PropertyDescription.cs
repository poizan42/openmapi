//
// openmapi.org - NMapi C# Mapi API - PropertyDescription.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	public sealed class PropertyDescription
	{
		private TypeResolver resolver;

		public int LineNumber {get; set; }
		public string Guid {get; set; }
		public string Name {get; set; }
		public bool NamedProperty {get; set;}
		public string PropertyTypeString {get; set;}
		public LoadMode LoadMode {get; set; }
		public bool DataBind {get; set; }

		public PropertyDescription (TypeResolver resolver)
		{
			this.resolver = resolver;
		}

		public string CSharpType {
			get {
				PropertyType pt = DeterminePropertyType ();
				switch (pt) {
					case PropertyType.Unspecified:
						throw new Exception ("PropertyType.Unspecified is invalid!");
					case PropertyType.Null:		return "System.Int32";
					case PropertyType.I2:		return "System.Short";
					case PropertyType.Long:		return "System.Int32";
					case PropertyType.R4:		return "System.Float";
					case PropertyType.Double:	return "System.Double";
					case PropertyType.Currency:	return "System.Long";
					case PropertyType.AppTime:	return "System.Double";
					case PropertyType.Error:	return "System.Int32";
					case PropertyType.Boolean:	return "System.Short";
					case PropertyType.Object:	return "System.Int32";
					case PropertyType.I8:		return "System.Int64";
					case PropertyType.String8:	return "System.String";
					case PropertyType.Unicode:	return "System.String";
					case PropertyType.SysTime:	return "NMapi.FileTime";
					case PropertyType.ClsId:	return "NMapi.NMapiGuid";
					case PropertyType.Binary:	return "NMapi.SBinary";
					case PropertyType.MvI2:		return "System.Short[]";
					case PropertyType.MvLong:	return "System.Int32[]";
					case PropertyType.MvR4:		return "System.Float[]";
					case PropertyType.MvDouble:	return "System.Double[]";
					case PropertyType.MvCurrency:	return "System.Int64[]";
					case PropertyType.MvAppTime:	return "System.Double[]";
					case PropertyType.MvSysTime:	return "NMapi.FileTime[]";
					case PropertyType.MvString8:	return "System.String[]";
					case PropertyType.MvBinary:	return "NMapi.SBinary[]";
					case PropertyType.MvUnicode:	return "System.String[]";
					case PropertyType.MvClsId:	return "NMapi.NMapiGuid[]";
					case PropertyType.MvI8:		return "System.Int64[]";
				}
				throw new Exception ("Unknown type!");
			}
		}

		public string MapiType {
			get {
				PropertyType pt = DeterminePropertyType ();
				switch (pt) {
					case PropertyType.Unspecified:	return "PropertyType.Unspecified";
					case PropertyType.Null:		return "PropertyType.Null";
					case PropertyType.I2:		return "PropertyType.I2";
					case PropertyType.Long:		return "PropertyType.Long";
					case PropertyType.R4:		return "PropertyType.R4";
					case PropertyType.Double:	return "PropertyType.Double";
					case PropertyType.Currency:	return "PropertyType.Currency";
					case PropertyType.AppTime:	return "PropertyType.AppTime";
					case PropertyType.Error:	return "PropertyType.Error";
					case PropertyType.Boolean:	return "PropertyType.Boolean";
					case PropertyType.Object:	return "PropertyType.Object";
					case PropertyType.I8:		return "PropertyType.I8";
					case PropertyType.String8:	return "PropertyType.String8";
					case PropertyType.Unicode:	return "PropertyType.Unicode";
					case PropertyType.SysTime:	return "PropertyType.SysTime";
					case PropertyType.ClsId:	return "PropertyType.ClsId";
					case PropertyType.Binary:	return "PropertyType.Binary";
					case PropertyType.MvI2:		return "PropertyType.MvI2";
					case PropertyType.MvLong:	return "PropertyType.MvLong";
					case PropertyType.MvR4:		return "PropertyType.MvR4";
					case PropertyType.MvDouble:	return "PropertyType.MvDouble";
					case PropertyType.MvCurrency:	return "PropertyType.MvCurrency";
					case PropertyType.MvAppTime:	return "PropertyType.MvAppTime";
					case PropertyType.MvSysTime:	return "PropertyType.MvSysTime";
					case PropertyType.MvString8:	return "PropertyType.MvString8";
					case PropertyType.MvBinary:	return "PropertyType.MvBinary";
					case PropertyType.MvUnicode:	return "PropertyType.MvUnicode";
					case PropertyType.MvClsId:	return "PropertyType.MvClsId";
					case PropertyType.MvI8:		return "PropertyType.MvI8";
				}
				throw new NotSupportedException ("Unknown type!");
			}
		}

		private int DeterminePropertyTag ()
		{
			string[] tmp = TypeResolver.SplitTypeNameAndProperty (PropertyTypeString);
			string typeName = tmp [0];
			string fieldName = tmp [1];

			bool found = false;
			int propValue = -1;
			if (typeName == "Property") {
				Type type = typeof (NMapi.Flags.Property);
				FieldInfo field = type.GetField (fieldName);
				propValue = (int) field.GetValue (type);
			}
			else {
				TypeDefinition typeDef = resolver.ResolveDefinition (typeName);
				if (typeDef == null)
					throw new ArgumentException ("The specified type '" + 
						typeName + "' can't be found!");
				FieldDefinition field = typeDef.Fields.GetField (fieldName);
				try {
					propValue = (int) field.Constant;
				} catch (InvalidCastException) {
					throw new ArgumentException ("The specified property " + 
						"type must be constant and of type 'int'.");
				}
				found = true;
			}
			return propValue;
		}

		public PropertyType DeterminePropertyType ()
		{
			int tag = DeterminePropertyTag ();
			return PropertyTypeHelper.PROP_TYPE (tag);
		}
	}

}

