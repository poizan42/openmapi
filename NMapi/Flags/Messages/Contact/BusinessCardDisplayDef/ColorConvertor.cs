//
// openmapi.org - NMapi C# Mapi API - ColorConvertor.cs
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
using System.Drawing;
using System.IO;

using NMapi;

namespace NMapi.Flags {

	/// <summary>
	///  Helper class to convert .NET System.Drawing.Color objects to 
	///  MAPI Color integers and vice versa.
	/// </summary>
	public static class ColorConvertor
	{

		// TODO: CORRECT ???

		/// <summary></summary>
		/// <param name=""></param>
		/// <returns></returns>
		public static int EncodeColor (Color color)
		{
			return (color.R) +  (color.G << 2)  + (color.B << 4);
		}


		// TODO: CORRECT ???

		/// <summary></summary>
		/// <param name=""></param>
		/// <returns></returns>
		public static Color DecodeColor (int data)
		{
			return Color.FromArgb (data);
		}

	}
	
	
	
}
