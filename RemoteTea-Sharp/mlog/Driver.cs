//
// openmapi.org - CompactTeaSharp - MLog - Driver.cs
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
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;

using NDesk.Options;
using GoldParser;

namespace CompactTeaSharp.Mlog
{

	/// <summary>
	///	 MLog (Multi-Language-Onc-Generator) is a replacement for the
	///  "nrpcgen" rpc  stub generator, which was a modification of the 
	///  remotea-tea java rpc stub generator "jrpcgen". Not only did this 
	///  create a build dependency on Java, but also the code was hard to 
	///  maintain. MLog is designed with support for multiple backends in mind, 
	///  so we can generate rpc stubs for multiple languages with one tool.
	/// <summary>
	public class Driver
	{
		private XdrParserContext context;
		private ASTNode ast;
		private string errorString;
		private string xFile;
		private string outFile;
		private string typeMapFile;
		private Parser parser;
		private XmlTypeMap typeMap;
		private SymbolTable symbolTable;

		private AbstractVisitor generator;
		
		public ASTNode SyntaxTree {
			get { return ast; }
		}
		
		public int LineNumber {
			get { return parser.LineNumber; }
		}

		public int LinePosition {
			get { return parser.LinePosition; }
		}

		public string ErrorString {
			get { return errorString; }
		}

		public string XFile {
			get { return xFile; }
		}
		
		public string TypeMapFile {
			get { return typeMapFile; }
		}
		
		public string OutFile {
			get { return outFile; }
		}
		
		public XmlTypeMap TypeMap {
			get { return typeMap; }
		}
		
		public SymbolTable SymbolTable {
			get { return symbolTable; }
		}
		
		public static void Main (string[] args)
		{
			var driver = new Driver ();
			driver.Run (args);
		}
		
		public void Run (string[] args)
		{
			generator = new DataXmlVisitor (args);
			
			bool parseOnly = false;
			
			OptionSet p = new OptionSet ()
				.Add ("visitor=|vi=", l => {
					switch (l) {
						case "dataxml": generator = new DataXmlVisitor (args); break;
						case "astxml": generator = new AstXmlVisitor (); break;
					}
				})
				.Add ("parseonly", x => { parseOnly = true; })				
				.Add ("x=|xfile=", x => { xFile = x; })
				.Add ("typemap=|tmap=", tm => { typeMapFile = tm; })
				.Add ("out=|o=", o => { outFile = o; })
				.Add ("version|help|h|?", l => {
					PrintHelp ();
					return;
				});
				
			List<string> rest = p.Parse (args);

			ParserFactory.InitializeFactoryFromResource ("xdr.cgt");
			
			if (String.IsNullOrEmpty (xFile))
				throw new Exception (".x file must be specified!");		
			TextReader reader = File.OpenText (xFile);
			
			typeMap = new XmlTypeMap (typeMapFile);

			bool worked = Parse (reader);
			if (parseOnly)
				return;
			if (worked) {
				DotXNode root = SyntaxTree as DotXNode;
				var symbolTableVisitor = new SymbolTableVisitor ();
				symbolTableVisitor.SetDriver (this);
				symbolTableVisitor.VisitDotXNode (root, null);
				symbolTable = symbolTableVisitor.SymbolTable;
				
				generator.SetDriver (this);
				generator.VisitDotXNode (root, null);
				generator.Dump ();
			} else
				Console.WriteLine (ErrorString);
		}
		
		public static void PrintVersion ()
		{
			Version version = Assembly.GetExecutingAssembly ().GetName ().Version;
			Console.WriteLine ("openmapi.org MLog " + version);
			Console.WriteLine ("(C) 2008 by Topalis AG\n");
		}

		public static void PrintHelp ()
		{
			PrintVersion ();
			Console.WriteLine ();			
			Console.WriteLine ("Usage: mlog [OPTIONS]");
			Console.WriteLine ();
			Console.WriteLine ("  -visitor <name>    the visitor that should be used.");
			Console.WriteLine ("  -x <file>          the input .x file");
			Console.WriteLine ("  -out <file>        the generated file");
			Console.WriteLine ("  -typemap <file>    specify an xml type to map types to different names");
			Console.WriteLine ("  -ns <namespace>    specify namespace for generated source code files");
			Console.WriteLine ("  -constName <name>  specify name prefix of client/server stubs");
			Console.WriteLine ("  -parseonly         parse x-file only without generating anything");			
			Console.WriteLine ("  -? -help -version  print this help message and exit");
			Console.WriteLine ();
		}

		public bool Parse (TextReader sourceReader)
		{
			parser = ParserFactory.CreateParser (sourceReader);
			parser.TrimReductions = false;
			context = new XdrParserContext (parser);
			
			while (true) {
				switch (parser.Parse ()) {
					case ParseMessage.LexicalError:
						errorString = string.Format ("Lexical Error. Line {0}. " + 
							"Token {1} was not expected.", parser.LineNumber, parser.TokenText);
						return false;

					case ParseMessage.SyntaxError:
						StringBuilder text = new StringBuilder ();
						foreach (Symbol tokenSymbol in parser.GetExpectedTokens ()) {
							text.Append (' ');
							text.Append (tokenSymbol.ToString ());
						}
						errorString = string.Format ("Syntax Error. Line {0}. " + 
							"Expecting: {1}.", parser.LineNumber, text.ToString());	
						return false;
					
					case ParseMessage.Reduction:
						parser.TokenSyntaxNode = context.CreateASTNode ();
						break;

					case ParseMessage.Accept:
						ast = parser.TokenSyntaxNode as ASTNode;
						errorString = null;
						return true;
					
					case ParseMessage.TokenRead:
						parser.TokenSyntaxNode = context.GetTokenText ();
					break;
					
					case ParseMessage.InternalError:
						errorString = "Internal Error. Something is horribly wrong.";
						return false;
					
					case ParseMessage.NotLoadedError:
						errorString = "Grammar Table has not been loaded.";
						return false;
						
					case ParseMessage.CommentError:
						errorString = "Comment Error. Unexpected end of file.";
						return false;
						
					case ParseMessage.CommentBlockRead:
					case ParseMessage.CommentLineRead:
						// do nothing
						break;
				}
			}
		}
		
	}
}
