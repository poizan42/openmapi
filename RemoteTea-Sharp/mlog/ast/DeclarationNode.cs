//
// openmapi.org - CompactTeaSharp - MLog - DeclarationNode.cs
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
	///  
	/// <summary>
	public enum MultiValueType
	{
		Scalar,
		Counted,
		FixedByte
	}
	
	/// <summary>
	///   Represents a declaration in the AST.
	/// <summary>
	public class DeclarationNode : NonTerminalNode
	{
		private AbstractTypeNode type;
		private bool isPointer = false;
		private IdentifierNode identifier;
		private MultiValueType multiValueType;
		private AbstractValueNode sizeOrMax;
		
		/// <summary>
		///  The member name.
		/// </summary>
		public IdentifierNode Identifier {
			get { return identifier; }
			set { identifier = value; } // TODO: Hack for typedefs
		}
		
		/// <summary>
		///  True if the declaration is a pointer.
		/// </summary>
		public bool IsPointer {
			get { return isPointer; }
		}
		
		/// <summary>
		///  The type of the declared member.
		/// </summary>
		public AbstractTypeNode Type {
			get { return type; }
		}

		/// <summary>
		///  The type of the declaration (scalar/counted/fixed byte)
		/// </summary>
		public MultiValueType MultiValueType {
			get { return multiValueType; }
		}
		
		/// <summary>
		///  True if an array is declared.
		/// </summary>
		public bool IsArray {
			get {
				return multiValueType != MultiValueType.Scalar
					&& (	!(type is SimpleTypeNode)
							|| ((SimpleTypeNode) type).Type != SimpleType.Opaque);
			}
		}
		
		/// <summary>
		///  True if a fixed byte array is declared.
		/// </summary>
		public bool IsFixedByteArray {
			get {
				return IsArray && multiValueType == MultiValueType.FixedByte;
			}
		}
		
		/// <summary>
		///  True if a counted byte array is declared.
		/// </summary>
		public bool IsCountedByteArray {
			get {
				return IsArray && multiValueType == MultiValueType.Counted;
			}
		}
		
		/// <summary>
		///  True if a fixed opaque type is declared.
		/// </summary>
		public bool IsOpaqueFixed {
			get {
				return !IsArray && multiValueType == MultiValueType.FixedByte;
			}
		}
		
		/// <summary>
		///  True if a counted opaque type is declared.
		/// </summary>
		public bool IsOpaqueCounted {
			get {
				return !IsArray && multiValueType == MultiValueType.Counted;
			}
		}
		
		/// <summary>
		///   For Fixed byte arrays: returns the length
		///   For counted arrays: returns the maximum length
		///   For fixed opaque values: returns the length
		///   For counted opaque values: returns the maximum length
		/// </summary>
		public AbstractValueNode SizeOrMax {
			get {
				return sizeOrMax;
			}
		}
		
		
		public DeclarationNode (XdrParserContext context, IdentifierNode identifier, 
			AbstractTypeNode type) : this (context, identifier, type, false)
		{
		}
		
		public DeclarationNode (XdrParserContext context, IdentifierNode identifier, 
			AbstractTypeNode type, bool isPointer) : this (context, identifier, type, 
			MultiValueType.Scalar, null, isPointer)
		{
		}
		
		public DeclarationNode (XdrParserContext context, IdentifierNode identifier, 
			AbstractTypeNode type, MultiValueType multiValueType, 
			AbstractValueNode sizeOrMax, bool isPointer) : base (context)
		{
			this.identifier = identifier;
			this.isPointer = isPointer;
			this.type = type;
			this.multiValueType = multiValueType;
			this.sizeOrMax = sizeOrMax;
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitDeclarationNode (this, arg);
		}
	}

}
