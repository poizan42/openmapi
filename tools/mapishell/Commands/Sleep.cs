//
// openmapi.org - NMapi C# Mapi API - Sleep.cs
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
using System.Threading;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;

namespace NMapi.Tools.Shell {

	public sealed class SleepCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "sleep";
			}
		}

		public override string[] Aliases {
			get {
				return new string [0];
			}
		}

		public override string Description {
			get {
				return  "Sleep [x] microseconds";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public SleepCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			if (context.Param == String.Empty) {
				RequireMsg ("ms");
				return;
			}
			string msStr = ShellUtil.SplitParams (context.Param) [0].Trim ();
			int microSeconds = -1;
			bool worked = Int32.TryParse (msStr, out microSeconds);
			if (worked && microSeconds >= 0)
				Thread.Sleep (microSeconds);
			else
				Console.WriteLine ("ERROR: 'ms' needs to be a positive number.");
		}

	}
}
