//
// openmapi.org - CompactTeaSharp - MLog - AbstractVisitor.cs
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
	///   Visitors must be derived from this class.
	/// <summary>
	public abstract class AbstractVisitor
	{
		protected Driver driver = null;
		
		/// <summary>
		///  Targets (languages) supported by the visitor.
		/// <summary>
		public abstract string[] Targets { get; }

		/// <summary>
		///  Name of the visitor.
		/// <summary>
		public abstract string Name { get; }

		/// <summary>
		///   Description of the visitor.
		/// <summary>
		public abstract string Description { get; }

		/// <summary>
		/// 
		/// <summary>
		public abstract void Dump ();
		
		public void SetDriver (Driver driver)
		{
			this.driver = driver;
		}
		
		private void VisitNodeList<T> (AbstractNodeList<T> list, object arg)
			where T : ASTNode
		{
			if (list != null)
				foreach (var node in list)
					node.Accept (this, arg);
		}
		
		public virtual void VisitDotXNode (DotXNode node, object arg)
		{
			if (node.HasDefinitions)
				node.Definitions.Accept (this, arg);
			if (node.Program == null)
				throw new Exception ("Program node must not be NULL.");
			node.Program.Accept (this, arg);
		}
		
		public virtual void VisitProgramNode (ProgramNode node, object arg)
		{
			if (node.Versions == null)
				throw new Exception ("There are no Versions defined in the program block!");
			node.Versions.Accept (this, arg);
		}
		
		public virtual void VisitVersionNode (VersionNode node, object arg)
		{
			if (node.Calls != null)
				node.Calls.Accept (this, arg);
		}

		public virtual void VisitCallNode (CallNode node, object arg)
		{
		}
			
		public virtual void VisitVersionNodeList (VersionNodeList list, object arg)
		{
			VisitNodeList<VersionNode> (list, arg);
		}
		
		public virtual void VisitDefinitionNodeList (DefinitionNodeList list, object arg)
		{
			VisitNodeList<AbstractDefNode> (list, arg);
		}
		public virtual void VisitCallNodeList (CallNodeList list, object arg)
		{
			VisitNodeList<CallNode> (list, arg);
		}
		
		public virtual void VisitDefConstNode (DefConstNode node, object arg)
		{
			// TODO
		}
		
		public virtual void VisitDefEnumNode (DefEnumNode node, object arg)
		{
			if (node.Assignments != null)
				VisitNodeList<AssignmentNode> (node.Assignments, arg);
			if (node.Identifiers != null)
				VisitNodeList<IdentifierNode> (node.Identifiers, arg);
		}
		
		public virtual void VisitAssignmentNode (AssignmentNode node, object arg)
		{
			// TODO
		}
		
		public virtual void VisitIdentifierNode (IdentifierNode node, object arg)
		{
			// TODO
		}
		
		public virtual void VisitDefStructNode (DefStructNode node, object arg)
		{
			if (node.Declarations != null)
				VisitNodeList<DeclarationNode> (node.Declarations, arg);
		}
		
		public virtual void VisitDefTypeDefNode (DefTypeDefNode node, object arg)
		{
			node.TargetDeclaration.Accept (this, arg);
		}
		
		public virtual void VisitDefUnionNode (DefUnionNode node, object arg)
		{
			// TODO
		}

		public virtual void VisitConstantNode (ConstantNode node, object arg)
		{
		}
		
		public virtual void VisitDeclarationNode (DeclarationNode node, object arg)
		{
			// TODO
		}
		
		public virtual void VisitIdentifierNodeList (IdentifierNodeList list, object arg)
		{
			VisitNodeList<IdentifierNode> (list, arg);
		}
		
		public virtual void VisitAssignmentNodeList (AssignmentNodeList list, object arg)
		{
			VisitNodeList<AssignmentNode> (list, arg);
		}
		
		public virtual void VisitDeclarationNodeList (DeclarationNodeList list, object arg)
		{
			VisitNodeList<DeclarationNode> (list, arg);
		}
		
		public virtual void VisitSimpleTypeNode (SimpleTypeNode node, object arg)
		{
			// TODO
		}
		
		public virtual void VisitIdentifierTypeNode (IdentifierTypeNode node, object arg)
		{
			// TODO
		}
				
		public virtual void VisitEnumTypeNode (EnumTypeNode node, object arg)
		{
			// TODO
		}
		
	}
	
}
