//
// openmapi.org - CompactTeaSharp - MLog - TerminalNode.cs
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
using GoldParser;

namespace CompactTeaSharp.Mlog
{
	/// <summary>
	///   Base class for terminal nodes.
	/// </summary>
	public class TerminalNode : ASTNode
	{
		private Symbol symbol;
		private string text;
		private int lineNumber;
		private int linePosition;

		public TerminalNode (XdrParserContext context)
		{
			symbol = context.Parser.TokenSymbol;
			text = context.Parser.TokenSymbol.ToString ();
			lineNumber = context.Parser.LineNumber;
			linePosition = context.Parser.LinePosition;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			// TODO! hack!
		}

		public override bool IsTerminal {
			get { return true; }
		}
		
		/// <summary>
		///  The terminal symbol.
		/// </summary>
		public Symbol Symbol {
			get { return symbol; }
		}

		/// <summary>
		///  The "token".
		/// </summary>
		public string Text {
			get { return text; }
		}

		/// <summary>
		///  The line number of the terminal node.
		/// </summary>
		public int LineNumber {
			get { return lineNumber; }
		}

		/// <summary>
		///  The line position of the terminal node.
		/// </summary>
		public int LinePosition {
			get { return linePosition; }
		}
	}

}
