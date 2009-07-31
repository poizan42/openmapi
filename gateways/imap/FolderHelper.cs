// openmapi.org - NMapi C# IMAP Gateway - FolderHelper.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;
using System.Threading;
using System.Diagnostics;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.DirectoryModel;
using System.Security.Cryptography;

namespace NMapi.Gateways.IMAP {
	
	
	public class FolderHelper : IDisposable
	{

		private ServerConnection servCon;
		
		
		private IMapiFolder currentFolder;
		private IMapiTable currentFolderTable;
		private string currentPath;
		private SBinary currentFolderEntryId;
		private SequenceNumberList sequenceNumberList;
		private	long uidNext = 0;
		private	long uidValidity = 0;
		private int uidNextTag = 0;
		private int uidValidityTag = 0;
		private static int uidPropTag = 0;
		private static int uidPathPropTag = 0;
		private static int uidEntryIdPropTag = 0;
		private static int additionalFlagsPropTag = 0;


		public FolderHelper (ServerConnection servCon, string path) {
			this.servCon = servCon;

			ChangeDir (path); // Change to root dir
			sequenceNumberList = new SequenceNumberList ();

		}

		/// <summary>
		/// 
		/// </summary>
		public string CurrentPath {
			get { return currentPath; }
		}
			
		/// <summary>
		/// 
		/// </summary>
		public SBinary CurrentFolderEntryId {
			get { return currentFolderEntryId; }
		}

		/// <summary>
		/// 
		/// </summary>
		public long UIDNEXT {
			get { return uidNext; }
		}

		/// <summary>
		/// 
		/// </summary>
		public long UIDVALIDITY {
			get { return uidValidity; }
		}

		public SequenceNumberList SequenceNumberList { 
			get { return sequenceNumberList; }
			set { sequenceNumberList = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public IMapiFolder CurrentFolder {
			get { return currentFolder; }
		}

		/// <summary>
		/// 
		/// </summary>
		public IMapiTable CurrentFolderTable {
			get { return currentFolderTable; }
		}

		public static int AdditionalFlagsPropTag {
			get { return additionalFlagsPropTag; }
		}

		public static int UIDPropTag {
			get { 
				if (uidPropTag == 0)
					throw new Exception ("Use only, if select has been executed before in the session");
				return uidPropTag; 
			}
		}
		
		
		internal object _SharedGetSubDir (IMapiContainer parent, string match, 
			Func<IMapiContainer, SBinary, object> action)
		{
			IMapiTableReader tableReader = null;
			try {
				tableReader = parent.GetHierarchyTable (Mapi.Unicode);
			} catch (MapiException e) {
				if (e.HResult == Error.NoSupport)
					return null;
				throw;
			}

			while (true) {
				RowSet rows = tableReader.GetRows (50);
				if (rows.Count == 0)
					break;

				int nameIndex = -1;
				int entryIdIndex = -1;
				foreach (Row row in rows) {
					if (nameIndex == -1) {
						nameIndex = PropertyValue.GetArrayIndex (row.Props, Property.DisplayNameW);
						entryIdIndex  = PropertyValue.GetArrayIndex (row.Props, Property.EntryId);
					}
				
					UnicodeProperty name = (UnicodeProperty) PropertyValue.GetArrayProp (row.Props, nameIndex);
					BinaryProperty eid  = (BinaryProperty) PropertyValue.GetArrayProp (row.Props, entryIdIndex);

					if (name != null && name.Value == match)
						return action (parent, eid.Value);
				}
			}
			return null;
		}

		public IMapiContainer GetSubDir (IMapiContainer parent, string match)
		{
			if (parent == null)
				return null;
			Func<IMapiContainer, SBinary, object> action = (prnt, entryId) => {
				return (IMapiContainer) prnt.OpenEntry (
					entryId.ByteArray, null, Mapi.Modify);
			};

			return (IMapiContainer) _SharedGetSubDir (parent, match, action);
		}

		public SBinary GetSubDirEntryId (IMapiContainer parent, string match)
		{
			if (parent == null)
				return null;
			Func<IMapiContainer, SBinary, object> action = (prnt, entryId) => entryId;

			return (SBinary) _SharedGetSubDir (parent, match, action);
		}

		public IMapiFolder OpenFolder (string path)
		{
			if (path [0] != PathHelper.PathSeparator)
				throw new Exception ("path must start with '/'.");
			string[] parts = PathHelper.Path2Array (servCon.RootDir + path);

			if (path == string.Empty)
				return (IMapiFolder) servCon.Store.Root;

			IMapiContainer container = (IMapiContainer) servCon.Store.Root;

			foreach (string part in parts) {
				if (container == null)
					break;
				container = GetSubDir (container, part);
			}
			return (IMapiFolder) container; // TODO: Container? Folder?
		}

		public string Input2AbsolutePath (string input)
		{
			string path = null;
			string relPath = input;
			if (relPath [0] == PathHelper.PathSeparator)
				path = PathHelper.ResolveAbsolutePath (relPath);
			else
				path = PathHelper.Combine (currentPath, relPath);
			return path;
		}

		public bool ChangeDir (string path)
		{
			servCon.State.Log ("ChangeDir: " + path);
			if (!servCon.CheckStore())
				return false;
			IMapiFolder newFolder = null;
servCon.State.Log ("changedir0");
			newFolder = OpenFolder (path);
servCon.State.Log ("changedir1");
			if (newFolder == null) {
				servCon.State.Log ("cd: " + path + ": No such folder.");
				return false;
			}
			currentFolder = newFolder;
			currentPath = path;
			currentFolderTable = null;

			MapiPropHelper mph = new MapiPropHelper (currentFolder);
servCon.State.Log ("changedir2");
			BinaryProperty eid = (BinaryProperty) mph.HrGetOneProp (Property.EntryId);
servCon.State.Log ("changedir3");
			currentFolderEntryId = (eid != null) ? eid.Value : null;

			if (currentFolderEntryId == null)
				throw new Exception ("ServerConnection.ChangeDir: could not prepare currentFolderEntryId");
servCon.State.Log ("changedir almost done");
			return GetFolderProps ();
		}

		
		public string[] GetSubDirNames (IMapiContainer parent)
		{
			List<string>  names = new List<string> ();
			IMapiTableReader tableReader = null;
			try {
				tableReader = parent.GetHierarchyTable (Mapi.Unicode);
			} catch (MapiException e) {
				if (e.HResult != Error.NoSupport)
					throw;
				return new string [] {};
			}
			while (true) {
				RowSet rows = tableReader.GetRows (50);
				if (rows.Count == 0)
					break;
				int nameIndex = -1;
				foreach (Row row in rows) {
					if (nameIndex == -1)
						nameIndex = PropertyValue.GetArrayIndex (row.Props, Property.DisplayNameW);
					UnicodeProperty name = (UnicodeProperty) PropertyValue.GetArrayProp (row.Props, nameIndex);
					names.Add (name.Value);
				}
			}
			return names.ToArray ();
		}

		public bool GetFolderProps ()
		{
			return _GetFolderProps (out uidValidity, out uidNext, currentFolder);
		}

		public bool _GetFolderProps (out long _uidValidity, out long _uidNext, IMapiFolder folder)
		{
			_uidValidity = 0;
			_uidNext = 0;
			try {
				// UIDNEXT
				IntProperty val = (IntProperty) servCon.GetNamedProp (folder, IMAPGatewayNamedProperty.UIDNEXT);
				if (val != null) {
					_uidNext = val.Value;
					uidNextTag = val.PropTag;
				}
				servCon.State.Log ("uidnext from folder: "+ uidNext);
				servCon.State.Log ("uidNextTag from folder: "+ uidNextTag);
				
				//UIDVALIDITY
				val = (IntProperty) servCon.GetNamedProp (folder, IMAPGatewayNamedProperty.UIDVALIDITY);
				if (val != null) {
					_uidValidity = val.Value;
					uidValidityTag = val.PropTag;
				}
				servCon.State.Log ("uidvalidity from folder: "+ uidValidity);
				servCon.State.Log ("uidValidityTag from folder: "+ uidValidityTag);
				
				return true;
			} catch (Exception e) {
				servCon.State.Log ("GetFolderProps: Error: " + e.Message);
				return false;
			}
		}			

		private void SetUID (SequenceNumberListItem snli)
		{
			
			snli.UID = UpdateNextUid();
			snli.Path = currentPath;
			snli.CreationEntryId = snli.EntryId;

			IMapiProp message = null;
			try {
				message = (IMapiProp) currentFolder.OpenEntry(snli.EntryId.ByteArray, null, Mapi.Modify);
			} catch (MapiException e) {
				servCon.State.Log ("SetUID: uid " + snli.UID + " error: "+ e.Message  );
				throw;
			}
			
			List<PropertyValue> lv = new List<PropertyValue> ();
			IntProperty longValue = (IntProperty) servCon.GetNamedPropFrame (currentFolder, IMAPGatewayNamedProperty.UID);
			longValue.Value = (int) snli.UID;
			lv.Add (longValue);
			
			UnicodeProperty unicodeValue = (UnicodeProperty) servCon.GetNamedPropFrame (currentFolder, IMAPGatewayNamedProperty.UID_Path);
			unicodeValue.Value = snli.Path;
			lv.Add (unicodeValue);

			BinaryProperty binaryValue = (BinaryProperty) servCon.GetNamedPropFrame (currentFolder, IMAPGatewayNamedProperty.UID_Creation_EntryId);
			binaryValue.Value = snli.CreationEntryId;
			lv.Add (binaryValue);

			
			servCon.State.Log ("Select: Message loaded");
			((IMapiProp) message).SetProps(lv.ToArray ());

			servCon.State.Log ("setUID");
			
			servCon.State.Log ("Select: Props set");
			((IMapiProp) message).SaveChanges (0);
				
		}

		internal long UpdateNextUid () {
			// get current UIDNEXT
			GetFolderProps ();
			// save current uidnext
			long luidNext = uidNext;

			// update uidnext value in the store
			List<PropertyValue> lv = new List<PropertyValue> ();
			IntProperty longValue = new IntProperty();
			longValue.PropTag = uidNextTag;
			longValue.Value = (int) luidNext + 1;
			lv.Add (longValue);
			servCon.State.Log ("new uidnext: "+ longValue.Value);

			// if not available, set UIDVALIDITY
			if (uidValidity == 0) {
				longValue = new IntProperty ();
				longValue.PropTag = uidValidityTag;
				DateTime dt = DateTime.Now;
				// this gives us UIDVALIDITYs that are about 1 second sharp for 130 years starting from Nov. 2008
				longValue.Value = (int) ((dt.Ticks >> 22) & 0xFFFFFFFF) - 2^29;
				lv.Add (longValue);
				servCon.State.Log ("new uidvalidity: "+ longValue.Value);
			}

			
			currentFolder.SetProps(lv.ToArray ());
			currentFolder.SaveChanges (0);

			//re-get UIDNEXT with updated value
			GetFolderProps ();
			
			return luidNext;
		}
		

		internal void BuildSequenceNumberList ()
		{
			sequenceNumberList = _BuildSequenceNumberList (out currentFolderTable, currentFolder);
		}

		public SequenceNumberList _BuildSequenceNumberList (IMapiFolder folder)
		{
			IMapiTable dummyTable;
			return _BuildSequenceNumberList (out dummyTable, folder);
		}

		internal PropertyTag[] PropTagsForSequenceNumberList (IMapiProp iMapiProp)
		{
			if (uidPropTag == 0 || uidPathPropTag == 0 || uidEntryIdPropTag == 0 || additionalFlagsPropTag == 0) {
				uidPropTag = servCon.GetNamedPropFrame (iMapiProp, IMAPGatewayNamedProperty.UID).PropTag;
				uidPathPropTag = servCon.GetNamedPropFrame (iMapiProp, IMAPGatewayNamedProperty.UID_Path).PropTag;
				uidEntryIdPropTag = servCon.GetNamedPropFrame (iMapiProp, IMAPGatewayNamedProperty.UID_Creation_EntryId).PropTag;
				additionalFlagsPropTag = servCon.GetNamedPropFrame (iMapiProp, IMAPGatewayNamedProperty.AdditionalFlags).PropTag;
			}

			int [] propTags = new int[] 
			{ 
				Property.EntryId, Property.InstanceKey, Property.Subject, uidPropTag,
				uidPathPropTag, uidEntryIdPropTag
			}
			.Union (FlagHelper.PropsFlagProperties).ToArray ();

			return PropertyTag.ArrayFromIntegers (propTags);
		}

		public SequenceNumberList _BuildSequenceNumberList (out IMapiTable currentTable, IMapiFolder folder)
		{
			SequenceNumberList snl = new SequenceNumberList();
			servCon.State.Log ("Selectstart");
			currentTable = null;
			try {
				currentTable = folder.GetContentsTable (Mapi.Unicode);
				servCon.State.Log ("Select2");
			} catch (MapiException e) {
				if (e.HResult != Error.NoSupport)
					throw;
				return snl;
			}

			currentTable.SetColumns (PropTagsForSequenceNumberList (folder), 0);
			
			servCon.State.Log ("Select1");
			while (true) {
				RowSet rows = currentTable.QueryRows (50, Mapi.Unicode);
				if (rows.Count == 0)
					break;
				foreach (Row row in rows) {
					snl.Add (_BuildSequenceNumberListItem (row.Props));
				}
			}
			return snl;
		}

		internal SequenceNumberListItem SequenceNumberListItemFromIMessage (IMessage im)
		{
			PropertyValue[] props = im.GetProps (PropTagsForSequenceNumberList (im), Mapi.Unicode);
			return _BuildSequenceNumberListItem (props);
		}

		internal SequenceNumberListItem AppendAndFixNewMessage (IMessage im)
		{
			SequenceNumberListItem snli = SequenceNumberListItemFromIMessage (im);
			SetUID (snli);
			sequenceNumberList.Add (snli);
			return snli;
		}

		internal SequenceNumberListItem _BuildSequenceNumberListItem (PropertyValue[] props)
		{
			servCon.State.Log ("Select5");
			SequenceNumberListItem snli = new SequenceNumberListItem ();
			BinaryProperty entryId = (BinaryProperty) PropertyValue.GetArrayProp(props, 0);
			
			servCon.State.Log ("Select5a");
			if (entryId != null) 
				snli.EntryId = entryId.Value;
			servCon.State.Log ("Select5b");
			
			PropertyValue val = PropertyValue.GetArrayProp(props, 1);
			if (val != null) snli.InstanceKey = ((BinaryProperty) val).Value;
			
			val = PropertyValue.GetArrayProp(props, 3);
			if (val != null) snli.UID = ((IntProperty) val).Value;
			
			val = PropertyValue.GetArrayProp(props, 4);
			if (val != null) snli.Path = ((UnicodeProperty) val).Value;
			
			val = PropertyValue.GetArrayProp(props, 5);
			if (val != null) snli.CreationEntryId = ((BinaryProperty) val).Value;
			
			val = PropertyValue.GetArrayProp(props, 6);
			if (val != null) snli.MsgStatus = (ulong) ((IntProperty) val).Value;
Console.WriteLine ("MsgStatus: " + snli.MsgStatus + "UID: " + snli.UID);
			
			val = PropertyValue.GetArrayProp(props, 7);
			if (val != null) snli.MessageFlags = (ulong) ((IntProperty) val).Value;

			val = PropertyValue.GetArrayProp(props, 8);
			if (val != null) snli.FlagStatus = (ulong) ((IntProperty) val).Value;

			val = PropertyValue.GetArrayProp(props, 9);
			try {
				if (val != null) 
					snli.AdditionalFlags = new List<string> ((string []) ((UnicodeArrayProperty) val).Value);
				else
					snli.AdditionalFlags = new List<string> ();
			} catch (Exception)
			{
				snli.AdditionalFlags = new List<string> ();
			}

			return snli;
		}

		internal int FixUIDsInSequenceNumberList ()
		{
			var query = from snl in sequenceNumberList
						where snl.UID == 0 || 
								snl.Path != currentPath ||
								!servCon.CompareEntryIDs (snl.CreationEntryId.ByteArray, snl.EntryId.ByteArray)
						select snl;
			int cnt = query.Count();
			
			foreach (SequenceNumberListItem snli in query) {
servCon.State.Log ("FixUIDsIn");
				SetUID (snli);
				snli.Recent = true;
			}

			sequenceNumberList.Sort ();
			return cnt;
		}

		internal int SequenceNumberOf (SequenceNumberListItem snli)
		{
			return sequenceNumberList.SequenceNumberOf (snli);
		}


		public List<SequenceNumberListItem> BuildSequenceSetQuery(Command command)
		{
			return _BuildSequenceSetQuery (sequenceNumberList, command);
		}

		public static List<SequenceNumberListItem> _BuildSequenceSetQuery(SequenceNumberList snl, Command command)
		{
			List<SequenceNumberListItem> masterQuery = null;
			IQueryable<SequenceNumberListItem> query = null;
			IQueryable<SequenceNumberListItem> query2 = null;
			long first = 0;
			long second = 0;

			foreach (Pair sel in command.Sequence_set) {
				/* prepare Maxvalues, depending on occurences of * */
				try { first = Convert.ToUInt32 ((string) sel.First); } catch {}
				try { second = Convert.ToUInt32 ((string) sel.Second); } catch {}
				if ((string) sel.Second == "*" || (string) sel.First == "*")
					if (command.UIDCommand)
						second = snl.Max (n => n.UID);
					else
						second = snl.Max (n => (uint) snl.IndexOfSNLI (n));
				/* handle cases */
				if (sel.Second != null) { /* Range */
					if (command.UIDCommand)
						query = from els in snl.AsQueryable ()
								where els.UID >= Math.Min (first, second) && 
										els.UID <= Math.Max (first, second)
								select els;
					else
						query = from els in snl.AsQueryable ()
								where snl.IndexOfSNLI (els) >= Math.Min (first, second) && 
									  snl.IndexOfSNLI (els) <= Math.Max (first, second)
								select els;
				}
				else if ((string) sel.First != "*") { /* singular id */
					if (command.UIDCommand)
						query = from els in snl.AsQueryable ()
								where els.UID == first
								select els;
					else
						query = from els in snl.AsQueryable ()
								where snl.IndexOfSNLI (els) == first
								select els;
				}
				else if ((string) sel.First == "*") { /* singular * */
					if (command.UIDCommand)
						query = from els in snl.AsQueryable ()
								where els.UID == second
								select els;
					else
						query = from els in snl.AsQueryable ()
								where snl.IndexOfSNLI (els) == second
								select els;
				}
				// special handling for *. Needs to be explicitly selected
				// In case client select sequence-set 100:* with highest existing ID being 90,
				// the item with ID 90 must be returned. (RFC3501)
				query2 = null;
				if ((string) sel.Second == "*" || (string) sel.First == "*") {
					if (command.UIDCommand)
						query2 = from els in snl.AsQueryable ()
								where els.UID == second
								select els;
					else
						query2 = from els in snl.AsQueryable ()
								where snl.IndexOfSNLI (els) == second
								select els;
				}
				if (masterQuery == null)
					masterQuery = query.ToList ();
				else
					masterQuery.AddRange (query.ToList ());
				if (query2 != null)
					masterQuery.AddRange (query2.ToList ());
			}
			return masterQuery.Distinct().OrderBy(n => snl.IndexOf(n)).ToList();
		}

		public uint IndexOfSNLI (SequenceNumberListItem snli)
		{
			return sequenceNumberList.IndexOfSNLI(snli);
		}

		public int RebuildSequenceNumberListPlusUIDFix ()
		{
			// lock this part against any other Session, that wants to execute this as well 
			// (in case they try to update their table of the same mailbox and conflicts occur)
			//lock (IMAPConnectionState.LockObject) 
				{
			int retrys = 2;
			while (retrys > 0) {

				try {
					servCon.State.Log ("RebuildSequenceNumberListPlusUIDFix, retrys left: " + retrys);
					retrys --;
					// build sequence number list
					BuildSequenceNumberList ();

					// fix UIDS in Messagesif missing or broken
					servCon.State.Log ("fixUIDs");
					int recent = FixUIDsInSequenceNumberList ();
					return recent;
				} catch (MapiException e) {
					servCon.State.Log ("RebuildSequenceNumberListPlusUIDFix, Exception: " + e.Message);
					if (retrys <= 0)
						throw;
					Thread.Sleep(500);
				}
			}
				}
			return 0; // should never be reached
		}						

		public void Dispose ()
		{
			if (currentFolderTable != null)
				currentFolderTable.Dispose ();

			if (currentFolder != null)
				currentFolder.Dispose ();
		}
	}
}
