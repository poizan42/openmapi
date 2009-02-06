//
// openmapi.org - NMapi - preproc.cs
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
using System.IO;
using System.Collections.Generic;
using System.Linq;

using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

namespace NMapi.Tools.PreProc
{
	/// <summary>
	///  Simply strips the "hints"; This basically outputs something 
	///  very close to the original COM MAPI api.
	/// <summary>
	public sealed class StripProc : BasePreProc
	{
		protected override string Transform (string txt, IdentifierType itype)
		{
			return StripChars (txt, '^', '#');
		}
	}


	/// <summary>
	///  Generates JavaScript identifiers.
	/// <summary>
	public sealed class JavaScriptProc : BasePreProc
	{
		public JavaScriptProc ()
		{
			SetKeywords ("abstract", "boolean", "break", "byte", "case", 
				"catch", "char", "class", "const", "continue", "debugger", 
				"default", "delete", "do", "double", "else", "enum", "export", 
				"extends", "false", "final", "finally", "float", "for", 
				"function", "goto", "if", "implements", "import", "in", 
				"instanceof", "int", "interface", "long", "native", "new", 
				"null", "package", "private", "protected", "public", "return", 
				"short", "static", "super", "switch", "synchronized", "this", 
				"throw", "throws", "transient", "true", "try", "typeof", 
				"var", "void", "volatile", "while", "with");
		}
		
		// copied from JavaProc
		protected override string Transform (string txt, IdentifierType itype)
		{
			string result = "";
			bool nextUpper = false;
			string lowerStripped = StripChars (txt, '_').ToLower ();
			foreach (char _c in lowerStripped) {
				char currentChar = _c;
				if (currentChar == '^' || currentChar == '#') {
					if ( (itype != IdentifierType.Method && 
							itype != IdentifierType.Property)
						|| result.Length > 0)
					{
						nextUpper = true;
					}
					continue;
				}
				if (nextUpper) {
					currentChar = Char.ToUpper (currentChar);
					nextUpper = false;
				}
				result += currentChar;
			}
			return result;
		}
	}
	
	
	/// <summary>
	///  Generates pythonic identifiers.
	/// <summary>
	public sealed class PythonProc : BasePreProc
	{
		public PythonProc ()
		{
			SetKeywords ("and", " del", "for", "is", "raise", "assert", 
				"elif", "from", "lambda", "return", "break", "else", 
				"global", "not", "try", "class", "except", "if", "or", 
				"while", "continue", "exec", "import", "pass", "yield", 
				"def", "finally", "in", "print");
		}
		
		protected override string Transform (string txt, IdentifierType itype)
		{
			string result = "";
			string lowerStripped = StripChars (txt, '_', '#').ToLower ();
			foreach (char currentChar in lowerStripped) {
				if (currentChar == '^') {
					if (result.Length > 0)
						result += "_";
					continue;
				}
				result += currentChar;
			}
			return result;
		}
	}
	
	/// <summary>
	///  Generates Java-like identifiers.
	/// <summary>
	public sealed class JavaProc : BasePreProc
	{
		public JavaProc ()
		{
			SetKeywords (
				"abstract", "continue", "for", "new", "switch", "assert", 
				"default", "goto", "package", "synchronized", "boolean", 
				"do", "if", "private", "this", "break", "double", 
				"implements", "protected", "throw", "byte", "else", "import", 
				"public", "throws", "case", "enum", "instanceof", "return", 
				"transient", "catch", "extends", "int", "short", "try", 
				"char", "final", "interface", "static", "void", "class", 
				"finally", "long", "strictfp", "volatile", "const", "float", 
				"native", "super", "while");
		}
		
		protected override string Transform (string txt, IdentifierType itype)
		{
			string result = "";
			bool nextUpper = false;
			string lowerStripped = StripChars (txt, '_').ToLower ();
			foreach (char _c in lowerStripped) {
				char currentChar = _c;
				if (currentChar == '^' || currentChar == '#') {
					if ( (itype != IdentifierType.Method && 
							itype != IdentifierType.Property)
						|| result.Length > 0)
					{
						nextUpper = true;
					}
					continue;
				}
				if (nextUpper) {
					currentChar = Char.ToUpper (currentChar);
					nextUpper = false;
				}
				result += currentChar;
			}
			return result;
		}
	}
	
	/// <summary>
	///  Generates C#-like identifiers.
	/// <summary>
	public sealed class CSharpProc : BasePreProc
	{
		public CSharpProc ()
		{
			SetKeywords (
				"abstract", "event", "new", "struct", "as", "explicit", 
				"null", "switch", "base", "extern", "object", "this", 
				"bool", "false", "operator", "throw", "break", "finally", 
				"out", "true", "byte", "fixed", "override", "try", "case", 
				"float", "params", "typeof", "catch", "for", "private", 
				"uint", "char", "foreach", "protected", "ulong", "checked", 
				"goto", "public", "unchecked", "class", "if", "readonly", 
				"unsafe", "const", "implicit", "ref", "ushort", "continue", 
				"in", "return", "using", "decimal", "int", "sbyte", "virtual", 
				"default", "interface", "sealed", "volatile", "delegate", 
				"internal", "short", "void", "do", "is", "sizeof", "while", 
				"double", "lock", "stackalloc", "else", "long", "static", 
				"enum", "namespace", "string");
		}
		
		protected override string Transform (string txt, IdentifierType itype)
		{
			string result = "";
			bool nextUpper = false;
			string lowerStripped = StripChars (txt, '_').ToLower ();
			foreach (char _c in lowerStripped) {
				char currentChar = _c;
				if (currentChar == '^' || currentChar == '#') {
					nextUpper = true;
					continue;
				}
				if (nextUpper) {
					currentChar = Char.ToUpper (currentChar);
					nextUpper = false;
				}
				result += currentChar;
			}
			return result;
		}
	}
	
	public enum IdentifierType {
		Method,
		Property,
		Interface,
		None
	}

	/// <summary>
	///  Transforms identifiers in the mapi.xml file according to 
	///  language-specific rules. Bascially outputs a "skinned" api definition.
	/// <summary>
	public abstract class BasePreProc
	{
		private const string seeMsdnPrefix = "See MSDN: http://msdn2.microsoft.com/en-us/library/ms";
		private string[] keywords;
		private string currentInterface;
		private string currentMethod;
		
		public static void Main (string[] args)
		{
			if (args.Length < 3) {
				Console.WriteLine ("ERROR: Not enough arguments.");
				Console.WriteLine ("usage: preproc [language] [inputFile] [outputFile]");
				return;
			}
			string lang = args [0];
			string schemaFile = args [1];
			string inFile = args [2];
			string outFile = args [3];

			BasePreProc proc = null;
			switch (lang) {
				case "csharp": proc = new CSharpProc (); break;
				case "java": proc = new JavaProc (); break;
				case "python": proc = new PythonProc (); break;
				case "jscript": proc = new JavaScriptProc (); break;
				case "strip": proc = new StripProc (); break;
				default:
					Console.WriteLine ("ERROR: Invalid language selected!");
					return;
			}

			proc.Run (inFile, schemaFile, outFile);
		}

		public void Run (string inFile, string schemaFile, string outFile)
		{
			if (!File.Exists (inFile) || !File.Exists (schemaFile))
				throw new FileNotFoundException (inFile);

			var schemaSet = new XmlSchemaSet ();
			schemaSet.Add (null, schemaFile);
			var settings = new XmlReaderSettings ();
			settings.ValidationEventHandler += (sender, args) => {
				Console.WriteLine ("ERROR: The file '" + inFile + "' is not " + 
					"valid for the schema '" + schemaFile +"'!");
				Environment.Exit (1);
			};
			settings.ValidationType = ValidationType.Schema;
			settings.Schemas = schemaSet;
			
			XElement xml = XElement.Load (XmlReader.Create (inFile, settings));
			xml.AddFirst (new XComment ("This file has been AUTOGENERATED from '" + 
				inFile + "' using 'preproc'. DO NOT EDIT!"));

			foreach (var node in xml.Elements ("interface"))
				ProcessInterfaceNode (node);

			xml.Save (outFile);
		}
		
		protected void SetKeywords (params string[] keywords)
		{
			this.keywords = keywords;
		}

		private string ReplaceKeywords (string name)
		{
			if (name == null || keywords == null)
				return name;

			string result = name;
			bool changed = false;
			do {
				changed = false;
				foreach (var current in keywords) {
					if (result == current) {
						result = "_" + result;
						changed = true;
						break;
					}
				}
			} while (changed);
			return result;
		}
		
		private string Process (string txt, IdentifierType itype)
		{
			return Transform (ReplaceVariables (txt), itype);
		}
			
		protected abstract string Transform (string txt, IdentifierType itype);

		protected string StripChars (string txt, params char[] stripChars)
		{
			var stripped = txt.Where (c => !stripChars.Contains (c));
			return new String (stripped.ToArray ());
		}

		private void ProcessAttributes (XElement interfaceNode, 
			string[] attribs, IdentifierType itype)
		{
			foreach (var attrib in attribs) {
				if (interfaceNode.Attribute (attrib) != null) {
					interfaceNode.Attribute (attrib).Value = Process (
							(string) interfaceNode.Attribute (attrib), itype);
				}
			}
		}
		
		private void ProcessValueNode (XElement node)
		{
			node.Value = Process ((string) node.Value, IdentifierType.None);
		}

		private string ReplaceVariables (string txt)
		{
			string replaced = txt;
			if (currentInterface != null)
				replaced = replaced.Replace ("$$INTERFACE", currentInterface);
			if (currentMethod != null)
				replaced = replaced.Replace ("$$METHOD", currentMethod);
			replaced = replaced.Replace ("$$MSDN_", seeMsdnPrefix);
			return replaced;
		}
		
		private void ProcessDocNode (XElement docNode)
		{
			foreach (var node in docNode.Elements ("summary"))
				node.Value = ReplaceVariables (node.Value);
			foreach (var node in docNode.Elements ("remarks"))
				node.Value = ReplaceVariables (node.Value);
		}
		
		private void ProcessInterfaceNode (XElement interfaceNode)
		{
			string[] attribs = new string[] {"id", "implements"};
			ProcessAttributes (interfaceNode, attribs, IdentifierType.Interface);
			currentInterface = interfaceNode.Attribute ("id").Value;

			foreach (var node in interfaceNode.Elements ("doc"))
				ProcessDocNode (node);
			foreach (var node in interfaceNode.Elements ("method"))
				ProcessMethodOrPropertyNode (node, IdentifierType.Method);
			foreach (var node in interfaceNode.Elements ("property"))
				ProcessMethodOrPropertyNode (node, IdentifierType.Property);
		}
		
		private void ProcessMethodOrPropertyNode (XElement methodNode, IdentifierType itype)
		{
			ProcessAttributes (methodNode, new string[] {"id"}, itype);
			currentMethod = methodNode.Attribute ("id").Value;

			foreach (var node in methodNode.Elements ("doc"))
				ProcessDocNode (node);
			foreach (var node in methodNode.Elements ("returns"))
				ProcessValueNode (node);
			foreach (var paramNode in methodNode.Elements ("param")) {
				ProcessAttributes (paramNode, new string[] {"type"}, 
					IdentifierType.Interface); // probably interface ...
				paramNode.Value = ReplaceKeywords ((string) paramNode.Value);
			}
		}
		
	}
	
}
