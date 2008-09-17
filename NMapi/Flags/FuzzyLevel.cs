//
// openmapi.org - NMapi C# Mapi API - FuzzyLevel.cs
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

	/// <summary>
	///  Indicates the fuzzy level to be used with SContentRestrictions.
	/// </summary>
	[Flags]
	public enum FuzzyLevel
	{
		FullString     = 0x00000000, // string completely contained to match!
		Substring      = 0x00000001,
		Prefix         = 0x00000002,

		IgnoreCase     = 0x00010000,
		IgnoreNonSpace = 0x00020000, // ignore non-spacing characters!
		Loose          = 0x00040000  // ignore case + nonspacing, etc.
	}
}
