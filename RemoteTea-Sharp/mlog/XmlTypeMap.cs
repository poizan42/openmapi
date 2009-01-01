//
// openmapi.org - CompactTeaSharp - MLog - XmlTypeMap.cs
//
// Copyright 2009 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CompactTeaSharp.Mlog
{
	/// <summary>
	///	 Processes the "nrpcgen" xml-file that contains additional 
	///  information like type mappings, etc.
	/// <summary>
	public sealed class XmlTypeMap
	{
		private Dictionary<string, string> typeMap;
		private List<string> staticTypes;

		public XmlTypeMap  (string fileName)
		{
			if (fileName != null) {
				typeMap = new Dictionary<string, string> ();
				staticTypes = new List <string> ();

				XElement xml = XElement.Load (fileName);
			
				var map = from type in 
					xml.Elements ("map").Elements ("type")
					select type;

				foreach (var typeElement in map)
					typeMap [(string) typeElement.Attribute ("from")] = 
						(string) typeElement.Attribute ("to");

				var types = from type in 
				xml.Elements ("staticDec").Elements ("type")
					select (string) type.Attribute ("name");
				
				foreach (var type in types)
					staticTypes.Add (type);
			}
		}
		
		/// <summary>
		///	 Maps a type from the .x file to a new name.
		/// <summary>
		public string Map (string key)
		{
			if (typeMap == null)
				return key;
			string result = null;
			if (typeMap.ContainsKey (key))
				result = typeMap [key];
			if (result == null)
				result = key;
			return result;
		}
		
		/// <summary>
		///	 True if the type with name "key" must be decoded using a static method.
		/// <summary>
		public bool IsStatic (string key)
		{
			if (staticTypes == null)
				return false;
			return staticTypes.Contains (key);
		}

	}
	
}
