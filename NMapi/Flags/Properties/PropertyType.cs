//
// openmapi.org - NMapi C# Mapi API - PropertyType.cs
//
// Copyright 2010 Topalis AG
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

using System.IO;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	/// <summary>
	///  A list of the MAPI property types.
	/// </summary>
	/// <remarks>
	///   0 and ffff are not real property types and may be used for other purposes!
	/// </remarks>
	public enum PropertyType
	{
		Unspecified = 0,     // Reserved
		Null        = 1,     //
		Int16       = 2,     // Signed 16-bit integer "I2"
		Int32       = 3,     // Signed 32-bit integer "Long"
		Float       = 4,     // 4-byte floating point "R4"
		Double      = 5,     // Floating point double
		Currency    = 6,     // Signed 64-bit int (decimal w/ 4 digits right of decimal pt)
		AppTime     = 7,     // Application Time
		Error       = 10,    // 32-bit Error Value
		Boolean     = 11,    // 16-bit Boolean (0 = false, otherwise true)
		Object      = 13,    // Object embedded in property
		Int64       = 20,    // 8-byte signed integer "I8"
		String8     = 30,    // 8-bit character string (Null-Terminated)
		Unicode     = 31,    // Unicode string  (Null-Terminated)
		SysTime     = 64,    // FILETIME 64-bit integer (number of 100 ns periods since 1. January 1601)
		ClsId       = 72,    // A Guid
		Binary      = 258,   // An arbitrary array of bytes

		Restriction = 253,   // Used to specify the conditions of the rules. // TODO: we must check for this type in switch-statements!
		Actions	    = 254,   // Used to specify the actions of the rules. // TODO: we must check for this type in switch-statements!

		Pointer		= 259,   // This was introduced in Outlook 2010. Not sure how/if we need to support it.
		                     // An alias for this is "FileHandle".
	

		MvInt16     = (NMAPI.MV_FLAG | Int16),
		MvInt32     = (NMAPI.MV_FLAG | Int32),
		MvFloat     = (NMAPI.MV_FLAG | Float),
		MvDouble    = (NMAPI.MV_FLAG | Double),
		MvCurrency  = (NMAPI.MV_FLAG | Currency),
		MvAppTime   = (NMAPI.MV_FLAG | AppTime),
		MvSysTime   = (NMAPI.MV_FLAG | SysTime),
		MvString8   = (NMAPI.MV_FLAG | String8),
		MvBinary    = (NMAPI.MV_FLAG | Binary),
		MvUnicode   = (NMAPI.MV_FLAG | Unicode),
		MvClsId     = (NMAPI.MV_FLAG | ClsId),
		MvInt64     = (NMAPI.MV_FLAG | Int64),

	}

}
