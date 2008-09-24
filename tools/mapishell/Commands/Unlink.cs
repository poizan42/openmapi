//
// openmapi.org - NMapi C# Mapi API - Unlink.cs
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
using NMapi.Properties.Special;

namespace NMapi.Tools.Shell {

	public sealed class UnlinkCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "unlink";
			}
		}

		public override string[] Aliases {
			get {
				return new string [] {"del"};
			}
		}

		public override string Description {
			get {
				return  "Delete object";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public UnlinkCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			Action<IMapiFolder, string> op = (parent, fileHashName) => {
				SBinary entryId = state.KeyList.InteractiveResolveEntryID (parent, fileHashName);
				if (entryId == null) {
					Console.WriteLine ("Unknown Key ID!");
					return;
				}

				var entryList = new EntryList (new SBinary [] { entryId });
				parent.DeleteMessages (entryList, null, 0);
			};
	
			state.PerformOperationOnFolder (context, op);
		}

	}
}
