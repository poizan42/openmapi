//
// openmapi.org - CompactTeaSharp - MLog - Types.cs
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
	///   List of the types understood by xdr.
	/// <summary>
	public enum SimpleType
	{
		String,
		Float,
		Int,
		UnsignedInt,
		Double,
		Bool, 
		Hyper,
		UnsignedHyper,
		Long,
		Char,
		Short,
		Quadruple,
		BoolT,
		Void,
		Opaque
	}
	
	/// <summary>
	///   Represents a type in the AST.
	/// <summary>
	public abstract class AbstractTypeNode : TerminalNode
	{	
		protected AbstractTypeNode (XdrParserContext context) : base (context)
		{
		}
	}
	
	/// <summary>
	///
	/// <summary>
	public class SimpleTypeNode : AbstractTypeNode
	{
		private SimpleType type;
		
		public SimpleType Type {
			get { return type; }
		}
		
		public SimpleTypeNode (XdrParserContext context, SimpleType type) : base (context)
		{
			this.type = type;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitSimpleTypeNode (this, arg);
		}
	}
	
	/// <summary>
	///   
	/// <summary>
	public class IdentifierTypeNode : AbstractTypeNode
	{
		private string name;
		
		public string Name {
			get { return name; }
		}
		
		public IdentifierTypeNode (XdrParserContext context, string identifier) : base (context)
		{
			this.name = identifier;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitIdentifierTypeNode (this, arg);
		}
	}
	
	/// <summary>
	///   
	/// <summary>
	public class EnumTypeNode : AbstractTypeNode
	{	
		public EnumTypeNode (XdrParserContext context) : base (context)
		{
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitEnumTypeNode (this, arg);
		}
	}
}
