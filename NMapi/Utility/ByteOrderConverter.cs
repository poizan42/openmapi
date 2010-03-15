//
// openmapi.org - NMapi - ByteOrderConverter.cs
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
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace NMapi.Utility {
	
	/// <summary></summary>
	public enum ByteOrder
	{
		/// <summary></summary>
		BigEndian,
		
		/// <summary></summary>
		LittleEndian
	}
	
	/// <summary>Converts primitive types to byte arrays of the expected endianess.</summary>
	/// <remarks>
	///  .NET does not have a way to properly convert primitive types to 
	///  bytes that are expected in a certain byte order. This class 
	///  should provide a way to reliably convert the data accross platforms.
	/// </remarks>
	public static class ByteOrderConverter
	{

		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="outputOrder"></param>
		/// <returns></returns>
		public static byte[] GetBytes (short data, ByteOrder outputOrder)
		{
			return FixOrder (BitConverter.GetBytes (data), outputOrder);
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="outputOrder"></param>
		/// <returns></returns>
		public static byte[] GetBytes (int data, ByteOrder outputOrder)
		{
			return FixOrder (BitConverter.GetBytes (data), outputOrder);
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="outputOrder"></param>
		/// <returns></returns>
		public static byte[] GetBytes (long data, ByteOrder outputOrder)
		{
			return FixOrder (BitConverter.GetBytes (data), outputOrder);
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="outputOrder"></param>
		/// <returns></returns>
		public static byte[] GetBytes (ushort data, ByteOrder outputOrder)
		{
			return FixOrder (BitConverter.GetBytes (data), outputOrder);
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="outputOrder"></param>
		/// <returns></returns>
		public static byte[] GetBytes (uint data, ByteOrder outputOrder)
		{
			return FixOrder (BitConverter.GetBytes (data), outputOrder);
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="outputOrder"></param>
		/// <returns></returns>
		public static byte[] GetBytes (ulong data, ByteOrder outputOrder)
		{
			return FixOrder (BitConverter.GetBytes (data), outputOrder);
		}
		
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="inputOrder"></param>
		/// <returns></returns>
		public static int ToInt32 (byte[] data, ByteOrder inputOrder)
		{
			return ToInt32 (data, 0, inputOrder);
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <param name="inputOrder"></param>
		/// <returns></returns>
		public static int ToInt32 (byte[] data, int offset, ByteOrder inputOrder)
		{
			// TODO: verify parameters: if data != null, offset must be positive and < data.Length-SIZE_OF_TYPE_IN_BYTES
		
		
			byte[] fixedOrder = (ReadAndFixOrder (data, offset, inputOrder, 4));
			foreach (byte b in fixedOrder)
				Console.WriteLine (b.ToString ("x") + ", ");
			int converted = BitConverter.ToInt32 (fixedOrder, 0);
			Console.WriteLine ("= converted: " + converted.ToString ("x"));
			Console.WriteLine ("Little endian? : " + BitConverter.IsLittleEndian);
			return converted;
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="inputOrder"></param>
		/// <returns></returns>
		public static ushort ToUInt16 (byte[] data, ByteOrder inputOrder)
		{
			return ToUInt16 (data, 0, inputOrder);
		}
		
		/// <summary></summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <param name="inputOrder"></param>
		/// <returns></returns>
		public static ushort ToUInt16 (byte[] data, int offset, ByteOrder inputOrder)
		{
			// TODO: verify parameters: if data != null, offset must be positive and < data.Length-SIZE_OF_TYPE_IN_BYTES
			return BitConverter.ToUInt16 (ReadAndFixOrder (data, offset, inputOrder, 2), 0);
		}
		
		
		
		
		
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		
		private static byte[] ReadAndFixOrder (byte[] data, int offset, ByteOrder inputOrder, int dataTypeByteLength)
		{
			if (data == null)
				return null;
			byte[] buf = new byte [dataTypeByteLength];
			Array.Copy (data, offset, buf, 0, dataTypeByteLength);
			return FixOrder (buf, inputOrder);			
		}
		
		
		//input is bigendian
		
		
		
		private static byte[] FixOrder (byte[] data, ByteOrder outputOrder)
		{
			if (data == null)
				return null;
			bool reverse = ((BitConverter.IsLittleEndian && (outputOrder == ByteOrder.BigEndian)) ||
				(!BitConverter.IsLittleEndian && (outputOrder == ByteOrder.LittleEndian)));
			if (reverse)
				return data.Reverse ().ToArray ();
			return data;
		}

	}
	
}
