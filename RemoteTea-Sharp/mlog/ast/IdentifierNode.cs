//
// openmapi.org - CompactTeaSharp - MLog - IdentifierNode.cs
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
	///   Represents an identifier in the AST.
	/// <summary>
	public class IdentifierNode : AbstractValueNode
	{
		private string identifier;
		
		/// <summary>
		///  The actual identifier string.
		/// </summary>
		public string NameStr {
			get { return identifier; }
		}
		
		public override int Resolve () {
			throw new NotImplementedException ("Not yet implemented!"); // TODO
		}
		
		public IdentifierNode (XdrParserContext context, string identifier) : base (context)
		{
			this.identifier = identifier;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitIdentifierNode (this, arg);
		}
	}

}
