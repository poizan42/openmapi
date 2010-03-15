//
// openmapi.org - NMapi C# Mapi API - BusinessCardDisplayDefinition.cs
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
using System.Drawing;
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
	public class BusinessCardDisplayDefinitionFormat
	{
		/// <summary>
		///  
		/// </summary>
		public const int MAJOR_VERSION_MINIMUM_VALUE = 3;

		/// <summary>
		///  
		/// </summary>
		public const int MINOR_VERSION_RECOMMENDED_VALUE = 0;
		
		
		
		
		
		/// <summary>
		///  
		/// </summary>
		public byte MinorVersion { get; set; }
		
		/// <summary>
		/// </summary>
		public TemplateId TemplateId { get; set; }

		/// <summary>
		///  
		/// </summary>
		public ImageAlignment ImageAlignment { get; set; }

		/// <summary>
		///  
		/// </summary>
		public ImageSource ImageSource { get; set; }
		
		/// <summary>
		///  
		/// </summary>
		public Color BackgroundColor { get; set; }

// TODO: major???		
		
		/// <summary>
		///  
		/// </summary>
		public byte MajorVersion { get; set; }
		
		/// <summary>
		///  
		/// </summary>
		public byte ImageArea { get; set; }
		
		/// <summary>
		///  
		/// </summary>
		public FieldInfo[] FieldInfos { get; set; }
				
		/// <summary>
		///  
		/// </summary>
		public ExtraInfoBuffer ExtraInfo { get; set; }



		/// <summary>
		///  
		/// </summary>
		public BusinessCardDisplayDefinitionFormat ()
		{
		}
		
		/// <summary>
		///  
		/// </summary>
		public BusinessCardDisplayDefinitionFormat (byte[] data)
		{
			
			// TODO: decode
			
		}
		
		/// <summary>
		///  
		/// </summary>
		public byte[] Generate ()
		{
			
			throw new NotImplementedException ("NOT YET IMPLEMENTED!");
			
			// TODO: encode
		}
		
	}
	
}
