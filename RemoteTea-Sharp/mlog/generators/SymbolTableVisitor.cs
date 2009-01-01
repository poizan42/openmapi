//
// openmapi.org - CompactTeaSharp - MLog - SymbolTableVisitor.cs
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
	///   Build a simple symbol table.
	/// </summary>
	public sealed class SymbolTableVisitor : AbstractVisitor
	{
		private SymbolTable table = new SymbolTable ();

		public override string[] Targets {
			get {
				return new string [] {"INTERNAL"};
			}
		}
		
		public override string Name {
			get {
				return "Symbol Table Generator";
			}
		}

		public override string Description {
			get {
				return "Builds the symbol table.";
			}
		}
		
		public SymbolTable SymbolTable {
			get { return table; }
		}

		private string GetMappedIdentifier (IdentifierNode node)
		{
			return driver.TypeMap.Map (node.NameStr);
		}
		
		public override void Dump ()
		{
		}

		public override void VisitDefConstNode (DefConstNode node, object arg)
		{
			string typeName = GetMappedIdentifier (node.Identifier);
			table.RegisterSymbol (typeName, node);
			base.VisitDefConstNode (node, arg);
		}

		public override void VisitDefEnumNode (DefEnumNode node, object arg)
		{
			string typeName = GetMappedIdentifier (node.Identifier);
			table.RegisterSymbol (typeName, node);
			base.VisitDefEnumNode (node, arg);
		}

		public override void VisitDefStructNode (DefStructNode node, object arg)
		{
			string typeName = GetMappedIdentifier (node.Identifier);
			table.RegisterSymbol (typeName, node);
			base.VisitDefStructNode (node, arg);
		}

		public override void VisitDefTypeDefNode (DefTypeDefNode node, object arg)
		{
			string typeName = GetMappedIdentifier (node.Identifier);
			table.RegisterSymbol (typeName, node);
			base.VisitDefTypeDefNode (node, arg);
		}

		public override void VisitDefUnionNode (DefUnionNode node, object arg)
		{
//			string typeName = GetMappedIdentifier (node.Identifier);
//			table.RegisterSymbol (typeName, node);
			base.VisitDefUnionNode (node, arg);
		}
		
	}
}
