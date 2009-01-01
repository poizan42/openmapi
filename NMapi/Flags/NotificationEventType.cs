//
// openmapi.org - NMapi C# Mapi API - NotificationEventType.cs
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
using System.IO;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	/// <summary>
	///  The Notification event types. Used to build a filtering bitmask.
	/// </summary>
	/// <remarks>
	///  <para>Each event type one has a parameter structure 
	///  associated with it:</para>
	///  
	///  <para>fnevCriticalError       ErrorNotification</para>
	///  <para>fnevNewMail             NewMailNotification</para>
	///  <para>fnevObjectCreated       ObjectNotification</para>
	///  <para>fnevObjectDeleted       ObjectNotification</para>
	///  <para>fnevObjectModified      ObjectNotification</para>
	///  <para>fnevObjectCopied        ObjectNotification</para>
	///  <para>fnevSearchComplete      ObjectNotification</para>
	///  <para>fnevTableModified       TableNotification</para>
	///  <para>fnevStatusObjectModified ?</para>
	///  <para>fnevExtended            ExtendedNotification</para>
	/// </remarks>
	[Flags]
	public enum NotificationEventType
	{
		CriticalError           = 0x00000001,
		NewMail                 = 0x00000002,
		ObjectCreated           = 0x00000004,
		ObjectDeleted           = 0x00000008,
		ObjectModified          = 0x00000010,
		ObjectMoved             = 0x00000020,
		ObjectCopied            = 0x00000040,
		SearchComplete          = 0x00000080,
		TableModified           = 0x00000100,
		StatusObjectModified    = 0x00000200,
		ReservedForMapi         = 0x40000000,
		Extended                = unchecked ( (int) 0x80000000 )

	}
}
