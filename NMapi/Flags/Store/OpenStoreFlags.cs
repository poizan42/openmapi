//
// openmapi.org - NMapi C# Mapi API - OpenStoreFlags.cs
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

namespace NMapi.Flags {

	/// <summary>Flags for <see cref="M:IMapiSession.OpenStore" />.</summary>
	/// <remarks>
	///  <para></para>
	///  <para></para>
	/// </remarks>
	[Flags]
	public enum OpenStoreFlags
	{
		
		
		
//		MAPI_BEST_ACCESS
//		Requests that the message store be opened with the maximum network permissions allowed for the user and the maximum client application permissions. For example, if the client has read/write permission, the message store should be opened with read/write permission; if the client has read-only permission, the message store should be opened with read-only permission.
//
//		MAPI_DEFERRED_ERRORS
//		Allows OpenMsgStore to return successfully, possibly before the message store is fully available to the calling client. If the message store is not available, making a subsequent object call can raise an error.

		
		
		
		/// <summary></summary>
		//		Prevents the display of logon dialog boxes. If this flag is set, and OpenMsgStore does not have enough configuration information to open the message store without the user's help, it returns MAPI_E_LOGON_FAILED. If this flag is not set, the message store provider can prompt the user to correct a name or password, to insert a disk, or to perform other actions necessary to establish connection to the message store.
		/// <remarks>MDB_NO_DIALOG</remarks>
		NoDialog  = 0x00000001,
		
		/// <summary></summary>
//		Requests read/write access to the message store.
		/// <remarks>MDB_WRITE</remarks>
		Write = 0x00000004,
		
		/// <summary></summary>
//		Instructs MAPI that the message store is not permanent and should not be added to the message store table. This flag is used to log on the message store so that information can be retrieved programmatically from the profile section.
		/// <remarks>MDB_TEMPORARY</remarks>
		Temporary = 0x00000020,
		
		/// <summary></summary>
//		The message store should not be used for sending or receiving mail. When this flag is set, MAPI does not notify the MAPI spooler that this message store is being opened.		
		/// <remarks>MDB_NO_MAIL</remarks>
		NoMail = 0x00000080
	}
}

