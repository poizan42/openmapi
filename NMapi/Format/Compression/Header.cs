//
// openmapi.org - NMapi - Header.cs
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

using NMapi.Utility;

namespace NMapi.Format.Compression {

	/// <summary>Represents a CRTF Compression Header.</summary>
	/// <remarks></remarks>
	public sealed class Header : ICloneable
	{
		private const int COMPRESSED_MAGIC_NUMBER = 0x75465a4C;
		private const int UNCOMPRESSED_MAGIC_NUMBER = 0x414C454D;

		public const int LENGTH = 16;

		private CompressionType compressionType;
		private int rawSize;
		private int compressedSize;
		private int crc;

		/// <summary></summary>
		public CompressionType CompressionType {
			get { return compressionType; }
		}

		/// <summary></summary>
		public int RawSize {
			get { return rawSize; }
		}
		
		/// <summary></summary>
		public int CompressedSize { // value: size of content + "rest of header". (= 16-4 = 12)
			get { return compressedSize; }
		}

		/// <summary></summary>
		public int Crc {
			get { return crc; }
		}
		
		
		public Header () : this (CompressionType.Uncompressed)
		{
		}
		
		public Header (CompressionType compType) : this (compType, 0)
		{
		}

		public Header (CompressionType compType, int rawSize) : this (compType, rawSize, 0)
		{
		}
		
		public Header (CompressionType compType, int rawSize, int compressedSize) : this (compType, rawSize, compressedSize, 0)
		{
		}
		
		public Header (CompressionType compType, int rawSize, int compressedSize, int crc)
		{
			this.compressionType = compType;
			this.rawSize = rawSize;
			this.compressedSize = compressedSize;
			this.crc = crc;
		}
		
		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder ();
			builder.Append ("{Header: ");
			builder.Append ("CompressionType=0x");
			builder.Append (compressionType.ToString ("x"));
			builder.Append (", RawSize=0x");
			builder.Append (rawSize.ToString ("x"));
			builder.Append (", CompressedSize=0x");
			builder.Append (compressedSize.ToString ("x"));
			builder.Append (", Crc=0x");
			builder.Append (crc.ToString ("x"));
			builder.Append ("}");
			return builder.ToString ();
		}
		
		/// <summary></summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static Header DecodeFromStream (Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException ("stream");
			if (stream.CanRead == false)
				throw new ArgumentException ("stream is not readable.");
			byte[] buffer = new byte [LENGTH];
			
			int countRead = stream.Read (buffer, 0, LENGTH);
			if (countRead != LENGTH)
				throw new Exception ("Stream ended unexpectedly.");
			return DecodeFromBytes (buffer);
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static Header DecodeFromBytes (byte[] data, int offset)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			if (offset < 0 || data.Length-offset < LENGTH)
				throw new IndexOutOfRangeException ("offset");

			Header result = new Header ();
			result.compressedSize = ByteOrderConverter.ToInt32 (data, offset, ByteOrder.LittleEndian);
			result.rawSize = ByteOrderConverter.ToInt32 (data, offset+4, ByteOrder.LittleEndian);

			int magicNumber = ByteOrderConverter.ToInt32 (data, offset+8, ByteOrder.LittleEndian); // why little endian here?
			switch (magicNumber) {
				case COMPRESSED_MAGIC_NUMBER: result.compressionType = CompressionType.Compressed; break;
				case UNCOMPRESSED_MAGIC_NUMBER: result.compressionType = CompressionType.Uncompressed; break;
				default:
					throw new Exception ("unknown compression type!");
			}

			result.crc = ByteOrderConverter.ToInt32 (data, offset+12, ByteOrder.LittleEndian);

			bool isUncompressed = (result.CompressionType == CompressionType.Uncompressed);
			if (isUncompressed && result.crc != 0)
				throw new Exception ("Invalid CRC in  header. 0 expected.");

			return result;
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static Header DecodeFromBytes (byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
 			if (data.Length < LENGTH)
				throw new ArgumentException ("buffer 'data' is to small.");
			return DecodeFromBytes (data, 0);
		}

		/// <summary></summary>
		/// <returns></returns>
		public byte[] EncodeBytes ()
		{
			if (compressionType == CompressionType.Uncompressed && crc != 0)
				throw new Exception ("Invalid CRC!");
			
			byte[] result = new byte [LENGTH];
			
			byte[] compSizeBytes = ByteOrderConverter.GetBytes (compressedSize, ByteOrder.LittleEndian);
			Array.Copy (compSizeBytes, 0, result, 0, 4);

			byte[] rawSizeBytes = ByteOrderConverter.GetBytes (rawSize, ByteOrder.LittleEndian);
			Array.Copy (rawSizeBytes, 0, result, 4, 4);

			// Compression Type.
			switch (compressionType) {
				case CompressionType.Compressed: // TODO: generate from constant!
					// magic
					result [8] = 0x4c;
					result [9] = 0x5a;
					result [10] = 0x46;
					result [11] = 0x75;
				break;
				case CompressionType.Uncompressed:
					// magic
					result [8] = 0x4d;
					result [9] = 0x45;
					result [10] = 0x4c;
					result [11] = 0x41;
				break;
				default: throw new Exception ("Unknown compression type!"); break;
			}

			byte[] crcBytes = ByteOrderConverter.GetBytes (crc, ByteOrder.LittleEndian);
			Array.Copy (crcBytes, 0, result, 12, 4);

			return result;
		}
		
		/// <summary></summary>
		public object Clone ()
		{
			return MemberwiseClone ();
		}

		/// <summary></summary>
		/// <param name="stream"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void EncodeToStream (Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException ("stream");

			byte[] result = EncodeBytes ();
			stream.Write (result, 0, result.Length);
		}

	}

}
