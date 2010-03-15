//
// openmapi.org - NMapi C# Mapi API - LogoffFlags.cs
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
using System.IO;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	/// <summary></summary>
	/// <remarks></remarks>
	[Flags]
	public enum LogoffFlags
	{
		
		// in-flags:
		
		/// <summary></summary>
		/// <remarks></remarks>


//		The message store should not wait for messages from transport providers before closing. Outbound messages that are ready to be sent are sent. If this store contains the default Inbox, any in-process messages are received, and then further reception is disabled. When all activity is complete, the MAPI spooler releases the store, and control is immediately returned to the caller.


		NoWait = 0x00000001,

		/// <summary></summary>
		/// <remarks></remarks>
		

//		The message store should not wait for information from transport providers before closing. Messages that are currently being processed are completed, but no new messages are processed. When all activity is complete, the MAPI spooler releases the store, and control is immediately returned to the store provider.


		Orderly = 0x00000002,

		/// <summary></summary>
		/// <remarks></remarks>

//		The logoff should work the same as if the LOGOFF_NO_WAIT flag is set, but either the IXPLogon::FlushQueues or IMAPIStatus::FlushQueues method for the appropriate transport providers should be called. The LOGOFF_PURGE flag returns control to the caller after completion.


		Purge = 0x00000004,

		/// <summary></summary>
		/// <remarks></remarks>

//		Any transport provider activity for this message store should be stopped before logoff. Control is returned to the caller after activity is stopped. If any transport provider activity is taking place, the logoff does not occur and no change in the behavior of the MAPI spooler or transport providers occurs. If transport provider activity is idle, the MAPI spooler releases the store.


		Abort = 0x00000008,

		/// <summary></summary>
		/// <remarks></remarks>
// 		????		
		Quiet = 0x00000010,

		/// <summary></summary>
		/// <remarks></remarks>

//		If any transport provider activity is taking place, the logoff should not occur.

		Complete = 0x00010000,

		
		// below: out-flags ...

		/// <summary></summary>
		/// <remarks></remarks>
//		Inbound messages are currently arriving.
		Inbound = 0x00020000,

		/// <summary></summary>
		/// <remarks></remarks>
//		Outbound messages are in the process of being sent.
		Outbound = 0x00040000,

		/// <summary></summary>
		/// <remarks></remarks>
//		Outbound messages are pending (that is, they are in the Outbox).
		OutboundQueue = 0x00080000
	}


}
