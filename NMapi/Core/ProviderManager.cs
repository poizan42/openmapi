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
	
	// TODO: This should be a proper class not a collection of static methods ...

	/// <summary>
	///  A static helper class that contains some methods to load/select 
	///  a provider from an assembly or find the providers available.
	/// </summary>
	public static class ProviderManager
	{
		
		private static byte[] trustedPublicKey;
		
		/// <summary>
		///  Creates an instance of a provider (an IMapiFactory-class) from the 
		///  specified assembly/type-pair.
		/// </summary>
		public static IMapiFactory GetFactory (string[] pair)
		{
			return GetFactory (pair [0], pair [1]);
		}

		/// <summary>
		///  Creates an instance of a provider (an IMapiFactory-class) from the 
		///  specified assembly/type-pair.
		/// </summary>
		public static IMapiFactory GetFactory (string assemblyName, string typeName)
		{
			object o = Activator.CreateInstance (assemblyName, typeName).Unwrap () as IMapiFactory;
			if (o == null)
				throw new Exception ("Couldn't create backend factory!");
			return (IMapiFactory) o;
		}
		
		/// <summary>Checks if a file is an assembly and if it can be trusted.</summary>
		/// <paeram name="filename">The filename of the assembly.</param>
		/// <returns>True if the file is an assembly AND it has a matching public key.</returns>
		private static bool IsTrustedAssembly (string filename)
		{			
			if (filename == null)
				throw new ArgumentNullException (filename);
			try {
				
				
//				byte[] opk = Assembly.GetExecutingAssembly ().GetName ().GetPublicKey ();
//				foreach (var b in opk)
//					Console.Write ("0x" + b.ToString ("X") + ", ");
//				Console.WriteLine ("---");
				
				// TODO: throw proper exceptions ...
				
				// TODO: here we can check if the public key does match 
				//       our public key.
				AssemblyName an = AssemblyName.GetAssemblyName (filename);
				if (an != null) {
					byte[] publicKey = an.GetPublicKey ();
			
					if (publicKey == null || publicKey.Length < 1)
						return false;

					// TODO: integrity check ...
					
					if (trustedPublicKey == null) {
					
						trustedPublicKey = Assembly.GetExecutingAssembly ().GetName ().GetPublicKey ();

			/*					trustedPublicKey = new byte[] { 
						0x0, 0x24, 0x0, 0x0, 0x4, 0x80, 0x0, 0x0, 0x94, 0x0, 0x0, 0x0, 0x6, 0x2, 
						0x0, 0x0, 0x0, 0x24, 0x0, 0x0, 0x52, 0x53, 0x41, 0x31, 0x0, 0x4, 0x0, 0x0, 
						0x11, 0x0, 0x0, 0x0, 0x1D, 0x8A, 0xAF, 0x52, 0x69, 0x7A, 0x77, 0xB4, 0x3E, 
						0x98, 0x27, 0x33, 0xB1, 0x78, 0xB6, 0xB6, 0xA7, 0x9B, 0x9B, 0x3D, 0x88, 0x6C, 
						0x70, 0xEE, 0x63, 0xB, 0xB3, 0x7A, 0x6F, 0xA8, 0x12, 0x16, 0x63, 0xB5, 0x5F, 
						0x8F, 0x94, 0xFD, 0xB6, 0xFB, 0x52, 0x66, 0x61, 0x81, 0xEF, 0x7, 0x11, 0x16, 
						0xB, 0xC0, 0x64, 0xBF, 0x8A, 0x1B, 0xC7, 0x73, 0x44, 0xC6, 0x49, 0xFD, 0xFF, 
						0x18, 0xBF, 0x89, 0x13, 0xA5, 0x14, 0x8F, 0x54, 0x5A, 0x9B, 0xE0, 0xAC, 0xEF, 
						0xD1, 0x69, 0x42, 0x80, 0xCE, 0xFA, 0xA2, 0x9A, 0xB9, 0x7E, 0x36, 0xE2, 0x22, 
						0x35, 0xEA, 0x4F, 0x27, 0x7C, 0x22, 0x25, 0x7F, 0xAE, 0xD9, 0xC1, 0xD2, 0x60, 
						0x7B, 0xC1, 0xB3, 0xB9, 0x4D, 0xAA, 0x8B, 0xBF, 0x19, 0x93, 0xE4, 0xA3, 0x25, 
						0x71, 0xEE, 0xE7, 0x32, 0x39, 0xFD, 0x47, 0xA2, 0xA4, 0x41, 0x3E, 0x8F, 0x1, 
						0x99, 0xB9 } ;
					*/
					
					}
				
					if (publicKey.Length != trustedPublicKey.Length)
						return false;
					for (int i=0; i < trustedPublicKey.Length; i++)
						if (publicKey [i] != trustedPublicKey [i])
							return false;

				}
				return true;
			} catch (FileNotFoundException) {
				// ignore
			} catch (BadImageFormatException) {
				// ignore
			} catch (FileLoadException) {
				// ignore
			} catch (Exception) {
				// ignore
			}
			return false;
		}
		

		/// <summary>
		///  Finds the providers that exist in the same directory as the NMapi 
		///  assembly itself.
		/// </summary>
		public static Dictionary<string, string[]> FindProviders ()
		{
			var list = new Dictionary<string, string[]> ();

			string codebase = Assembly.GetExecutingAssembly ().CodeBase;
			string baseDirectory = Path.GetDirectoryName (new Uri (codebase).LocalPath);

			foreach (string asmName in Directory.GetFiles (baseDirectory, "*.dll")) {

				AssemblyDefinition assembly = null;
				
				if (!IsTrustedAssembly (asmName))
					continue;
				
				try {
					assembly = AssemblyFactory.GetAssembly (asmName);
				} catch (Mono.Cecil.Binary.ImageFormatException) {
					// Ignore, because this is probably just a win32 dll.
					continue;
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

		/// <summary>
		///  Returns the name of the provider.
		/// </summary>
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
