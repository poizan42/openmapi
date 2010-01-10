//
// openmapi.org - NMapi C# Mapi API - Guids.cs
//
// Copyright 2008-2009 Topalis AG
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
	
	// TODO: This class should be removed by merging the stuff elsewhere.
	
	public static class Guids
	{
		public static NMapiGuid DefineGuid (int  doubleWord, short word1, 
			short word2, params byte[] bytes)
		{
			if (bytes.Length != 8)
				throw new ArgumentException ("Length of 'bytes' must be 8.");
			NMapiGuid ret = new NMapiGuid ();
			ret.Data1 = doubleWord;
			ret.Data2 = word1;
			ret.Data3 = word2;
			ret.Data4 = new byte[8];
			ret.Data4 [0] = bytes [0];
			ret.Data4 [1] = bytes [1];
			ret.Data4 [2] = bytes [2];
			ret.Data4 [3] = bytes [3];
			ret.Data4 [4] = bytes [4];
			ret.Data4 [5] = bytes [5];
			ret.Data4 [6] = bytes [6];
			ret.Data4 [7] = bytes [7];
			return ret;
		}

		public static NMapiGuid DefineOleGuid (int doubleWord, 
			short word1, short word2)
		{
			return DefineGuid (doubleWord, word1, word2, 
				(byte) 0xC0, (byte) 0, (byte) 0, (byte) 0, 
				(byte) 0, (byte) 0, (byte) 0, (byte) 0x46);
		}



		//  NamedProperty Set: The name of MAPI's property set
		public static readonly NMapiGuid PS_MAPI = DefineOleGuid (0x00020328, 0, 0);

		/// <summary>Named-Property Namespace Guid for calendar-related properties.</summary>
		public static readonly NMapiGuid PSETID_Appointment = DefineOleGuid (0x00062002, 0, 0);

		/// <summary>Named-Property Namespace Guid for task-related properties.</summary>
		public static readonly NMapiGuid PSETID_Task = DefineOleGuid (0x00062003, 0, 0);

		/// <summary>Named-Property Namespace Guid for contact-related properties.</summary>
		public static readonly NMapiGuid PSETID_Address = DefineOleGuid (0x00062004, 0, 0);

		/// <summary></summary>
		public static readonly NMapiGuid PSETID_Common = DefineOleGuid (0x00062008, 0, 0);

		/// <summary></summary>
		public static readonly NMapiGuid PSETID_Log = DefineOleGuid (0x0006200A, 0, 0);

		/// <summary>Presumably the space for arbitrary Internet-EMail-Headers ...</summary>
		public static readonly NMapiGuid PS_INTERNET_HEADERS = DefineOleGuid (0x00020386, 0, 0);

		//  NamedProperty Set: TThe name of the set of public strings
		public static readonly NMapiGuid PS_PUBLIC_STRINGS = DefineOleGuid (0x00020329, 0, 0);

		public static readonly NMapiGuid PS_ROUTING_EMAIL_ADDRESSES = DefineOleGuid (0x00020380, 0, 0);
		public static readonly NMapiGuid PS_ROUTING_ADDRTYPE = DefineOleGuid (0x00020381, 0, 0);
		public static readonly NMapiGuid PS_ROUTING_DISPLAY_NAME = DefineOleGuid (0x00020382, 0, 0);
		public static readonly NMapiGuid PS_ROUTING_ENTRYID = DefineOleGuid (0x00020383, 0, 0);
		public static readonly NMapiGuid PS_ROUTING_SEARCH_KEY = DefineOleGuid (0x00020384, 0, 0);
		
		public static readonly NMapiGuid MUID_PROFILE_INSTANCE = DefineOleGuid (0x00020385, 0, 0);



	}

}
