//
// openmapi.org - NMapi C# Mapi API - GroupwisePhoneFlags.cs
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
		public enum GroupwisePhoneFlags
		{
			// Called check box 
			Called = 0x0001,

			// Please Call check box 
			PleaseCall = 0x0002,

			// Will Call check box 
			WillCall = 0x0004,

			// Returned Your Call check box 
			ReturnedYourCall = 0x0008,

			// Wants To See You check box 
			WantsToSeeYou = 0x0010,

			// Came to See You check box 
			CameToSeeYou = 0x0020,

			// Urgent check box
			Urgent = 0x0040

		}

}
