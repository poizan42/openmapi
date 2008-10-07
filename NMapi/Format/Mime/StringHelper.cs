//
// openmapi.org - NMapi C# Mime API - StringHelper.cs
//
// Copyright (C) 2008 The Free Software Foundation, Topalis AG
//
// Author C#: Andreas Huegel, Topalis AG
//
// GNU JavaMail is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// GNU JavaMail is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMapi.Format.Mime
{
	internal class StringHelper
	{

		/// <summary>
		/// returns as String-Array where each String includes the delimiter character
		/// that seperates it from the rest of the string. (String.Split() removes
		/// the separator characters.
		/// </summary>
		public static string[] SplitKeep (String s, char[] delimiters)
		{
			return SplitKeep (s, delimiters, -1);
		}
		/// <summary>
		/// returns as String-Array where each String includes the delimiter character
		/// that seperates it from the rest of the string. (String.Split() removes
		/// the separator characters.
		/// </summary>
		public static string[] SplitKeep (String s, char[] delimiters, int count)
		{
			//Split-method doesn't work here, as it removes the delimiter characters from the result strings
			int pos = 0;
			int posAlt = 0;
			List<String> strings = new List<String> ();

			while ((count == -1 || strings.Count + 1 < count)
				&& (pos = s.IndexOfAny (delimiters, posAlt)) > -1) {
				strings.Add (s.Substring (posAlt, pos - posAlt + 1));
				posAlt = pos + 1;
			}

			if (posAlt < s.Length)
				strings.Add (s.Substring (posAlt));

			return strings.ToArray ();

		}
	}
}
