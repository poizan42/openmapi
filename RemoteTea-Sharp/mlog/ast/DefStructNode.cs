//
// openmapi.org - CompactTeaSharp - MLog - DefStructNode.cs
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
	///   Represents a struct definition in the AST.
	/// <summary>
	public class DefStructNode : AbstractDefNode
	{
		private IdentifierNode identifier;
		private DeclarationNodeList declarations;

		/// <summary>
		///  The name of the struct.
		/// </summary>
		public IdentifierNode Identifier {
			get { return identifier; }
		}
		
		/// <summary>
		///  Declarations of the struct members.
		/// <summary>
		public DeclarationNodeList Declarations {
			get { return declarations; }
		}
		
		public DefStructNode (XdrParserContext context, IdentifierNode identifier, 
			DeclarationNodeList declarations) : base (context)
		{
			this.identifier = identifier;
			this.declarations = declarations;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitDefStructNode (this, arg);
		}
	}

}
