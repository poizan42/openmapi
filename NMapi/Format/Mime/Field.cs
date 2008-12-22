//
// openmapi.org - NMapi C# Mime API - Field.cs
//
// Copyright (C) 2008 The Free Software Foundation, Topalis AG
//
// Author Java: <a href="mailto:dog@gnu.org">Chris Burdess</a>
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
	public class Field
	{

		public static String AppendItemsFormat<T> (IEnumerable<T> items, String delimiter, int offset)
		{
			if (items == null || items.Count () == 0) {
				return null;
			}

			StringBuilder buffer = new StringBuilder ();
			bool firstPass = true;
			int delLen = 0;
			int used = offset;
			foreach (Object item in items) {
				if (firstPass) {
					if (delimiter == null) {
						try {
							delimiter = ((InternetHeader)item).Delimiter;
						}
						catch (Exception) {
							delimiter = ";";
						}
					}
					delLen = delimiter.Length + 1;
					firstPass = false;
				} else {
					buffer.Append (delimiter + " ");
					used += delLen;
				}
				String addressText = item.ToString ();

				int len = addressText.Length;
				int fl = addressText.IndexOf ("\r\n");
				if (fl < 0) {
					fl = addressText.Length;
				}
				int ll = addressText.LastIndexOf ("\r\n");

				if ((used + fl) > 76) {
					buffer.Append ("\r\n\t");
					used = 8;
				}
				buffer.Append (addressText);
				used = (ll == -1) ? (used + len) : (len - ll - 2);
			}
			return buffer.ToString ();
		}


	}
}
