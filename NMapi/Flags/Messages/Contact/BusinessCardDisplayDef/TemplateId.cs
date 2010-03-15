//
// openmapi.org - NMapi C# Mapi API - TemplateId.cs
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
	///  
	/// </summary>
	/// <remarks>
	///  Note: This file is based on information published by Microsoft
	///        [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public enum TemplateId
	{
		/// <summary>
		///  
		/// </summary>
		Template_ImageLeft = 0,// The image will be left aligned, stretching the full height of the card vertically; text fields will appear to the right of the image.

		/// <summary>
		///  
		/// </summary>
		Template_ImageRight, // The image will be right aligned, stretching the full height of the card vertically; text fields will appear to the left of the image.

		/// <summary>
		///  
		/// </summary>
		Template_ImageTop, // The image will be aligned to the top, stretching the full width of the card horizontally; text fields will appear under the image.

		/// <summary>
		///  
		/// </summary>
		Template_ImageBottom, // The image will be aligned to the bottom, stretching the full width of the card horizontally; text fields will appear above the image

		/// <summary>
		///  
		/// </summary>
		Template_NoImage, // No image is included in the card, only text fields are included. PidLidBusinessCardCardPicture SHOULD NOT be set on the Contact object in this case.

		/// <summary>
		///  
		/// </summary>
		Template_BackgroundImage // The image will be used as a background for the card, stretching the full height and width of the card. Text fields are displayed on top of the image.
	}
	
	
}
