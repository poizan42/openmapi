//
// openmapi.org - NMapi C# Mapi API - FieldInfo.cs
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
using System.Diagnostics;

using NMapi;

namespace NMapi.Flags {
	
	/// <summary>
	///  Represents the FieldInfo data that is stored as part of the 
	///  BusinessCardDisplayDefinition binary property.
	/// </summary>
	/// <remarks>
	///  Note: This file is based on information published by Microsoft
	///        [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public sealed class FieldInfo
	{
		/// <summary>The Mapi Property Id the FieldInfo applies to.</summary>
		public short TextPropertyId { get; set; }

		/// <summary>Format of the text.</summary>		
		public TextFormat TextFormat { get; set; }

		/// <summary>Format of the label.</summary>
		public LabelFormat LabelFormat  { get; set; }
		
		/// <summary>The font size in points. (MUST be &gt;= 3 and &lt;= 32)</summary>
		public int FontSize { get; set; }
		
// TODO!
		/// <summary>Byte offset into extra byte information.</summary>
		public short LabelOffset  { get; set; }

		/// <summary>The color of the value.</summary>
		public Color ValueFontColor { get; set; }
		
		/// <summary>The color of the label.</summary>		
		public Color LabelFontColor { get; set; }
		
		
		/// <summary>
		///  
		/// </summary>
		public FieldInfo ()
		{
		}
		
		/// <summary>
		/// 
		/// </summary>
		public FieldInfo (byte[] data)
		{
			Debug.Assert (data.Length == 16);
			
			TextPropertyId = BitConverter.ToInt16 (data, 0);
			
			TextFormat = new TextFormat (data [2]);
			LabelFormat = new LabelFormat (data [3]);
			FontSize = Convert.ToInt32 (data [4]);
			LabelOffset = BitConverter.ToInt16 (data, 6);
			ValueFontColor = ColorConvertor.DecodeColor (BitConverter.ToInt32 (data, 8));
			LabelFontColor = ColorConvertor.DecodeColor (BitConverter.ToInt32 (data, 12));
		}
		
		/// <summary>
		/// 
		/// </summary>
		public byte[] Generate ()
		{
			byte[] result = new byte [16];

			byte[] tmp = BitConverter.GetBytes (TextPropertyId);
			result [0] = tmp [0];
			result [1] = tmp [1];	// correct order?
			
			result [2] = TextFormat.Generate ();
			result [3] = LabelFormat.Generate ();
			result [4] = (byte) FontSize;
			result [5] = 0; // reserved

			tmp = BitConverter.GetBytes (LabelOffset);
			result [6] = tmp [0];
			result [7] = tmp [1];	// correct order?

			int fColor = ColorConvertor.EncodeColor (ValueFontColor);
			tmp = BitConverter.GetBytes (fColor);
			Array.Copy (tmp, 0, result, 8, 4);

			int lColor = ColorConvertor.EncodeColor (LabelFontColor);
			tmp = BitConverter.GetBytes (lColor);
			Array.Copy (tmp, 0, result, 12, 4);
			
			return result;
		}
		
		
	}

	
}
