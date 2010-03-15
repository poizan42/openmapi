//
// openmapi.org - NMapi C# Mapi API - ModifyRecipientsMode.cs
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

	/// <summary>
	///  Mode for the <see cref="M:NMapi.Properties.Special.IMessage.ModifyRecipients" /> method.
	/// </summary>
	/// <remarks>
	///  The <see cref="M:NMapi.Properties.Special.IMessage.ModifyRecipients" /> method 
	///  is a somewhat strange call that can be used to add, modify and delete 
	///  recipients from the recipient table of a message. These different 
	///  operations are distinuished by passing one of these mode values.
	/// </remarks>
	public static class ModifyRecipientsMode
	{
		/// <summary>If set, the Modify-call adds the recipient to the list.</summary>
		/// <remarks>Classic MAPI defines this as MODRECIP_ADD.</remarks>
		public const int Add = 0x00000002;

		/// <summary>If set, the Modify-call modifies a recipient already present in the list.</summary>
		/// <remarks>Classic MAPI defines this as MODRECIP_MODIFY.</remarks>
		public const int Modify = 0x00000004;

		/// <summary>If set, the Modify-call removes the recipient from the list.</summary>
		/// <remarks>Classic MAPI defines this as MODRECIP_REMOVE.</remarks>
		public const int Remove = 0x00000008;
	}

}
