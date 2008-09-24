//
// openmapi.org - NMapi C# Mapi API - Variables.cs
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

	public sealed class Variables
	{
		private ShellState state;
		private Stack<Variable> varStack;

		internal class Variable
		{
			private string name;
			private string val;
			private int scope;

			internal string Name {
				get { return name; }
			}

			internal string Value {
				get { return val; }
				set { val = value; }
			}

			internal int Scope {
				get { return scope; }
			}

			internal Variable (string name, string val, int scope)
			{
				this.name = name;
				this.val = val;
				this.scope = scope;
			}
		}

		public Variables (ShellState state)
		{
			this.state = state;
			this.varStack =  new Stack<Variable> ();
		}

		internal void DropCurrentScope ()
		{
			// for this to work the variables in the currentScope
			// must be on the top of the stack.
			while (varStack.Count > 0 && 
				varStack.Peek ().Scope == state.CurrentScope)
					varStack.Pop ();
		}

		internal void DefineVariable (string name, string val)
		{
			varStack.Push (new Variable (name, val, state.CurrentScope));
		}

		internal void AssignVariable (string name, string val)
		{
			Variable definedVar = GetVariable (name);
			if (definedVar == null)
				DefineVariable (name, val);
			else
				definedVar.Value = val;
		}

		// fixme: Slow!
		internal Variable GetVariable (string name)
		{
			foreach (Variable var in varStack)
				if (var.Name == name)
					return var;
			return null;
		}

	}

}
