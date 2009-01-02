//
// openmapi.org - NMapi C# Mapi API - TypeHelper.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;
using System.Diagnostics;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.Server {

	public static class TypeHelper
	{
		public static int GetMapiType (object queriedObject)
		{
			int MAPI_STORE = 0x00000001;    // Message Store 
			//#define MAPI_ADDRBOOK   ((ULONG) 0x00000002)    // Address Book 
			int MAPI_FOLDER = 0x00000003;    // Folder 
			//#define MAPI_ABCONT     ((ULONG) 0x00000004)    // Address Book Container 
			int MAPI_MESSAGE = 0x00000005;    // Message 
			//#define MAPI_MAILUSER   ((ULONG) 0x00000006)    // Individual Recipient 
			int MAPI_ATTACH = 0x00000007;    // Attachment 
			//#define MAPI_DISTLIST   ((ULONG) 0x00000008)    // Distribution List Recipient 
			//#define MAPI_PROFSECT   ((ULONG) 0x00000009)    // Profile Section 
			//#define MAPI_STATUS     ((ULONG) 0x0000000A)    // Status Object 
			//#define MAPI_SESSION    ((ULONG) 0x0000000B)    // Session 
			//#define MAPI_FORMINFO   ((ULONG) 0x0000000C)    // Form Information 
			

			// umapi types

			int MAPI_TBLDATA			= 0x00000100; // exported
			int MAPI_EVSUB				= 0x00000101; // exported
			int MAPI_EVDATA				= 0x00000102; // server
			int MAPI_SIMPLESTREAM		= 0x00000103; // exported
			int MAPI_TABLE				= 0x00000104; // exported
			int MAPI_ADMIN              = 0x00000105; // admin interface
			int MAPI_RECIP              = 0x00000106; // server
			int MAPI_FOLDERHELPER       = 0x00000107; // server
			// int MAPI_MVPHELPER       =    0x00000108; // server
			int MAPI_SORTHELPER         = 0x00000109; // server
			int MAPI_SEARCHSTORE        = 0x0000010a; // server
			int MAPI_SEARCHFOLDER       = 0x0000010b; // server
			int MAPI_STREAMDATA         = 0x0000010c; // server
			int MAPI_MODIFYTABLE        = 0x0000010d; // exported

			Trace.WriteLine ("GetType: " + queriedObject);			

			int typeID = -1;

			if (queriedObject is IMsgStore)
				typeID = MAPI_STORE;
			else if (queriedObject is IMapiFolder)
				typeID = MAPI_FOLDER;
			else if (queriedObject is IMessage)
				typeID = MAPI_MESSAGE;
			else if (queriedObject is IAttach)
				typeID = MAPI_ATTACH;
			else if (queriedObject is IMapiTableReader)
				typeID = MAPI_TBLDATA;		/// TXC thingie ...
//			else if (queriedObject is )
//				typeID = MAPI_EVSUB;
//			else if (queriedObject is )
//				typeID = MAPI_EVDATA;
			else if (queriedObject is IStream)
				typeID = MAPI_SIMPLESTREAM;
			else if (queriedObject is IMapiTable)
				typeID = MAPI_TABLE;
//			else if (queriedObject is )
//				typeID = MAPI_ADMIN;
//			else if (queriedObject is )
//				typeID = MAPI_RECIP;
//			else if (queriedObject is )
//				typeID = MAPI_FOLDERHELPER;
//			else if (queriedObject is )
//				typeID = MAPI_SORTHELPER;
//			else if (queriedObject is )
//				typeID = MAPI_SEARCHSTORE;
//			else if (queriedObject is )
//				typeID = MAPI_SEARCHFOLDER;
//			else if (queriedObject is )
//				typeID = MAPI_STREAMDATA;
//			else if (queriedObject is )
//				typeID = MAPI_MODIFYTABLE;

			return typeID;
		}
	}
}
