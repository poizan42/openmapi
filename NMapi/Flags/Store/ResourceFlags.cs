//
// openmapi.org - NMapi C# Mapi API - ResourceFlags.cs
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

namespace NMapi.Flags {


	/// <summary>
	///  Flags that can be set for the property tag Property.ResourceFlags.
	/// </summary>
	[Flags]
	public enum ResourceFlags
	{
		
		//
		// Message services
		//
		
		/// <summary>
		///  Indicates, that the provider includes the default store.
		/// </summary>
		Service_DefaultStore = 0x0001,

		/// <summary>
		///  The provider may not be copied into a profile more than once.
		/// </summary>
		Service_SingleCopy = 0x0002,

		/// <summary>
		///  ?
		/// </summary>
		Service_CreateWithStore = 0x0004,

		/// <summary>
		///  Indicates, that the store can act as the primary identity supplier.
		/// </summary>
		Service_PrimaryIdentity = 0x0008,

		/// <summary>
		///  Indicates, that the store can not act as the primary identity supplier.
		/// </summary>
		Service_NoPrimaryIdentity = 0x0020,
		
	
	
	
	
	
		//
		// Service Providers
		//
	
	
		/// <summary>
		///  Static. The spooler hook needs to process inbound messages.
		/// </summary>
		Hook_Inbound = 0x00000200,
		
		
		
		/// <summary>
		///  Static. The spooler hook needs to process outbound messages.
		/// </summary>
		Hook_Outbound = 0x00000400,





		///
		/// Status TABLES (still in "Service Providers" section)
		///



		/// <summary>
		///  Static. The spooler hook needs to process outbound messages.
		/// </summary>
		Status_DefaultOutbound = 0x00000001,
		
		/// <summary>
		///  Modifiable. This message store is the default store for the profile.
		/// </summary>
		Status_DefaultStore = 0x00000002,

		/// <summary>
		///  Dynamic. The standard folders in this message store, including the interpersonal 
		///  message (IPM) root folder, have not yet been verified. MAPI sets and clears this flag.
		/// </summary>
		Status_NeedIpmTree = 0x00000800,

		/// <summary>
		///  Modifiable. This message store is to be used when a client application logs on. 
		//   Once opened, this store should be set as the default store for the profile.
		/// </summary>
		Status_PrimaryStore = 0x00001000,
		
		/// <summary>
		///  Modifiable. This message store is to be used if the primary store is not available 
		///  when a client application logs on. Once opened, this store should be set as the default store for the profile.
		/// </summary>
		Status_SecondaryStore = 0x00002000,
		
		/// <summary>
		///  Modifiable. This provider furnishes the primary identity for the session; 
		///  the entry identifier for the object furnishing the identity is returned from IMAPISession::QueryIdentity. Either this flag or STATUS_NO_PRIMARY_IDENTITY must be set.
		/// </summary>
		Status_PrimaryIdentity = 0x00000004,
		
		/// <summary>
		///  Dynamic. This message store will be used by Simple MAPI as its default message store.
		/// </summary>
		Status_SimpleStore = 0x00000008,
		
		/// <summary>
		///  Static. This transport expects to be the last transport selected to send a message 
		///   when multiple transport providers are able to transmit the message.
		/// </summary>
		Status_XpPreferLast = 0x00000010,
		
		/// <summary>
		///  Static. This provider does not furnish an identity in its status row. Either this 
		//   flag or STATUS_PRIMARY_IDENTITY must be set.
		/// </summary>
		Status_NoPrimaryIdentity = 0x00000020,
		
		/// <summary>
		///  Static. This message store is incapable of becoming the default message store for the profile.
		/// </summary>
		Status_NoDefaultStore = 0x00000040,
		
		/// <summary>
		///  Dynamic. This message store should not be published in the message store table and will 
		//  be deleted from the profile after logoff.
		/// </summary>
		Status_TempSection = 0x00000080,
		
		/// <summary>
		///  Static. This transport provider is tightly coupled with a message store and 
		//  furnishes the PR_OWN_STORE_ENTRYID (PidTagOwnStoreEntryId) property in its status row.
		/// </summary>
		Status_OwnStore = 0x00000100
		

	}

}
