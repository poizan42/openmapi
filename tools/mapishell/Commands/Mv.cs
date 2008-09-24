//
// openmapi.org - NMapi C# Mapi API - Mv.cs
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

	public sealed class MvCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "mv";
			}
		}

		public override string[] Aliases {
			get {
				return new string [] {"move"};
			}
		}

		public override string Description {
			get {
				return  "Move object";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public MvCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			string[] prms = ShellUtil.SplitParams (context.Param);
			if (prms.Length < 2) {
				RequireMsg ("src", "dest");
				return;
			}

			if (!state.CheckStore())
				return;

			string srcPath = state.Input2AbsolutePath (prms [0]);
			string destPath = state.Input2AbsolutePath (prms [1]);
			state.CopyOrMove (srcPath, destPath, true);
		}

	}
}
