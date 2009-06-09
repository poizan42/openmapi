//
// openmapi.org - NMapi C# Mapi API - MapiObjectType.cs
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
	///  
	/// </summary>
	public static class MapiObjectType
	{
		public const int Store    = 0x00000001;
		public const int AddrBook = 0x00000002;
		public const int Folder   = 0x00000003;
		public const int AbCont   = 0x00000004;
		public const int Message  = 0x00000005;
		public const int MailUser = 0x00000006;
		public const int Attach   = 0x00000007;
		public const int DistList = 0x00000008;
		public const int ProfSect = 0x00000009;
		public const int Status   = 0x0000000A;
		public const int Session  = 0x0000000B;
		public const int FormInfo = 0x0000000C;
		
		
		//
		// Internal
		//
		
		public const int Tbldata      = 0x00000100;
		public const int TableReader  = Tbldata;
		public const int Evsub        = 0x00000101;
		public const int SimpleStream = 0x00000103;
		public const int Table	      = 0x00000104;
		public const int ModifyTable  = 0x0000010d;
		public const int MsgSync      = 0x0000010e;
		public const int FldSync      = 0x0000010f;
		public const int MsgImp       = 0x00000110;
		public const int FldImp       = 0x00000111;

/*
		// internal2

		int MAPI_EVDATA				= 0x00000102; // server
		int MAPI_ADMIN              = 0x00000105; // admin interface
		int MAPI_RECIP              = 0x00000106; // server
		int MAPI_FOLDERHELPER       = 0x00000107; // server
		// int MAPI_MVPHELPER       =    0x00000108; // server
		int MAPI_SORTHELPER         = 0x00000109; // server
		int MAPI_SEARCHSTORE        = 0x0000010a; // server
		int MAPI_SEARCHFOLDER       = 0x0000010b; // server
		int MAPI_STREAMDATA         = 0x0000010c; // server
*/


	}


}
