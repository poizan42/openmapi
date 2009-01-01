//
// openmapi.org - NMapi C# Mapi API - Close.cs
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
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;

namespace NMapi.Tools.Shell {

	public sealed class CloseCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "close";
			}
		}

		public override string[] Aliases {
			get {
				return new string [0];
			}
		}

		public override string Description {
			get {
				return "Close the message store";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public CloseCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			if (state.Session == null) {
				driver.WriteLine ("ERROR: Session is not open.");
				return;
			}
			state.CloseSession ();
			state.Factory = null;
			if (state.Logging)
				driver.WriteLine ("Session closed.");
		}

	}
}
