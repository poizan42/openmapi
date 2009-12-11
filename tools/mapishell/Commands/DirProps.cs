//
// openmapi.org - NMapi C# Mapi API - Props.cs
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

	public sealed class DirPropsCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "dirprops";
			}
		}

		public override string[] Aliases {
			get {
				return new string [0];
			}
		}

		public override string Description {
			get {
				return "List the Properties of an directory";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public DirPropsCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			if (context.Param == String.Empty) {
				RequireMsg ("key");
				return;
			}

			string keyName = ShellUtil.SplitParams (context.Param) [0];

			using (IMapiProp obj = state.OpenFolder (state.Input2AbsolutePath (keyName))) {
				if (obj == null) { 
					driver.WriteLine ("Unknown Key ID!");
					return;
				}
				var tags = obj.GetPropList (0);
				int i = 0;
				foreach (var propTag in tags) {
					string name = state.PropTag2Name (propTag.Tag);

					if (name.Length <= 39) {
						driver.Write (String.Format ("{0,-40}", name));
						if ((i % 2) == 0)
							driver.WriteLine ();
					}
					else
						driver.WriteLine (name);

					i++;
				}
				driver.WriteLine ();
			}
		}

	}
}
