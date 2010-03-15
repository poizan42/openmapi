//
// openmapi.org - NMapi C# Mapi API - ExtraInfoBuffer.cs
//
// Copyright 2009 Topalis AG
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
using System.IO;
using System.Linq;

using NMapi;

namespace NMapi.Flags {
		
	/// <summary>
	///  The data stored in the ExtraInfoBuffer, as well as an array of 
	///  offsets into the byte array.
	/// </summary>
	public sealed class ExtraInfoBufferProcessed
	{
		private byte[] data;
		private int[] offsets;
			
		/// <summary>
		///  The data.
		/// </summary>
		public byte[] Data {
			get { return data; }
		}
		
		/// <summary>
		///  An array of offsets into the array. Each of them points to the 
		///  first character of an UTF-16-encoded, NULL-terminated string.
		/// </summary>
		public int[] Offsets {
			get { return offsets; }
		}
		
		internal ExtraInfoBufferProcessed (byte[] data, int[] offsets)
		{
			this.data = data;
			this.offsets = offsets;
		}
		
	}
	
	
	/// <summary>
	///  The TextFormat byte of the BusinessCardDisplayDefinition property. 
	/// </summary>
	/// <remarks>
	///  Note: This file is based on information published by Microsoft
	///        [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public sealed class ExtraInfoBuffer
	{
		/// <summary>
		///  
		/// </summary>
		public string[] LabelValues { get; set; }
		
		
		/// <summary>
		///  
		/// </summary>
		public ExtraInfoBuffer (string[] strings)
		{
			this.LabelValues = strings;
		}
		
		/// <summary>
		///  
		/// </summary>
		public ExtraInfoBuffer (byte[] data, int[] offsets)
		{
			
			
			// TODO
			
		}
		
		/// <summary>
		///  Concatenates the strings, storing them as an array of 
		///  bytes using UTF-16 encoding; Returns the data as well as an 
		///  array of offsets, each matching the offset of the string, 
		///  respectively.
		/// </summary>
		public ExtraInfoBufferProcessed Generate ()
		{
			int[] length = new int [LabelValues.Length];
			byte[][] data = new byte [LabelValues.Length] [];
			for (int i=0; i < LabelValues.Length; i++) {
				data [i] = Encoding.Unicode.GetBytes (LabelValues [i]);
				length [i] = data [i].Length + 1; // add NULL-Termination.
			}
			
			byte[] result = new byte [length.Sum ()];
			int position = 0;
			
			for (int i=0; i < data.Length; i++) {
				byte[] strBytes = data [i];
				Array.Copy (strBytes, 0, result, position, strBytes.Length);
				position += strBytes.Length + 1; // NULL-Termination.
			}
			
			return new ExtraInfoBufferProcessed (result, length);
		}
		
	}

	
}
