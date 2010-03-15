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
	/// <remarks>Provides some control on how a message store is opened.</remarks>
	[Flags]
	public enum OpenStoreFlags
	{

//		TODO: NMAPI.MAPI_BEST_ACCESS and NMAPI.MAPI_DEFERRED_ERRORS are possible as well!
//		       This can lead to problems, for example when casting to (OpenStoreFlags) in the server.
		
		/// <summary>This is used to logon to the store in a strictly non-interactive way.</summary>
		/// <remarks>
		///  <para>
		///   The absence of this flag allows a certain degree of interaction
		///   with the user to go on. Still, since this basically breaks proper 
		///   layering and I'm not convinced that it's a good idea if some lower 
		///   layer starts popping up some GUI, this is a feature of rather dubious value.
		/// </para>
		///  <para>The classic MAPI constant for this is called MDB_NO_DIALOG.</para>
		/// </remarks>
		NoDialog  = 0x00000001,
		
		/// <summary>If set, the client attempts to open the store with write-access.</summary>
		/// <remarks>The classic MAPI constant for this is called MDB_WRITE.</remarks>
		Write = 0x00000004,
		
		/// <summary>The store, although opened, should not appear in the store table.</summary>
		/// <remarks>The classic MAPI constant for this is called MDB_TEMPORARY.</remarks>
		Temporary = 0x00000020,
		
		/// <summary>TODO: documentation!</summary>
		/// <remarks>The classic MAPI constant for this is called MDB_NO_MAIL.</remarks>
		NoMail = 0x00000080
	}
}

