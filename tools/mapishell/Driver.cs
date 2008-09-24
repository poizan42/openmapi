//
// openmapi.org - NMapi C# Mapi API - Driver.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

using NDesk.Options;
using Mono.Terminal;

using NMapi;
using NMapi.Meta;

namespace NMapi.Tools.Shell {

	public sealed class Driver
	{
		private const string RC_FILE = ".mashrc";

		private ShellState state;
		private MetaManager metaMan;

		public static void Main (string [] args)
		{
			new Driver (args);
		}

		private Version Version {
			get {
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}

		private string UserPrefix {
			get { return (state.User == null 
				|| state.User == String.Empty) ? "" : state.User + "@"; }
		}

		private string PathPostfix {
			get { return (state.FullPath == null ||
					state.FullPath == String.Empty) ? "" : ":" + state.FullPath; }
		}

		private string StatusString {
			get {
				string result = UserPrefix + state.Host + PathPostfix;
				if (result != String.Empty)
					return "#" + result;
				return result;
			}
		}

		internal MetaManager MetaManager {
			get { return metaMan; }
		}

		private void RunRcScript ()
		{
			string absPath = Path.Combine (ShellUtil.GetHomeDir (), RC_FILE);
			if (File.Exists (absPath))
				ExecFile (absPath);
		}

		public Driver (string[] args)
		{
			state = new ShellState (this);
			metaMan = new MetaManager ();

			Console.WriteLine ("\nOpenMapi.org - MapiShell " +  
						Version.Major + "."  + Version.Minor);
			Console.WriteLine ("(C) 2008 by Topalis AG, http://www.openmapi.org/\n\n");
			Console.WriteLine ("For more information, type 'help'.\n");

			state.Resolver.AddAssembly ("mscorlib", new Version (2, 0));
			state.Resolver.AddAssembly ("System", new Version (2, 0));
			state.Resolver.AddAssembly ("NMapi", new Version (0, 1));

			// Register properties

			Console.Write ("Loading Property names ... ");
			state.PropertyLookup.RegisterClass (typeof (NMapi.Flags.Property).FullName);
			state.PropertyLookup.RegisterClass (typeof (NMapi.Flags.Outlook).FullName);
			Console.Write (" OK\n\n");

			Assembly asm = Assembly.GetExecutingAssembly ();

			foreach (Type current in asm.GetTypes ())
				if (current.BaseType == typeof (AbstractBaseCommand)) {
					var cmd = (AbstractBaseCommand) Activator.CreateInstance (
						current, new object[] {this, state}, null);
					state.Commands.Add (cmd.Name, cmd);
					foreach (string alias in cmd.Aliases)
						state.Commands.Add (alias, cmd);
			}

			// TODO: Partially/unimplemented commands:
			//       cat, head, tail, get, set, find, grep, expr and let

			// stat (Fix: Linq)

			string loadScript = null;

			OptionSet p = new OptionSet ()
				.Add ("script=|s=", s => loadScript = s)
				.Add ("version|help|h|?", l => {
					PrintHelp ();
					Environment.Exit (1);
				});
			List<string> rest;
			try {
				rest = p.Parse (args);
			} catch (OptionException e) {
				Console.WriteLine ("ERROR: " + e.Message);
				return;
			}

			state.Commands ["providers"].Run (null);

			LineEditor editor = new LineEditor ("MapiShell");

			Func<string, string> getNext = (prefix) => { 
				if (prefix == null)
					prefix = "mapi" + StatusString;
				return editor.Edit (prefix + "> ", "");
			};

			ExecResource ("default.mss");

			RunRcScript ();

			if (loadScript != null)
				Exec ("load " + loadScript, false, getNext);

			string line;
			while ((line = getNext (null)) != null)
				Exec (line, false, getNext);
		}

		internal void Exec (string line, bool echo, 
			Func<string, string> nextLineFunc)
		{
			line = line.Trim ();

			if (line.Length < 1)
				return;

			if (line [0] == '#')
				return;

			if (echo && line [0] != '@')
				Console.WriteLine (line);
			if (line [0] == '@')
				line = line.Substring (1).TrimStart ();

			// Variable Assignment
			if (line [0] == '$') {
				int pos = line.IndexOf ('=');
				if (pos != -1) {
					string name = line.Substring (1, pos-1).Trim ();
					string val = line.Substring (pos+1).Trim ();
					Console.WriteLine (name);
					Console.WriteLine (val);
					// TODO: Support variables in right block!
					state.Variables.AssignVariable (name, val);
					return;
				}
			}

			string cmd = line;
			string prms = String.Empty;

			int pos2 = line.IndexOf (' ');
			if (pos2 != -1) {
				cmd = line.Substring (0, pos2).ToLower ();
				prms = line.Substring (pos2+1);
			}

			prms = ResolveVariables (prms);

			bool unknown = false;
			if (cmd == String.Empty)
				return;
			else if (state.Commands.ContainsKey (cmd)) {
				var context = new CommandContext (prms, nextLineFunc);
				state.Commands [cmd].Run (context);
			}
			else {
				if (state.Functions.FunctionExists (cmd))
					state.Functions.CallFunction (cmd, 
						ShellUtil.SplitParams (prms), 
						nextLineFunc);
				else
					unknown = true;
			}

			if (unknown)
				Console.WriteLine ("Unknown command or function.");				
		}

		private string ResolveVariables (string prms)
		{
			StringBuilder result = new StringBuilder ();
			StringBuilder current = new StringBuilder ();

			bool skipOutput = false;
			bool escapeMode = false;
			bool inVar  = false;
			foreach (char c in prms) {
				switch (c) {
					case  '\\':
						escapeMode = !escapeMode;
						if (escapeMode)
							continue;
					break;
					case '$':
						if (escapeMode) {
							inVar = true;
							continue;
						}
					break;
					default:
						if (inVar) {
							if (Char.IsWhiteSpace (c)) {
								inVar = false;
								var variable = state.Variables.GetVariable (current.ToString());
								if (variable != null)
									result.Append (variable.Value);
								current = new StringBuilder ();
								skipOutput = false;
							} else {
								current.Append (c);
								skipOutput = true;
							}
						}
						escapeMode = false;
					break;
				}

				if (!skipOutput)
					result.Append (c);
			}
			if (current.ToString ().Length != 0) {
				var variable = state.Variables.GetVariable (current.ToString());
				if (variable != null)
					result.Append (variable.Value);
			}
			return result.ToString ();
		}

		internal void ExecFile (string batchFile)
		{
			ExecStream (File.Open (batchFile, FileMode.Open));
		}

		internal void ExecStream (Stream stream)
		{
			using (var reader = new StreamReader (stream)) {
				string line;

				Func<string, string> getNext = (prm) => reader.ReadLine ();

				while ((line = getNext (null)) != null)
					Exec (line, true, getNext);
			}
		}

		internal void ExecResource (string name)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			Stream batchStream = asm.GetManifestResourceStream (name);
			ExecStream (batchStream);
		}

		internal void PrintHelp ()
		{
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			Console.WriteLine ("For more information see: http://www.openmapi.org\n");
			Console.WriteLine ("Usage: mapishell [OPTION] ...  [FILE]\n");
			Console.WriteLine ("-s, -script            execute batch script");
			Console.WriteLine ("-h, -help              Show this help\n\n");

			Console.WriteLine ("Commands:\n");

			foreach (var pair in state.Commands) {
				Console.Write (String.Format ("{0,-15}", pair.Key));
				Console.WriteLine (pair.Value.Description);
			}

		}

	}

}
