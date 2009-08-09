//
// openmapi.org - NMapi C# Mapi API - Misc.cs
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

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;

namespace NMapi.Utility {
	
	/// <summary>
	///  
	/// </summary>
	public static class Misc
	{
		
		/// <summary>
		///  
		/// </summary>
		public static byte[] ConcatBytes (byte[] data1, byte[] data2)
		{
			byte[] result = new byte [data1.Length + data2.Length];
			Array.Copy (data1, 0, result, 0, data1.Length);
			Array.Copy (data2, 0, result, data1.Length, data2.Length);
			return result;
		}
		
	}
	
}
