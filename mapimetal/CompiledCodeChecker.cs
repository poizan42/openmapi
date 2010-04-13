//
// openmapi.org - NMapi C# Mapi API - CompiledCodeChecker.cs
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

	/// <summary>
	///  Checks a hand-written class for some common mistakes.
	/// </summary>
	public sealed class CompiledCodeChecker
	{
		private TypeResolver resolver;

		public TypeResolver Resolver {
			get { return resolver; }
		}

		public CompiledCodeChecker ()
		{
			resolver = new TypeResolver ();
			resolver.AddAssembly ("NMapi", new Version (0, 1));
		}

		private MethodReference CheckForCall (MethodBody mbody, string callName)
		{
			foreach (Instruction instr in mbody.Instructions) {
				if (instr.OpCode.Name == "callvirt") {
					var methRef = instr.Operand as MethodReference;
					if (methRef != null && methRef.Name == callName)
						return methRef;
				}
			}
			return null;
		}

		private bool ImplementsInterface (TypeDefinition type, string ns, string name)
		{
			foreach (TypeReference interf in type.Interfaces)
				if (interf.Name == name && interf.Namespace == ns)
					return true;
			return false;
		}

		public void CheckAssembly (string fileName)
		{
			Console.WriteLine ("Checking: " + fileName);
			AssemblyDefinition assembly = null;
			try {
#if CECIL_0_9
				assembly = AssemblyDefinition.ReadAssembly (fileName);
#else
				assembly = AssemblyFactory.GetAssembly (fileName);
#endif
			} catch (System.BadImageFormatException) {
				Driver.WriteError ("File '" + fileName + "' is not a valid assembly.");
				Driver.ExitWithStats ();				
			}
			foreach (ModuleDefinition module in assembly.Modules)
				foreach (TypeDefinition type in module.Types)
					CheckType (type);
			Console.WriteLine ("Done.");
		}

		private void CheckType (TypeDefinition type)
		{
			Console.WriteLine ("Checking type: " + type.Name);
			bool containsPropertyAttributes = false;
			foreach (PropertyDefinition property in type.Properties) {
				int count = 0;
				bool isMarkedLazy = false;
				foreach (CustomAttribute attribute in property.CustomAttributes) {
					if (attribute.Constructor.DeclaringType.Name 
						== "MapiPropertyAttribute")
					{
						count++;
						foreach (var paramDef in attribute.Constructor.Parameters) {
							// TODO: check: data in parameters!
							//	if ( ...  == Driver.MAPI_LINQ_NS + ".LoadMode") {
							//		isMarkedLazy = true;
						}
					}
				}
				if (count == 0)
					continue;
				containsPropertyAttributes = true;
				//
				// We now only process properties with 
				// MapiPropertyAttribute-Attributes!
				//

				if (count > 1)
					Driver.WriteError ("Multiple are attributes not allowed " +
						"on property " + property.Name);
/*
#if CECIL_0_9
				string returnTypeName = property.GetMethod.ReturnType.Name;
#else
				string returnTypeName = property.GetMethod.ReturnType.ReturnType.Name;
#endif
				// TODO! -> compare with type!

*/
				// TODO: Check parameters!
				MethodReference method;
				if (isMarkedLazy) {
					method = CheckForCall (property.GetMethod.Body, "LazyLoad");
					if (method == null)
						Driver.WriteError ("LazyLoad () MUST be called " + 
							"in property " + property.Name + " getter. ");
		
				} else {
					method = CheckForCall (property.GetMethod.Body, "LazyLoad");
				// TODO: uncomment
				//	if (method != null)
				//		Driver.WriteError ("LazyLoad () must NOT called " + 
				//			"in " + property.FullName " getter, "
				//			"because it is not marked as \"Lazy\"."));

				}

				// TODO: Check parameters!
				method = CheckForCall (property.SetMethod.Body, "OnPropertyChanging");
				if (method == null)
					Driver.WriteError ("setter MUST call OnPropertyChanging ()" + 
							"in property " + property.Name + " setter, ");

				method = CheckForCall (property.SetMethod.Body, "OnPropertyChanged");
				if (method == null)
					Driver.WriteError ("setter MUST call OnPropertyChanged ()" + 
							"in property " + property.Name + " setter, ");
			}

			if (containsPropertyAttributes) {
				bool mapEntityFound = false;
				TypeDefinition curTypeDef = type;

				while (curTypeDef != null) {
					if (ImplementsInterface (curTypeDef, 
						"NMapi.Table.Linq", "IMapiEntity"))
					{
						mapEntityFound = true;
						break;
					}
					curTypeDef =  resolver.ResolveDefinition (curTypeDef.BaseType);
				}
				if (!mapEntityFound)
					throw new Exception ("Class contains members with " + 
						"MapiPropertyAttributes, but does not implement " + 
						"IMapiEntity or BaseClass can't be found!");
			}

		}

	}

}

