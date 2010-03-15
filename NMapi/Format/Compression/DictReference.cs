//
// openmapi.org - NMapi - DictReference.cs
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


//
// TODO: We need to review all of our code for issues with endianness!
//


	/// <summary></summary>
	/// <remarks>
	///  
	/// </remarks>
	internal struct DictReference
	{
//		private const int BITS16 = 16;
//		private ushort data;



		/// <summary>An offset into the Dictionary.</summary>
		public ushort Offset {
			get; set; 
		} 
		
		/// <summary>The Length of the data in the dictionary.</summary>
		public ushort Length {
			get; set; 
		}

/*

		/// <summary>An offset into the Dictionary.</summary>
		public ushort Offset {
			get { return Convert.ToUInt16 ((data & 0xFFF0) >> 4); }
			set {
				if (value < 0 || value > 0xFFFF)
					throw new ArgumentOutOfRangeException ("Offset");
			}
		} 
		
		/// <summary>The Length of the data in the dictionary.</summary>
		public ushort Length {
			get { return Convert.ToUInt16 ((data & 0xF) + 2); }
			set {
				if (value < 0 || value > 0xFFFF)
					throw new ArgumentOutOfRangeException ("Length");
				data = Convert.ToUInt16 (;
			}
		}
		
		*/
		
		private ushort EncodeData ()
		{
			int correctedLength = Length-2;
			if (correctedLength < 0)
				correctedLength = 0;
			return Convert.ToUInt16 ((Offset << 4) | (0x000F & correctedLength));
		}
		
		
		/// <summary></summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public void WriteTo (byte[] buffer, int offset)
		{
			if (buffer == null)
				throw new ArgumentException ("buffer");
			if (offset < 0)
				throw new ArgumentException ("offset");
			if (offset+2 > buffer.Length)
				throw new IndexOutOfRangeException ("buffer");

			ushort data = EncodeData ();
			byte[] dataBytes = ByteOrderConverter.GetBytes (data, ByteOrder.BigEndian); // TODO: byte-order correct?
			Array.Copy (dataBytes, 0, buffer, offset, dataBytes.Length);
		}
		
		/// <summary></summary>
		/// <returns></returns>
		public byte[] Encode ()
		{
			byte[] result = new byte [2];
			WriteTo (result, 0);
			return result;
		}
		
		
		/// <summary></summary>
		/// <param name="input"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		private static DictReference Decode (byte[] buffer, int offset)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			if (offset < 0 || buffer.Length-offset < 2)
				throw new IndexOutOfRangeException ("offset");
			
			ushort val = ByteOrderConverter.ToUInt16 (buffer, offset, ByteOrder.BigEndian); // TODO: byte-order correct?

			DictReference result = new DictReference ();
			result.Offset = Convert.ToUInt16 (val >> 4);
			result.Length = Convert.ToUInt16 ((val & 0x000F) + 2);
			return result;
		}
		
		/// <summary></summary>
		/// <param name="input"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static DictReference Decode (Stream input)
		{
			if (input == null)
				throw new ArgumentNullException ("input");
			if (!input.CanRead)
				throw new ArgumentException ("Can't read from stream 'input'.");
			
			byte[] buf = new byte [2];
			int countRead = input.Read (buf, 0, 2);
			if (countRead != 2)
				throw new Exception ("could not read 2 bytes from stream.");
			return Decode (buf, 0);
		}
		
		
		/*
		public override string ToString ()
		{
			StringBuilder bitsBuilder = new StringBuilder ();
			for (int i=0; i<BITS16; i++)
				bitsBuilder.Append ((data >> (BITS16-1-i)) & 0x1);
			
			return "{DictReference: " + bitsBuilder.ToString () + "}";
		}
*/		
	}	

}
