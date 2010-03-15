//
// openmapi.org - NMapi C# Mapi API - FolderStatus.cs
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
	
	/// <summary>Defined flags for Property.Status, indicating the status of a folder.</summary>
	/// <remarks>
	/// <para>
	///  These flags should not be interpreted by providers. In particular, 
	///  the flag <see cref="DelMarked" /> is just set by the client and does 
	///  just have a meaning for the client. It may mean that the user selected   
	///  the folder for deletion, but the client will deal with this, if it does 
	///  wish so and can make call to the server to actually delete the folder. 
	/// </para>
	/// <para>
	///   NOTE: The client may also set other flags than the ones mentioned here, 
	///         because it has as reserved range for that purpose. Providers must 
	///         deal with this and store the field, as well as be careful with casts.
	/// </para>
	/// <para>
	///  The 16 higher bits of the 32-bit interger value are reserved for the client to use.
	/// </para>
	/// </remarks>
	[Flags]
	public enum FolderStatus
	{
		/// <summary>The folder is highlighted is some way.</summary>
		/// <remarks>The classic MAPI name of this constant is FLDSTATUS_HIGHLIGHTED.</remarks>
		Highlighted = 1,
		
		/// <summary>Whatever that means.</summary>
		/// <remarks>The classic MAPI name of this constant is FLDSTATUS_HIGHLIGHTED.</remarks>
		Tagged = 2,

		/// <summary>
		///  The folder should be hidden from the user (i.e. not be visually 
		///  displayed in the folder hierarchy).
		/// </summary>
		/// <remarks>
		///  <para>The flag may be ignored by the client.</para>
		///  <para>he classic MAPI name of this constant is FLDSTATUS_HIDDEN.</para>
		/// </remarks>
		Hidden = 4,

		/// <summary>The client marked the folder for deletion.</summary>
		/// <remarks>
		///  <para>This does not trigger any action on the side of the provider at all.</para>
		///  <para>The flag may be ignored by the client.</para>	
		///  <para>The classic MAPI name of this constant is FLDSTATUS_DELMARKED.</para>
		/// </remarks>
		DelMarked = 8
	}
}
