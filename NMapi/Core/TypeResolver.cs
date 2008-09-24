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

		public void AddAssembly (string name, string version)
		{
			AddAssembly (name, new Version (version));
		}
		public void AddAssembly (string name, Version version)
		{
			AssemblyNameReference asm = new AssemblyNameReference ();
			asm.Name = name;
			asm.Version = version;
			AddAssembly (asm);
		}

		public void AddAssembly (AssemblyNameReference asmRef)
		{
			deferredResolution.Enqueue (asmRef);
		}

		public void ProcessQueue ()
		{
			while (deferredResolution.Count > 0) {
				AssemblyNameReference asmRef = deferredResolution.Dequeue ();
				string name = asmRef.Name + asmRef + "___" + asmRef.Version.ToString ();
				if (!map.ContainsKey (name)) {
					AssemblyDefinition asmDef = resolver.Resolve (asmRef);
					if (asmDef != null)
						map [name] = asmDef;
					else {
						// TODO: Driver.WriteWarning 
						Console.WriteLine ("Assembly '" + 
							asmRef.Name + "' can't be loaded.");
					}
				}
			}
		}

		public static string[] SplitTypeNameAndProperty (string name)
		{
			int index = name.LastIndexOf ('.');
			if (index < 0)
				throw new NotSupportedException ("Full typename required.");
			string[] result = new string [2];
			result [0] = name.Substring (0, index);
			result [1] = name.Substring (index+1);
			return result;
		}


		public TypeDefinition ResolveDefinition (string typeName)
		{
			ProcessQueue ();
			TypeDefinition td = null;
			foreach (AssemblyDefinition asmDef in map.Values)
				foreach (ModuleDefinition module in asmDef.Modules) {
					td = module.Types [typeName];
					if (td != null)
						return td;
				}
			return null;
		}

		public TypeDefinition ResolveDefinition (TypeReference typeRef)
		{
			Console.WriteLine (typeRef.FullName);
			return ResolveDefinition (typeRef.FullName);
		}

	}

}

