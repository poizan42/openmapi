//
// openmapi.org - NMapi C# Mapi API - Providers.cs
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

	public sealed class ProvidersCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "providers";
			}
		}

		public override string[] Aliases {
			get {
				return new string [0];
			}
		}

		public override string Description {
			get {
				return "Print a list of providers";
			}
		}

		public override string Manual {
			get {
				return	"Prints a list of providers that can be " + 
				"loaded as the current OpenMapi.org \n" + 
				"provider using the 'use' command. Each record " + 
				"shows the identifier and the class\n" + 
				"implementing the provider.\n";
			}
		}

		public ProvidersCommand (Driver driver, ShellState state)
			: base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			state.Providers = ProviderManager.FindProviders ();

			// null = called at start.
			if (context != null) {
				foreach (var pair in state.Providers) {
					driver.WriteLine ("Found provider: " + 
						pair.Key + " => " + 
						pair.Value [1] + 
						" (Assembly: " + pair.Value [0] + ")\n");
				}
				driver.WriteLine ();
			}
		}

	}

}
