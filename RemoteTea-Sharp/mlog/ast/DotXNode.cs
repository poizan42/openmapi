//
// openmapi.org - CompactTeaSharp - MLog - DotXNode.cs
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

namespace CompactTeaSharp.Mlog
{	
	
	/// <summary>
	///  Represents the root node in the syntax tree.
	/// </summary>
	public class DotXNode : NonTerminalNode
	{
		private DefinitionNodeList definitions;
		private ProgramNode program;
		
		/// <summary>
		///  A list definitions.
		/// </summary>
		public DefinitionNodeList Definitions {
			get { return definitions; }
		}
		
		/// <summary>
		///  The "program" block of the file.
		/// </summary>
		public ProgramNode Program {
			get { return program; }
		}
		
		/// <summary>
		///  True if the program block contains any definitions.
		/// </summary>
		public bool HasDefinitions {
			get {
				return (definitions != null) && (definitions.Count > 0);
			}
		}
		
		public DotXNode (XdrParserContext context, DefinitionNodeList defList, 
			ProgramNode program) : base (context)
		{
			this.definitions = defList;
			this.program = program;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitDotXNode (this, arg);
		}
		
    }
    
}
