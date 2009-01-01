//
// openmapi.org - CompactTeaSharp - MLog - ASTNode.cs
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
	///   Abstract base class for AST nodes.
	/// </summary>
	public abstract class ASTNode
	{
		/// <summary>
		///   True if the node is a terminal node.
		/// </summary>
		public abstract bool IsTerminal {
			get;
		}
		
		/// <summary>
		///  Used by the AbstractVisitor for double dispatch.
		/// </summary>	
		public abstract void Accept (AbstractVisitor visitor, object arg);
	}
	
}