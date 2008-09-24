//
// openmapi.org - NMapi C# Mapi API - ProviderManager.cs
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
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using NMapi;
using NMapi.Events;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Utility;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NMapi {

	public static class ProviderManager
	{
		public static IMapiFactory GetFactory (string[] pair)
		{
			return GetFactory (pair [0], pair [1]);
		}

		public static IMapiFactory GetFactory (string assemblyName, string typeName)
		{
			object o = Activator.CreateInstance (assemblyName, typeName).Unwrap () as IMapiFactory;
			if (o == null)
				throw new Exception ("Couldn't create backend factory!");
			return (IMapiFactory) o;
		}

		public static Dictionary<string, string[]> FindProviders ()
		{
			var list = new Dictionary<string, string[]> ();

			string codebase = Assembly.GetExecutingAssembly ().CodeBase;
			string baseDirectory = Path.GetDirectoryName (new Uri (codebase).LocalPath);

			foreach (string asmName in Directory.GetFiles (baseDirectory, "*.dll")) {

				AssemblyDefinition assembly = null;
				try {
					assembly = AssemblyFactory.GetAssembly (asmName);
				} catch (System.BadImageFormatException) {
					throw new Exception ("File '" + asmName + "' is not a valid assembly.");
				}

				foreach (ModuleDefinition module in assembly.Modules)
					foreach (TypeDefinition type in module.Types) {
						if (IsFactory (type)) {
							string provId = GetProviderID (type);
							if (list.ContainsKey (provId))
								throw ProviderConflict ();
							if (provId == null)
								throw new Exception ("Provider must have an attribute with a name != null!");
							list [provId] = new string [] { assembly.Name.Name, type.FullName};
						}
					}
			}

			return list;
		}

		private static Exception ProviderConflict ()
		{ 
			return new Exception ("Conflict: Two NMapi providers " + 
				"with the same name were found!");
		}

		private static string GetProviderID (TypeDefinition type)
		{
			foreach (CustomAttribute attribute in type.CustomAttributes) {
				if (attribute.Constructor.DeclaringType.Name == 
					"MapiFactoryAttribute")
						return (string) attribute.ConstructorParameters [0];
			}
			return null;
		}

		private static bool IsFactory (TypeDefinition type)
		{ 
			return CecilUtil.ImplementsInterface (type, "NMapi", typeof (IMapiFactory).Name);
		}

		public static string GetName (IMapiFactory factory)
		{
			if (factory == null)
				throw new ArgumentException ("factory");
			var attribute = (MapiFactoryAttribute)  Attribute.GetCustomAttribute (
						factory.GetType (), 
						typeof (MapiFactoryAttribute));
			return attribute.Name;
		}


	}

}



