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
		private IMessage imapFolderAttributesStore;
		private	long uidNext = 0;
		private	long uidValidity = 0;
		private int uidNextTag = 0;
		private int uidValidityTag = 0;
		private static int uidPropTag = 0;
		private static int uidCreationPathPropTag = 0;
		private static int uidCreationEntryIdPropTag = 0;
		private static int uidCreationUIDValidityPropTag = 0;
		private static int additionalFlagsPropTag = 0;

		private static int[] propsAssociatedMessages = new int[] 
		{
			Property.EntryId,
			Property.Subject
		};

		private const string ASSOCIATED_MESSAGE_IMAP_FOLDER_ATTRIBUTES = "NMapi IMAP Gateway IMAP Folder Attributes";

		// this object is used as lock object for testing of SetUID method only
		private object setUIDTestLockObject = new Object ();
		
		public FolderHelper (ServerConnection servCon, string path) {
			this.servCon = servCon;

			ChangeDir (path); // Change to root dir
			sequenceNumberList = new SequenceNumberList ();

		}

		public void Dispose () {
			DisposeFolderRelevantProperties ();
		}
		
		public void DisposeFolderRelevantProperties ()
		{
			
			if (imapFolderAttributesStore != null)
				imapFolderAttributesStore.Dispose ();
			imapFolderAttributesStore = null;
			
			if (currentFolderTable != null)
				currentFolderTable.Dispose ();
			currentFolderTable = null;

			if (currentFolder != null)
				currentFolder.Dispose ();
			currentFolder = null;
		}

		/// <value>
		/// use to retrieve lock object for testing of SetUID method 
		/// </value>
		public Object SetUIDTestLockObject {
			get { return setUIDTestLockObject; }
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
		/// <summary>
		/// 
		/// </summary>
		public int UIDNEXTTag {
			get { return uidNextTag; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int UIDVALIDITYTag {
			get { return uidValidityTag; }
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
					entryId.ByteArray, null, Mapi.Unicode | Mapi.Modify);
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

			// now reset and refill variables and objects
			DisposeFolderRelevantProperties ();
			currentFolder = newFolder;
			currentPath = path;
			uidValidity = 0;
			uidNext = 0;

			MapiPropHelper mph = new MapiPropHelper (currentFolder);
			BinaryProperty eid = (BinaryProperty) mph.HrGetOneProp (Property.EntryId);
			currentFolderEntryId = (eid != null) ? eid.Value : null;

			if (currentFolderEntryId == null)
				throw new Exception ("ServerConnection.ChangeDir: could not prepare currentFolderEntryId");
			return GetFolderAttributes ();
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

		public bool GetFolderAttributes ()
		{
			servCon.State.Log ("GetFolderAttributes  uidvalidity:" + uidValidity);
			if (uidValidity > 0 && imapFolderAttributesStore != null)
				return true;
			
			return _GetFolderAttributes (out uidValidity, out uidNext, currentFolder);
		}

		public bool _GetFolderAttributes (out long _uidValidity, out long _uidNext, IMapiFolder folder)
		{
			servCon.State.Log ("_GetFolderAttributes");
			_uidValidity = 0;
			_uidNext = 0;
			
			SetIMAPFolderAttributesStore ();
			
			try {
				// UIDNEXT
				IntProperty val = (IntProperty) servCon.GetNamedProp (imapFolderAttributesStore , IMAPGatewayNamedProperty.UIDNEXT);
				if (val != null) {
					_uidNext = val.Value;
					uidNextTag = val.PropTag;
				}
				servCon.State.Log ("uidnext from folder: "+ uidNext);
				servCon.State.Log ("uidNextTag from folder: "+ uidNextTag);
				
				//UIDVALIDITY
				val = (IntProperty) servCon.GetNamedProp (imapFolderAttributesStore, IMAPGatewayNamedProperty.UIDVALIDITY);
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

		/// <summary>
		/// helper method to test SetUIDTest
		/// </summary>
		/// <param name="snli">
		/// A <see cref="System.Object"/>
		/// </param>
		public void SetUIDTest (Object snli) {
			SetUID ((SequenceNumberListItem) snli);
		}
		
		public void SetUID (SequenceNumberListItem snli)
		{
			IMessage message = null;
			int countRetriesLeft = 3;
			
			while (true) {
			
				// we first (reread) the email, so we can test, if the UID-info has still
				// not changed.
				try {
					message = GetMessageFromSequenceNumberListItem (snli);
				} catch (MapiException e) {
					servCon.State.Log ("SetUID: uid " + snli.UID + " error: "+ e.Message  );
					throw;
				}
				
				// when method is being tested, it will wait until lock of setUIDTestLockObject is
				// released by caller
				lock (setUIDTestLockObject) { 
				};
				
				using (message) {
					try {
						_SetUID (snli, message);
						return;
					} catch (Exception e) {
						if (countRetriesLeft == 0) {
							servCon.State.Log ("SetUID: failed on last retry");
							throw new Exception ("SetUID: could not update UIDNEXT: " + e.ToString ());
						}
	
						// if we fail in setting the UID due to concurrent access to that email
						// we need to reload the message and see if it is supplied with a valid UID now.
						// Another process might have done that in the meantime. In that case we should not
						// overwrite that UID. See _SetUID.
						servCon.State.Log ("SetUID: failed to store new values. Retries left: " + countRetriesLeft);
						--countRetriesLeft;
						
					}
				}
			}
		}

		private void _SetUID (SequenceNumberListItem snli, IMessage message) {
			
			// refill snli with the values of the passed messages
			SequenceNumberListItemFromIMessage (snli, message);
			
			// do changes only, if new values do not pass test for valid UID information
			if (!TestSNLIUID (snli)) {
			
				SequenceNumberListItem snliLocal = new SequenceNumberListItem ();
				
				// it is important, that we increase UIDNEXT every time we try to set an UID to a message
				// Once a UIDNEXT is communicated to a client, any new message must have an UID >= that UIDNEXT
				// So, in any conflict situation that might appear while setting an UID that causes a retry
				// we need to fetch a new UID.
				snliLocal.UID = UpdateUIDNEXT();
				snliLocal.CreationPath = currentPath;
				snliLocal.CreationEntryId = snli.EntryId;
				snliLocal.CreationUIDValidity = uidValidity;
	
				List<PropertyValue> lv = new List<PropertyValue> ();
				IntProperty longValue = (IntProperty) servCon.GetNamedPropFrame (currentFolder, IMAPGatewayNamedProperty.UID);
				longValue.Value = (int) snliLocal.UID;
				lv.Add (longValue);
				
				UnicodeProperty unicodeValue = (UnicodeProperty) servCon.GetNamedPropFrame (currentFolder, IMAPGatewayNamedProperty.UID_Creation_Path);
				unicodeValue.Value = snliLocal.CreationPath;
				lv.Add (unicodeValue);
	
				BinaryProperty binaryValue = (BinaryProperty) servCon.GetNamedPropFrame (currentFolder, IMAPGatewayNamedProperty.UID_Creation_EntryId);
				binaryValue.Value = snliLocal.CreationEntryId;
				lv.Add (binaryValue);
	
				longValue = (IntProperty) servCon.GetNamedPropFrame (currentFolder, IMAPGatewayNamedProperty.UID_Creation_UIDValidity);
				longValue.Value = (int) snliLocal.CreationUIDValidity;
				lv.Add (longValue);
	
				
				servCon.State.Log ("SetUID: Message loaded");
				message.SetProps(lv.ToArray ());
	
				servCon.State.Log ("setUID");

				servCon.State.Log ("SetUID: Props set");
				message.SaveChanges (0);
				
				// now update snli, as saving to the store has succeeded
				snli.UID = snliLocal.UID;
				snli.CreationPath = snliLocal.CreationPath;
				snli.CreationEntryId = snliLocal.CreationEntryId;
				snli.CreationUIDValidity = snliLocal.CreationUIDValidity;
				
			}
		}
		
		public long UpdateUIDNEXT () {
			int countRetrysLeft = 3;
			long luidNext = 0;
			long luidValidity = 0;
			
			while (true) {
				// get folder
				GetFolderAttributes ();
				// save current uidnext
				luidNext = uidNext;
				luidValidity = uidValidity;
	
				// update uidnext value in memory and attributes store
				List<PropertyValue> lv = new List<PropertyValue> ();
				IntProperty longValue = new IntProperty();
				longValue.PropTag = uidNextTag;
				longValue.Value = (int) uidNext + 1;
				lv.Add (longValue);
				servCon.State.Log ("UpdateUIDNEXT: new uidnext: "+ longValue.Value);
	
				// if not available, set UIDVALIDITY
				if (uidValidity == 0) {
					// this gives us UIDVALIDITYs that are about 1 second sharp for 130 years starting from Nov. 2008
					DateTime dt = DateTime.Now;
					luidValidity = ((dt.Ticks >> 22) & 0xFFFFFFFF) - 2^29;
					
					longValue = new IntProperty ();
					longValue.PropTag = uidValidityTag;
					longValue.Value = (int) luidValidity;
					lv.Add (longValue);
					servCon.State.Log ("UpdateUIDNEXT: new uidvalidity: "+ longValue.Value);
				}
	
				try {
					imapFolderAttributesStore.SetProps(lv.ToArray ());
					imapFolderAttributesStore.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
					
					uidNext ++;
					uidValidity = luidValidity;
					
					return luidNext;
					
				} catch (Exception e) {
					if (countRetrysLeft == 0) {
						servCon.State.Log ("UpdateUIDNEXT: failed on last retry");
						throw new Exception ("UpdateUIDNEXT: could not update UIDNEXT: " + e.ToString ());
					}

					servCon.State.Log ("UpdateUIDNEXT: failed to store new values. Retries left: " + countRetrysLeft);
					--countRetrysLeft;

					// now reset attribute store and start over. GetFolderProps will reload the attribute store.
					// That is required, as we want to keep it with NMAPI.KEEP_OPEN_READWRITE to save reloading
					// it for any updates until we encounter another conflict.
					imapFolderAttributesStore.Dispose ();
					imapFolderAttributesStore = null;
				}
			}
		}
		

		public void BuildSequenceNumberList ()
		{
			sequenceNumberList = _BuildSequenceNumberList (out currentFolderTable, currentFolder);
		}

		public SequenceNumberList _BuildSequenceNumberList (IMapiFolder folder)
		{
			IMapiTable dummyTable;
			return _BuildSequenceNumberList (out dummyTable, folder);
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

		public PropertyTag[] PropTagsForSequenceNumberList (IMapiProp iMapiProp)
		{
			if (uidPropTag == 0 || uidCreationPathPropTag == 0 || uidCreationEntryIdPropTag == 0 || uidCreationUIDValidityPropTag == 0
			    || additionalFlagsPropTag == 0) {
				uidPropTag = servCon.GetNamedPropFrame (iMapiProp, IMAPGatewayNamedProperty.UID).PropTag;
				uidCreationPathPropTag = servCon.GetNamedPropFrame (iMapiProp, IMAPGatewayNamedProperty.UID_Creation_Path).PropTag;
				uidCreationEntryIdPropTag = servCon.GetNamedPropFrame (iMapiProp, IMAPGatewayNamedProperty.UID_Creation_EntryId).PropTag;
				uidCreationUIDValidityPropTag = servCon.GetNamedPropFrame (iMapiProp, IMAPGatewayNamedProperty.UID_Creation_UIDValidity).PropTag;
				additionalFlagsPropTag = servCon.GetNamedPropFrame (iMapiProp, IMAPGatewayNamedProperty.AdditionalFlags).PropTag;
			}

			int [] propTags = new int[] 
			{ 
				Property.EntryId, Property.InstanceKey, Property.Subject, uidPropTag,
				uidCreationPathPropTag, uidCreationEntryIdPropTag, uidCreationUIDValidityPropTag
			}
			.Union (FlagHelper.PropsFlagProperties).ToArray ();

			return PropertyTag.ArrayFromIntegers (propTags);
		}

		public SequenceNumberListItem SequenceNumberListItemFromIMessage (IMessage im)
		{
			PropertyValue[] props = im.GetProps (PropTagsForSequenceNumberList (im), Mapi.Unicode);
			return _BuildSequenceNumberListItem (props);
		}

		public SequenceNumberListItem SequenceNumberListItemFromIMessage (SequenceNumberListItem snli, IMessage im)
		{
			PropertyValue[] props = im.GetProps (PropTagsForSequenceNumberList (im), Mapi.Unicode);
			return _UpdateSequenceNumberListItem (snli, props);
		}

		public SequenceNumberListItem AppendAndFixNewMessage (IMessage im)
		{
			SequenceNumberListItem snli = SequenceNumberListItemFromIMessage (im);
			SetUID (snli);
			sequenceNumberList.Add (snli);
			return snli;
		}
		
		public IMessage GetMessageFromSequenceNumberListItem (SequenceNumberListItem snli) {
			return (IMessage) currentFolder.OpenEntry(snli.EntryId.ByteArray, null, Mapi.Modify);
		}

		internal SequenceNumberListItem _BuildSequenceNumberListItem (PropertyValue[] props)
		{
			return _UpdateSequenceNumberListItem (new SequenceNumberListItem (), props);
		}
		
		internal SequenceNumberListItem _UpdateSequenceNumberListItem (SequenceNumberListItem snli, PropertyValue[] props)
		{
			
			servCon.State.Log ("Select5");
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
			if (val != null) snli.CreationPath = ((UnicodeProperty) val).Value;
			
			val = PropertyValue.GetArrayProp(props, 5);
			if (val != null) snli.CreationEntryId = ((BinaryProperty) val).Value;
			
			val = PropertyValue.GetArrayProp(props, 6);
			if (val != null) snli.CreationUIDValidity = ((IntProperty) val).Value;
			
			val = PropertyValue.GetArrayProp(props, 7);
			if (val != null) snli.MsgStatus = (ulong) ((IntProperty) val).Value;
			servCon.State.Log ("MsgStatus: " + snli.MsgStatus + "UID: " + snli.UID);
			
			val = PropertyValue.GetArrayProp(props, 8);
			if (val != null) snli.MessageFlags = (ulong) ((IntProperty) val).Value;

			val = PropertyValue.GetArrayProp(props, 9);
			if (val != null) snli.FlagStatus = (ulong) ((IntProperty) val).Value;

			val = PropertyValue.GetArrayProp(props, 10);
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

		/// <summary>
		/// search for mails, that need to get a new UID
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		internal int FixUIDsInSequenceNumberList ()
		{
			int cnt = 0;
			var query = from snli in sequenceNumberList
						where !TestSNLIUID (snli)
						select snli;

			servCon.State.Log ("FixUIDsInSequenceNumberList 1");
			
			foreach (SequenceNumberListItem snli in query) {
				servCon.State.Log ("FixUIDsInSequenceNumberList Loop");
				SetUID (snli);
				snli.Recent = true;
				cnt ++;
			}
			servCon.State.Log ("FixUIDsInSequenceNumberList 2");

			sequenceNumberList.Sort ();
			servCon.State.Log ("FixUIDsInSequenceNumberList 3");
			return cnt;
		}
		
		/// <summary>
 		/// * check UID=0 for emails without UID
		/// * check CreationPath being different from the path of the folder for emails 
		/// which have been moved into this folder by a MAPI tool. An existing UID must
		/// be replaced in that case
		/// * check CreationEntryId being different from existing entry id for mails
		/// which have been copied inside this folder by a MAPI tool. An existing UID must 
		/// be replaced in that case
		/// * check CreationUIDValidity being different from current UIDVALIDITY of folder
		/// for the case that UIDVALIDITY has been lost and needed to be recreated. An 
		/// existing UID must be replaced in that case
		/// </summary>
		/// <param name="snli">
		/// A <see cref="SequenceNumberListItem"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		internal bool TestSNLIUID (SequenceNumberListItem snli) {
			
			bool result =  snli.UID != 0 && 
						snli.CreationPath == currentPath &&
						servCon.CompareEntryIDs (snli.CreationEntryId.ByteArray, snli.EntryId.ByteArray) &&
						snli.CreationUIDValidity == uidValidity;
			
			servCon.State.Log ("TestSNLIUID, UID: " + snli.UID + " Testresult: " + result);
			
			return result;
		}

		internal int SequenceNumberOf (SequenceNumberListItem snli)
		{
			return sequenceNumberList.SequenceNumberOf (snli);
		}


		public static string BuildSequenceSetString<T> (IEnumerable<T> list)
		{
			StringBuilder seqStrings = new StringBuilder ();
			bool isFirst = true;

			List<Pair> sequenceSet = BuildSequenceSet (list);

			foreach (Pair range in sequenceSet) {
				if (!isFirst) 
					seqStrings.Append (",");
				else
					isFirst = false;

				seqStrings.Append (BuildSequenceRange (range) );
			}

			return seqStrings.ToString ();
		}

		private static string BuildSequenceRange (Pair range)
		{
			return (range.Second == null) ?
					( (string) range.First) : ( ( (string) range.First ) + ":" + ( (string) range.Second ) );
		}


		/// compact a list of ids to a sequence set notation. ---> 1,2,3,5,6 to 1:3,5:6 for example
		/// method will NOT sort the list upfront
		public static List<Pair> BuildSequenceSet<T> (IEnumerable<T> list)
		{
			List<Pair> sequenceSet = new List<Pair> ();
			IEnumerator<T> listEnum = list.GetEnumerator ();
			object startId = null;
			object prevId = null;
			bool moveNextResult = false;

			while ( (moveNextResult = listEnum.MoveNext () ) || startId != null ) {

				prevId = startId;
				if (prevId != null) {
					while (moveNextResult && Convert.ToUInt64 ( (T) prevId) + 1 == Convert.ToUInt64 ( (T) listEnum.Current) ) {
						prevId = listEnum.Current;
						moveNextResult = listEnum.MoveNext ();
					}

					if (prevId != startId) {
						sequenceSet.Add (new Pair ( startId.ToString (), prevId.ToString () ) );
					} else {
						sequenceSet.Add (new Pair ( startId.ToString (), null) );
					}
				}

				if (moveNextResult)
					startId = listEnum.Current;
				else
					startId = null;
			}

			return sequenceSet;
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
					servCon.State.Log ("fixUIDs done");
					return recent;
				} catch (MapiException e) {
					servCon.State.Log ("RebuildSequenceNumberListPlusUIDFix, Exception: " + e.Message);
					if (retrys <= 0)
						throw;
					Thread.Sleep(500);
				}
			}
			return 0; // should never be reached
		}						

		
		public IMessage SetIMAPFolderAttributesStore ()
		{
			if (imapFolderAttributesStore != null)
				return imapFolderAttributesStore;
			
			return imapFolderAttributesStore = GetIMAPFolderAttributesStore ();
		}

		public IMessage GetIMAPFolderAttributesStore ()
		{
			IMapiTable localTable = null;
			try {
				// !!! need to use independent version of folder. Otherwise unexpected influences appear
				// !!! e.g. Notifications might fail
//				IMapiFolder localFolder = OpenFolder (currentPath);
				localTable = currentFolder.GetContentsTable (Mapi.Unicode | NMAPI.MAPI_ASSOCIATED);
			} catch (MapiException e) {
				if (e.HResult != Error.NoSupport)
					throw;
			}

			using (localTable) {
				localTable.SetColumns (PropertyTag.ArrayFromIntegers (propsAssociatedMessages), 0);
	
				// search item in Table
				while (true) {
					RowSet rows = localTable.QueryRows (50, Mapi.Unicode);
					if (rows.Count == 0)
						break;
					foreach (Row row in rows) {
						PropertyValue val = PropertyValue.GetArrayProp(row.Props, 1);
						if (val != null && ( (UnicodeProperty) val).Value == ASSOCIATED_MESSAGE_IMAP_FOLDER_ATTRIBUTES) {
							BinaryProperty entryId = (BinaryProperty) PropertyValue.GetArrayProp(row.Props, 0);
							servCon.State.Log ("GetIMAPFolderAttributesStore, returning existing AttributesStore");
							return (IMessage) currentFolder.OpenEntry (entryId.Value.ByteArray , null, Mapi.Unicode | Mapi.Modify);
						}
					}
				}
			}
			
			// not found, so create it
			servCon.State.Log ("GetIMAPFolderAttributesStore, creating new AttributeStore");
			try {
				IMessage imAss = currentFolder.CreateMessage (null, NMAPI.MAPI_ASSOCIATED);

				PropertyValue pv = PropertyValue.CreateFromTag (new UnicodePropertyTag (Property.Subject));
				( (UnicodeProperty) pv).Value = ASSOCIATED_MESSAGE_IMAP_FOLDER_ATTRIBUTES;
				imAss.SetProperty (pv);
				imAss.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
				return imAss;
			} catch (Exception e) {
				throw new Exception ("GetIMAPFolderAttributesStore Error: " + e.ToString () + " in Folder: " + currentFolder);
			}
			
		}
	}
}
