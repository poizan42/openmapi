//
// openmapi.org - NMapi C# Mapi API - NMapiGuid.cs
//
// Copyright 2008 Topalis AG
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
using System.Runtime.Serialization;
using System.IO;

using System.Diagnostics;
using CompactTeaSharp;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi {
	
	/// <summary>
	///  
	/// </summary>
	public partial class NMapiGuid : IComparable
	{
		public const int LENGTH = 16;
		
		public byte[] ToByteArray ()
		{
			byte[] result = new byte [LENGTH];
			byte[] part1 = BitConverter.GetBytes (Data1);
			Array.Copy (part1, 0, result, 0, 4);
			byte[] part2 = BitConverter.GetBytes (Data2);
			Array.Copy (part2, 0, result, 4, 2);
			byte[] part3 = BitConverter.GetBytes (Data3);
			Array.Copy (part3, 0, result, 6, 2);
			Array.Copy (Data4, 0, result, 8, 8);
			return result;
		}
		
		public long[] ToInt64Pair ()
		{
			byte[] bytes = ToByteArray ();
			long[] result = new long [2];
			result [0] = BitConverter.ToInt64 (bytes, 0);
			result [1] = BitConverter.ToInt64 (bytes, 8);
			return result;
		}

		public static NMapiGuid FromInt64Pair (long part1, long part2) 
		{
			byte[] part1Bytes = BitConverter.GetBytes (part1);
			byte[] part2Bytes = BitConverter.GetBytes (part2);			
			byte[] result = new byte [16];
			Array.Copy (part1Bytes, 0, result, 0, part1Bytes.Length);
			Array.Copy (part2Bytes, 0, result, 8, part2Bytes.Length);
			return new NMapiGuid (result);
		}
		
		public NMapiGuid (byte[] bytes) 
		{
			Data1 = BitConverter.ToInt32 (bytes, 0);
			Data2 = BitConverter.ToInt16 (bytes, 4);
			Data3 = BitConverter.ToInt16 (bytes, 6);
			Data4 = new byte [8];
			Array.Copy (bytes, 8, Data4, 0, 8);
		}
		
		/// <summary>
		///  
		/// </summary>
		public static NMapiGuid MakeNew ()
		{
			return new NMapiGuid (Guid.NewGuid ());
		}
		
		public NMapiGuid (Guid guid) : this (guid.ToByteArray ()) 
		{
		}
		
		/* Implements kind-of Value-Equality! */
		
		public bool Equals (NMapiGuid guid2)
		{
			if (guid2 == null)
				return false;

			if (Data1 != guid2.Data1 || Data2 != guid2.Data2 || Data3 != guid2.Data3)
				return false;
			
			if (Data4 == null && guid2.Data4 == null) // both null
				return true;
			
			if (Data4 != null && guid2.Data4 != null) { // both NOT null
				if (Data4.Length != guid2.Data4.Length)
					return false;
				for (int i=0;i < Data4.Length;i++)
					if (Data4 [i] != guid2.Data4 [i])
						return false;
				return true;
			}
			return false; // one is NOT null!
		}
		
		public override bool Equals (object o)
		{
			if (o == this)
				return true;
			NMapiGuid guid2 = o as NMapiGuid;
			if (guid2 == null)
				return false;
			return this.Equals (guid2);
		}
		
		/// <summary>
		///  
		/// </summary>
		public string ToHexString ()
		{
			return new SBinary (ToByteArray ()).ToHexString ();
		}

		public override int GetHashCode ()
		{
			/*
			byte[] flat = ToByteArray (); // TODO: This is VERY slow !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! -> Maybe we should save the byte[] instead of numbers and/or use the native MS Guid type.
			int hash = 0;
			if (flat != null) {
				for (int i = 0; i < flat.Length; i++)
					hash = 31 * hash + flat [i];
			}
			return hash;
			*/
			
			// algo from System.Guid.cs (Mono).
			int res;
			res = (int) Data1; 
			res = res ^ ((int) Data2 << 16 | Data3);
			res = res ^ ((int) Data4 [0] << 24);
			res = res ^ ((int) Data4 [1] << 16);
			res = res ^ ((int) Data4 [2] << 8);
			res = res ^ ((int) Data4 [3]);
			res = res ^ ((int) Data4 [4] << 24);
			res = res ^ ((int) Data4 [5] << 16);
			res = res ^ ((int) Data4 [6] << 8);
			res = res ^ ((int) Data4 [7]);
			return res;			
		}
		
		/// <summary>
		///  Implementation of the IComparable interface.
		/// </summary>
		public int CompareTo (object obj)
		{
			if (!(obj is NMapiGuid))
				throw new ArgumentException ("Not an NMapiGuid object.");
			if (Equals ((NMapiGuid) obj))
				return 0;
			return 1; // TODO: correct?
		}

		
	}

}
