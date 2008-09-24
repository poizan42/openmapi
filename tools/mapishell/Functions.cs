//
// openmapi.org - NMapi C# Mapi API - Functions.cs
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
using System.Linq;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Tools.Shell {

	public sealed class Functions
	{
		private Driver driver;
		private ShellState state;
		private Dictionary<string, string[]> funcTable;

		public Functions (Driver driver, ShellState state)
		{
			this.driver = driver;
			this.state = state;
			this.funcTable = new Dictionary<string, string[]> ();
		}

		internal bool FunctionExists (string name)
		{
			return funcTable.ContainsKey (name);
		}

		internal void DefineFunction (string name, string[] lines)
		{
			if (state.CommandExists (name))
				throw new Exception ("Function 'name' can't be defined, " + 
					"because a command with the name exists!");

			if (FunctionExists (name))
				throw new Exception ("Function exists!");

			funcTable [name] = lines;
		}

		internal void CallFunction (string name, string[] parameters, 
			Func<string, string> nextLineFunc)
		{
			if (!FunctionExists (name))
				throw new Exception ("Unknown function '" + name + "' called!");

			state.EnterScope ();

			int i = 1;
			foreach (string parameter in parameters)
				state.Variables.DefineVariable (String.Empty + (i++), parameter);

			foreach (string line in funcTable [name])
				driver.Exec (line, false, nextLineFunc);
			state.ExitScope ();
		}

	}
}
