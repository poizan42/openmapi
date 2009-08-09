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
		///  
		/// </summary>
		Service_DefaultStore = 0x0001,

		/// <summary>
		///  
		/// </summary>
		Service_SingleCopy = 0x0002,

		/// <summary>
		///  
		/// </summary>
		Service_CreateWithStore = 0x0004,

		/// <summary>
		///  
		/// </summary>
		Service_PrimaryIdentity = 0x0008,

		/// <summary>
		///  
		/// </summary>
		Service_NoPrimaryIdentity = 0x0020,
		
	
	
	
	
	
		//
		// Service Providers
		//
	
	
		/// <summary>
		///  
		/// </summary>
		Hook_Inbound = 0x00000200,
		
		
		
		/// <summary>
		///  
		/// </summary>
		Hook_Outbound = 0x00000400,





		///
		/// Status TABLES (still in "Service Providers" section)
		///



		/// <summary>
		///  
		/// </summary>
		Status_DefaultOutbound = 0x00000001,
		
		/// <summary>
		///  
		/// </summary>
		Status_DefaultStore = 0x00000002,

		/// <summary>
		///  
		/// </summary>
		Status_NeedIpmTree = 0x00000800,

		/// <summary>
		///  
		/// </summary>
		Status_PrimaryStore = 0x00001000,
		
		/// <summary>
		///  
		/// </summary>
		Status_SecondaryStore = 0x00002000,
		
		/// <summary>
		///  
		/// </summary>
		Status_PrimaryIdentity = 0x00000004,
		
		/// <summary>
		///  
		/// </summary>
		Status_SimpleStore = 0x00000008,
		
		/// <summary>
		///  
		/// </summary>
		Status_XpPreferLast = 0x00000010,
		
		/// <summary>
		///  
		/// </summary>
		Status_NoPrimaryIdentity = 0x00000020,
		
		/// <summary>
		///  
		/// </summary>
		Status_NoDefaultStore = 0x00000040,
		
		/// <summary>
		/// 
		/// </summary>
		Status_TempSection = 0x00000080,
		
		/// <summary>
		///  
		/// </summary>
		Status_OwnStore = 0x00000100
		

	}

}
