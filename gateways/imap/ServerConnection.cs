// openmapi.org - NMapi C# IMAP Gateway - ServerConnection.cs
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

namespace NMapi.Gateways.IMAP
{

	
	public enum IMAPGatewayNamedProperty {Subscriptions, UID, UID_Path, UID_Creation_EntryId, UIDNEXT, UIDVALIDITY, AdditionalFlags};
	
//	public delegate void LogDelegate(string message);
	
	public class ServerConnection
	{
		private LogDelegate logInput;
		private LogDelegate logOutput;

		private IMAPConnectionState state;
		private IMapiFactory factory;
		private IMapiSession session;
		private MapiContext mapiContext;
		private IMsgStore store;
		private IMapiFolder currentFolder;
		private IMapiTable currentFolderTable;
		private string currentPath;
		private SBinary currentFolderEntryId;
		private string rootDir;
		private string inboxPath;
		private SequenceNumberList sequenceNumberList;
		private	long uidNext = 0;
		private	long uidValidity = 0;
		private int uidNextTag = 0;
		private int uidValidityTag = 0;
		private bool loggedOn;
		private string user;
		private static int uidPropTag = 0;
		private static int uidPathPropTag = 0;
		private static int uidEntryIdPropTag = 0;
		private static int additionalFlagsPropTag = 0;
		

		/// <summary>
		/// 
		/// </summary>
		internal string CurrentPath {
			get { return currentPath; }
		}
			
		/// <summary>
		/// 
		/// </summary>
		internal SBinary CurrentFolderEntryId {
			get { return currentFolderEntryId; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal long UIDNEXT {
			get { return uidNext; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal long UIDVALIDITY {
			get { return uidValidity; }
		}

		internal SequenceNumberList SequenceNumberList { 
			get { return sequenceNumberList; }
			set { sequenceNumberList = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal IMsgStore Store {
			get { return store; }
			set { store = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal string RootDir {
			get { return rootDir; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal string InboxPath {
			get { return inboxPath; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal IMapiFolder CurrentFolder {
			get { return currentFolder; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal IMapiTable CurrentFolderTable {
			get { return currentFolderTable; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal string User {
			get { return user; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal bool LoggedOn {
			get { return loggedOn; }
			set { loggedOn = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		internal IMapiFactory Factory {
			get { return factory; }
			set { factory = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal IMapiSession Session {
			get { return session; }
			set { session = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal MapiContext MapiContext {
			get { return mapiContext; }
			set { mapiContext = value; }
		}

		internal static int AdditionalFlagsPropTag {
			get { return additionalFlagsPropTag; }
		}

		public ServerConnection (IMAPConnectionState state, string host, string user, string password )
		{

			this.state = state;
			this.user = user;
state.Log ("Server0");

/*
//this just takes endless
			providers = ProviderManager.FindProviders ();
state.Log ("Server1");
			factory = ProviderManager.GetFactory (providers ["org.openmapi.txc"]);
*/
// 			factory = (IMapiFactory) Activator.CreateInstance ("NMapi.Provider.TeamXChange", "NMapi.Provider.TeamXChange.TeamXChangeMapiFactory").Unwrap () as IMapiFactory;

			IMAPGatewayConfig config = IMAPGatewayConfig.read ();
			object o = Activator.CreateInstance (
				config.Mapiproviderassembly, config.Mapiproviderfactorytype).Unwrap () as IMapiFactory;
			if (o == null)
				throw new Exception ("Couldn't create backend factory!");
			factory = (IMapiFactory) o;
			
			state.Log ("Server2");
			try {
				session = factory.CreateMapiSession ();
				state.Log ("Server3");
//				mapiContext = new MapiContext (session);
				state.Log ("Server4");
			} catch (MapiException e) {
				state.Log ("ERROR: Can't open Mapi-Session!\n\n" + e.Message);
				return;
			}

			if (!CheckSessionMsg ())
				return;

			try {
				session.Logon (host, user, password);
				loggedOn = true;
			} catch (MapiException e) {
				if (e.HResult == Error.NetworkError) {
					state.Log ("Couldn't connect to host!"+e.Message);
					throw;
				}
				else if (e.HResult == Error.NoAccess) {
					state.Log ("No permission!");
					throw;
				}
				throw;
			}

			
			try {
				store = session.PrivateStore;
				state.Log ("Server5");
			} catch (Exception) {
				throw;
			}
			
			SetRootDir();
			
			ChangeDir (string.Empty + PathHelper.PathSeparator); // Change to root dir
			state.Log ("Server6");
			sequenceNumberList = new SequenceNumberList ();
		
		}

		public void Disconnect()
		{
			if (session != null) {
				state.Log ("serverconnection, disconnect");
				session.Dispose();
				session = null;
			}
		}

		public LogDelegate LogInput {
			set { logInput = value; }
		}
		public LogDelegate LogOutput {
			set { logOutput = value; }
		}


		internal void SetRootDir()
		{
			if (!CheckStore ())
				return;
			
			GetReceiveFolderResult grfr = store.GetReceiveFolder (null, Mapi.Unicode);
			if (grfr.EntryID != null) {
				BinaryProperty eId = new BinaryProperty ();
				eId.PropTag = Property.EntryId;
				eId.Value = new SBinary (grfr.EntryID);
				IMapiFolder folder = null;
				IMapiFolder prevFolder = null;

				byte[] prevEId = new byte[0];
				List<string> inboxPathEls = new List<string> ();
				state.Log ("setrootdir 1");
				
				while (eId != null && !CompareEntryIDs(eId.Value.ByteArray, prevEId)) {
					state.Log ("setrootdir 2");
					prevFolder = folder;
					folder = (IMapiFolder) store.OpenEntry (eId.Value.ByteArray);
					MapiPropHelper folderHelper = new MapiPropHelper (folder);
					UnicodeProperty propDisplayName = (UnicodeProperty) folderHelper.HrGetOnePropNull (Property.DisplayName);
					string displayName = (propDisplayName != null) ? propDisplayName.Value : null;
						
					state.Log (folder.GetType ().ToString ());
					state.Log (displayName);
					state.Log ("setrootdir 3");
					
					prevEId = eId.Value.ByteArray;
					if (folder != null) {
						inboxPathEls.Add (displayName);
						eId = (BinaryProperty) folderHelper.HrGetOnePropNull (Property.ParentEntryId);
state.Log ("setrootdir 4");
					}
				}
				if (prevFolder != null) {
					PropertyValue dir = new MapiPropHelper (prevFolder).HrGetOnePropNull (Property.DisplayNameW);
					rootDir = PathHelper.PathSeparator + ((UnicodeProperty) dir).Value;
					inboxPath = PathHelper.Array2Path (inboxPathEls.Take (inboxPathEls.Count - 2).Reverse ().ToArray ());
					return;
				}
			}
			rootDir = "";
			inboxPath = "";
		}
			
			
		internal bool CheckStore ()
		{
			if (store == null) {
				state.Log ("ERROR: Message-Store not open!");
				return false;
			}
			return true;
		}

		internal bool CheckSessionMsg ()
		{
			if (session == null) {
				state.Log ("Session must be open!");
				return false;
			}
			return true;
		}

		internal bool CheckLoggedOnMsg ()
		{
			if (!loggedOn) {
				state.Log ("ERROR: Not logged on!");
				return false;
			}
			return true;
		}


		internal string FullPath {
			get {
				return currentPath;
			}
		}

		internal void CloseSession ()
		{
			loggedOn = false;
			if (mapiContext != null)
				mapiContext.Dispose ();
			
			if (currentFolder != null)
				currentFolder.Dispose ();
			
			if (store != null)
				store.Dispose ();
			
			if (session != null)
				session.Dispose ();
			
			mapiContext = null;
			currentFolder = null;
			session = null;
			store = null;
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
				RowSet rows = tableReader.GetRows (10);
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

		internal IMapiContainer GetSubDir (IMapiContainer parent, string match)
		{
			if (parent == null)
				return null;
			Func<IMapiContainer, SBinary, object> action = (prnt, entryId) => {
				return (IMapiContainer) prnt.OpenEntry (
					entryId.ByteArray, null, Mapi.Modify);
			};

			return (IMapiContainer) _SharedGetSubDir (parent, match, action);
		}

		internal SBinary GetSubDirEntryId (IMapiContainer parent, string match)
		{
			if (parent == null)
				return null;
			Func<IMapiContainer, SBinary, object> action = (prnt, entryId) => entryId;

			return (SBinary) _SharedGetSubDir (parent, match, action);
		}

		internal IMapiFolder OpenFolder (string path)
		{
			if (path [0] != PathHelper.PathSeparator)
				throw new Exception ("path must start with '/'.");
			string[] parts = PathHelper.Path2Array (rootDir + path);

			if (path == string.Empty)
				return (IMapiFolder) store.Root;

			IMapiContainer container = (IMapiContainer) store.Root;

			foreach (string part in parts) {
				if (container == null)
					break;
				container = GetSubDir (container, part);
			}
			return (IMapiFolder) container; // TODO: Container? Folder?
		}

		internal string Input2AbsolutePath (string input)
		{
			string path = null;
			string relPath = input;
			if (relPath [0] == PathHelper.PathSeparator)
				path = PathHelper.ResolveAbsolutePath (relPath);
			else
				path = PathHelper.Combine (currentPath, relPath);
			return path;
		}

		internal bool ChangeDir (string path)
		{
			state.Log ("ChangeDir: " + path);
			if (!CheckStore())
				return false;
			IMapiFolder newFolder = null;
state.Log ("changedir0");
			newFolder = OpenFolder (path);
state.Log ("changedir1");
			if (newFolder == null) {
				state.Log ("cd: " + path + ": No such folder.");
				return false;
			}
			currentFolder = newFolder;
			currentPath = path;
			currentFolderTable = null;

			MapiPropHelper mph = new MapiPropHelper (currentFolder);
state.Log ("changedir2");
			BinaryProperty eid = (BinaryProperty) mph.HrGetOneProp (Property.EntryId);
state.Log ("changedir3");
			currentFolderEntryId = (eid != null) ? eid.Value : null;

			if (currentFolderEntryId == null)
				throw new Exception ("ServerConnection.ChangeDir: could not prepare currentFolderEntryId");
state.Log ("changedir almost done");
			return GetFolderProps ();
		}

		
		internal string[] GetSubDirNames (IMapiContainer parent)
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
				RowSet rows = tableReader.GetRows (30);
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

		internal bool GetFolderProps ()
		{
			return _GetFolderProps (out uidValidity, out uidNext, currentFolder);
		}

		public bool _GetFolderProps (out long _uidValidity, out long _uidNext, IMapiFolder folder)
		{
			_uidValidity = 0;
			_uidNext = 0;
			try {
				// UIDNEXT
				IntProperty val = (IntProperty) GetNamedProp (folder, IMAPGatewayNamedProperty.UIDNEXT);
				if (val != null) {
					_uidNext = val.Value;
					uidNextTag = val.PropTag;
				}
				state.Log ("uidnext from folder: "+ uidNext);
				state.Log ("uidNextTag from folder: "+ uidNextTag);
				
				//UIDVALIDITY
				val = (IntProperty) GetNamedProp (folder, IMAPGatewayNamedProperty.UIDVALIDITY);
				if (val != null) {
					_uidValidity = val.Value;
					uidValidityTag = val.PropTag;
				}
				state.Log ("uidvalidity from folder: "+ uidValidity);
				state.Log ("uidValidityTag from folder: "+ uidValidityTag);
				
				return true;
			} catch (Exception e) {
				state.Log ("GetFolderProps: Error: " + e.Message);
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
				state.Log ("SetUID: uid " + snli.UID + " error: "+ e.Message  );
				throw;
			}
			
			List<PropertyValue> lv = new List<PropertyValue> ();
			IntProperty longValue = (IntProperty) GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UID);
			longValue.Value = (int) snli.UID;
			lv.Add (longValue);
			
			UnicodeProperty unicodeValue = (UnicodeProperty) GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UID_Path);
			unicodeValue.Value = snli.Path;
			lv.Add (unicodeValue);

			BinaryProperty binaryValue = (BinaryProperty) GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UID_Creation_EntryId);
			binaryValue.Value = snli.CreationEntryId;
			lv.Add (binaryValue);

			
			state.Log ("Select: Message loaded");
			((IMapiProp) message).SetProps(lv.ToArray ());

			state.Log ("setUID");
			
			state.Log ("Select: Props set");
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
			state.Log ("new uidnext: "+ longValue.Value);

			// if not available, set UIDVALIDITY
			if (uidValidity == 0) {
				longValue = new IntProperty ();
				longValue.PropTag = uidValidityTag;
				DateTime dt = DateTime.Now;
				// this gives us UIDVALIDITYs that are about 1 second sharp for 130 years starting from Nov. 2008
				longValue.Value = (int) ((dt.Ticks >> 22) & 0xFFFFFFFF) - 2^29;
				lv.Add (longValue);
				state.Log ("new uidvalidity: "+ longValue.Value);
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

		public SequenceNumberList _BuildSequenceNumberList (out IMapiTable currentTable, IMapiFolder folder)
		{
			SequenceNumberList snl = new SequenceNumberList();
			state.Log ("Selectstart");
			currentTable = null;
			try {
				currentTable = folder.GetContentsTable (Mapi.Unicode);
				state.Log ("Select2");
			} catch (MapiException e) {
				if (e.HResult != Error.NoSupport)
					throw;
				return snl;
			}

			if (uidPropTag == 0 || uidPathPropTag == 0 || uidEntryIdPropTag == 0 || additionalFlagsPropTag == 0) {
				uidPropTag = GetNamedProp (folder, IMAPGatewayNamedProperty.UID).PropTag;
				uidPathPropTag = GetNamedProp (folder, IMAPGatewayNamedProperty.UID_Path).PropTag;
				uidEntryIdPropTag = GetNamedProp (folder, IMAPGatewayNamedProperty.UID_Creation_EntryId).PropTag;
				additionalFlagsPropTag = GetNamedProp (folder, IMAPGatewayNamedProperty.AdditionalFlags).PropTag;
			}
			
			currentTable.SetColumns( 
				PropertyTag.ArrayFromIntegers (
					new int[] { Property.EntryId, Property.InstanceKey, Property.Subject, uidPropTag, 
								uidPathPropTag, uidEntryIdPropTag, Property.MsgStatus, Property.MessageFlags,
								Outlook.Property_FLAG_STATUS, additionalFlagsPropTag
							  }), 0);
			
			state.Log ("Select1");
			while (true) {
				state.Log ("Select3");
				state.Log ("Select3b");
				RowSet rows = currentTable.QueryRows (10, Mapi.Unicode);
				state.Log ("Select4");
				if (rows.Count == 0)
					break;
				foreach (Row row in rows) {
					state.Log ("Select5");
					SequenceNumberListItem snli = new SequenceNumberListItem ();
					BinaryProperty entryId = (BinaryProperty) PropertyValue.GetArrayProp(row.Props, 0);
						
					state.Log ("Select5a");
					if (entryId != null) 
						snli.EntryId = entryId.Value;
					state.Log ("Select5b");
						
					PropertyValue val = PropertyValue.GetArrayProp(row.Props, 1);
					if (val != null) snli.InstanceKey = ((BinaryProperty) val).Value;
						
					val = PropertyValue.GetArrayProp(row.Props, 3);
					if (val != null) snli.UID = ((IntProperty) val).Value;
						
					val = PropertyValue.GetArrayProp(row.Props, 4);
					if (val != null) snli.Path = ((UnicodeProperty) val).Value;
						
					val = PropertyValue.GetArrayProp(row.Props, 5);
					if (val != null) snli.CreationEntryId = ((BinaryProperty) val).Value;
						
					val = PropertyValue.GetArrayProp(row.Props, 6);
					if (val != null) snli.MsgStatus = (ulong) ((IntProperty) val).Value;
Console.WriteLine ("MsgStatus: " + snli.MsgStatus + "UID: " + snli.UID);
						
					val = PropertyValue.GetArrayProp(row.Props, 7);
					if (val != null) snli.MessageFlags = (ulong) ((IntProperty) val).Value;

					val = PropertyValue.GetArrayProp(row.Props, 8);
					if (val != null) snli.FlagStatus = (ulong) ((IntProperty) val).Value;

					val = PropertyValue.GetArrayProp(row.Props, 9);
					try {
						if (val != null) 
							snli.AdditionalFlags = new List<string> ((string []) ((UnicodeArrayProperty) val).Value);
					} catch (Exception)
					{
					}

					state.Log ("Select8");
					snl.Add (snli);
				}
			}
			return snl;
		}

		internal int FixUIDsInSequenceNumberList ()
		{
			var query = from snl in sequenceNumberList
						where snl.UID == 0 || 
								snl.Path != currentPath ||
								!CompareEntryIDs (snl.CreationEntryId.ByteArray, snl.EntryId.ByteArray)
						select snl;
			int cnt = query.Count();
			
			foreach (SequenceNumberListItem snli in query) {
state.Log ("FixUIDsIn");
				SetUID (snli);
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

		internal static List<SequenceNumberListItem> _BuildSequenceSetQuery(SequenceNumberList snl, Command command)
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
					state.Log ("RebuildSequenceNumberListPlusUIDFix, retrys left: " + retrys);
					retrys --;
					// build sequence number list
					BuildSequenceNumberList ();

					// fix UIDS in Messagesif missing or broken
					state.Log ("fixUIDs");
					int recent = FixUIDsInSequenceNumberList ();
					return recent;
				} catch (MapiException e) {
					state.Log ("RebuildSequenceNumberListPlusUIDFix, Exception: " + e.Message);
					if (retrys <= 0)
						throw;
					Thread.Sleep(500);
				}
			}
				}
			return 0; // should never be reached
		}						


		public bool CompareEntryIDs (byte [] id1, byte [] id2)
		{
			
			if (id1 == null || id2 == null) 
				return false;
					
			if (id1.Length != id2.Length) 
				return false;
					
			for (int i=0; i < id1.Length; i++)
			{
				if (id1[i] != id2[i]) {
					return false;
				}
			}
			return true;
		}

		public PropertyValue GetNamedProp(IMapiProp mapiProperty, IMAPGatewayNamedProperty ignp)
		{
			state.Log ("GetNamedProp 1");
			NMapiGuid guid;
			PropertyValue prop = null;
			string name;
			PropertyType type = PropertyType.Error;
			switch (ignp) {
			case IMAPGatewayNamedProperty.Subscriptions:
				guid = Guids.PS_PUBLIC_STRINGS;
				name = "openmapi-root_folder-subscription_list2";
				type = PropertyType.MvUnicode;
				prop = new UnicodeArrayProperty ();
				break;
			case IMAPGatewayNamedProperty.UID:
				guid = Guids.PS_PUBLIC_STRINGS;
				name = "openmapi-message-UID";
				type= PropertyType.Long;
				prop = new IntProperty ();
				break;
			case IMAPGatewayNamedProperty.UID_Path:
				guid = Guids.PS_PUBLIC_STRINGS;
				name = "openmapi-message-UID_Path";
				type = PropertyType.Unicode;
				prop = new UnicodeProperty ();
				break;
			case IMAPGatewayNamedProperty.UID_Creation_EntryId:
				guid = Guids.PS_PUBLIC_STRINGS;
				name = "openmapi-message-UID_CreationEntryId";
				type = PropertyType.Binary;
				prop = new BinaryProperty ();
				break;
			case IMAPGatewayNamedProperty.UIDNEXT:
				guid = Guids.PS_PUBLIC_STRINGS;
				name = "openmapi-folder-UIDNEXT";
				type= PropertyType.Long;
				prop = new IntProperty ();
				break;
			case IMAPGatewayNamedProperty.UIDVALIDITY:
				guid = Guids.PS_PUBLIC_STRINGS;
				name = "openmapi-folder_UIDVALIDITY";
				type= PropertyType.Long;
				prop = new IntProperty ();
				break;
			case IMAPGatewayNamedProperty.AdditionalFlags:
				guid = Guids.PS_PUBLIC_STRINGS;
				name = "openmapi-message-AdditionalFlags";
				type = PropertyType.MvUnicode;
				prop = new UnicodeArrayProperty ();
				break;
			default:
				throw new Exception ("Named Property not defined");
			}
					
			state.Log ("GetNamedProp 2. name=" + name);
			state.Log ("GetNamedProp 2. guid=" + guid);
			state.Log ("GetNamedProp 2. type=" + type);
// This version has a problem, because r57 of NMapi.dll does return an Exception, if the property does not jet exist.
/*			MapiPropHelper mph = new MapiPropHelper (mapiProperty);
			PropertyValue spv = mph.HrGetNamedProp (guid, name);
			int tag = spv.PropTag;
*/
			StringMapiNameId mnid = new StringMapiNameId (name);
			mnid.Guid = guid;
			MapiNameId []  mnids = new MapiNameId [] { mnid };
			PropertyValue []  propsx = mapiProperty.GetProps (
					mapiProperty.GetIDsFromNames (mnids, NMAPI.MAPI_CREATE),
					Mapi.Unicode);			
			PropertyValue spv = propsx[0];

				

			int tag = spv.PropTag;
				
			state.Log ("GetNamedProp 3. Tag=" + tag);

// sicherheitsprüfung. Sonst wird evtl. der Store kaputt gemacht und outlook kann sich nicht mehr verbinden
			if (tag == 10)
				throw new Exception ("GetNamedProp: internal Problem. Error after getIdsFromNames");

				
			try {
				PropertyTag [] xx = PropertyTag.ArrayFromIntegers (new int [] {tag});
				PropertyValue[] props = mapiProperty.GetProps (xx, Mapi.Unicode);
				state.Log ("GetNamedProp 4");

				if (props.Length == 0  || props[0] is ErrorProperty){
					tag = PropertyTypeHelper.CHANGE_PROP_TYPE (tag, type).value__;
					prop.PropTag = tag;
					state.Log ("GetNamedProp 5");
				} else {
					prop = props[0];
				}
				
			} catch (Exception e) {
				throw new Exception ("GetNamedProp: Internal Problem: " + e.Message);
			}
			state.Log ("GetNamedProp 6, PropType:" + prop.GetType ());
			return prop;
		}
	}
}
