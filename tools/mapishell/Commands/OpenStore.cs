//
// openmapi.org - NMapi C# Mapi API - OpenStore.cs
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
using NMapi.DirectoryModel;

namespace NMapi.Tools.Shell {

	public sealed class OpenStoreCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "openstore";
			}
		}

		public override string[] Aliases {
			get {
				return new string [] {"os"};
			}
		}

		public override string Description {
			get {
				return "Open the private/public message store";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public OpenStoreCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			if (context.Param == String.Empty) {
				RequireMsg ("pub|priv");
				return;
			}
			if (context.Param == "priv" || context.Param == "pub") {
				if (!state.CheckLoggedOnMsg ())
					return;
				state.Store = (context.Param == "priv") ? state.Session.PrivateStore : state.Session.PublicStore;
				state.StoreStr = context.Param;
				state.ChangeDir (String.Empty + PathHelper.PathSeparator); // Change to root dir
			}
			else
				driver.WriteLine ("ERROR: Value must be 'pub' or 'priv'.");
		}

	}
}
