//
// openmapi.org - NMapi C# Mapi API - Ls.cs
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

using NDesk.Options;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;

namespace NMapi.Tools.Shell {

	public sealed class LsCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "ls";
			}
		}

		public override string[] Aliases {
			get {
				return new string [] {"dir"};
			}
		}

		public override string Description {
			get {
				return "List subfolders and objects of current folder";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public LsCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			if (!state.CheckStore())
				return;

			string[] dirNames = state.GetSubDirNames (state.CurrentFolder);
			
			//		- sort mapping?
			
			foreach (string name in dirNames)
				driver.WriteLine ("D  " + name, ConsoleColor.Magenta, null);

			bool asL, asA = false;
			string msgClass = null;

			OptionSet p = new OptionSet ()
				.Add ("l", l => asL = true)
				.Add ("a", a => asA = true)
				.Add ("class", cls => msgClass = cls)
				.Add ("help|?", h => {
					driver.WriteLine ("Options: ");
					driver.WriteLine ("-l      TODO");
					driver.WriteLine ("-a      TODO");
					driver.WriteLine ("-class  TODO");
					return; // TODO: break!
				});
			List<string> rest;
			try {
				rest = p.Parse (ShellUtil.SplitParams (context.Param));
			} catch (OptionException e) {
				driver.WriteLine ("ERROR: " + e.Message);
				return;
			}

			/* TODO: Optimize performance! */

			var objects = state.MapiContext.GetQuery<ShellObject> (state.CurrentFolder);

			var query = from o in objects
			//		where o.MessageClass == msgClass
					select o;

			foreach (var obj in query) {
				var key = new KeyID (obj.EntryId);

				using (IMapiProp prop = obj.GetAssociatedIMapiProp (0)) { // read-only
					driver.WriteLine ("F  " + key + "  " + driver.MetaManager.GetSummary (prop));
				}
					state.KeyList.AddKey (key); // Make the key known!
			}
		}

	}
}
