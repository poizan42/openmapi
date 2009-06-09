//
// openmapi.org - NMapi C# Mapi API - Mapi.cs
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

	public struct Mapi
	{
		public const int Modify = 0x00000001;
		public const int Create = 0x00000002;
		
		public const int Unicode = unchecked ( (int) 0x80000000 );
		
		// Recipient types
	
		public const int Orig      = 0;
		public const int To        = 1;
		public const int Cc        = 2;
		public const int Bcc       = 3;
		public const int P1        = 0x10000000;
		public const int Submitted = unchecked ( (int) 0x80000000 );

		public const int Compound    = 0x80;
		
	
	}


}
