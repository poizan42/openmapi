//
// openmapi.org - NMapi C# Mapi API - MapiAccess.cs
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

	/// <summary>Possible flags for the property tag Property.Access.</summary>
	/// <remarks>
	///  This property, together with the flags, can be used on messages, folders 
	///  and store objects. However, not all flags make sense everywhere. 
	///  The Create*-Flags can only be used on folders and stores.
	/// </remarks>
	[Flags]
	public enum MapiAccess
	{
		/// <summary>The object can be modified.</summary>
		/// <remarks>In classic MAPI this was called MAPI_ACCESS_MODIFY.</remarks>
		Modify = 0x00000001,

		/// <summary>The data of the object can be read.</summary>
		/// <remarks>In classic MAPI this was called MAPI_ACCESS_READ.</remarks>
		Read = 0x00000002,

		/// <summary>The object can be deleted.</summary>
		/// <remarks>In classic MAPI this was called MAPI_ACCESS_DELETE.</remarks>
		Delete = 0x00000004,

		/// <summary>The object can create subfolders.</summary>
		/// <remarks>In classic MAPI this was called MAPI_CREATE_HIERARCHY.</remarks>
		CreateHierarchy = 0x00000008,

		/// <summary>The object ca create message in its contents-table.</summary>
		/// <remarks>In classic MAPI this was called MAPI_CREATE_CONTENTS.</remarks>
		CreateContents = 0x00000010,

		/// <summary>The object ca create message in its associated contents-table.</summary>
		/// <remarks>In classic MAPI this was called MAPI_CREATE_ASSOCIATED.</remarks>
		CreateAssociated = 0x00000020
	}


}
