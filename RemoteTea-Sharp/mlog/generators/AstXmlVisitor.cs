//
// openmapi.org - CompactTeaSharp - MLog - AstXmlVisitor.cs
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
using System.Reflection;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;

using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace CompactTeaSharp.Mlog
{

	/// <summary>
	///   A generator that produces an xml representation of the AST.
	/// </summary>
	public sealed class AstXmlVisitor : AbstractVisitor
	{
		private XElement root;

		public override string[] Targets {
			get {
				return new string [] {"astxml"};
			}
		}
		
		public override string Name {
			get {
				return "AST XML Generator";
			}
		}
			
		public override string Description {
			get {
				return "Dumps the .x specification as xml.";
			}
		}
		
		private XElement MakeAndAddElement (object pObj, string name, 
			params XAttribute[] attribs)
		{
			var parent = pObj as XElement;
			var element = new XElement (name);
			element.ReplaceAttributes (attribs);
			parent.Add (element);
			return element;
		}
		
		public override void Dump ()
		{
			string tmpFile = "_generated__AST_xml_test.xml";
			using (TextWriter streamWriter = new StreamWriter (tmpFile))
			{
				streamWriter.WriteLine (root.ToString ());
			}
		}
		
		public override void VisitDotXNode (DotXNode node, object arg)
		{
			root = new XElement ("oncrpc");
			root.Add (new XComment (
				" This file was autogenerated by MLog from a .x file.\n" + 
				"      DO NOT EDIT! Instead edit the .x file and rerun the generator. "));
			base.VisitDotXNode (node, root);
		}

		public override void VisitProgramNode (ProgramNode node, object arg)
		{
			var programElement = MakeAndAddElement (arg, "program", 
				new XAttribute ("id", node.Identifier),
				new XAttribute ("num", node.ProgramNumber));
			base.VisitProgramNode (node, programElement);
		}

		public override void VisitVersionNode (VersionNode node, object arg)
		{
			var versionElement = MakeAndAddElement (arg, "version",
				new XAttribute ("id", node.Identifier),
				new XAttribute ("num", node.VersionNumber));
			base.VisitVersionNode (node, versionElement);
		}

		public override void VisitCallNode (CallNode node, object arg)
		{
			var callElement = MakeAndAddElement (arg, "call", 
				new XAttribute ("return", node.Ret),
				new XAttribute ("arg", node.Arg));
			callElement.Add (new XAttribute ("id", node.Id));
			base.VisitCallNode (node, callElement);
		}

		public override void VisitVersionNodeList (VersionNodeList list, object arg)
		{
			var versionsElement = MakeAndAddElement (arg, "versions");
			base.VisitVersionNodeList (list, versionsElement);
		}

		public override void VisitDefinitionNodeList (DefinitionNodeList list, object arg)
		{
			var definitionsElement = MakeAndAddElement (arg, "definitions");
			base.VisitDefinitionNodeList (list, definitionsElement);
		}

		public override void VisitCallNodeList (CallNodeList list, object arg)
		{
			var callsElement = MakeAndAddElement (arg, "calls");
			base.VisitCallNodeList (list, callsElement);
		}

		public override void VisitDefConstNode (DefConstNode node, object arg)
		{
			var constElement = MakeAndAddElement (arg, "const",
				new XAttribute ("id", node.Identifier),
				new XAttribute ("value", node.Constant));
			base.VisitDefConstNode (node, constElement);
		}

		public override void VisitDefEnumNode (DefEnumNode node, object arg)
		{
			var enumElement = MakeAndAddElement (arg, "enum",
				new XAttribute ("id", node.Identifier));
			base.VisitDefEnumNode (node, enumElement);
		}

		public override void VisitAssignmentNode (AssignmentNode node, object arg)
		{
			var assignmentElement = MakeAndAddElement (arg, "assignment");
			base.VisitAssignmentNode (node, assignmentElement);
		}

		public override void VisitIdentifierNode (IdentifierNode node, object arg)
		{
			var identifierElement = MakeAndAddElement (arg, "identifier");
			base.VisitIdentifierNode (node, identifierElement);
		}

		public override void VisitDefStructNode (DefStructNode node, object arg)
		{
			var structElement = MakeAndAddElement (arg, "struct", 
				new XAttribute ("id", node.Identifier));
			base.VisitDefStructNode (node, structElement);
		}

		public override void VisitDefTypeDefNode (DefTypeDefNode node, object arg)
		{
			var typeDefElement = MakeAndAddElement (arg, "typedef");
			base.VisitDefTypeDefNode (node, typeDefElement);			
		}

		public override void VisitDefUnionNode (DefUnionNode node, object arg)
		{
			var unionElement = MakeAndAddElement (arg, "union");
			base.VisitDefUnionNode (node, unionElement);
		}

		public override void VisitConstantNode (ConstantNode node, object arg)
		{
			var unionElement = MakeAndAddElement (arg, "constant");
			base.VisitConstantNode (node, unionElement);
		}
		
		public override void VisitDeclarationNode (DeclarationNode node, object arg)
		{
			var declElement = MakeAndAddElement (arg, "declaration");
			base.VisitDeclarationNode (node, declElement);
		}
		
		public override void VisitIdentifierNodeList (IdentifierNodeList node, object arg)
		{
			var identifierElement = MakeAndAddElement (arg, "identifiers");
			base.VisitIdentifierNodeList (node, identifierElement);
		}
		
		public override void VisitAssignmentNodeList (AssignmentNodeList node, object arg)
		{
			var assignmentElement = MakeAndAddElement (arg, "assignments");
			base.VisitAssignmentNodeList (node, assignmentElement);
		}
		
		public override void VisitDeclarationNodeList (DeclarationNodeList node, object arg)
		{
			var declsElement = MakeAndAddElement (arg, "declarations");
			base.VisitDeclarationNodeList (node, declsElement);
		}
		
		public override void VisitSimpleTypeNode (SimpleTypeNode node, object arg)
		{
			var simpleTypeNode = MakeAndAddElement (arg, "simpletype");
			base.VisitSimpleTypeNode (node, simpleTypeNode);
		}
		
		public override void VisitIdentifierTypeNode (IdentifierTypeNode node, object arg)
		{
			var identifierTypeNode = MakeAndAddElement (arg, "identifier");
			base.VisitIdentifierTypeNode (node, identifierTypeNode);
		}
				
		public override void VisitEnumTypeNode (EnumTypeNode node, object arg)
		{
			var enumTypeNode = MakeAndAddElement (arg, "enumtype");
			base.VisitEnumTypeNode (node, enumTypeNode);
		}
		
		
		
	}
}
