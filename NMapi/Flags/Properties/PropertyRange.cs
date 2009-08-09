//
// openmapi.org - NMapi C# Mapi API - PropertyRange.cs
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
	///  Property IDs are usually part of a certain range that 
	///  defines if the property should be transmitted by a transport 
	///  provider and also divides the whole set of possible IDs into subsets  
	///  which are used by different MAPI components to avoid conflicts.
	/// </summary>
	public enum PropertyRange
	{
		
		/// <summary>
		///  
		/// </summary>
		Core_Envelope,
		
		/// <summary>
		///  
		/// </summary>
		Core_PerRecipient,
	
		/// <summary>
		///  
		/// </summary>
		Core_NonTransmittable,
		
		/// <summary>
		///  
		/// </summary>
		Core_MessageContent,

		/// <summary>
		///  
		/// </summary>
		Core_System_CommonProperties,
	
		/// <summary>
		///  
		/// </summary>
		Core_System_MessageStoreObject,
	
		/// <summary>
		///  
		/// </summary>
		Core_System_FolderOrAbContainer,

		/// <summary>
		///  
		/// </summary>
		Core_System_Attachment,

		/// <summary>
		///  
		/// </summary>
		Core_System_AddressBookObject,
	
		/// <summary>
		///  
		/// </summary>
		Core_System_MailUser,
	
		/// <summary>
		///  
		/// </summary>
		Core_System_DistributionList,
	
		/// <summary>
		///  
		/// </summary>
		Core_System_ProfileSection,
	
		/// <summary>
		///  
		/// </summary>
		Core_System_StatusObject,
	
		/// <summary>
		///  
		/// </summary>
		CustomTransportEnvelopeTransmitted,

		/// <summary>
		///  
		/// </summary>
		RecipientAssociatedNotTransmitted,

		/// <summary>
		///  
		/// </summary>
		CustomNotTransmitted,

		/// <summary>
		///  
		/// </summary>
		ServiceProviderNotTransmitted,

		/// <summary>
		///  
		/// </summary>
		MessageClassTransmitted,

		/// <summary>
		///  
		/// </summary>
		MessageClassNotTransmitted,

		/// <summary>
		///  Indicates that the property tag is a named property.
		/// </summary>
		NamedTransmitted,

		/// <summary>
		///  The property id is not in any of the known property ranges.
		///  Do any properties exist in this range?
		/// </summary>
		Unknown
	}


}
