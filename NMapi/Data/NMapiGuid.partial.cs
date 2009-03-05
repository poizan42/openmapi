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
		
	public partial class NMapiGuid 
	{
		public byte[] ToByteArray ()
		{
			byte[] result = new byte [16];
			byte[] part1 = BitConverter.GetBytes (Data1);
			Array.Copy (part1, 0, result, 0, 4);
			byte[] part2 = BitConverter.GetBytes (Data2);
			Array.Copy (part2, 0, result, 4, 2);
			byte[] part3 = BitConverter.GetBytes (Data3);
			Array.Copy (part3, 0, result, 6, 2);
			Array.Copy (Data4, 0, result, 8, 8);
			return result;
		}

		public NMapiGuid (byte[] bytes) 
		{
			Data1 = BitConverter.ToInt32 (bytes, 0);
			Data2 = BitConverter.ToInt16 (bytes, 4);
			Data2 = BitConverter.ToInt16 (bytes, 6);
			Data4 = new byte [8];
			Array.Copy (bytes, 8, Data4, 0, 8);
		}
	}

}
