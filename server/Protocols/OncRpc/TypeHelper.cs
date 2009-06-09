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
			Trace.WriteLine ("GetType: " + queriedObject);			

			int typeID = -1;

			if (queriedObject is IMsgStore)
				typeID = MapiObjectType.Store;
			else if (queriedObject is IMapiFolder)
				typeID = MapiObjectType.Folder;
			else if (queriedObject is IMessage)
				typeID = MapiObjectType.Message;
			else if (queriedObject is IAttach)
				typeID = MapiObjectType.Attach;
			else if (queriedObject is IMapiTableReader)
				typeID = MapiObjectType.Tbldata;		/// TXC thingie ...
//			else if (queriedObject is )
//				typeID = MAPI_EVSUB;
//			else if (queriedObject is )
//				typeID = MAPI_EVDATA;
			else if (queriedObject is IStream)
				typeID = MapiObjectType.SimpleStream;
			else if (queriedObject is IMapiTable)
				typeID = MapiObjectType.Table;
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
			else if (queriedObject is IModifyTable)
				typeID = MapiObjectType.ModifyTable;

			return typeID;
		}
	}
}
