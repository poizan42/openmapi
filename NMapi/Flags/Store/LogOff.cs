//
// openmapi.org - NMapi C# Mapi API - LogOff.cs
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

	// StoreLogoff ()

	[Flags]
	public enum Logoff
	{
		NoWait         = 0x00000001,
		Orderly        = 0x00000002,
		Purge          = 0x00000004,
		Abort          = 0x00000008,
		Quiet          = 0x00000010,

		Complete       = 0x00010000,
		Inbound        = 0x00020000,
		Outbound       = 0x00040000,
		OutboundQueue  = 0x00080000
	}


}
