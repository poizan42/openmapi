//
// openmapi.org - NMapi C# Mapi API - TextFormat.cs
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
	///  The TextFormat byte of the BusinessCardDisplayDefinition property. 
	/// </summary>
	/// <remarks>
	///  Note: This file is based on information published by Microsoft
	///        [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public sealed class TextFormat
	{
		private int centerAlignMask = 0x0B;
		private int rightAlignMask = 0x0A;
		private int underLineMask = 0x08;
		private int italicMask = 0x04;
		private int boldMask = 0x02;
		private int multiLineMask = 0x01;

		/// <summary>
		///  The alignment of the text.
		/// </summary>
		public TextFormatAlignment Alignment { get; set; }

		/// <summary>
		///  The text should be formatted underlined.
		/// </summary>
		public bool Underline { get; set; }

		/// <summary>
		///  The text should be formatted italic.
		/// </summary>
		public bool Italic { get; set; }

		/// <summary>
		///  The text should be formatted bold.
		/// </summary>
		public bool Bold { get; set; }

		/// <summary>
		///  
		/// </summary>
		public bool MultiLine { get; set; }


		/// <summary>
		///  
		/// </summary>
		public TextFormat (byte value)
		{
			if ((value & centerAlignMask) != 0)
				Alignment = TextFormatAlignment.Center;
				
			if ((value & rightAlignMask) != 0)
				Alignment = TextFormatAlignment.Right;

			Underline = (value & underLineMask) != 0;
			Italic = (value & underLineMask) != 0;
			Bold = (value & underLineMask) != 0;
			MultiLine = (value & underLineMask) != 0;
		}
		
		/// <summary>
		///  
		/// </summary>
		public byte Generate ()
		{
			int value = ((Alignment == TextFormatAlignment.Center) ? centerAlignMask : 0) | 
				((Alignment == TextFormatAlignment.Right) ? rightAlignMask : 0) | 
				((Underline) ? underLineMask : 0) | 
				((Italic) ? italicMask : 0) | 
				((Bold) ? boldMask : 0) | 
				((MultiLine) ? multiLineMask : 0);
			return (byte) value;
		}
		
	}

	
}
