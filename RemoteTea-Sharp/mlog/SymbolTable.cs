//
// openmapi.org - CompactTeaSharp - MLog - SymbolTable.cs
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
using System.Collections.Generic;

namespace CompactTeaSharp.Mlog
{

	/// <summary>
	///   A very simple symbol table.
	/// </summary>
	public sealed class SymbolTable
	{
		private Dictionary<string, ASTNode> symbolTable;

		public SymbolTable ()
		{
			this.symbolTable = new Dictionary<string, ASTNode> ();
		}

		/// <summary>
		///  Adds a symbol with the corresponding node to the table.
		/// </summary>
		public void RegisterSymbol (string name, ASTNode node)
		{
			symbolTable.Add (name, node);
		}

		/// <summary>
		///  Returns a node for the name or null.
		/// </summary>
		public ASTNode GetNode (string name)
		{
			if (symbolTable.ContainsKey (name))
				return symbolTable [name];
			return null;
		}
	
		public bool IsEnum (string name)
		{
			var node = GetNode (name);
			return (node != null && node is DefEnumNode);
		}

		public bool IsTypeDef (string name)
		{
			var node = GetNode (name);
			return (node != null && node is DefTypeDefNode);
		}

		public bool IsStruct (string name)
		{
			var node = GetNode (name);
			return (node != null && node is DefStructNode);
		}
		
		public bool IsUnion (string name)
		{
			var node = GetNode (name);
			return (node != null && node is DefUnionNode);
		}
		
	}
}
