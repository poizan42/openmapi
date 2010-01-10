//
// openmapi.org - NMapi C# Mapi API - IMsgStoreExtender.cs
//
// Copyright 2009-2010 Topalis AG
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
		/// <param name="store">An IMsgStore object.</param>
		/// <param name="entryId1">The EntryId of the first object.</param>
		/// <param name="entryId2">The EntryId of the second object.</param>
		/// <returns>True if both ids refer to the same object, false otherwise.</returns>
		/// <exception cref="MapiUnknownEntryIdException"></exception>
		public static bool CompareEntryIDs (this IMsgStore store, 
			EntryId entryId1, EntryId entryId2)
		{
			return (store.CompareEntryIDs (entryId1.ToByteArray (), 
						entryId2.ToByteArray (), 0) ) != 0;
		}
		
		/// <summary>
		///  
		/// </summary>
		public static IMessage OpenMessage (this IMsgStore store, EntryId entryId)
		{
			return store.OpenEntry<IMessage> (entryId);
		}
		
		/// <summary>
		///  
		/// </summary>
		public static IMapiFolder OpenFolder (this IMsgStore store, EntryId entryId)
		{
			return store.OpenEntry<IMapiFolder> (entryId);
		}
		
		/// <summary>
		///  
		/// </summary>
		public static T OpenEntry<T> (this IMsgStore store, EntryId entryId)
		{
			IBase obj = store.OpenEntry (entryId, 0);
			if (obj is T)
				return (T) obj;
			throw new MapiInvalidParameterException (
					"object is not of type " + typeof (T) + ".");
		}
		
		/// <summary>
		///  
		/// </summary>
		public static IBase OpenEntry (this IMsgStore store, EntryId entryId)
		{
			return store.OpenEntry (entryId, 0);
		}
		
		/// <summary>
		///  Deprecated, because uses flags.
		/// </summary>
		public static IBase OpenEntry (this IMsgStore store, EntryId entryId, int flags)
		{
			return store.OpenEntry (entryId.ToByteArray (), null, flags);
		}

		/// <summary>
		///  Convenience method to access the default inbox folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    inbox folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenInboxFolder (this IMsgStore store)
		{
			var result = store.GetReceiveFolder (null, 0);
			if (result == null)
				return null;
			OpaqueEntryId entryId = new OpaqueEntryId (result.EntryID);
			return store.OpenFolder (entryId);
		}
		
		/// <summary>
		///  Convenience method to access the default subtree folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    subtree folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenSubtreeFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, Property.Typed.IpmSubtreeEntryId);
		}
		
		/// <summary>
		///  Convenience method to access the default wastebasket folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    wastebasket folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenWastebasketFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, Property.Typed.IpmWastebasketEntryId);
		}
		
		/// <summary>
		///  Convenience method to access the default outbox folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    outbox folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenOutboxFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, Property.Typed.IpmOutboxEntryId);
		}

		/// <summary>
		///  Convenience method to access the default sentmail folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    sentmail folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenSentmailFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, Property.Typed.IpmSentmailEntryId);
		}
		
		/// <summary>
		///  Convenience method to access the default views folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    views folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenViewsFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, Property.Typed.ViewsEntryId);
		}
		
		/// <summary>
		///  Convenience method to access the default common views folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    common views folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenCommonViewsFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, Property.Typed.CommonViewsEntryId);
		}
		
		/// <summary>
		///  Convenience method to access the default finder folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    finder folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenFinderFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, Property.Typed.FinderEntryId);
		}
		
		/// <summary>
		///  Convenience method to access the default appointment folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    appointment folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenAppointmentFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, OutlookProperty.Typed.IPM_APPOINTMENT_ENTRYID);
		}
		
		/// <summary>
		///  Convenience method to access the default contact folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    contact folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenContactFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, OutlookProperty.Typed.IPM_CONTACT_ENTRYID);
		}
		
		/// <summary>
		///  Convenience method to access the default drafts folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    drafts folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenDraftsFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, OutlookProperty.Typed.IPM_DRAFTS_ENTRYID);
		}
		
		/// <summary>
		///  Convenience method to access the default journal folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    journal folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenJournalFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, OutlookProperty.Typed.IPM_JOURNAL_ENTRYID);
		}
		
		/// <summary>
		///  Convenience method to access the default note folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    note folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenNoteFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, OutlookProperty.Typed.IPM_NOTE_ENTRYID);
		}
		
		/// <summary>
		///  Convenience method to access the default task folder. 
		/// </summary>
		/// <remarks>
		///  As with all objects opened with OpenEntry (), you are responsible 
		///  for closing or disposing the returned object when you are done with it.
		/// </remarks>
		/// <returns>
		///   A reference to a new folder object representing the default 
		//    task folder or null if the folder can't be determinded.
		/// </returns>
		public static IMapiFolder OpenTaskFolder (this IMsgStore store)
		{
			return OpenLinkedFolder (store, OutlookProperty.Typed.IPM_TASK_ENTRYID);
		}
		
		private static IMapiFolder OpenLinkedFolder (IMsgStore store, 
			PropertyTag entryIdLinkProperty)
		{
			// TODO: Performance is not great, probably...
			using (IMapiFolder root = (IMapiFolder) store.Root) {
				PropertyValue entryIdProp = root.GetProperty (entryIdLinkProperty);
				if (! (entryIdProp is BinaryProperty) )
					return null;
				OpaqueEntryId entryId = new OpaqueEntryId ((byte[]) entryIdProp);
				try {
					return store.OpenFolder (entryId);
				} catch (MapiNotFoundException) {
					// ignore
				}
				return null;
			}
		}		

		/// <summary>
		///  
		/// </summary>
		/// <param name="store">An IMsgStore object.</param>
		/// <param name="path"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public static IMapiFolder OpenIpmFolder (this IMsgStore store, string path, int flags)
		{
			// Copyright 2008 VIPcom AG
			// Copyright 2010 Topalis AG
			
			if (path == null)
				path = "/";

			byte[] eidRoot = (byte[]) store.GetProperty (Property.Typed.IpmSubtreeEntryId);
			IMapiFolder folder = (IMapiFolder) store.OpenEntry (eidRoot, null, flags);
			if (path == "/")
				return folder;

			bool found = false;
			string [] paths = path.Split ('/');		
			try {

				foreach (string match in paths) {
					IMapiTableReader tableReader = null;
					bool first = true;
					int idx_name = -1, idx_eid = -1;

					try {
						found = false;
						tableReader = folder.GetHierarchyTable ();
						while (!found) {

							RowSet rows = tableReader.GetRows (10);
							if (rows.ARow.Length == 0)
								break;

							for (int k = 0; k < rows.ARow.Length; k++) {
								PropertyValue [] prps = rows.ARow [k].lpProps;

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
									folder = (IMapiFolder) store.OpenEntry ((byte[]) eid, null, flags);
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
