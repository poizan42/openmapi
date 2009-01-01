//
// openmapi.org - CompactTeaSharp - MLog - CallNode.cs
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
	///   Represents an RPC call in the AST.
	/// <summary>
	public class CallNode : TerminalNode
	{
		private IdentifierNode ret;
		private IdentifierNode callName;
		private IdentifierNode arg;
		private int id;
		
		/// <summary>
		///  The return type.
		/// <summary>
		public IdentifierNode Ret {
			get { return ret; }
		}
		
		/// <summary>
		///  The name of the call.
		/// <summary>
		public IdentifierNode CallName {
			get { return callName; }
		}
		
		/// <summary>
		///  The identifier of the structure passed as argument.
		/// <summary>
		public IdentifierNode Arg {
			get { return arg; }
		}
		
		/// <summary>
		///  The ID of the RPC call.
		/// <summary>
		public int Id {
			get { return id; }
		}
		
		public CallNode (XdrParserContext context, IdentifierNode ret, IdentifierNode callName, 
			IdentifierNode arg, int id) : base (context)
		{
				this.ret = ret;
				this.callName = callName;
				this.arg = arg;
				this.id = id;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitCallNode (this, arg);
		}
		
	}

}
