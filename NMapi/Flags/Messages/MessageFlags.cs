//
// openmapi.org - NMapi C# Mapi API - MessageFlags.cs
//
// Copyright 2008-2010 Topalis AG
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

namespace NMapi.Flags {

	/// <summary>Possible values for the property tag Property.MessageFlags.</summary>
	/// <remarks></remarks>
	[Flags]
	public enum MessageFlags
	{
		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_READ.</remarks>
		Read = 0x00000001,

		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_UNMODIFIED.</remarks>
		Unmodified = 0x00000002,

		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_SUBMIT.</remarks>
		Submit = 0x00000004,

		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_UNSENT.</remarks>
		Unsent = 0x00000008,

		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_HAS_ATTACH.</remarks>
		HasAttach = 0x00000010,

		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_FROM_ME.</remarks>
		FromMe = 0x00000020,

		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_ASSOCIATED.</remarks>
		Associated = 0x00000040,

		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_RESEND.</remarks>
		Resend = 0x00000080,

		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_RN_PENDING.</remarks>
		RnPending = 0x00000100,

		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_NRN_PENDING.</remarks>
		NrnPending = 0x00000200,		
		
		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_INTERNET.</remarks>
		Internet = 0x00002000,
		
		/// <summary></summary>
		/// <remarks>The classic MAPI name of this flag is MSGFLAG_UNTRUSTED.</remarks>
		Untrusted = 0x00008000

	}

}
