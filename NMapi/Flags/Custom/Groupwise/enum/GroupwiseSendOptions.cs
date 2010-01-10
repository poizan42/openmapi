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
using System.IO;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags.Groupwise {



		/// <summary>
		///   
		/// </summary>
		[Flags]
		public enum GroupwiseSendOptions
		{
			
			// Automatically deletes message from sender’s Out Box when 
			// opened by recipient.
			AutoDelete  = 0x00000001,
			
			// The subject does not appear until the message is opened. 
			ConcealSubject  = 0x00000002,
			
			// The sender maintains the Out Box on the message. 
			InsertOutBox  = 0x00000008,
			
			// Specifies how to notify sender of status tracking. 
			MailReceipt  = 0x00000010,
			
			// Specifies how to notify the sender of status tracking. 
			NotifyReceipt  = 0x00000020,
			
			// Specifies how to notify the user of status tracking. 
			NotifyUser  = 0x00000040,
			
			// The date the recipient of an item should reply by. 
			ReplyByDate  = 0x00000100,
			
			// States that the recipient of an item should reply to the item. 
			ReplyRequested  = 0x00000200,
			
			// The sender of an item (e.g. an appointment) is a resource. 
			OrgIsResource  = 0x00000400,
			
			// Notify sender when an item is deleted. 
			NotifyDelete  = 0x00000800,
			
			// Send a mail message to the sender when an item is deleted. 
			MailDelete  = 0x00001000,
			
			// Notify the sender when an item is accepted.
			NotifyAccept  = 0x00002000,

			//Send a mail message to the sender when an item is accepted. 
			MailAccept  = 0x00004000,
			
			// Notify the sender when a task is completed. 
			NotifyComplete  = 0x00020000,
			
			//Send a mail message to the sender when a task is completed. 
			MailComplete  = 0x00040000,

			//Mark an item as private. 
			MarkPrivate  = 0x00080000,

			//Notify the sender if an item is undeliverable. 
			NotifyUndeliverable = 0x00100000,

			//Send a mail message to the sender when an item is undeliverable.
			MailUndeliverable = 0x00200000

		}

}
