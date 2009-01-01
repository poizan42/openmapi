//
// openmapi.org - CompactTeaSharp - MLog - DefTypeDefNode.cs
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
	///   Represents a definition of a typedef in the AST.
	/// <summary>
	public class DefTypeDefNode : AbstractDefNode
	{
		private IdentifierNode identifier;
		private DeclarationNode targetDeclaration;

		/// <summary>
		///  Name of the typedef.
		/// </summary>
		public IdentifierNode Identifier {
			get { return identifier; }
		}
		
		/// <summary>
		///  The declaration of the "target" type.
		/// </summary>
		public DeclarationNode TargetDeclaration {
			get { return targetDeclaration; }
		}
		
		public DefTypeDefNode (XdrParserContext context, 
			DeclarationNode targetDeclaration) : base (context)
		{
			this.targetDeclaration = targetDeclaration;
			this.identifier = targetDeclaration.Identifier; // swap 
			this.targetDeclaration.Identifier = new IdentifierNode (context, "value"); // swap
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitDefTypeDefNode (this, arg);
		}
	}

}
