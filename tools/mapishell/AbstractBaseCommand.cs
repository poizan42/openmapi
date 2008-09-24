//
// openmapi.org - NMapi C# Mapi API - AbstractBaseCommand.cs
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

namespace NMapi.Tools.Shell {

	public abstract class AbstractBaseCommand
	{
		protected ShellState state;
		protected Driver driver;

		public abstract string Name { get; }
		public abstract string[] Aliases { get; }
		public abstract string Description { get; }
		public abstract string Manual { get; }
		public abstract void Run (CommandContext context);

		public AbstractBaseCommand (Driver driver, ShellState state)
		{
			this.state = state;
			this.driver = driver;
		}

		internal static void RequireMsg (params string[] parameters)
		{
			Console.Write ("Required parameters: ");
			int i = 0;
			foreach (string param in parameters) {
				Console.Write ("[" + param + "]");
				if (i < parameters.Length-1)
					Console.Write (", ");
				i++;
			}
			Console.WriteLine ();
		}
		
	}

}
