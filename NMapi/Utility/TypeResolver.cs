//
// openmapi.org - NMapi C# Mapi API - TypeResolver.cs
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

namespace NMapi {

	/// <summary>
	///  A helper class used to resolve C# types from assemblies 
	///  (without loading the assemblies into the current application domain.)
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public sealed class TypeResolver
	{
		private IAssemblyResolver resolver;
		private Dictionary<string, AssemblyDefinition> map;
		private Queue<AssemblyNameReference> deferredResolution;

		public TypeResolver ()
		{
			deferredResolution = new Queue<AssemblyNameReference> ();
			map = new Dictionary<string,AssemblyDefinition> ();
			resolver = new DefaultAssemblyResolver ();
		}

		/// <summary></summary>
		/// <param name="name"></param>
		/// <param name="version"></param>
		public void AddAssembly (string name, string version)
		{
			AddAssembly (name, new Version (version));
		}
		
		/// <summary></summary>
		/// <param name="name">The name of the assembly.</param>
		/// <param name="version">The assembly version as a string (Format: "a.b.c.d") to be loaded.</param>
		public void AddAssembly (string name, Version version)
		{
			#if CECIL_0_9
				AssemblyNameReference asm = new AssemblyNameReference (name, version);
			#else
				AssemblyNameReference asm = new AssemblyNameReference ();
				asm.Name = name;
				asm.Version = version;
			#endif
			AddAssembly (asm);
		}

		/// <summary>
		///  
		/// </summary>
		/// <param name="asmRef"></param>
		public void AddAssembly (AssemblyNameReference asmRef)
		{
			deferredResolution.Enqueue (asmRef);
		}

		private void ProcessQueue ()
		{
			while (deferredResolution.Count > 0) {
				AssemblyNameReference asmRef = deferredResolution.Dequeue ();
				string name = asmRef.Name + asmRef + "___" + asmRef.Version.ToString ();
				if (!map.ContainsKey (name)) {
					AssemblyDefinition asmDef = resolver.Resolve (asmRef);
					if (asmDef != null)
						map [name] = asmDef;
					else {
						// TODO: Driver.WriteWarning -- exception ?
						Console.WriteLine ("Assembly '" + 
							asmRef.Name + "' can't be loaded.");
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static string[] SplitTypeNameAndProperty (string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			int index = name.LastIndexOf ('.');
			if (index < 0)
				throw new NotSupportedException ("Full typename required.");
			string[] result = new string [2];
			result [0] = name.Substring (0, index);
			result [1] = name.Substring (index+1);
			return result;
		}

		/// <summary></summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public TypeDefinition ResolveDefinition (string typeName)
		{
			if (typeName == null)
				throw new ArgumentNullException ("typeName");
			ProcessQueue ();
			TypeDefinition td = null;
			foreach (AssemblyDefinition asmDef in map.Values)
				foreach (ModuleDefinition module in asmDef.Modules) {
#if CECIL_0_9
					td = (from t in module.Types
						where t.Name == typeName
						select t).First ();
#else
					td = module.Types [typeName];
#endif
					if (td != null)
						return td;
				}
			return null;
		}

		/// <summary>
		///  
		/// </summary>
		/// <param name="typeRef"></param>
		/// <returns></returns>
		public TypeDefinition ResolveDefinition (TypeReference typeRef)
		{
			Console.WriteLine (typeRef.FullName);
			return ResolveDefinition (typeRef.FullName);
		}

	}

}

