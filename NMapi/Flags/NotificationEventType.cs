//
// openmapi.org - NMapi C# Mapi API - NotificationEventType.cs
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

	/// <summary>The Notification event types.</summary>
	/// <remarks>
	///  <para></para>
	///  <para>
	///   The types can also be used to build a filter, when subscribing 
	///   to events. Only events for flags contained in the bitmask will 
	///   be retrieved.
	///  </para>
	/// </remarks>
	[Flags]
	public enum NotificationEventType
	{
		/// <summary>Indicates that a critical error occured.</summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.ErrorNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		CriticalError = 0x00000001,

		/// <summary>Indicates that a new mail arrived in the store.</summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.NewMailNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		NewMail = 0x00000002,

		/// <summary>
		///  Indicates that a new MAPI object (like a folder or a message) 
		///  has been created.</summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.ObjectNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		ObjectCreated = 0x00000004,

		/// <summary>
		///  Indicates that a MAPI object (like a folder or a message) 
		///  has been deleted.</summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.ObjectNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		ObjectDeleted = 0x00000008,

		/// <summary>
		///  Indicates that a MAPI object (like a folder or a message) 
		///  has been modified.</summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.ObjectNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		ObjectModified = 0x00000010,

		/// <summary>
		///  Indicates that a MAPI object (like a folder or a message) 
		///  has been moved.</summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.ObjectNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		ObjectMoved = 0x00000020,

		/// <summary>
		///  Indicates that a MAPI object (like a folder or a message) 
		///  has been copied (to another folder).
		/// </summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.ObjectNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		ObjectCopied = 0x00000040,

		/// <summary>Indicates that a search (of a search folder) has completed.</summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.ObjectNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		SearchComplete = 0x00000080,

		/// <summary>
		///  Indicates that a MapiTable has been modified. There are several 
		///  subtypes of table notifications, depending on how the table changed.
		/// </summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.TableNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		TableModified = 0x00000100,

		/// <summary>TODO: add documentation!</summary>
		/// <remarks></remarks>
		StatusObjectModified = 0x00000200,
		
		/// <summary>Notifies an indexer when objects need to be reindexed.</summary>
		/// <remarks>
		///  <para>Shares the same structure as the extended notification.</para>
		///  <para>This seems to be an addition of Outlook 2007.</para>
		///  <para>It is used locally only (really?) by pusher stores.</para>
		/// </remarks>
		Indexing = 0x00010000,

		/// <summary>This value is reserved and should be ignored.</summary>
		ReservedForMapi = 0x40000000,

		/// <summary>
		///  Transmits an arbitrary byte array to the client.
		///  The format/meaning of the notification is defined by the store.
		/// </summary>
		/// <remarks>
		///  <para>The notification data is passed in <see cref="NMapi.Events.ExtendedNotification" />.</para>
		///  <para>This value is part of the core MAPI protocol.</para>
		/// </remarks>
		Extended  = unchecked ( (int) 0x80000000 )
		
	}
	
}
