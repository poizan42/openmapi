//
// openmapi.org - NMapi C# Mapi API - AddressBookProviderArrayTypeFlags.cs
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


/*


2.2.1.2.13 PidLidAddressBookProviderArrayType
This PtypInteger32 property specifies the state of the contact’s electronic addresses and represents a set of bit-flags. The value of the PidLidAddressBookProviderArrayType property MUST be a combination of flags that specify the state of the Contact object. Individual flags are specified in the following table. If this property is set, then PidLidAddressBookProviderEmailList MUST be set as well. These two properties MUST be kept in sync with each other.
For example, if this property has the bit “0x00000001” set, then one of the values of PidLidAddressBookProviderEmailList would be “0x00000000”.


*/
	/// <summary>
	///  
	/// </summary>
	/// <remarks>
	///  Note: This file is based on the documentation published by Microsoft
	///       [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	[Flags]
	public enum AddressBookProviderArrayTypeFlags
	{	
		/// <summary></summary>
		Email1Defined = 0x0001,

		/// <summary></summary>
		Email2Defined = 0x0002,

		/// <summary></summary>
		Email3Defined = 0x0004,

		/// <summary></summary>
		BusinessFaxDefined = 0x0008,

		/// <summary></summary>
		HomeFaxDefined = 0x0010,

		/// <summary></summary>
		PrimaryFaxDefined = 0x0020
	}
	
}
