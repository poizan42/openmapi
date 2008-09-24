//
// openmapi.org - NMapi C# Mapi API - Quit.cs
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
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;

namespace NMapi.Tools.Shell {

	public sealed class QuitCommand : AbstractBaseCommand
	{
		private const string LOGOUT_FILE = ".mash_logout";

		public override string Name {
			get {
				return "quit";
			}
		}

		public override string[] Aliases {
			get {
				return new string [] {"exit"};
			}
		}

		public override string Description {
			get {
				return "Exit the shell";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public QuitCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		private void RunExitScript ()
		{
			string absPath = Path.Combine (ShellUtil.GetHomeDir (), LOGOUT_FILE);
			if (File.Exists (absPath))
				driver.ExecFile (absPath);
		}

		public override void Run (CommandContext context)
		{
			RunExitScript ();
			state.CloseSession ();
			Environment.Exit (1);
		}

	}
}
