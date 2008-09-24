//
// openmapi.org - NMapi C# Mapi API - CreateMsg.cs
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
using NMapi.Properties.Special;

namespace NMapi.Tools.Shell {

	public sealed class CreateMsgCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "createmsg";
			}
		}

		public override string[] Aliases {
			get {
				return new string [] {"cm"};
			}
		}

		public override string Description {
			get {
				return "Creates a new message";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public CreateMsgCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			if (context.Param == String.Empty) {
				AbstractBaseCommand.RequireMsg ("parent dir");
				return;
			}

			if (!state.CheckStore())
				return;

			string param1 = ShellUtil.SplitParams (context.Param) [0].Trim ();
			string path = state.Input2AbsolutePath (param1);

			IMapiFolder parent = null;
			try {
				parent = state.OpenFolder (path);
				IMessage msg = parent.CreateMessage (null, 0);
				msg.SaveChanges (0);
				SPropValue val = msg.HrGetOneProp (Property.EntryId);
				string hash = new KeyID (val.Value.Binary).Hash;
				Console.WriteLine ("Created '" + hash + "'.");
			} catch (MapiException e) {
				if (e.HResult == Error.NotFound) {
					Console.WriteLine ("Folder not found!");
					return;
				} else if (e.HResult == Error.NoAccess) {
					Console.WriteLine ("No permission to create message!");
					return;
				} else
					throw;
			} finally {
				if (parent != null)
					parent.Close ();
			}

			// TODO: Support interfaces!
		}

	}
}
