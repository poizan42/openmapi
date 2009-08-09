//
// openmapi.org - NMapi C# Mapi API - FldStatus.cs
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

	// TODO: docs!

	/// <summary>
	///  PR_STATUS
	/// </summary>
	[Flags]
	public enum FldStatus
	{
		/// <summary>
		///  
		/// </summary>
		Highlighted = 1,
		
		/// <summary>
		///  
		/// </summary>
		Tagged = 2,

		/// <summary>
		///  
		/// </summary>
		Hidden = 4,

		/// <summary>
		///  
		/// </summary>
		DelMarked = 8
	}
}
