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

namespace NMapi.Utility {
	
	/// <summary></summary>
	public static class Misc
	{
		
		/// <summary></summary>
		/// <remarks>
		///  You may pass null-values.
		/// </remarks>
		/// <param name="data1"></param>
		/// <param name="data2"></param>
		/// <returns></returns>
		public static byte[] ConcatBytes (byte[] data1, byte[] data2)
		{
			int d1Length = (data1 != null) ? (data1.Length) : 0;
			int d2Length = (data2 != null) ? (data2.Length) : 0;
			byte[] result = new byte [d1Length + d2Length];
			if (d1Length != 0)
				Array.Copy (data1, 0, result, 0, d1Length);
			if (d2Length != 0)
				Array.Copy (data2, 0, result, d1Length, d2Length);
			return result;
		}
		
	}
	
}
