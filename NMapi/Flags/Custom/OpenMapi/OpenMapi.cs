//
// openmapi.org - NMapi C# Mapi API - OpenMapi.cs
//
// Copyright 2008 Topalis AG
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
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	/// <summary>
	///   OpenMapi-specific constants. 
	/// </summary>
	public static class OpenMapi
	{
		public static readonly NMapiGuid Guid = Guids.DefineGuid (0x6ED8DA90, 
				(short) 0x450B, (short) 0x101B, (byte) 0x98, (byte) 0xDA, (byte) 0x00, 
				(byte) 0xaa, (byte) 0x00, (byte) 0x3F, (byte) 0x13, (byte) 0x05, 
					(byte) 0xbb, (byte) 0x0000, (byte) 0x0000);
	}
}
