//
// openmapi.org - NMapi C# Mapi API - PropertyType.cs
//
// Copyright 2008 VipCom AG
//
// Author (Javajumapi): VipCOM AG
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

using System.IO;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	/// <summary>
	///  List of the MAPI property types.
	/// </summary>
	/// <remarks>
	///   0 and ffff are not real property types and may be used for other purposes!
	/// </remarks>
	public enum PropertyType
	{	
		Unspecified = 0,     // Reserved
		Null        = 1,     //
		I2          = 2,     // Signed 16-bit integer
		Long        = 3,     // Signed 32-bit integer
		R4          = 4,     // 4-byte floating point
		Double      = 5,     // Floating point double
		Currency    = 6,     // Signed 64-bit int (decimal w/    4 digits right of decimal pt)
		AppTime     = 7,     // Application Time
		Error       = 10,    // 32-bit Error Value
		Boolean     = 11,    // 16-bit Boolean (0 = false, otherwise true)
		Object      = 13,    // Object embedded in property
		I8          = 20,    // 8-byte signed integer
		String8     = 30,    // 8-bit character string (Null-Terminated)
		Unicode     = 31,    // Unicode string  (Null-Terminated)
		TString     = Unicode,
		SysTime     = 64,    // FILETIME 64-bit integer (number of 100 ns periods since 1. January 1601)
		ClsId       = 72,    // A Guid
		Binary      = 258,   // An arbitrary array of bytes

		MvI2        = (NMAPI.MV_FLAG | I2),
		MvLong      = (NMAPI.MV_FLAG | Long),
		MvR4        = (NMAPI.MV_FLAG | R4),
		MvDouble    = (NMAPI.MV_FLAG | Double),
		MvCurrency  = (NMAPI.MV_FLAG | Currency),
		MvAppTime   = (NMAPI.MV_FLAG | AppTime),
		MvSysTime   = (NMAPI.MV_FLAG | SysTime),
		MvString8   = (NMAPI.MV_FLAG | String8),
		MvBinary    = (NMAPI.MV_FLAG | Binary),
		MvUnicode   = (NMAPI.MV_FLAG | Unicode),
		MvTString   = MvUnicode,
		MvClsId     = (NMAPI.MV_FLAG | ClsId),
		MvI8        = (NMAPI.MV_FLAG | I8),

		// Aliases

		Short    = I2,
		I4       = Long,
		Float    = R4,
		R8       = Double,
		LongLong = I8,
	
		MvShort    = MvI2,
		MvI4       = MvLong,
		MvFloat    = MvR4,
		MvR8       = MvDouble,
		MvLongLong = MvI8

	}

}
