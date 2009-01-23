// openmapi.org - NMapi C# IMAP Gateway - CommandAnalyserParserInterface.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace NMapi.Gateways.IMAP
{
	public class CommandAnalyserParserInterface
	{

		private CommandAnalyser parent;
		private Command _command;
		/// <summary>
		/// the stream coming from the client connection
		/// </summary>
		private AbstractClientConnection clientConnection;
		private StreamReader _inSR;
		private parser parser_obj; 
		private Yylex yy;

		
		public CommandAnalyserParserInterface(CommandAnalyser parent, AbstractClientConnection client)
		{
			this.parent = parent;
			clientConnection = client;
		}

		public Command CheckCommand()
		{

			Trace.WriteLine ("statustest: " + StateAuthenticated());
			
			if (clientConnection.DataAvailable()) {

				Trace.WriteLine("pi_1");
				NewCommand();
				SetupStreamReader();
				Trace.WriteLine("pi_2_streamreader done");
				if (yy == null) {
					yy = new Yylex (_inSR);
				} else {
					yy.ReInit (_inSR);
				}

				Trace.WriteLine("pi_3_yylex done");
				parser_obj = new parser(yy , this);
				Trace.WriteLine("pi_4_parser done");
				/* open input files, etc. here */
				TUVienna.CS_CUP.Runtime.Symbol parse_tree = null;

				bool do_debug_parse = false;
				try {
					if (do_debug_parse)
						parse_tree = parser_obj.debug_parse();
					else
						parse_tree = parser_obj.parse();
				} catch (Exception e) {
					// set error in command. But only, if there hasn't been
					// an error stored yet. We want to preserve state errors,
					// as they can lead to further formal errors.
					if (_command.Parse_error == null)
						_command.Parse_error = "Formal parsing error: " + e.Message;
				} finally {
					/* do close out here */
				}
				Trace.WriteLine("pi_5");
				return _command;
		    }
			return null;
		}

		public Command command {
			get { return _command; }
		}
		
		public void NewCommand()
		{
			_command = new Command();
		}
	
		public string ReadLiteral (int count)
		{
			string s = null;
			Trace.WriteLine("readLiteral: "+count);

			// only read a literal, if no formal or state error has been
			// identified so far.
			if (command.Parse_error == null) {
			
				// initiate literal sending by client
				clientConnection.Send ("+ Ready for literal data\r\n");
				
				// read literal data
				s = clientConnection.ReadBlock(count);
				Trace.WriteLine("readLiteral: "+s.Length+" \""+s+"\"");
	
				/* read the rest of the command and set the parser up with the new stream reader*/
				SetupStreamReader();
				parser_obj.SetNewInput(_inSR);
			}
				
			return (s);
		}

		public void SetupStreamReader ()
		{
			string s = clientConnection.ReadLine();
			_inSR = new StreamReader( new MemoryStream( Encoding.ASCII.GetBytes(s+"\r\n")));
		}

		public bool StateNotAuthenticated () {
			try {
				return parent.StateNotAuthenticated ();
			} catch ( Exception) {
				throw new Exception ("State delegate for state non authenticated not set in CommandAnalyser");
			}			
		}
		
		public bool StateAuthenticated () {
			try {
				return parent.StateAuthenticated ();
			} catch ( Exception) {
				throw new Exception ("State delegate for state authenticated not set in CommandAnalyser");
			}			
		}
		
		public bool StateSelected () {
			try {
				return parent.StateSelected ();
			} catch ( Exception) {
				throw new Exception ("State delegate for state selected not set in CommandAnalyser");
			}			
		}
		
		public bool StateLogout () {
			try {
				return parent.StateLogout ();
			} catch ( Exception) {
				throw new Exception ("State delegate for state logout not set in CommandAnalyser");
			}			
		}
		
	}
}
