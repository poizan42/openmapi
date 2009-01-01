//
// openmapi.org - CompactTeaSharp - MLog - ConstantNode.cs
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
	///   Represents a constant in the AST.
	/// <summary>
	public class ConstantNode : AbstractValueNode
	{
		private int value;

		/// <summary>
		///  The constant integer value.
		/// </summary>
		public int Value {
			get { return value; }
		}

		public override int Resolve () {
			return value;
		}
		
		public ConstantNode (XdrParserContext context, int value) : base (context)
		{
			this.value = value;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitConstantNode (this, arg);
		}
	}

}
