//
// openmapi.org - NMapi C# Mapi API - IMapiFolderExtender.cs
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

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi {

	/// <summary>
	///  Provides some extension methods on IMapiFolder.
	/// </summary>
	public static class IMapiFolderExtender
	{
		
		
		
		/*
		CreateMessage
		CopyMessages
		DeleteMessages
		CreateFolder
		CopyFolder
		DeleteFolder
		SetReadFlags
		GetMessageStatus
		SetMessageStatus
		SaveContentsSort
		EmptyFolder
		
		*/
		
				
		// TODO: return EntryId[] 
		
		//
		// TODO: This method might or might not work. 
		//      TXC does not seem to implement Message copying, so its untested.
		//
		
		/// <summary>
		///  
		/// </summary>
		/// <returns></returns>
		public static SBinary[] CopyMessagesAndIdentify (this IMapiFolder folder, EntryList msgList, 
			NMapiGuid interFace, IMapiFolder destFolder, IMapiProgress progress, int flags)
		{
			if ((flags & NMAPI.MAPI_MOVE) != 0)
				throw new MapiNoSupportException ("move flag not supported.");
			
			IMessage[] messages = new IMessage [msgList.Bin.Length];
			SBinary[] entryIds = new SBinary [msgList.Bin.Length];
			IMapiTable table = null;
			try {
				
				//
				// The FindByProp method is potentially very expensive, depending 
				// on the implementation of the backend ... 
				//
		
				StringMapiNameId copyIdentifierProp = new StringMapiNameId ();
				copyIdentifierProp.Guid = Guids.PS_PUBLIC_STRINGS; // TODO: OpenMapi.Guid;
				copyIdentifierProp.StrName = "_copy_identifier";

				// TODO: for each entry: check if mapping signature is the same ....
				IntPropertyTag markerTag = (IntPropertyTag) folder.GetIDsFromNames (
					new MapiNameId[] { copyIdentifierProp }, 0) [0].AsType (PropertyType.Int32);

			
				int[] identifiers = new int [msgList.Bin.Length];
				
				for (int i=0; i < msgList.Bin.Length; i++) {
					var message = (IMessage) folder.OpenEntry (msgList.Bin [i].ByteArray, null, Mapi.Modify);
					identifiers [i] = folder.PutMarker (message, markerTag);
					messages [i] = message;
				}
			
				
				// TODO: we need to check if this failed ...
				
				folder.CopyMessages (msgList, interFace, destFolder, progress, flags);
				
				// We need to delete the markers, to ensure uniqueness, in case the 
				// destination and source folder are equal.
				for (int i=0; i < msgList.Bin.Length; i++)
					folder.DeleteMarker (messages [i], markerTag);


				// FindMarkers ...

				table = (IMapiTable) destFolder.GetNormalContentsTable ();
				table.SetColumns (PropertyTag.ArrayFromIntegers (Property.EntryId, markerTag.Tag), 0);
			
				// Expensive!!!
				for (int i=0; i < msgList.Bin.Length; i++)
					entryIds [i] = FindByMarker (table, markerTag, identifiers [i]);
				
				
				// TODO: Delete marker on new objects .... !!					
			}
			finally {
				foreach (var message in messages)
					message.Close ();
				if (table != null)
					table.Close ();
			}
			
			return entryIds;
		}
		
		/// <summary>
		///  Sets a new marker on an object and returns the (random) value of the marker.
		/// </summary>
		private static int PutMarker (this IMapiFolder folder, IMessage message, IntPropertyTag tag)
		{
			int marker = new Random ().Next ();
			IntProperty prop = tag.CreateValue (marker);
			message.SetProperties (prop);
			message.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
			return marker;
		}
		
		/// <summary>
		///  Deletes a marker from an object.
		/// </summary>
		private static void DeleteMarker (this IMapiFolder folder, IMessage message, PropertyTag tag)
		{
			message.DeleteProperties (tag);
			message.SaveChanges ();
		}
		
		/// <summary>
		///  Find a message using a marker property tag. If the message can 
		///  not be found, null is returned.
		/// </summary>
		private static SBinary FindByMarker (IMapiTable table, IntPropertyTag tag, int value)
		{
			try {
				IntProperty prop = tag.CreateValue (value);
				PropertyRestriction res = new PropertyRestriction (RelOp.Equal, prop);
				table.FindRow (res, Bookmark.Beginning, 0); // Potentially requires a full table scan. 
			
				RowSet rows = table.QueryRows (1, 0);
				if (rows != null && rows.ARow.Length == 1) {
					foreach (var current in rows.ARow [0].Props)
						if (current.PropTag == Property.EntryId)
							return (SBinary) current;
				}
			} catch (Exception e) {
				// suppress exception...
			}
			return null;		
		}

	}

}
