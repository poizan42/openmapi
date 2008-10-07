//
// openmapi.org - NMapi C# Mime API - Base64Stream.cs
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

	internal class Base64Stream : Stream
	{
		private Stream stream;
		private byte[] buffer;
		private int buflen;
		//private int count;
		//private int outLineLength;
		// read
		private int index;
		private byte[] decodeBuf;


		private static char[] src =
		{
		  'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 
		  'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 
		  'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 
		  'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 
		  'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 
		  'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', 
		  '8', '9', '+', '/'
		};

		private static byte[] dst;

		static Base64Stream ()
		{
			dst = new byte[256];
			for (int i = 0; i < 255; i++)
				dst[i] = 0xff;
			for (int i = 0; i < src.Length; i++)
				dst[src[i]] = (byte)i;

		}



		private const int LF = 10, CR = 13, EQ = 61;


		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return false; } }
		public override long Position { get { return -1; } set { } }
		public override long Seek (long offset, SeekOrigin origin)
		{ return -1; }
		public override void SetLength (long value)
		{ }



		/// <summary>
		/// Default constructor.
		/// This constructs a Base64OutputStream with a line length of 76.
		/// </summary>
		public Base64Stream (Stream s)
			: this (s, 76)
		{ }

		/// <summary>
		/// Constructor.
		/// @param out the underlying output stream to encode
		/// @param lineLength the line length
		/// </summary>
		public Base64Stream (Stream s, int outLineLen)
			: base ()
		{
			this.stream = s;
			buffer = new byte[3];
			//this.outLineLength = outLineLen;
			// read
			decodeBuf = new byte[4];
		}


		/// <summary>
		/// Has no funktion. Use Base64EncodeStream for encoding!
		/// 
		/// Writes the specified byte to this output stream.
		/// </summary>
		/// <param name="c"></param>
		/// throws IOException
		public override void WriteByte (byte c)
		{ }

		/// <summary>
		/// Has no funktion. Use Base64EncodeStream for encoding!
		/// </summary>
		/// throws IOException
		public override void Write (byte[] b, int off, int count)
		{ }


		/// <summary>
		/// Flushes this output stream and forces any buffered output bytes to be
		/// written out.
		/// </summary>
		/// throws IOException
		/// 

		public override void Flush ()
		{
			if (buflen > 0) {
				//Encode ();
				buflen = 0;
			}
			stream.Flush ();
		}


		//throws IOException 
		public override void Close ()
		{
			if (stream != null)
				stream.Close ();
			base.Close ();
		} // close()

		protected override void Dispose (bool disposing)
		{
			// If disposing equals true, dispose all managed
			// and unmanaged resources.
			if (disposing) {
				// Dispose managed resources.
				if (stream != null) {
					stream.Dispose ();
					stream = null;
				}
			}

			// Call the appropriate methods to clean up
			// unmanaged resources here.
			// If disposing is false,
			// only the following code is executed.

			// Note disposing has been done.
			base.Dispose (disposing);
		}

		~Base64Stream ()
		{
			Dispose (false);
		}


		//---------------------------------------------------
		// read
		//---------------------------------------------------


		/// <summary>
		/// Reads up to len bytes of data from the input stream into an array of 
		/// bytes.
		/// </summary>
		//throws IOException
		public override int Read (byte[] b, int off, int len)
		{
			try {
				int l = 0;
				for (; l < len; l++) {
					int c;
					if ((c = ReadByte ()) == -1) {
						if (l == 0)
							l = -1;
						break;
					}
					b[off + l] = (byte)c;
				}
				return l;
			}
			catch (IOException) {
				return -1;
			}
		}

		//throws IOException
		public override int ReadByte ()
		{
			if (index >= buflen) {
				decode ();
				if (buflen == 0)
					return -1;
				index = 0;
			}
			return buffer[index++] & 0xff;
		}


		/// <summary>
		/// Returns the number of bytes that can be read (or skipped over) from this
		/// input stream without blocking by the next caller of a method for this 
		/// input stream.
		/// </summary>
		//             throws IOException
		public override long Length {
			get { return (stream.Length * 3) / 4 + (buflen - index); }
		}

		//throws IOException
		private void decode ()
		{
			buflen = 0;
			int c;
			do {
				c = stream.ReadByte ();
				if (c == -1)
					return;
			}
			while (c == LF || c == CR);
			decodeBuf[0] = (byte)c;
			int j = 3, l;
			for (int k = 1; (l = stream.Read (decodeBuf, k, j)) != j; k += l) {
				if (l == -1)
					throw new IOException ("Base64 encoding error");
				j -= l;
			}

			byte b0 = dst[decodeBuf[0] & 0xff];
			byte b2 = dst[decodeBuf[1] & 0xff];
			buffer[buflen++] = (byte)(b0 << 2 & 0xfc | b2 >> 4 & 0x3);
			if (decodeBuf[2] != EQ) {
				b0 = b2;
				b2 = dst[decodeBuf[2] & 0xff];
				buffer[buflen++] = (byte)(b0 << 4 & 0xf0 | b2 >> 2 & 0xf);
				if (decodeBuf[3] != EQ) {
					b0 = b2;
					b2 = dst[decodeBuf[3] & 0xff];
					buffer[buflen++] = (byte)(b0 << 6 & 0xc0 | b2 & 0x3f);
				}
			}
		}
	}
}
