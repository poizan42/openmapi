//
// openmapi.org - NMapi - CompressionDictionary.cs
//
// Copyright 2010 Topalis AG
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
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace NMapi.Format.Compression {

	/// <summary></summary>
	/// <remarks>
	/// <para>
	///  The explanation below also serves as a spec.
	/// </para>
	/// <para>
	///  
	///  In order to make the code clear, this informal proof is provided, that 
	///  proofs that the decompression recovers the original data.
	///  
	///  The compression algorithm works, because the following loop invariants hold:
	///  
	///   * The write cursor continously writes the original data at the cursor position, 
	///     extending the virtual size of the array (the end cursor) and wrapping around 
	///     when the buffer is full. This holds true for compression and decompression.
	///
	///   * When decompressing the dictionary ALWAYS is in EXACTLY the same state 
	///     as it was at the point when compressing. This is ensured by writing the 
	///     uncompressed input to the dictionary and, when compressed stuff is encountered, 
	///     uncompressing it and writing it into the dictionary as well!
	///
	///   * The read cursor is required only for decompression. It can point to any 
	///     data that currently exists in the buffer. Therefore the compressed data 
	///     can basically always reference blocks (maximum 16 bytes) that have been part 
	///     of the original data.
	///
	///   * The compressed bits always consists of a tuple (offset, length) of data 
	///     to be extracted from the dictionary. If the offset points to the write-cursor 
	///     this has a SPECIAL meaning, indicating completion of the decompression. 
	///
	///     Proof that the case can't occur during regular operation:
	///
	///      There are two cases:
	///        * The writer has not yet reached the end of the buffer.
	///          In that case the end-pointer is equal to the writer offset 
	///          and no (valid) data has been written after the write offset, yet.
	///          Therefore no read offset can point there, because there is nothing to be pointed to.
	///
	///        * The writer has reached the end of the buffer and wrapped around.
	///          In this case there may be valid data after the cursor. 
	///          To ensure that no reference is created, a special scan rule has to be introduced.
	///   
	///   * The dictionary must be scanned 
	///      - from the beginning to the (unchanging during the op) virtual end if the buffer is not full.
	///      - or from (the writer offset + 1) wrapping around.
	///      This ensures that the writer offset can never be a valid reference, 
	///      which is necessary, because it indicates the end of compression.
	///
	///   * Another way to think about this is:
	///
	///      writer writes:
	///
	///      abc..............................
	///      .................................
	///      .................................
	///      ..............(END)
	///      
	///      Any data from the buffer can be referenced.
	/// 
	///              read
	///               |
	///         +-----|---------------------+
	///         |     |                     |
	///         |     v                     |
	///         | VWXYZABC                  |
	///         |                           |
	///         |                           |
	///  write -----+                       |
	///         |   |                       |
	///         |   v                       |
	///         | MNZABC                    |
	///         |                           |
	///         |          <----------------|--- end
	///         |                           |
	///         |                           |
	///         +---------------------------+
	///   
	/// </para>
	/// </remarks>
	internal class CompressionDictionary
	{
		
		
		// TODO: 
		// This is a little strange. Porbably it makes more sense for the logic to 
		// go into the dictionary, not the offset.
		
		/// <summary>A custom short implementation that can be wrapped automatically.</summary>
		internal struct WrappingShort
		{
			private const int WRAP_AT = CompressionDictionary.DICTIONARY_SIZE;
			private ushort offset;
			
			public WrappingShort (ushort offset)
			{
				if (offset > WRAP_AT)
					throw new Exception ("Invalid offset!");
				this.offset = offset;
			}
			
			public static implicit operator WrappingShort (ushort num)
			{
				return new WrappingShort (num);
			}
			
			public static implicit operator ushort (WrappingShort c)
			{
				return c.offset;
			}
			
			public static WrappingShort operator + (WrappingShort c1, WrappingShort c2) 
			{
				ushort wrapped = Convert.ToUInt16 ((c1.offset+c2.offset) % WRAP_AT);
				return new WrappingShort (wrapped);
			}
			
			public static WrappingShort operator + (WrappingShort c, ushort num) 
			{
				ushort wrapped = Convert.ToUInt16 ((c.offset+num) % WRAP_AT);
				return new WrappingShort (wrapped);
			}
			
			public static WrappingShort operator ++ (WrappingShort c) 
			{
				c.offset = (ushort) ((c.offset+1) % WRAP_AT);
				return c;
			}
			
			public static bool operator == (WrappingShort a, WrappingShort b)
			{
				return (a.offset == b.offset);
			}
			
			public static bool operator != (WrappingShort a, WrappingShort b)
			{
			    return !(a == b);
			}
			
			public override bool Equals (object obj)
			{
				if (obj is WrappingShort)
					return this == (WrappingShort) obj;
				return false;
			}

			public override int GetHashCode ()
			{
				return offset.GetHashCode ();
			}
			
			public override string ToString ()
			{
				return "" + offset;
			}
		}
		
		
		
		private const int DICTIONARY_SIZE = 4096;
		private const int MAX_MATCH_LENGTH = 16;
		
		// just in case something gets messed up: 
		// The md5 of the content of the string should be: 
		// 5075283f24d3fae3623cd85db003f7a0
		private const string DEFAULT_INIT_STRING = 
			"{\\rtf1\\ansi\\mac\\deff0\\deftab720{\\fonttbl;}" +				// TODO: type off spec again.
			"{\\f0\\fnil \\froman \\fswiss \\fmodern \\fscript " +
			"\\fdecor MS Sans SerifSymbolArialTimes New RomanCourier" +
			"{\\colortbl\\red0\\green0\\blue0\r\n\\par " +		// TODO: \r\n might be the other way around. not sure.
			"\\pard\\plain\\f0\\fs20\\b\\i\\u\\tab\\tx";


		private static Encoding defaultEncoding = Encoding.ASCII;

		private Encoding encoding;
		private byte[] dictionary = new byte [DICTIONARY_SIZE]; // NOTE: the size is fixed!
		private ushort realLength;		
		private WrappingShort writeOffset;

		internal WrappingShort WriteOffset {
			get { return writeOffset; }
		}

		internal bool BufferIsFull {
			get { return realLength == dictionary.Length; }
		}
		
		internal void WriteByte (byte b)
		{
			dictionary [writeOffset++] = b;
			if (!BufferIsFull)
				realLength++;
		}
		
		internal void Write (byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			Write (buffer, 0, buffer.Length);
		}
		
		internal void Write (byte[] buffer, int offset, int length)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			if (offset < 0)
				throw new IndexOutOfRangeException ("offset");
			// TODO: can probably be optimized!
			int writeLength = Math.Min (buffer.Length-offset, length);
			for (int i=0; i<writeLength; i++)
				WriteByte (buffer [i]);
		}
		
		internal byte Read (WrappingShort offset)
		{
			return dictionary [offset];
		}
		
		internal byte[] Read (WrappingShort offset, int length)
		{
			byte[] data = new byte [length];
			for (int i=0; i<length; i++)
				data [i] = dictionary [new WrappingShort (offset+(ushort)i)];
			return data;
		}
		
		
		
		
		
		
		
		
		
		
		
		//
		// **************** Performance critical stuff ******************
		//
		
		
		
		internal DictReference? Scan (RewindableBufferedStream input, out int bytesWritten, out byte theResultingByteIfAny)
		{
			WrappingShort origWritePos = writeOffset;
			WrappingShort matchFromPos = new WrappingShort (0);
			if (BufferIsFull)
				matchFromPos = writeOffset + (ushort) 1;

			DictReference best = new DictReference ();
			best.Offset = 0;
			best.Length = 0;
			
			bytesWritten = 0;


			// stub.
			theResultingByteIfAny = 0;
			
			// TODO: length must be > 2!


			// TODO: performance!
			// why does this run so slow if NO mATCHES are possible?
			
			
			try {
				input.SetMarker ();
				
//				int i=matchFromPos;
				// matchFromPos
//				int copyOrig = origWritePos;
				
				int bLengthCopy = best.Length;
				
				while (matchFromPos != origWritePos && bLengthCopy <= MAX_MATCH_LENGTH) {				

					DictReference drNew = Match (best.Length, matchFromPos, input);
					if (drNew.Length > best.Length)
						best = drNew;

					// NOTE: each character is matched with other 4000 potential starts.
					
					// -> we need to get the first few characters, and put them into an array to allow quick comparisons.

// EXPENSIVE!
					input.ResetToMarker ();

					matchFromPos++;
//					i = (i+1) % 4096;
				}
				
			}
			 finally {
				input.DeleteMarker ();
			}
			
			
			bytesWritten = best.Length;
			
			// no match! - we must write the byte(s)? ourselfes.
			// - we must _ALSO_ return it, though.
			if (best.Length < 2) {
				int b = input.ReadByte ();
				if (b != -1) {
					// TODO: this code is difficult to understand !!! improve!
					if (best.Length == 0)
						WriteByte ((byte) b);
					theResultingByteIfAny = (byte) b;
					bytesWritten = 1; // TODO: RETURN THE BYTE PROPERLY !!!!!!!!!!!!!!
//					Console.WriteLine ("Wrote byte " + ((byte) b) + "!");
				}
				return null;
			} else {
				
				
				// TODO: stream ablesen / leeren.!!!!!!!!!!!!!!!!! return properly!
				for (int i=0; i<best.Length; i++)
					input.ReadByte ();
					
			}
			return best;
		}
		
		private DictReference Match (ushort origLength___, WrappingShort matchFromPos, RewindableBufferedStream stream)
		{
			DictReference dr = new DictReference ();
			dr.Offset = matchFromPos;
			dr.Length = 0;

			int input = -1;

			while ((dr.Length <= MAX_MATCH_LENGTH) && (input = stream.ReadByte ()) != -1) {
				byte inputByte = (byte) input;
//				Console.WriteLine ("RAW INPUT: " + inputByte);
				WrappingShort index = (matchFromPos + dr.Length);
				if (dictionary [index] != inputByte)
					break;

// NOTE: something is wrong with this!

				dr.Length++;
// && dr.Length >= 2
				if (dr.Length > origLength___ ) { // write the new matched characters to dictionary.
					WriteByte (inputByte);
//					Console.WriteLine ("Wrote a (compressed) byte " + ((byte) inputByte) + "!");
				}
			}

			return dr;
		}
		
		
		
		//
		// **************** END performance critical stuff ******************
		//
		
		
		
		
		
		
		
		
		public CompressionDictionary () : this (DEFAULT_INIT_STRING)
		{
		}
		
		public CompressionDictionary (string initialization) : this (initialization, defaultEncoding)
		{
		}

		public CompressionDictionary (Encoding encoding) : this (null, encoding)
		{
		}
		
		public CompressionDictionary (string initialization, Encoding encoding)
		{
			this.encoding = encoding;
			byte[] bytes = encoding.GetBytes (initialization);
			InitWithBytes (bytes);
		}
		
		public CompressionDictionary (byte[] initialization) // TODO ???????
		{
			this.encoding = defaultEncoding;
			InitWithBytes (initialization);
		}
		
		private void InitWithBytes (byte[] initialization)
		{

// - TODO: enable again, when we are done with debugging.
// - ensure that this is not too easily overwritten.

		
			if (initialization != null) {
				if (initialization.Length > DICTIONARY_SIZE)
					throw new ArgumentException ("initialization must be smaller than " + DICTIONARY_SIZE + ".");
				Array.Copy (initialization, 0, dictionary, 0, initialization.Length);
				realLength = Convert.ToUInt16 (initialization.Length);
				writeOffset = realLength;
			}
		}
		
	}

}
