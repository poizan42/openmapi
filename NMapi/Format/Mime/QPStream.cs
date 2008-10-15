//
// openmapi.org - NMapi C# Mime API - QPStream.cs
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
	public class QPStream : Stream
	{

		///Char array used in decimal to hexidecimal conversion.
		private static char[] hex = {'0','1','2','3','4','5','6',
			   '7','8','9','A','B','C','D',
			   'E','F'};

		// Current byte position in output.
		private int count;

		// Number of bytes per line.
		private int bytesPerLine;
		// Flag when a space is seen.
		private bool gotSpace;
		// Flag when a CR is seen.
		private bool gotCR;

		protected Stream stream;

		// Read
		protected char[] buf;
		protected List<int> unreadList;

		/// <summary>
		///The number of times read() will return a space.
		/// </summary>
		protected int spaceCount;

		private static int LF = 10;
		private static int CR = 13;
		private static int SPACE = 32;
		private static int EQ = 61;


		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return true; } }
		public override long Position { get { return -1; } set { } }
		public override long Seek (long offset, SeekOrigin origin)
		{ return -1; }
		public override void SetLength (long value)
		{ }

		/// <summary>
		/// Create a new Quoted Printable Encoding stream.#
		/// @param stream Output stream#
		/// @param length Number of bytes per line
		/// </summary>
		public QPStream (Stream stream, int length)
			: base ()
		{
			this.stream = stream;
			this.bytesPerLine = length;
			this.count = 0;
			this.gotSpace = false;
			this.gotCR = false;
			// Read
			buf = new char[2];
			unreadList = new List<int> ();
		} // QPEncoderStream()

		/// <summary>
		///Create a new Quoted Printable Encoding stream with
		///the default 76 bytes per line.
		///@param stream Output stream
		/// </summary>
		public QPStream (Stream stream)
			: this (stream, 76)
		{
		} // QPEWncoderStream()


		//-------------------------------------------------------------
		// Methods ----------------------------------------------------
		//-------------------------------------------------------------

		/// <summary>
		///Flush encoding buffer.
		///@exception IOException IO Exception occurred
		/// </summary>
		//throws IOException 
		public override void Flush ()
		{
			if (gotSpace) {
				output (0x20, false);
				gotSpace = false;
			}
			stream.Flush ();
		} // flush()

		/// <summary>
		///Write bytes to encoding stream.
		///@param bytes Byte array to read values from
		///@param offset Offset to start reading bytes from
		///@param length Number of bytes to read
		///@exception IOException IO Exception occurred
		/// </summary>
		//throws IOException 
		public override void Write (byte[] bytes, int offset, int length)
		{

			// Variables
			int index;

			// Process Each Byte
			for (index = offset; index < length; index++) {
				WriteByte (bytes[index]);
			} // for

		} // write()

		/// <summary>
		///Write bytes to stream.
		///@param bytes Byte array to write to stream
		///@exception IOException IO Exception occurred
		/// </summary>
		//throws IOException 
		public void Write (byte[] bytes)
		{
			Write (bytes, 0, bytes.Length);
		} // write()

		/// <summary>
		///Write a byte to the stream.
		///@param b Byte to write to the stream
		///@exception IOException IO Exception occurred
		/// </summary>
		//throws IOException 
		public override void WriteByte (byte b)
		{
			b &= 0xff;
			if (gotSpace) {
				if (b == '\n' || b == '\r')
					output ((byte)' ', true);
				else
					output ((byte)' ', false);
				gotSpace = false;
			}
			if (b == ' ')
				gotSpace = true;
			else if (b == '\r') {
				gotCR = true;
				outputCRLF ();
			} else if (b == '\n') {
				if (gotCR)
					gotCR = false;
				else
					outputCRLF ();
			} else {
				if (b < ' ' || b >= 127 || b == '=')
					output (b, true);
				else
					output (b, false);
			}
		} // write()

		/// <summary>
		///Close stream.
		///@exception IOException IO Exception occurred
		/// </summary>
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

			base.Dispose (disposing);
		}

		~QPStream ()
		{
			Dispose (false);
		}

		/// <summary>
		///????
		///@param b ??
		///@param value ??
		///@exception IOException IO Exception occurred
		/// </summary>
		//throws IOException 
		protected void output (byte b, bool value)
		{
			if (value) {
				if ((count += 3) > bytesPerLine) {
					stream.WriteByte ((byte)'=');
					stream.WriteByte ((byte)'\r');
					stream.WriteByte ((byte)'\n');
					count = 3;
				}
				stream.WriteByte ((byte)'=');
				stream.WriteByte ((byte)hex[b >> 4]);
				stream.WriteByte ((byte)hex[b & 0xf]);
			} else {
				if (++count > bytesPerLine) {
					stream.WriteByte ((byte)'=');
					stream.WriteByte ((byte)'\r');
					stream.WriteByte ((byte)'\n');
					count = 1;
				}
				stream.WriteByte (b);
			}
		} // output()

		/// <summary>
		///Write CRLF byte series to stream.
		///@exception IOException IO Exception occurred
		/// </summary>
		//throws IOException 
		private void outputCRLF ()
		{
			stream.WriteByte ((byte)'\r');
			stream.WriteByte ((byte)'\n');
			count = 0;
		} // outputCRLF()


		//-------------------------------------------
		// Read
		//-------------------------------------------
		protected int ReadByteUnread ()
		{
			if (unreadList.Count > 0) {
				int buff = unreadList[unreadList.Count - 1];
				unreadList.RemoveAt (unreadList.Count - 1);
				return buff;
			} else {
				return stream.ReadByte ();
			}
		}

		protected void UnreadByte (int b)
		{
			unreadList.Add (b);
		}

		/// <summary>
		///Read a character from the stream.
		/// </summary>

		// throws IOException
		public override int ReadByte ()
		{
			if (spaceCount > 0) {
				spaceCount--;
				return SPACE;
			}

			int c = ReadByteUnread ();
			if (c == SPACE) {
				while ((c = ReadByteUnread ()) == SPACE)
					spaceCount++;
				if (c == LF || c == CR || c == -1)
					spaceCount = 0;
				else {
					UnreadByte (c);
					c = SPACE;
				}
				return c;
			}
			if (c == EQ) {
				int c2 = ReadByteUnread (); // im Java stand hier super.in.read(); Ich weiß aber nicht was das soll, da super.in und in auf dasselbe Objekt verweisen, oder ?!?
				if (c2 == LF)
					return ReadByte (); // im Java stand hier read, nicht in.read
				if (c2 == CR) {
					int peek = ReadByteUnread ();
					if (peek != LF)
						UnreadByte (peek);
					return ReadByte (); // im Java stand hier read, nicht in.read
				}
				if (c2 == -1)
					return c2;

				buf[0] = (char)c2;
				buf[1] = (char)ReadByteUnread ();
				try {
					return Convert.ToInt32 (new String (buf, 0, 2), 16);
				}
				catch (FormatException) {
					UnreadByte (buf[1]);
					UnreadByte (buf[0]);
				}
				catch (OverflowException) {
					UnreadByte (buf[1]);
					UnreadByte (buf[0]);
				}
				catch (ArgumentException) {
					UnreadByte (buf[1]);
					UnreadByte (buf[0]);
				}
				return c;
			} else
				return c;
		}

		/// <summary>
		///Reads from the underlying stream into the specified byte array.
		/// </summary>
		//     throws IOException
		public override int Read (byte[] bytes, int off, int len)
		{
			int pos = 0;
			try {
				while (pos < len) {
					int c = ReadByte ();
					if (c == -1) {
						if (pos == 0)
							pos = -1;
						break;
					}
					bytes[off + pos] = (byte)c;
					pos++;
				}

			}
			catch (IOException) {
				pos = -1;
			}
			return pos;
		}

		/// <summary>
		///Returns the number of bytes that can be read without blocking.
		/// </summary>
		//throws IOException
		public override long Length {
			get { return stream.Length; }
		}


	}
}
