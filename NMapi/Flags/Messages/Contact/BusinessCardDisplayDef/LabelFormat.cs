//
// openmapi.org - NMapi C# Mapi API - LabelFormat.cs
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
using System.IO;

using NMapi;

namespace NMapi.Flags {
	
	/// <summary>
	///  The LabelFormat byte of the BusinessCardDisplayDefinition property. 
	/// </summary>
	/// <remarks>
	///  Note: This file is based on information published by Microsoft
	///        [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public sealed class LabelFormat
	{
		private int rightToLeftReadingOrderMask = 0x04;
		private int labelToTheRightMask = 0x02;
		private int labelToTheLeftMask = 0x01;

		/// <summary>
		///  
		/// </summary>
		public LabelFormatAlignment Alignment { get; set; }

		/// <summary>
		///  
		/// </summary>
		public bool RightToLeftReadingOrder { get; set; }

		/// <summary>
		///  
		/// </summary>
		public LabelFormat (byte value)
		{
			if ((value & labelToTheRightMask) != 0)
				Alignment = LabelFormatAlignment.Right;

			if ((value & labelToTheLeftMask) != 0)
				Alignment = LabelFormatAlignment.Left;

			RightToLeftReadingOrder = (value & rightToLeftReadingOrderMask) != 0;
		}
		
		/// <summary>
		///  
		/// </summary>
		public byte Generate ()
		{
			int value = ((Alignment == LabelFormatAlignment.Left) ? labelToTheLeftMask : 0) | 
				((Alignment == LabelFormatAlignment.Right) ? labelToTheRightMask : 0) | 
				((RightToLeftReadingOrder) ? rightToLeftReadingOrderMask : 0);
			return (byte) value;
		}
		
		

		/// <summary>
		///  Checks if the label exists.
		/// </summary>
		public static bool HasLabel (byte value)
		{
			return (value & 0xFFF) != 0;
		}

		
	}

	
}
