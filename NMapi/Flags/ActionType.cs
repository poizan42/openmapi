//
// openmapi.org - NMapi C# Mapi API - ActionType.cs
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

	/// <summary>The possible types of actions used in server-side rules.</summary>
	/// <remarks>
	///  <para>
	///   MAPI makes it possible to set a list of rules on each folder, where 
	///   each of the rules is triggered under certain circumstances and may 
	///   lead to a certain action that should be performed.
	///  </para>
	///  <para>
	///   The rules as stored in the rules table associated with the folder. 
	///   For more information about rules, check out <see cref="NMapi.Rules.Rule" />. 
	///  </para>
	///  <para>This enum defines the set of possible actions to be performed.</para>
	///  <para>In classic MAPI these constants are defined as OP_MOVE, OP_COPY, etc.</para>
	/// </remarks>
	public enum ActionType
	{

		/// <summary>The message object should be moved to another folder.</summary>
		/// <remarks>Only valid in private stores.</remarks>
		Move = 1,

		/// <summary>The message object  should be copied to another folder.</summary>
		/// <remarks>Only valid in private stores.</remarks>
		Copy,

		/// <summary>A reply should be sent.</summary>
		/// <remarks>Valid in private or public stores.</remarks>
		Reply,

		/// <summary>An Out-Of-Office-Reply should be sent.</summary>
		/// <remarks>Valid in private or public stores.</remarks>
		OofReply,

		/// <summary>An action that should be handled by the client.</summary>
		/// <remarks>Only valid in private stores.</remarks>
		DeferAction,

		/// <summary>The message is bounced.</summary>
		/// <remarks>Valid in private or public stores.</remarks>
		Bounce,

		/// <summary>The message should be forwarded.</summary>
		/// <remarks>Valid in private or public stores.</remarks>
		Forward,

		/// <summary>The message should be delegated (?).</summary>
		/// <remarks>Valid in private or public stores.</remarks>
		Delegate,

		/// <summary>The message object is tagged using a certain property.</summary>
		/// <remarks>Valid in private or public stores.</remarks>
		Tag,

		/// <summary>The message should be deleted.</summary>
		/// <remarks>Valid in private or public stores.</remarks>
		Delete,

		/// <summary>Sets the MSGFLAG_READ in the PidTagMessageFlags property on the Message (see [MS-OXPROPS])</summary>
		/// <remarks>Valid in private or public stores.</remarks>
		MarkAsRead // 0xB = 11

	}

}
