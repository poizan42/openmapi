//
// openmapi.org - CompactTeaSharp - MLog - Rules.cs
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
	///	 Enumeration of the rules used by the parser.
	/// <summary>
	enum Rules : int
	{
		Specification								=  0, // <specification> ::= <definitions> <program>
		DefinitionsRecursive						=  1, // <definitions> ::= <definition> <definitions>
		DefinitionsSimple							=  2, // <definitions> ::= <definition>
		DefinitionTypeDef							=  3, // <definition> ::= <type def>
		DefinitionConst								=  4, // <definition> ::= <constant def>
		TypeDef_TypeDef								=  5, // <type def> ::= typedef <declaration> ';'
		TypeDef_Enum_Identifier						=  6, // <type def> ::= enum identifier <enum body> ';'
		TypeDef_Struct_Identifier					=  7, // <type def> ::= struct identifier <struct body> ';'
		TypeDef_Union_Identifier					=  8, // <type def> ::= union identifier <union body> ';'
		ConstantDef									=  9, // <constant def> ::= const identifier '=' const ';'
		Case										= 10, // <case> ::= case <value> ':' <declaration> ';'
		CasesRecursive								= 11, // <cases> ::= <case> <cases>
		CasesSimple									= 12, // <cases> ::= <case>
		CaseDefault									= 13, // <case default> ::= default ':' <declaration> ';'
		CaseDefaultNone								= 14, // <case default> ::= 
		UnionBody									= 15, // <union body> ::= switch '(' <declaration> ')' '{' <cases> <case default> '}'
		UnionTypeSpec								= 16, // <union type spec> ::= union union body
		StructBody									= 17, // <struct body> ::= '{' <declarations> '}'
		StructTypeSpec								= 18, // <struct type spec> ::= struct <struct body>
		Assignment									= 19, // <assignment> ::= identifier '=' <value>
		AssignmentsRecursive						= 20, // <assignments> ::= <assignment> ',' <assignments>
		AssignmentsSimple							= 21, // <assignments> ::= <assignment>
		IdentifiersRecursive						= 22, // <identifiers> ::= identifier ',' <identifiers>
		IdentifiersSimple							= 23, // <identifiers> ::= identifier
		EnumBodyAssignments							= 24, // <enum body> ::= '{' <assignments> '}'
		EnumBodyIdentifiers							= 25, // <enum body> ::= '{' <identifiers> '}'
		EnumTypeSpec								= 26, // <enum type spec> ::= enum <enum body>
		OptionalUnsignedUnsigned					= 27, // <optional unsigned> ::= unsigned
		OptionalUnsignedNone					    = 28, // <optional unsigned> ::= 
		TypeSpecifier_Int                           = 29, // <type specifier> ::= <optional unsigned> int                                        
        TypeSpecifier_UInt                          = 30, // <type specifier> ::= <optional unsigned> 'u_int'
        TypeSpecifier_Hyper                         = 31, // <type specifier> ::= <optional unsigned> hyper                                      
        TypeSpecifier_Long                          = 32, // <type specifier> ::= long                            
        TypeSpecifier_ULong                         = 33, // <type specifier> ::= 'u_long'
        TypeSpecifier_Char                          = 34, // <type specifier> ::= char
        TypeSpecifier_Short                         = 35, // <type specifier> ::= short 
        TypeSpecifier_UShort                        = 36, // <type specifier> ::= 'u_short'       
        TypeSpecifier_Float                         = 37, // <type specifier> ::= float                                                          
        TypeSpecifier_Double                        = 38, // <type specifier> ::= double                                                         
        TypeSpecifier_Quadruple                     = 39, // <type specifier> ::= quadruple
        TypeSpecifier_Bool                          = 40, // <type specifier> ::= bool                                                           
        TypeSpecifier_BoolT                         = 41, // <type specifier> ::= 'bool_t'
        TypeSpecifier_EnumComplex                   = 42, // <type specifier> ::= <enum type spec>                                               
        TypeSpecifier_StructComplex                 = 43, // <type specifier> ::= <struct type spec>                                             
        TypeSpecifier_StructIdentifier              = 44, // <type specifier> ::= struct identifier                                              
        TypeSpecifier_UnionComplex                  = 45, // <type specifier> ::= <union type spec>                                              
        TypeSpecifier_Identifier                    = 46, // <type specifier> ::= identifier                                                     
        Value_Constant                              = 47, // <value> ::= constant                                                                
        Value_Identifier                            = 48, // <value> ::= identifier                                                              
        OptionalValue                               = 49, // <optional value> ::= <value>                                                        
        OptionalValueEmpty                          = 50, // <optional value> ::=                                                                
		DeclarationComplexType					    = 51, // <declaration> ::= <type specifier> identifier
		DeclarationComplexTypeFixedArray			= 52, // <declaration> ::= <type specifier> identifier '[' <value> ']'
		DeclarationComplexTypeCountedArray			= 53, // <declaration> ::= <type specifier> identifier '<' <optional value> '>'
		DeclarationFixedOpaqueArray					= 54, // <declaration> ::= opaque identifier '[' <value> ']'
		DeclarationCountedOpaqueArray				= 55, // <declaration> ::= opaque identifier '<' <optional value> '>'
		DeclarationCountedStringArray				= 56, // <declaration> ::= string identifier '<' <optional value> '>'
		DeclarationPointer							= 57, // <declaration> ::= <type specifier> '*' identifier
		DeclarationVoid								= 58, // <declaration> ::= void
		DeclarationsRecursive						= 59, // <declarations> ::= <declaration> ';' <declarations>
		DeclarationsSimple							= 60, // <declarations> ::= <declaration> ';'
		Program										= 61, // <program> ::= program identifier '{' <versions> '}' '=' constant ';'
		Call										= 62, // <call> ::= identifier identifier '(' identifier ')' '=' constant ';'
		CallsRecursive								= 63, // <calls> ::= <call> <calls>
		CallsSimple									= 64, // <calls> ::= <call>
		Version										= 65, // <version> ::= version identifier '{' <calls> '}' '=' constant ';'
		VersionsRecursive							= 66, // <versions> ::= <version> <versions>
		VersionsSimple								= 67  // <versions> ::= <version>
	};
	
}
