//
// openmapi.org - CompactTeaSharp - MLog - DefConstNode.cs
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
	///   Represents a definition of a constant in the AST.
	/// <summary>
	public class DefConstNode : AbstractDefNode
	{
		private IdentifierNode identifier;
		private int constant;

		/// <summary>
		/// The name/alias of the constant. 
		/// </summary>
		public IdentifierNode Identifier {
			get { return identifier; }
		}

		/// <summary>
		///  The value.
		/// </summary>		
		public int Constant {
			get { return constant; }
		}
		
		public DefConstNode (XdrParserContext context, 
			IdentifierNode identifier, int constant) : base (context)
		{
				this.identifier = identifier;
				this.constant = constant;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitDefConstNode (this, arg);
		}
	}

}
