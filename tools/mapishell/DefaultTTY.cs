//
// openmapi.org - NMapi C# Mapi API - DefaultTTY.cs
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

using Mono.Terminal;

namespace NMapi.Tools.Shell {

	public sealed class DefaultTTY
	{
		public static void Main (string[] args)
		{
			LineEditor editor = new LineEditor ("MapiShell", 300);

			Driver driver = new Driver (args);
			Thread driverThread = new Thread (new ThreadStart (driver.Start));
			driverThread.Start ();

			driver.WaitUntilInput ();

			string input = null;
			while (true) {
				input = editor.Edit (driver.CurrentPrefix, String.Empty);
				if (input == null) // Exit on EOF
					input = "quit";
				driver.PutInputAndWait (input);
			}

		}
	}

}
