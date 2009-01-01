//
// openmapi.org - CompactTeaSharp - MLog - VersionNode.cs
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
	///  Represents the "program" block in the AST.
	/// </summary>
	public class ProgramNode : NonTerminalNode
	{
		private int programNumber;
		private IdentifierNode identifier;
		private VersionNodeList versions;
		
		/// <summary>
		///  A list of "version" block contained in the "program" block.
		/// </summary>
		public VersionNodeList Versions {
			get  { return versions; }
		}
		
		/// <summary>
		///  The version number of the program.
		/// </summary>
		public int ProgramNumber {
			get  { return programNumber; }
		}
		
		/// <summary>
		///  An identifier of the program.
		/// </summary>
		public IdentifierNode Identifier {
			get  { return identifier; }
		}
		
		public ProgramNode (XdrParserContext context, VersionNodeList versions, 
				IdentifierNode identifier, int programNumber) : base (context)
		{
			this.versions = versions;
			this.identifier = identifier;
			this.programNumber = programNumber;
		}

		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitProgramNode (this, arg);
		}
	}

}
