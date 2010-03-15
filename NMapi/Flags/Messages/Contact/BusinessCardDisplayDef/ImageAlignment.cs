//
// openmapi.org - NMapi C# Mapi API - ImageAlignment.cs
//
// Copyright 2009-2010 Topalis AG
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

	/// <summary>Values for ImageAlignment.</summary>
	/// <remarks>
	///  Note: This file is based on information published by Microsoft
	///        [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public enum ImageAlignment
	{
		
		/// <summary></summary>
		Fit = 0,

		/// <summary></summary>
		TopLeft,

		/// <summary></summary>
		TopCenter,

		/// <summary></summary>
		TopRight,

		/// <summary></summary>
		MiddleLeft,

		/// <summary></summary>
		MiddleCenter,

		/// <summary></summary>
		MiddleRight,

		/// <summary></summary>
		BottomLeft,

		/// <summary></summary>
		BottomCenter,

		/// <summary></summary>
		BottomRight	
	}
	
}
