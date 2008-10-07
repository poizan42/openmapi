//
// openmapi.org - NMapi C# Mime API - QStream.cs
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMapi.Format.Mime
{
	internal class QStream : QPStream
	{

		private const int SPACE = 32;
		private const int UNDERSCORE = 95;
		//Read
		private const int EQ = 61;

		private const String TEXT_SPECIALS = "=_?";
		private const String WORD_SPECIALS = "=_?\"#$%&'(),.:;<>@[\\]^`{|}~";

		private String specials;


		/// <summary>
		/// Constructor.
		/// The <code>word</code> parameter is used to indicate whether the bytes
		/// passed to this stream are considered to be RFC 822 "word" tokens or
		/// "text" tokens.
		/// @param out the underlying output stream
		/// @param word word mode if true, text mode otherwise
		/// </summary>
		public QStream (Stream s, bool word)
			: base (s, 0x7fffffff)
		{
			specials = word ? WORD_SPECIALS : TEXT_SPECIALS;
		}

		public QStream (Stream s)
			: this (s, false)
		{
		}

		/// <summary>
		/// Write a character to the stream.
		/// </summary>
		/// <param name="c"></param>
		/// throws IOException
		public override void WriteByte (byte c)
		{
			c &= 0xff;
			if (c == SPACE)
				output (UNDERSCORE, false);
			else {
				if (c < 32 || c >= 127 || specials.IndexOf ((char)c) >= 0)
					output ((byte)c, true);
				else
					output ((byte)c, false);
			}
		}

		public static int EncodedLength (byte[] bytes, bool word)
		{
			int len = 0;
			String specials = word ? WORD_SPECIALS : TEXT_SPECIALS;
			for (int i = 0; i < bytes.Length; i++) {
				int c = bytes[i] & 0xff;
				if (c < 32 || c >= 127 || specials.IndexOf ((char)c) >= 0)
					len += 3;
				else
					len++;
			}
			return len;
		}


		//----------------------------------------------
		// Read
		// ---------------------------------------------
		//throws IOException
		public override int ReadByte ()
		{
			int c = stream.ReadByte ();
			if (c == UNDERSCORE)
				return SPACE;
			if (c == EQ) {
				buf[0] = (char)stream.ReadByte ();
				buf[1] = (char)stream.ReadByte ();
				try {
					return Convert.ToInt32 (new String (buf, 0, 2), 16);
				}
				catch (FormatException e) {
					throw new IOException ("Quoted-Printable encoding error: " +
						e.Message);
				}
				catch (ArgumentException e) {
					throw new IOException ("Quoted-Printable encoding error: " +
						e.Message);
				}
				catch (OverflowException e) {
					throw new IOException ("Quoted-Printable encoding error: " +
						e.Message);
				}
			}
			return c;
		}

	}
}
