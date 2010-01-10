//
// openmapi.org - NMapi C# Mapi API - GroupwiseItemType.cs
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
		public enum GroupwiseItemType
		{
			// Mail 
			Mail = 0x00000001,
			
			// Note 
			Note = 0x00000002,
			
			// Task 
			Todo = 0x00000004,
			
			// Appointment 
			Appointment = 0x00000008,
			
			// Phone message 
			Phone = 0x00000010,
			
			// Busy search 
			Search = 0x00000020,
			
			// Profile 
			Profile = 0x00002000,
			
			// ODMA reference 
			OdmaReference = 0x00004000,
			
			// Independent Service Vendor object 
			ISVObject = 0x00010000,
			
			// Workflow
			WorkFlow = 0x00020000

		}


}
