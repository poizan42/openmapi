//
// openmapi.org - NMapi C# Mapi API - GroupwiseSendOptions.cs
//
// Copyright 2009 Topalis AG
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

namespace NMapi.Flags.Groupwise {

	/// <summary></summary>
	[Flags]
	public enum GroupwiseSendOptions
	{
		
		/// <summary>Automatically deletes message from sender’s Out Box when 
		// opened by recipient.</summary>
		AutoDelete  = 0x00000001,
		
		/// <summary>The subject does not appear until the message is opened.</summary>
		ConcealSubject  = 0x00000002,
		
		/// <summary>The sender maintains the Out Box on the message.</summary>
		InsertOutBox  = 0x00000008,
		
		/// <summary>Specifies how to notify sender of status tracking.</summary>
		MailReceipt  = 0x00000010,
		
		/// <summary>Specifies how to notify the sender of status tracking.</summary>
		NotifyReceipt  = 0x00000020,
		
		/// <summary>Specifies how to notify the user of status tracking.</summary>
		NotifyUser  = 0x00000040,
		
		/// <summary>The date the recipient of an item should reply by.</summary>
		ReplyByDate  = 0x00000100,
		
		/// <summary>States that the recipient of an item should reply to the item.</summary>
		ReplyRequested  = 0x00000200,
		
		/// <summary>The sender of an item (e.g. an appointment) is a resource.</summary>
		OrgIsResource  = 0x00000400,
		
		/// <summary>Notify sender when an item is deleted.</summary>
		NotifyDelete  = 0x00000800,
		
		/// <summary>Send a mail message to the sender when an item is deleted.</summary>
		MailDelete  = 0x00001000,
		
		/// <summary>Notify the sender when an item is accepted.</summary>
		NotifyAccept  = 0x00002000,

		/// <summary>Send a mail message to the sender when an item is accepted.</summary>
		MailAccept  = 0x00004000,
		
		/// <summary>Notify the sender when a task is completed.</summary>
		NotifyComplete  = 0x00020000,
		
		/// <summary>Send a mail message to the sender when a task is completed.</summary>
		MailComplete  = 0x00040000,

		/// <summary>Mark an item as private.</summary>
		MarkPrivate  = 0x00080000,

		/// <summary>Notify the sender if an item is undeliverable.</summary>
		NotifyUndeliverable = 0x00100000,

		/// <summary>Send a mail message to the sender when an item is undeliverable.</summary>
		MailUndeliverable = 0x00200000

	}

}
