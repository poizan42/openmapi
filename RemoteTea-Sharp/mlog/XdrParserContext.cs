//
// openmapi.org - CompactTeaSharp - MLog - XdrParserContext.cs
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
	///	 Constructs the AST.
	/// </summary>
	public class XdrParserContext
	{
		private Parser parser;

		public XdrParserContext (Parser parser)
		{
			this.parser = parser;	
		}

		public Parser Parser {
			get { return parser; }
		}

		public string GetTokenText ()
		{
			switch ((Symbols) parser.TokenSymbol.Index)
			{
				// Token Kind: 1
				case Symbols.RParan:
				case Symbols.Times:
				case Symbols.Comma:
				case Symbols.Colon:
				case Symbols.Semi:
				case Symbols.LBracket:
				case Symbols.RBracket:
				case Symbols.LBrace:
				case Symbols.RBrace:
				case Symbols.Lt:
				case Symbols.Eq:
				case Symbols.Gt:
				case Symbols.Body:
				case Symbols.Bool:
				case Symbols.BoolT:
				case Symbols.Case:
		        case Symbols.Char:
				case Symbols.Const:
				case Symbols.Constant:
				case Symbols.Default:
				case Symbols.Double:
				case Symbols.Enum:
				case Symbols.Float:
				case Symbols.Hyper:
				case Symbols.Identifier:
				case Symbols.Int:
				case Symbols.Long:
				case Symbols.Opaque:
				case Symbols.Program:
				case Symbols.Quadruple:
				case Symbols.Short:
				case Symbols.String:
				case Symbols.Struct:
				case Symbols.Switch:
				case Symbols.TypeDef:
				case Symbols.UInt:
		        case Symbols.Ulong:
		        case Symbols.UShort:
				case Symbols.Union:
				case Symbols.Unsigned:
				case Symbols.Version:
				case Symbols.Void:
					return parser.TokenString;
			}
			return null;			
		}

		private int ParseIntOrError (string str)
		{
			int result;
			bool worked = Int32.TryParse (str, out result);
			if (!worked)
				throw new Exception ("ERROR line " + parser.LineNumber + 
					":'" + str + "' is not a valid integer."); // TODO
			return result;
		}

		private ASTNode GetNode (int index)
		{
			return (ASTNode) parser.GetReductionSyntaxNode (index);
		}
		
		private string GetToken (int index)
		{
			return (string) parser.GetReductionSyntaxNode (index);
		}
		
		private int GetConstant (int index)
		{
			return Convert.ToInt32 ((string) parser.GetReductionSyntaxNode (index));
		}
			
		public ASTNode CreateASTNode ()
		{
			switch ((Rules) parser.ReductionRule.Index) {
				case Rules.Specification: return new DotXNode (this, (DefinitionNodeList) GetNode (0), (ProgramNode) GetNode (1));
				case Rules.DefinitionsRecursive: return BuildDefinitionsNodeRecursive ();
				case Rules.DefinitionsSimple: return new DefinitionNodeList (this, (AbstractDefNode) GetNode (0));
				case Rules.DefinitionTypeDef: return (AbstractDefNode) GetNode (0);
				case Rules.DefinitionConst: return (AbstractDefNode) GetNode (0);
				case Rules.TypeDef_TypeDef: return new DefTypeDefNode (this, (DeclarationNode) GetNode (1));
				case Rules.TypeDef_Enum_Identifier: return new DefEnumNode (this, new IdentifierNode (this, GetToken (1)), GetNode (2));
				case Rules.TypeDef_Struct_Identifier: return new DefStructNode (this, new IdentifierNode (this, GetToken (1)), (DeclarationNodeList) GetNode (2));
				case Rules.TypeDef_Union_Identifier: return null; // <type def> ::= union identifier <union body> ';'  TODO!
				case Rules.ConstantDef: return new DefConstNode (this, new IdentifierNode (this, GetToken (1)), ParseIntOrError (GetToken (3)));

				// TODO: unions are not supported, yet.
				case Rules.Case: return null; //<case> ::= case <value> ':' <declaration> ';'   TODO!
				case Rules.CasesRecursive: return null; //<cases> ::= <case> <cases>  TODO!
				case Rules.CasesSimple: return null; //<cases> ::= <case>  TODO!
				case Rules.CaseDefault: return null; //<case default> ::= default ':' <declaration> ';'  TODO!
				case Rules.CaseDefaultNone: return null; //<case default> ::=   TODO!
				case Rules.UnionBody: return null; //<union body> ::= switch '(' <declaration> ')' '{' <cases> <case default> '}'  TODO!
				case Rules.UnionTypeSpec: return null; //<union type spec> ::= union union body  TODO! // -> pass on DefUniontNode

				case Rules.StructBody: return GetNode (1);
				case Rules.StructTypeSpec: return GetNode (1);
				case Rules.Assignment: return new AssignmentNode (this, new IdentifierNode (this, GetToken (0)), (AbstractValueNode) GetNode (2));
				case Rules.AssignmentsRecursive: return ((AssignmentNodeList) GetNode (2)).Prepend ((AssignmentNode) GetNode (0));
				case Rules.AssignmentsSimple: return new AssignmentNodeList (this, (AssignmentNode) GetNode (0));
				case Rules.IdentifiersRecursive: return ((IdentifierNodeList) GetNode (2)).Prepend (new IdentifierNode (this, GetToken (0)));
				case Rules.IdentifiersSimple: return new IdentifierNodeList (this, new IdentifierNode (this, GetToken (0)));
				case Rules.EnumBodyAssignments: return (AssignmentNodeList) GetNode (1);
				case Rules.EnumBodyIdentifiers: return (IdentifierNodeList) GetNode (1);

				case Rules.EnumTypeSpec:
				//<enum type spec> ::= enum <enum body>
					var node = GetNode (1);
					if (!(node is IdentifierNodeList) &&
							!(node is AssignmentNodeList))
					{
						throw new RuleException ("Enums may only contain assignments or identifiers!");
					}
				//	return new DefEnumNode (this, node);		TODO: This is for nested enums only!			
					//todo: Perhaps create an object in the AST.		// -> pass on DefEnumNode
					return null;

				case Rules.OptionalUnsignedUnsigned: return new TerminalNode (this);	// TODO: this seems to be a little hackish ...
				case Rules.OptionalUnsignedNone: return null;

				case Rules.TypeSpecifier_Int:
					if ((TerminalNode) GetNode (0) != null)
						return new SimpleTypeNode (this, SimpleType.UnsignedInt);
					return new SimpleTypeNode (this, SimpleType.Int);

				case Rules.TypeSpecifier_UInt: return new SimpleTypeNode (this, SimpleType.Int);
				case Rules.TypeSpecifier_Hyper:
					if ((TerminalNode) GetNode (0) != null)
						return new SimpleTypeNode (this, SimpleType.UnsignedHyper);
					return new SimpleTypeNode (this, SimpleType.Hyper);

				case Rules.TypeSpecifier_Long:
				case Rules.TypeSpecifier_ULong: return new SimpleTypeNode (this, SimpleType.Long);
				case Rules.TypeSpecifier_Char: return new SimpleTypeNode (this, SimpleType.Char);
				case Rules.TypeSpecifier_UShort:
				case Rules.TypeSpecifier_Short: return new SimpleTypeNode (this, SimpleType.Short);
				case Rules.TypeSpecifier_Float: return new SimpleTypeNode (this, SimpleType.Float);
				case Rules.TypeSpecifier_Double: return new SimpleTypeNode (this, SimpleType.Double);
				case Rules.TypeSpecifier_Quadruple: return new SimpleTypeNode (this, SimpleType.Quadruple);
				case Rules.TypeSpecifier_Bool:	
        		case Rules.TypeSpecifier_BoolT: return new SimpleTypeNode (this, SimpleType.Bool);
				case Rules.TypeSpecifier_EnumComplex: return null; //<type specifier> ::= <enum type spec>  TODO
				case Rules.TypeSpecifier_StructComplex: return new DefStructNode (this, new IdentifierNode (this, "TODO_FIX_ME!!!!"), (DeclarationNodeList) GetNode (1));
				case Rules.TypeSpecifier_StructIdentifier: return new IdentifierTypeNode (this, GetToken (1)); // HERE we basically "swallow" the 'struct' keyword
				case Rules.TypeSpecifier_UnionComplex: return null; //<type specifier> ::= <union type spec>  TODO
				case Rules.TypeSpecifier_Identifier: return new IdentifierTypeNode (this, GetToken (0));
				case Rules.Value_Constant: return new ConstantNode (this, ParseIntOrError (GetToken (0)));
				case Rules.Value_Identifier: return new IdentifierNode (this, (string) GetToken (0));
				case Rules.OptionalValue: return GetNode (0);
				case Rules.OptionalValueEmpty: return null;
				case Rules.DeclarationComplexType: return new DeclarationNode (this, new IdentifierNode (this, GetToken (1)), (AbstractTypeNode) GetNode (0));
				case Rules.DeclarationComplexTypeFixedArray: return new DeclarationNode (this, new IdentifierNode (this, GetToken (1)), (AbstractTypeNode) GetNode (0), MultiValueType.FixedByte, (AbstractValueNode) GetNode (3), false);
				case Rules.DeclarationComplexTypeCountedArray:return new DeclarationNode (this, new IdentifierNode (this, GetToken (1)), (AbstractTypeNode) GetNode (0), MultiValueType.Counted, (AbstractValueNode) GetNode (3), false);
				case Rules.DeclarationFixedOpaqueArray: return new DeclarationNode (this, new IdentifierNode (this, GetToken (1)), new SimpleTypeNode (this, SimpleType.Opaque), MultiValueType.FixedByte, (AbstractValueNode) GetNode (3), false);
				case Rules.DeclarationCountedOpaqueArray:
				case Rules.DeclarationCountedStringArray: return BuildStringOrOpaqueArrayDeclaration ();				
				case Rules.DeclarationPointer: return new DeclarationNode (this, new IdentifierNode (this, GetToken (2)), (AbstractTypeNode) GetNode (0), true);				
				case Rules.DeclarationVoid: return new DeclarationNode (this, null, new SimpleTypeNode (this, SimpleType.Void));
				case Rules.DeclarationsRecursive: return ((DeclarationNodeList) GetNode (2)).Prepend ((DeclarationNode) GetNode (0));
				case Rules.DeclarationsSimple: return new DeclarationNodeList (this, (DeclarationNode) GetNode (0));
				case Rules.Program: return new ProgramNode (this, (VersionNodeList) GetNode (3), new IdentifierNode (this, GetToken (1)), GetConstant (6));
				case Rules.Call: return new CallNode (this, new IdentifierNode (this, GetToken (0)), 
					new IdentifierNode (this, GetToken (1)), new IdentifierNode (this, GetToken (3)), 
					ParseIntOrError ((string) GetToken (6)));
				
				case Rules.CallsRecursive: return ((CallNodeList) GetNode (1)).Prepend ((CallNode) GetNode (0));
				case Rules.CallsSimple: return new CallNodeList (this, (CallNode) GetNode (0));
				case Rules.Version: return new VersionNode (this, new IdentifierNode (this, GetToken (1)), GetConstant (6), (CallNodeList) GetNode (3));
				case Rules.VersionsRecursive: return ((VersionNodeList) GetNode (1)).Prepend ((VersionNode) GetNode (0));
				case Rules.VersionsSimple: return new VersionNodeList (this, (VersionNode) GetNode (0));
				default: throw new RuleException ("Unknown rule: Does your CGT match your code revision?");
			}
			
		}

		private ASTNode BuildDefinitionsNodeRecursive ()
		{
			var restList = ((DefinitionNodeList) GetNode (1));
			var node = (AbstractDefNode) GetNode (0);
			if (node != null) // TODO: This hides the fact that unions don't work, yet!
				restList.Prepend (node);
			return restList;
		}
		
		private ASTNode BuildStringOrOpaqueArrayDeclaration ()
		{
			var type = SimpleType.Opaque;
			if (GetToken (0) == "string")
				type = SimpleType.String;
			var typeNode = new SimpleTypeNode (this, type);
			return new DeclarationNode (this, new IdentifierNode (this, GetToken (1)), 
				typeNode, MultiValueType.Counted, (AbstractValueNode) GetNode (3), false);
		}
		
	}
	
}