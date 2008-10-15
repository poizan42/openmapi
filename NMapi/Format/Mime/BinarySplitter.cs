//
// openmapi.org - NMapi C# Mime API - BinarySplitter.cs
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMapi.Format.Mime
{
	internal class BinarySplitter : BinaryReader
	{

		public BinarySplitter (Stream inS)
			: base (inS)
		{ }

		public Stream ReadToDelimiter (byte[] delimiter)
		{
			if (this.PeekChar () == -1)
				return null;

			MemoryStream os = new MemoryStream ();
			MemoryStream os1 = new MemoryStream ();

			int countDel = 0;
			try {
				while (true) {
					int b = ReadByte ();
					if (b != delimiter[countDel] && countDel > 0) {
						//delimiter has not been covered in full lenght, 
						//write the covered section to output
						long len = os1.Length;
						os.Write (os1.ToArray (), 0, (int)len);
						os1 = new MemoryStream ();
						countDel = 0;
					}
					if (b == delimiter[countDel]) {
						//recognize delimiter
						countDel++;
						os1.WriteByte ((byte)b);
						if (countDel == delimiter.Length) {
							return new MemoryStream (os.ToArray ());
						}
					} else {
						// a regular character has appeared. Add to output
						os.WriteByte ((byte)b);
					}
				}
			} catch (EndOfStreamException) { }
			long len1 = os1.Length;
			os.Write (os1.ToArray (), 0, (int)len1);
			return new MemoryStream (os.ToArray ());
		}


	}
}
