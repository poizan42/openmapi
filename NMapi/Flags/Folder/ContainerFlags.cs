//
// openmapi.org - NMapi C# Mapi API - ContainerFlags.cs
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

	/// <summary>Possible flags for Property.ContainerFlags.</summary>
	/// <remarks>
	///  <para>
	///   Despite their name, these flags (and the property) seem to apply only to 
	///   Addressbook-Containers, not IMapiFolder-Objects that also implement the 
	///   IMapiContainer interface. 
	///  </para>
	///  <para>The flags specify certain capabilities of addressbook containers.</para>
	/// </remarks>
	[Flags]
	public enum ContainerFlags
	{
		/// <summary>The container may contain recipients.</summary>
		/// <remarks>Classic MAPI name is AB_RECIPIENTS.</remarks>
		Recipients = 0x00000001,

		/// <summary>The container may contain subcontainers.</summary>
		/// <remarks>Classic MAPI name is AB_SUBCONTAINERS.</remarks>
		SubContainers = 0x00000002,

		/// <summary>Indicates that entries can be added/removed.</summary> 
		/// <remarks>
		///  <para>
		///   NOTE: If <see cref="NMapi.Flags.Ab.Unmodifiable" /> is set at the 
		///   same time, the meaning is undefined or unknown to the provider. 
		///  </para>
		///  <para>If this flag is not set, <see cref="NMapi.Flags.Ab.Unmodifiable" /> must be set.</para>
		///  <para>Classic MAPI name is AB_MODIFIABLE.</para>
		/// </remarks>
		Modifiable = 0x00000004,

		/// <summary>Indicates that entries can't be added/removed.</summary> 
		///  <para>
		///   NOTE: If <see cref="NMapi.Flags.Ab.Modifiable" /> is set at the 
		///   same time, the meaning is undefined or unknown to the provider. 
		///  </para>
		///  <para>If this flag is not set, <see cref="NMapi.Flags.Ab.Modifiable" /> must be set.</para>
		///  <para>Classic MAPI name is AB_UNMODIFIABLE.</para>
		/// </remarks>
		Unmodifiable = 0x00000008,

		/// <summary>The data is filtered at the beginning (before it is displayed).</summary>
		/// <remarks>
		///  <para>This can help to improve performance.</para>
		///  <para>Classic MAPI name is AB_FIND_ON_OPEN.</para>
		/// </remarks>
		FindOnOpen = 0x00000010,

		/// <summary>TODO: document!</summary>
		/// <remarks>Classic MAPI name is AB_NOT_DEFAULT.</remarks>
		NotDefault = 0x00000020
	}

}
