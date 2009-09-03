//
// openmapi.org - NMapi C# Mapi API - IMsgStoreExtender.cs
//
// Copyright 2009 Topalis AG
// Copyright 2008 VIPcom AG
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
	///  Provides some extension methods on IMsgStore.
	/// </summary>
	public static class IMsgStoreExtender
	{
		
		/// <summary>
		///  Checks if two EntryIds refer to the same object.
		/// </summary>
		/// <param name="store">A IMsgStore object.</param>
		/// <param name="entryId1">The EntryId of the first object.</param>
		/// <param name="entryId2">The EntryId of the second object.</param>
		/// <returns>True if both ids refer to the same object, false otherwise.</returns>
		/// <exception cref="MapiUnknownEntryIdException"></exception>
		public static bool CompareEntryIDs (this IMsgStore store, EntryId entryId1, EntryId entryId2)
		{
			return ( store.CompareEntryIDs (entryId1.ToByteArray (), entryId2.ToByteArray (), 0) ) != 0;
		}
		
		/*
		/// <summary>
		///  
		/// </summary>
		public static IBase OpenEntry (this IMsgStore store)
		{
			
		}
		*/

		

		/// <summary>
		///  
		/// </summary>
      /// <param name="store">A IMsgStore object.</param>
		/// <param name="path"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public static IMapiFolder OpenIpmFolder (this IMsgStore store, string path, int flags)
		{
			if (path == null)
				path = "/";

			byte[] eidRoot = (byte[]) store.GetProperty (Property.Typed.IpmSubtreeEntryId);
			IMapiFolder folder = (IMapiFolder) store.OpenEntry (eidRoot, null, flags);
			if (path == "/")
				return folder;

			bool found = false;
			string [] paths = path.Split ('/');		
			try {

				for (int i = 1; i < paths.Length; i++) {
					IMapiTableReader tableReader = null;
					bool first = true;
					int idx_name = -1, idx_eid = -1;
					string  match = paths[i];

					try {
						found = false;
						tableReader = folder.GetHierarchyTable ();
						while (!found) {

							RowSet rows = tableReader.GetRows (10);
							if (rows.ARow.Length == 0)
								break;

							for (int idx = 0; idx < rows.ARow.Length; idx++) {
								PropertyValue [] prps = rows.ARow [idx].lpProps;


								if (first) {
									first = false;
									idx_name = PropertyValue.GetArrayIndex (
										prps, Property.DisplayNameW);

									idx_eid  = PropertyValue.GetArrayIndex (
										prps, Property.EntryId);

								}

								PropertyValue name = PropertyValue.GetArrayProp (prps, idx_name);

								BinaryProperty eid = (BinaryProperty) PropertyValue.GetArrayProp (prps, idx_eid);
								if (name != null && ((UnicodeProperty) name).Value == match) {
									folder = (IMapiFolder) store.OpenEntry (eid.Value.lpb, null, flags);
									found = true;
									break;
								}
							}
						}
						if (!found)
							throw new MapiNotFoundException ();
					}
					finally {
						if (tableReader != null)
							tableReader.Close ();
					}
				}
			}
			finally {
				if (!found && folder != null) {
					folder.Close ();
					folder = null;
				}
			}
			return folder;
		}

	}

}
