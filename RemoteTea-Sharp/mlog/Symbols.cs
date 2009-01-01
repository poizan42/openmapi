//
// openmapi.org - CompactTeaSharp - MLog - Symbols.cs
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
	///	 Enumeration of the symbols used.
	/// <summary>
	enum Symbols
	{
		EOF				=  0, // (EOF)
		Error			=  1, // (Error)
		WhiteSpace		=  2, // (Whitespace)
		CommentEnd		=  3, // (Comment End)
		CommentStart	=  4, // (Comment Start)
		LParan			=  5, // '('
		RParan			=  6, // ')'
		Times			=  7, // '*'
		Comma			=  8, // ','
		Colon			=  9, // ':'
		Semi			= 10, // ';'
		LBracket		= 11, // '['
		RBracket		= 12, // ']'
		LBrace			= 13, // '{'
		RBrace			= 14, // '}'
		Lt				= 15, // '<'
		Eq				= 16, // '='
		Gt				= 17, // '>'
		Body			= 18, // body
		Bool			= 19, // bool
        BoolT           = 20, // 'bool_t'
        Case             = 21, // case
        Char             = 22, // char
        Const            = 23, // const
        Constant         = 24, // constant
        Default          = 25, // default
        Double           = 26, // double
        Enum             = 27, // enum
        Float            = 28, // float
        Hyper            = 29, // hyper
        Identifier       = 30, // identifier
        Int              = 31, // int
        Long             = 32, // long
        Opaque           = 33, // opaque
        Program          = 34, // program
        Quadruple        = 35, // quadruple
        Short            = 36, // short
        String           = 37, // string
        Struct           = 38, // struct
        Switch           = 39, // switch
        TypeDef          = 40, // typedef
        UInt             = 41, // 'u_int'
        Ulong            = 42, // 'u_long'
        UShort           = 43, // 'u_short'
        Union            = 44, // union
        Unsigned         = 45, // unsigned
        Version          = 46, // version
        Void             = 47, // void
        Assignment       = 48, // <assignment>
        Assignments      = 49, // <assignments>
        Call             = 50, // <call>
        Calls            = 51, // <calls>
        Case2            = 52, // <case>
        CaseDefault      = 53, // <case default>
        Cases            = 54, // <cases>
        ConstantDef      = 55, // <constant def>
        Declaration      = 56, // <declaration>
        Declarations     = 57, // <declarations>
        Definition       = 58, // <definition>
        Definitions      = 59, // <definitions>
        EnumBody         = 60, // <enum body>
        EnumTypeSpec     = 61, // <enum type spec>
        Identifiers      = 62, // <identifiers>
        OptionalUnsigned = 63, // <optional unsigned>
        OptionalValue    = 64, // <optional value>
        Program2         = 65, // <program>
        Specification    = 66, // <specification>
        StructBody       = 67, // <struct body>
        StructTypeSpec   = 68, // <struct type spec>
        TypeDef2         = 69, // <type def>
        TypeSpecifier    = 70, // <type specifier>
        UnionBody        = 71, // <union body>
        UnionTypeSpec    = 72, // <union type spec>
        Value            = 73, // <value>
        Version2         = 74, // <version>
        Versions         = 75  // <versions>
	};

}
