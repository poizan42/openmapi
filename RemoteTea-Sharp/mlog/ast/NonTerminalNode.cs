//
// openmapi.org - CompactTeaSharp - MLog - NonTerminalNode.cs
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
using System.Collections.Generic;

using GoldParser;

namespace CompactTeaSharp.Mlog
{
	/// <summary>
	///   Base class for non-terminal nodes in the AST.
	/// </summary>
	public abstract class NonTerminalNode : ASTNode
	{
		private int reductionNumber;
		private Rule rule;

		protected NonTerminalNode (XdrParserContext context)
		{
			rule = context.Parser.ReductionRule;
		}

		public override bool IsTerminal {
			get { return false; }
		}

		/// <summary>
		///  
		/// </summary>
		public int ReductionNumber  {
			get { return reductionNumber; }
			set { reductionNumber = value; }
		}

		/// <summary>
		///  Reference of the rule used to construct the non-terminal node.
		/// </summary>
		public Rule Rule {
			get { return rule; }
		}

	}

}
