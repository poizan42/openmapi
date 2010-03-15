//
// openmapi.org - NMapi C# Mapi API - FileUnderFormat.cs
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


	
//		If a property is not present, then the separator characters surrounding it can be removed by the application.
//		Computing the value of the PidLidFileUnder property based on PidLidFileUnderId property value
	
	
	// Nl = \r\n
	
	/// <summary>
	///  Possible values for the Property "PidLidFileUnderId". (TODO: the MS name!). 
	///  This property specifies how the client should update the property "PidLidFileUnder" 
	///  if any contact properties are changed.
	/// </summary>
	/// <remarks>
	///  Note: This file is based on the documentation published by Microsoft
	///       [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public enum FileUnderFormat : uint
	{
		Update_Empty = 0x0000,
		Update_DisplayName = 0x3001,
		Update_GivenName = 0x3A06,
		Update_Surname = 0x3A11,
		Update_CompanyName = 0x3A16,
		
		Update_Surname_Comma_Space_GivenName_Space_MiddleName = 0x8017,
		Update_CompanyName_Nl_Surname_Comma_Space_GivenName_Space_MiddleName = 0x8018,
		Update_Surname_Comma_Space_GivenName_Space_MiddleName_Nl_CompanyName = 0x8019,
		Update_Surname_GivenName_Space_MiddleName = 0x8030,
		Update_Surname_Space_GivenName_Space_MiddleName = 0x8031,
		Update_CompanyName_Nl_Surname_GivenName_Space_MiddleName = 0x8032,
		Update_CompanyName_Nl_Surname_Space_GivenName_Space_MiddleName = 0x8033,
		Update_Surname_GivenName_Space_MiddleName_Nl_CompanyName = 0x8034,
		Update_Surname_Space_GivenName_Space_MiddleName_Nl_CompanyName = 0x8035,
		Update_Surname_Space_GivenName_Space_MiddleName_Space_Generation = 0x8036,
		Update_GivenName_Space_MiddleName_Space_Surname_Space_Generation = 0x8037,
		Update_Surname_GivenName_Space_MiddleName_Space_Generation = 0x8038,

		Update_BestMatch = 0xfffffffd,	// Application should use the current value of PidLidFileUnder and other contact properties 
									// to find a “best match” for PidLidFileUnderId to one of the previous values in this table.

		Update_UseLocaleDefaultsAndUpdateFileUnder = 0xfffffffe,		// application is to choose the appropriate default values (according to the language locale) for PidLidFileUnderId 
																	// and update PidLidFileUnder to match the choice.
									
		Keep_CustomUserProvidedFileUnder = 0xffffffff			// PidLidFileUnder (!) is a user-provided PtypString, and SHOULD NOT be changed when another 
															// Contact Name property changes.
	}
	
}
