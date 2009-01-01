//
// openmapi.org - CompactTeaSharp - MLog - Lists.cs
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
using System.Collections;
using System.Collections.Generic;

namespace CompactTeaSharp.Mlog
{	
	
	/// <summary>
	///  Represents a list of Identifier nodes in the AST.
	/// </summary>
	public class IdentifierNodeList : AbstractNodeList<IdentifierNode>
	{
		public IdentifierNodeList (XdrParserContext context, IdentifierNode node) 
			: base (context, node)
		{
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitIdentifierNodeList (this, arg);
		}
	}
	
	/// <summary>
	///  Represents a list of VersionNodes in the AST.
	/// </summary>
	public class VersionNodeList : AbstractNodeList<VersionNode>
	{
		public VersionNodeList (XdrParserContext context, VersionNode node) 
			: base (context, node)
		{
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitVersionNodeList (this, arg);
		}
	}
	
	/// <summary>
	///  Represents a list of assignments nodes in the AST.
	/// </summary>
	public class AssignmentNodeList : AbstractNodeList<AssignmentNode>
	{
		public AssignmentNodeList (XdrParserContext context, AssignmentNode node) 
			: base (context, node)
		{
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitAssignmentNodeList (this, arg);
		}
	}
		
	/// <summary>
	///  Represents a list of definition nodes in the AST.
	/// </summary>
	public class DefinitionNodeList : AbstractNodeList<AbstractDefNode>
	{
		public DefinitionNodeList (XdrParserContext context, AbstractDefNode node) 
			: base (context, node)
		{
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitDefinitionNodeList (this, arg);
		}
	}
	
	/// <summary>
	///  Represents a list of call nodes in the AST.
	/// </summary>
	public class CallNodeList : AbstractNodeList<CallNode>
	{
		public CallNodeList (XdrParserContext context, CallNode call) 
			: base (context, call)
		{
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitCallNodeList (this, arg);
		}
	}
	
	/// <summary>
	///  Represents a list of declaration nodes in the AST.
	/// </summary>
	public class DeclarationNodeList : AbstractNodeList<DeclarationNode>
	{
		public DeclarationNodeList (XdrParserContext context, DeclarationNode node) 
			: base (context, node)
		{
		}
		
		public override void Accept (AbstractVisitor visitor, object arg)
		{
			visitor.VisitDeclarationNodeList (this, arg);
		}
	}

    
}
