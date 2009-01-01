//
// openmapi.org - CompactTeaSharp - MLog - ParserFactory.cs
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
using System.Reflection;
using System.IO;

using GoldParser;

namespace CompactTeaSharp.Mlog
{
	/// <summary>
	///   This factory class can create a new Parser while 
	///   keeping the processed CGT table in memory.
	///
	///   MUST be initialized with CreateParser () before first use!
	/// </summary>
	public sealed class ParserFactory
	{
		static Grammar grammar;
		static bool init;
		
		private ParserFactory ()
		{
		}
		
		private static BinaryReader GetResourceReader (string resourceName)
		{  
			Assembly assembly = Assembly.GetExecutingAssembly ();
			Stream stream = assembly.GetManifestResourceStream (resourceName);
			return new BinaryReader (stream);
		}
		
		public static void InitializeFactoryFromFile (string FullCGTFilePath)
		{
			if (!init) {
			   var reader = new BinaryReader (new FileStream (FullCGTFilePath, FileMode.Open));
			   grammar = new Grammar (reader);
			   init = true;
			}
		}
		
		public static void InitializeFactoryFromResource (string resourceName)
		{
			if (!init) {
				BinaryReader reader = GetResourceReader (resourceName);
				grammar = new Grammar (reader);
				init = true;
			}
		}
		
		public static Parser CreateParser (TextReader reader)
		{
			if (init)
				return new Parser (reader, grammar);
			throw new Exception ("You must first Initialize the " + 
				"Factory before creating a parser!");
		}
	}

}
