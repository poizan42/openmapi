//
// openmapi.org - NMapi C# Mapi API - Use.cs
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
using NMapi.Linq;
using NMapi.Flags;
using NMapi.Properties;

namespace NMapi.Tools.Shell {

	public sealed class UseCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "use";
			}
		}

		public override string[] Aliases {
			get {
				return new string [0];
			}
		}

		public override string Description {
			get {
				return "Use an OpenMapi.org provider";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public UseCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			if (context.Param == String.Empty) {
				RequireMsg ("provider");
				return;
			}

			string providerId = context.Param;
			if (!state.Providers.ContainsKey (providerId)) {
				Console.WriteLine ("unknown provider ID.");
				return;
			}

			if (state.Factory != null) {
				state.CloseSession ();
				if (state.Logging)
					Console.WriteLine ("Closed existing session.");
			}

			state.Factory = ProviderManager.GetFactory (state.Providers [providerId]);
			try {
				state.Session = state.Factory.CreateMapiSession ();
				state.MapiContext = new MapiContext (state.Session);
			} catch (MapiException e) {
				Console.WriteLine ("ERROR: Can't open Mapi-Session!\n\n" + e.Message);
				return;
			}
			if (state.Logging)
				Console.WriteLine ("Session opened for '" + 
					ProviderManager.GetName (state.Factory) + "'.");
		}

	}
}
