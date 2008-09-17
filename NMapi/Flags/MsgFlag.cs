//
// openmapi.org - NMapi C# Mapi API - MsgFlag.cs
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

using RemoteTea.OncRpc;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	[Flags]
	public enum MsgFlag
	{
		// Flags for PR_MESSAGE_FLAGS

		Read        = 0x00000001,
		Unmodified  = 0x00000002,
		Submit      = 0x00000004,
		Unsent      = 0x00000008,
		HasAttach   = 0x00000010,
		FromMe      = 0x00000020,
		Associated  = 0x00000040,
		Resend      = 0x00000080,
		RnPending   = 0x00000100,
		NrnPending  = 0x00000200
	}

}
