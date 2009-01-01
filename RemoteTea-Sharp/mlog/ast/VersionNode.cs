//
// openmapi.org - CompactTeaSharp - MLog - VersionNode.cs
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
	///  Represents a "version" block in the AST.
	/// </summary>	
	public class VersionNode : NonTerminalNode
	{
		private int versionNumber;
		private IdentifierNode identifier;
		private CallNodeList calls;
		
		/// <summary>
		///  A list of RPC calls.
		/// </summary>
		public CallNodeList Calls {
			get  { return calls; }
		}
		
		/// <summary>
		///  The version number.
		/// </summary>
		public int VersionNumber {
			get  { return versionNumber; }
		}
		
		/// <summary>
		///  An identifier of the version.
		/// </summary>
		public IdentifierNode Identifier {
			get  { return identifier; }
		}
		
		public VersionNode (XdrParserContext context, IdentifierNode identifier, 
			int versionNumber, CallNodeList calls) : base (context)
		{
			this.identifier = identifier;
			this.versionNumber = versionNumber;
			this.calls = calls;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitVersionNode (this, arg);
		}
	}

}
