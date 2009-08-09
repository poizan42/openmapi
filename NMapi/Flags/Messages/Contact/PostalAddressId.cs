//
// openmapi.org - NMapi C# Mapi API - PostalAddressId.cs
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
	///  Values for PidLidPostalAddressId.
	///  
	/// </summary>
	/// <remarks>
	///  Note: This file is based on the documentation published by Microsoft
	///       [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public enum PostalAddressId
	{
		/// <summary>
		///  No postal (mailing) address selected.
		/// </summary>
		/// <remarks>
		/// 	NOT:
		/// 		PidTagStreetAddress, 
		/// 		PidTagLocality, 
		/// 		PidTagStateOrProvince, 
		/// 		PidTagPostalCode, 
		/// 		PidTagCountry, 
		/// 		PidLidAddressCountryCode, 
		/// 		PidTagPostalAddress
		/// </remarks>
		NoAddress = 0,
		
		
		
		/// <summary>
		///  Home address is selected.
		/// </summary>
		/*
		The values of the 
		PidTagStreetAddress, 
		PidTagLocality, 
		PidTagStateOrProvince, 
		PidTagPostalCode, 
		PidTagPostOfficeBox, 
		PidTagCountry, 
		PidLidAddressCountryCode,
		PidTagPostalAddress
		
		MUST be equal to the values of the 
		PidTagHomeAddressStreet, 
		PidTagHomeAddressCity, 
		PidTagHomeAddressStateOrProvince, 
		PidTagHomeAddressPostalCode, 
		PidTagHomeAddressPostOfficeBox, 
		PidTagHomeAddressCountry, 
		PidLidHomeAddressCountryCode, 
		PidLidHomeAddress properties.
		*/		
		HomeAddress = 1,

		/// <summary>
		///  Work address is selected.
		/// </summary>
		
		 // PidTagStreetAddress, PidTagLocality, PidTagStateOrProvince, PidTagPostalCode, PidTagPostOfficeBox, PidTagCountry, PidLidAddressCountryCode, and PidTagPostalAddress 
		// MUST be equal to the values of the PidLidWorkAddressStreet, PidLidWorkAddressCity, PidLidWorkAddressState, PidLidWorkAddressPostalCode, PidLidWorkAddressPostOfficeBox, 
		//		PidLidWorkAddressCountry, PidLidWorkAddressCountryCode, and PidLidWorkAddress properties, respectively.
		
		WorkAddress = 2,
		
		/// <summary>
		///  "Other" address is selected.
		/// </summary>

		// PidTagStreetAddress, PidTagLocality, PidTagStateOrProvince, PidTagPostalCode, 
		//  PidTagPostOfficeBox, PidTagCountry, PidLidAddressCountryCode, and PidTagPostalAddress properties MUST be equal to the values of the PidTagOtherAddressStreet, PidTagOtherAddressCity, PidTagOtherAddressStateOrProvince, PidTagOtherAddressPostalCode, PidTagOtherAddressPostOfficeBox, PidTagOtherAddressCountry, PidLidOtherAddressCountryCode, and PidLidOtherAddress properties, respectively.

		OtherAddress = 3
		
	}
	
}
