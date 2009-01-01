//
// openmapi.org - CompactTeaSharp - MLog - DefEnumNode.cs
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
	///   Represents an enum definition in the AST.
	/// <summary>
	public class DefEnumNode : AbstractDefNode
	{
		private IdentifierNode identifier;
		private object content;

		/// <summary>
		///  The name of the enum.
		/// </summary>
		public IdentifierNode Identifier {
			get { return identifier; }
		}
		
		/// <summary>
		///  List of "assignments". Enums may either contain assignments 
		//   or only a list of identifiers. (But not both.)
		/// <summary>
		public AssignmentNodeList Assignments {
			get { return content as AssignmentNodeList; }
		}
		
		/// <summary>
		///  List of identifiers. Enums may either contain assignments 
		//   or only a list of identifiers. (But not both.)
		/// <summary>
		public IdentifierNodeList Identifiers {
			get { return content as IdentifierNodeList; }
		}
		
		public DefEnumNode (XdrParserContext context, IdentifierNode identifier, 
			object content) : base (context)
		{
			this.identifier = identifier;
			this.content = content;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitDefEnumNode (this, arg);
		}
	}

}
