//
// openmapi.org - NMapi C# Mapi API - Driver.cs
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

using NDesk.Options;

using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Microsoft.JScript;

#if WITH_BOO
using Boo.CodeDom;
#endif

namespace NMapi.Linq {

	public sealed class Driver
	{
		public const string MAPI_LINQ_NS = "NMapi.Linq";

		private static int errors = 0;

		public static int Errors {
			get { return errors; }
		}

		public static void Main (string [] args)
		{
			List<string> asmList = new List<string> ();
			string outputFile = null;
			string fileExtension = null;
			string action = "generate";

			var checker = new CompiledCodeChecker ();
			var generator = new Generator ();

			if (args.Length < 1) {
				Console.WriteLine ("Please specify at least one xml file to process.");
				return;
			}

			// get options ...

			string lang = "csharp";

			OptionSet p = new OptionSet ()
				.Add ("language=|l=", l => lang = l)
				.Add ("o=|out=", o => outputFile = o)
				.Add ("a=|assembly=", name => asmList.Add (name) )
				.Add ("c|check", c => action = "check")
				.Add ("axml|autoxml", c => action = "autoxml")
				.Add ("version|help|h|?", l => {
					PrintAbout ();
					return;
				});
			List<string> rest = p.Parse (args);

			foreach (string nameAndVersion in asmList) {
				string[] pair = nameAndVersion.Split (',');
				if (pair.Length != 2) {
					Console.WriteLine ("Assemblies must be specified as 'name,version'!");
					return;
				}
				string assemblyName = pair [0].Trim ();
				string versionString = pair [1].Trim ();
				generator.Resolver.AddAssembly (assemblyName, versionString);
				checker.Resolver.AddAssembly (assemblyName, versionString);
			}

			switch (lang) {
				case "cs":
				case "csharp":
					generator.CodeGenerator = new CSharpCodeProvider ().CreateGenerator ();
					fileExtension = "cs";
				break;
				case "vb":
				case "basic":
					generator.CodeGenerator = new VBCodeProvider ().CreateGenerator ();
					fileExtension = "vb";
				break;
				case "js":
				case "jscript":
					generator.CodeGenerator = new JScriptCodeProvider ().CreateGenerator ();
					fileExtension = "js";
				break;
				#if WITH_BOO
				case "boo":

					generator.CodeGenerator = new BooCodeGenerator ().CreateGenerator ();
					fileExtension = "boo";
				break;
				#endif
			}

			foreach (string fileName in rest) {
				if (File.Exists (fileName)) {
					switch (action) {
						case "generate":
							if (generator.CodeGenerator == null) {
								Console.WriteLine ("You must specify a valid target-language.");
								return;
							}
							string ofile = outputFile;
							if (ofile == null)
								ofile = fileName + "_Generated." + fileExtension;
							generator.Process (fileName, ofile);
						break;
						case "check":
							checker.CheckAssembly (fileName);
						break;
						case "autoxml":
							throw new NotImplementedException (
								"This feature has not been implemented, yet.");
					}
				}
				else
					Console.WriteLine ("File '" + fileName + "' not found. Skipped. ");
			}

		}

		private static void PrintAbout ()
		{
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine ("MapiMetal " + version.Major + "." 
				+ version.Minor + " - NMapi Linq Entity-Class generator");
			Console.WriteLine ("For more information see: http://www.openmapi.org\n");
			Console.WriteLine ("Usage: mapimetal [OPTION] ...  [FILE]\n");
			Console.WriteLine ("-l, -language=LANG     Specify output language (e.g. csharp, vb, jscript, boo)");
			Console.WriteLine ("-c, -check             Check manually generated assembly.");
			Console.WriteLine ("-axml, -autoxml        Generate the xml file from a mapi object.");
			Console.WriteLine ("-a, -assembly          Reference assembly for type checking.");
			Console.WriteLine ("-o, -out               Output file. Optional.");
			Console.WriteLine ("-h, -help              Show this help\n\n");
		}


		public static void ExitWithStats ()
		{
			Console.WriteLine ("Failed - " + errors + " Errors.");
			Environment.Exit (-1);
		}

		public static void WriteWarning (string msg)
		{
			WriteWarning (-1, -1, msg);
		}

		public static void WriteWarning (int line, int pos, string msg)
		{
			WriteMsg ("warning", line, pos, msg);
		}

		public static void WriteError (string msg)
		{
			WriteError (-1, -1, msg);
		}

		public static void WriteError (int line, int pos, string msg)
		{
			WriteMsg ("error", line, pos, msg);
			errors++;
		}

		private static void WriteMsg (string etype, int line, int pos, string msg)
		{
			if (line < 0 || pos < 0)
				Console.WriteLine (etype + ": " + msg);
			else
				Console.WriteLine ("Line " + line + 
					", Position " + pos + " (" + etype + ") : " + msg);
		}
	}

}

