//
// openmapi.org - NMapi C# Mime API - Base64EncodeStream.cs
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

	/// <summary>
	/// In difference to Base64Stream, this class now enables encoding while reading
	/// as well as while writing. The write functionality in Base64Stream has been
	/// obsoleted. Only the read functionality to decode is left.
	/// </summary>
	public class Base64EncodeStream : Stream
	{
		private Stream stream;
		private byte[] buffer;
		private int buflen;
		private int count;
		private int outLineLength;
		private long position;
		private long origStreamPos;
		// read
		private int index;


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

		static Base64EncodeStream ()
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
		public override bool CanWrite { get { return true; } }
		public override void SetLength (long value)
		{ }



		/// <summary>
		/// Default constructor.
		/// This constructs a Base64OutputStream with a line length of 76.
		/// </summary>
		public Base64EncodeStream (Stream s)
			: this (s, 76)
		{

		}

		/// <summary>
		/// Constructor.
		/// @param out the underlying output stream to encode
		/// @param lineLength the line length
		/// </summary>
		public Base64EncodeStream (Stream s, int outLineLen)
			: base ()
		{
			this.stream = s;
			buffer = new byte[6];
			this.outLineLength = outLineLen;
			// read

			origStreamPos = stream.Position;
		}



		/// <summary>
		/// Writes the specified byte to this output stream.
		/// </summary>
		/// throws IOException
		public override void WriteByte (byte c)
		{
			buffer[buflen++] = (byte)c;
			if (buflen == 3) {
				Encode ();
				buflen = 0;
			}
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified byte array 
		/// starting at offset <code>off</code> to this output stream.
		/// </summary>
		/// throws IOException
		public override void Write (byte[] b, int off, int count)
		{
			for (int i = 0; i < count; i++)
				WriteByte (b[off + i]);
		}

		/// <summary>
		/// Flushes this output stream and forces any buffered output bytes to be
		/// written out.
		/// </summary>
		/// throws IOException
		public override void Flush ()
		{
			if (buflen > 0) {
				Encode ();
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

		~Base64EncodeStream ()
		{
			Dispose (false);
		}

		//throws IOException
		private void Encode ()
		{
			if ((count + 4) > outLineLength) {
				stream.WriteByte (CR);
				stream.WriteByte (LF);
				count = 0;
			}
			if (buflen == 1) {
				byte b = buffer[0];
				int i = 0;
				stream.WriteByte ((byte)src[b >> 2 & 0x3f]);
				stream.WriteByte ((byte)src[(b << 4 & 0x30) + (i >> 4 & 0xf)]);
				stream.WriteByte ((byte)EQ);
				stream.WriteByte ((byte)EQ);
			} else if (buflen == 2) {
				byte b1 = buffer[0], b2 = buffer[1];
				int i = 0;
				stream.WriteByte ((byte)src[b1 >> 2 & 0x3f]);
				stream.WriteByte ((byte)src[(b1 << 4 & 0x30) + (b2 >> 4 & 0xf)]);
				stream.WriteByte ((byte)src[(b2 << 2 & 0x3c) + (i >> 6 & 0x3)]);
				stream.WriteByte (EQ);
			} else {
				byte b1 = buffer[0], b2 = buffer[1], b3 = buffer[2];
				stream.WriteByte ((byte)src[b1 >> 2 & 0x3f]);
				stream.WriteByte ((byte)src[(b1 << 4 & 0x30) + (b2 >> 4 & 0xf)]);
				stream.WriteByte ((byte)src[(b2 << 2 & 0x3c) + (b3 >> 6 & 0x3)]);
				stream.WriteByte ((byte)src[b3 & 0x3f]);
			}
			count += 4;
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
				buflen = 0;
				EncodeRead ();
				if (buflen == 0)
					return -1;
				index = 0;
			}
			position++;
			return buffer[index++] & 0xff;
		}

		//throws IOException
		private void EncodeRead ()
		{
			//Fill InputBuffer
			byte[] inBuf = new byte[3];
			int inBuflen = 0;
			while (inBuflen < 3) {
				int bx;
				bx = stream.ReadByte ();
				if (bx == -1) {
					if (buflen == 0 && inBuflen == 0) return;
					break;
				}
				inBuf[inBuflen++] = (byte)bx;
			}

			if ((count + 4) > outLineLength) {
				buffer[buflen++] = (CR);
				buffer[buflen++] = (LF);
				count = 0;
			}

			if (inBuflen == 1) {
				byte b = inBuf[0];
				int i = 0;
				buffer[buflen++] = ((byte)src[b >> 2 & 0x3f]);
				buffer[buflen++] = ((byte)src[(b << 4 & 0x30) + (i >> 4 & 0xf)]);
				buffer[buflen++] = ((byte)EQ);
				buffer[buflen++] = ((byte)EQ);
			} else if (inBuflen == 2) {
				byte b1 = inBuf[0], b2 = inBuf[1];
				int i = 0;
				buffer[buflen++] = ((byte)src[b1 >> 2 & 0x3f]);
				buffer[buflen++] = ((byte)src[(b1 << 4 & 0x30) + (b2 >> 4 & 0xf)]);
				buffer[buflen++] = ((byte)src[(b2 << 2 & 0x3c) + (i >> 6 & 0x3)]);
				buffer[buflen++] = (EQ);
			} else {
				byte b1 = inBuf[0], b2 = inBuf[1], b3 = inBuf[2];
				buffer[buflen++] = ((byte)src[b1 >> 2 & 0x3f]);
				buffer[buflen++] = ((byte)src[(b1 << 4 & 0x30) + (b2 >> 4 & 0xf)]);
				buffer[buflen++] = ((byte)src[(b2 << 2 & 0x3c) + (b3 >> 6 & 0x3)]);
				buffer[buflen++] = ((byte)src[b3 & 0x3f]);
			}
			count += 4;
		}



		/// <summary>
		/// Returns the number of bytes that can be read (or skipped over) from this
		/// input stream without blocking by the next caller of a method for this 
		/// input stream.
		/// </summary>
		//             throws IOException
		public override long Length {
			get {
				long l = (long)Math.Ceiling ((double)(stream.Length - origStreamPos) / 3) * 4;
				return l + (long)Math.Floor ((double)l / this.outLineLength) * 2;
			}
		}


		public override long Position { 
			get { return position;	} 
			set { 	}
		}

		/// <summary>
		/// only relevant and usable to reset the Stream to Position 0.
		/// Is primarily needed to reset the contained stream, so that
		/// The stream can be repeatedly used to retreive data.
		/// </summary>
		public override long Seek (long offset, SeekOrigin origin)
		{
			if (offset == 0) {
				stream.Seek (origStreamPos, SeekOrigin.Begin);
				position = 0;
				buflen = 0;
				index = 0;
			}
			return position;
		}

		public Stream Stream {
			get { return Stream; }
		}

	}
}