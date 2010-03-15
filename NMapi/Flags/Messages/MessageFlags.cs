//
// openmapi.org - NMapi C# Mapi API - MsgFlag.cs
//
// Copyright 2008-2009 Topalis AG
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
	///  
	/// </summary>
	[Flags]
	public enum MsgFlag
	{
		// Flags for PR_MESSAGE_FLAGS

		/// <summary>
		///  
		/// </summary>
		Read = 0x00000001,

		/// <summary>
		///  
		/// </summary>
		Unmodified = 0x00000002,

		/// <summary>
		///  
		/// </summary>
		Submit = 0x00000004,

		/// <summary>
		///  
		/// </summary>
		Unsent = 0x00000008,

		/// <summary>
		///  
		/// </summary>
		HasAttach = 0x00000010,

		/// <summary>
		///  
		/// </summary>
		FromMe = 0x00000020,

		/// <summary>
		///  
		/// </summary>
		Associated = 0x00000040,

		/// <summary>
		///  
		/// </summary>
		Resend = 0x00000080,

		/// <summary>
		///  
		/// </summary>
		RnPending = 0x00000100,

		/// <summary>
		///  
		/// </summary>
		NrnPending = 0x00000200,		
		
		/// <summary>
		/// 
		/// </summary>
		Internet = 0x00002000,
		
		/// <summary>
		/// 
		/// </summary>
		Untrusted = 0x00008000

	}

}
