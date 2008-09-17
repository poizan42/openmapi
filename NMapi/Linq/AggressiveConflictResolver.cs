//
// openmapi.org - NMapi C# Mapi API - AggressiveConflictResolver.cs
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
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NMapi.Flags;
using NMapi.Table;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	/// <summary>
	///  Trivial conflict resolver. The local change always wins.
	/// </summary>
	public class AggressiveConflictResolver : IConflictResolver
	{
		public AggressiveConflictResolver ()
		{
		}

		public bool Decide (object local, object remote)
		{
			return false; // local wins
		}
	}
}
