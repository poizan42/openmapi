//
// openmapi.org - NMapi C# Mapi API - Stat.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.IO;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Tools.Shell {

	public sealed class StatCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "stat";
			}
		}

		public override string[] Aliases {
			get {
				return new string [0];
			}
		}

		public override string Description {
			get {
				return "";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public StatCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		public override void Run (CommandContext context)
		{
			if (context.Param == String.Empty) {
				RequireMsg ("key");
				return;
			}

			// TODO
			// NMapi.Flags.Property.MessageSize
			// NMapi.Flags.Property.Access

			DateTime created = DateTime.Now;
			DateTime modified = DateTime.Now;
			SBinary entryId = null;
			int size = 1;
			string fname = "testName!";
			string objType = "testType";



			Console.WriteLine ("  File: \"" + fname + "\"");
			Console.Write (String.Format("  Size: {0,-11} Object Type: ", size));
			Console.WriteLine (objType);

			string entryIdStr = "NULL";
			if (entryId != null)
				entryIdStr = entryId.ToHexString ();
			Console.WriteLine ("Entry Id: " + entryIdStr);

//			Console.WriteLine ("Access: " + modified.ToString ("yyyy-MM-dd HH:mm:ss.000000000 zzzz"));
			Console.WriteLine ("Created: " + created.ToString ("yyyy-MM-dd HH:mm:ss.000000000 zzzz"));
			Console.WriteLine ("Modified: " + modified.ToString ("yyyy-MM-dd HH:mm:ss.000000000 zzzz"));





		}

	}
}
