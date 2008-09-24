//
// openmapi.org - NMapi C# Mapi API - ShellUtil.cs
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
using System.Text;
using System.Collections.Generic;

namespace NMapi.Tools.Shell {

	public static class ShellUtil
	{
		public static string[] SplitParams (string prms)
		{
			List<string> parameters = new List<string> ();
			var currentWordBuilder = new StringBuilder ();

			Action<bool> nextWord = (ignored) => {
				string currentWord = currentWordBuilder.ToString ();
				if (currentWord != String.Empty) {
					parameters.Add (currentWord);
					currentWordBuilder = new StringBuilder ();
				}
			};

			bool quoteMode = false;
			foreach (char c in prms) {
				if (c == '\"') {
					quoteMode = !quoteMode;
					continue;
				}
				if (!quoteMode && Char.IsWhiteSpace (c)) {
					nextWord (true);
					continue;
				}
				currentWordBuilder.Append (c);
			}

			nextWord (true);
			return parameters.ToArray ();
		}

		public static string GetHomeDir ()
		{
			return Environment.GetFolderPath (Environment.SpecialFolder.Personal);
		}


	}

}
