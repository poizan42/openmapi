//
// openmapi.org - CompactTeaSharp - MLog - Exceptions.cs
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
using System.Runtime.Serialization;

namespace CompactTeaSharp.Mlog
{

	/// <summary>
	///   Base class for Exceptions related to the Parser.
	/// </summary
	[Serializable]
	public class ParserException : Exception
	{
		public ParserException (string message) : base (message)
		{
		}

		public ParserException (string message, Exception inner)
		 	: base (message, inner)
		{
		}

		protected ParserException (SerializationInfo info, StreamingContext context) 
			: base (info, context)
		{
		}

	}
	
	/// <summary>
	///   Exception related to rules.
	/// </summary
	[Serializable]
	public class RuleException : ParserException
	{
		public RuleException (string message) : base (message)
		{
		}

		public RuleException (string message, Exception inner)
		 	: base (message, inner)
		{
		}

		protected RuleException (SerializationInfo info, StreamingContext context) 
			: base (info, context)
		{
		}

	}
	
	/// <summary>
	///   Exception related to symbols.
	/// </summary
	[Serializable]
	public class SymbolException : ParserException
	{
		public SymbolException (string message) : base (message)
		{
		}

		public SymbolException (string message, Exception inner) 
			: base (message, inner)
		{
		}

		protected SymbolException (SerializationInfo info,
			StreamingContext context) : base (info, context)
		{
		}

	}
	
}
