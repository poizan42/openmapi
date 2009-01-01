//
// openmapi.org - CompactTeaSharp - MLog - AssignmentNode.cs
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
	///  Represents an "assignment" in the AST.
	///  Actually this is an enum member with a specified value.
	/// </summary>	
	public class AssignmentNode : NonTerminalNode
	{
		private IdentifierNode identifier;
		private AbstractValueNode rvalue;
		
		/// <summary>
		///  The name of the enum member.
		/// </summary>
		public IdentifierNode Identifier {
			get  { return identifier; }
		}
		
		/// <summary>
		///  The value of the member.
		/// </summary>
		public AbstractValueNode RValue {
			get  { return rvalue; }
		}
		
		public AssignmentNode (XdrParserContext context, IdentifierNode identifier, 
			AbstractValueNode rvalue) : base (context)
		{
			this.identifier = identifier;
			this.rvalue = rvalue;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitAssignmentNode (this, arg);
		}
	}

}
