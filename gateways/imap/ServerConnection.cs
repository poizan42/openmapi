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
using System.Diagnostics;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Utility;

namespace NMapi.Gateways.IMAP
{

	public enum IMAPGatewayNamedProperty {Subscriptions, UID, UID_Path, UID_Creation_EntryId, UIDNEXT, UIDVALIDITY};
	
//	public delegate void LogDelegate(string message);
	
	public class ServerConnection
	{
		private LogDelegate logInput;
		private LogDelegate logOutput;

		private Dictionary<string, string[]> providers;
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


		public ServerConnection (string host, string user, string password )
		{

Trace.WriteLine ("Server0");

/*
//this just takes endless
			providers = ProviderManager.FindProviders ();
Trace.WriteLine ("Server1");
			factory = ProviderManager.GetFactory (providers ["org.openmapi.txc"]);
*/
// 			factory = (IMapiFactory) Activator.CreateInstance ("NMapi.Provider.TeamXChange", "NMapi.Provider.TeamXChange.TeamXChangeMapiFactory").Unwrap () as IMapiFactory;

			IMAPGatewayConfig config = IMAPGatewayConfig.read ();
			object o = Activator.CreateInstance (
				config.Mapiproviderassembly, config.Mapiproviderfactorytype).Unwrap () as IMapiFactory;
			if (o == null)
				throw new Exception ("Couldn't create backend factory!");
			factory = (IMapiFactory) o;
			
			Trace.WriteLine ("Server2");
			try {
				session = factory.CreateMapiSession ();
				Trace.WriteLine ("Server3");
				mapiContext = new MapiContext (session);
				Trace.WriteLine ("Server4");
			} catch (MapiException e) {
				Trace.WriteLine ("ERROR: Can't open Mapi-Session!\n\n" + e.Message);
				return;
			}

			if (!CheckSessionMsg ())
				return;

			try {
				session.Logon (host, user, password);
				loggedOn = true;
			} catch (MapiException e) {
				if (e.HResult == Error.NetworkError) {
					Trace.WriteLine ("Couldn't connect to host!"+e.Message);
					throw;
				}
				else if (e.HResult == Error.NoAccess) {
					Trace.WriteLine ("No permission!");
					throw;
				}
				throw;
			}

			
			try {
				store = session.PrivateStore;
				Trace.WriteLine ("Server5");
			} catch (Exception) {
				throw;
			}
			
			SetRootDir();
			
			ChangeDir (string.Empty + PathHelper.PathSeparator); // Change to root dir
			Trace.WriteLine ("Server6");
			sequenceNumberList = new SequenceNumberList ();
		
		}

		public void Disconnect()
		{
			if (session != null)
				session.Dispose();
		}

		public LogDelegate LogInput {
			set { logInput = value; }
		}
		public LogDelegate LogOutput {
			set { logOutput = value; }
		}


		internal void SetRootDir()
		{
			GetReceiveFolderResult grfr = store.GetReceiveFolder (null, Mapi.Unicode);
			if (grfr.EntryID != null) {
				SPropValue eId = new SPropValue (Property.EntryId);
				eId.Value = new UPropValue ();
				eId.Value.Binary = new SBinary (grfr.EntryID);
				IMapiFolder folder = null;
				IMapiFolder prevFolder = null;
				byte[] prevEId = new byte[0];
				List<string> inboxPathEls = new List<string> ();
				Trace.WriteLine ("setrootdir 1");
				
				while (eId != null && !CompareEntryIDs(eId.Value.Binary.ByteArray, prevEId)) {
					Trace.WriteLine ("setrootdir 2");
					prevFolder = folder;
					OpenEntryResult oer = store.OpenEntry (eId.Value.Binary.ByteArray);
					if (oer != null)
						folder = (IMapiFolder) oer.Unk;
					
					Trace.WriteLine (folder.GetType ());
					Trace.WriteLine ( folder.HrGetOnePropNull (Property.DisplayNameW).Value.Unicode);
					Trace.WriteLine ("setrootdir 3");
					
					prevEId = eId.Value.Binary.ByteArray;
					if (folder != null) {
						inboxPathEls.Add (folder.HrGetOnePropNull (Property.DisplayNameW).Value.Unicode);
						eId = folder.HrGetOnePropNull (Property.ParentEntryId);
Trace.WriteLine ("setrootdir 4");
					}
				}
				if (prevFolder != null) {
					SPropValue dir = prevFolder.HrGetOnePropNull (Property.DisplayNameW);
					rootDir = PathHelper.PathSeparator + dir.Value.Unicode;
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
				Trace.WriteLine ("ERROR: Message-Store not open!");
				return false;
			}
			return true;
		}

		internal bool CheckSessionMsg ()
		{
			if (session == null) {
				Trace.WriteLine ("Session must be open!");
				return false;
			}
			return true;
		}

		internal bool CheckLoggedOnMsg ()
		{
			if (!loggedOn) {
				Trace.WriteLine ("ERROR: Not logged on!");
				return false;
			}
			return true;
		}


		internal string FullPath {
			get {
				string colon = ":";
				if (currentPath == null || currentPath == string.Empty)
					colon = "";
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
				SRowSet rows = tableReader.GetRows (10);
				if (rows.Count == 0)
					break;

				int nameIndex = -1;
				int entryIdIndex = -1;
				foreach (SRow row in rows) {
					if (nameIndex == -1) {
						nameIndex = SPropValue.GetArrayIndex (row.Props, Property.DisplayNameW);
						entryIdIndex  = SPropValue.GetArrayIndex (row.Props, Property.EntryId);
					}
				
					SPropValue name = SPropValue.GetArrayProp (row.Props, nameIndex);
					SPropValue eid  = SPropValue.GetArrayProp (row.Props, entryIdIndex);

					if (name != null && name.Value.Unicode == match)
						return action (parent, eid.Value.Binary);
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
					entryId.ByteArray, null, Mapi.Modify).Unk;
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
				return (IMapiFolder) store.Root.Unk;

			IMapiContainer container = (IMapiContainer) store.Root.Unk;

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
			if (!CheckStore())
				return false;
			IMapiFolder newFolder = null;
			newFolder = OpenFolder (path);
			if (newFolder == null) {
				Trace.WriteLine ("cd: " + path + ": No such folder.");
				return false;
			}
			currentFolder = newFolder;
			currentPath = path;
			currentFolderTable = null;
			
			SPropValue eid = currentFolder.HrGetOneProp (Property.EntryId);
			currentFolderEntryId = eid.Value.Binary;

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
				SRowSet rows = tableReader.GetRows (30);
				if (rows.Count == 0)
					break;
				int nameIndex = -1;
				foreach (SRow row in rows) {
					if (nameIndex == -1)
						nameIndex = SPropValue.GetArrayIndex (row.Props, Property.DisplayNameW);
					SPropValue name = SPropValue.GetArrayProp (row.Props, nameIndex);
					names.Add (name.Value.Unicode);
				}
			}
			return names.ToArray ();
		}

		internal bool GetFolderProps ()
		{
			try {
				// UIDNEXT
				SPropValue val = GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UIDNEXT);
				if (val != null) {
					uidNext = (long) val.Value.l;
					uidNextTag = val.PropTag;
				}
				Trace.WriteLine ("uidnext from folder: "+ uidNext);
				Trace.WriteLine ("uidNextTag from folder: "+ uidNextTag);
				
				//UIDVALIDITY
				val = GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UIDVALIDITY);
				if (val != null) {
					uidValidity = (long) val.Value.l;
					uidValidityTag = val.PropTag;
				}
				Trace.WriteLine ("uidvalidity from folder: "+ uidValidity);
				Trace.WriteLine ("uidValidityTag from folder: "+ uidValidityTag);
				
				return true;
			} catch (Exception) {
				return false;
			}
		}			

		private void SetUID (SequenceNumberListItem snli)
		{
			
			snli.UID = UpdateNextUid();
			snli.Path = currentPath;
			snli.CreationEntryId = snli.EntryId;
			
			IMapiProp message = (IMapiProp) currentFolder.OpenEntry(snli.EntryId.ByteArray, null, 1 /* MAPI_MODIFY*/).Unk;
			
			List<SPropValue> lv = new List<SPropValue> ();
			UPropValue uvalue = new UPropValue ();
			uvalue.l = (int)snli.UID;
			SPropValue svalue = GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UID);
			svalue.Value = uvalue;
			lv.Add (svalue);
			
			uvalue = new UPropValue ();
			uvalue.Unicode = snli.Path;
			svalue = GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UID_Path);
			svalue.Value= uvalue;
			lv.Add (svalue);

			uvalue = new UPropValue ();
			uvalue.Binary = snli.CreationEntryId;
			svalue = GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UID_Creation_EntryId);
			svalue.Value= uvalue;
			lv.Add (svalue);

			Trace.WriteLine ("Select: Message loaded");
			((IMapiProp) message).SetProps(lv.ToArray ());
			
			Trace.WriteLine ("Select: Props set");
			((IMapiProp) message).SaveChanges (0);
				
			}

		internal long UpdateNextUid () {
			// get current UIDNEXT
			GetFolderProps ();
			// save current uidnext
			long luidNext = uidNext;

			// update uidnext value in the store
			List<SPropValue> lv = new List<SPropValue> ();
			SPropValue svalue = new SPropValue (uidNextTag);
			svalue.Value.l = (int) luidNext + 1;
			lv.Add (svalue);
			Trace.WriteLine ("new uidnext: "+ svalue.Value.li);

			// if not available, set UIDVALIDITY
			if (uidValidity == 0) {
				svalue = new SPropValue (uidValidityTag);
				DateTime dt = DateTime.Now;
				// this gives us UIDVALIDITYs that are about 1 second sharp for 130 years starting from Nov. 2008
				svalue.Value.l = (int) ((dt.Ticks >> 22) & 0xFFFFFFFF) - 2^29;
				lv.Add (svalue);
				Trace.WriteLine ("new uidvalidity: "+ svalue.Value.li);
			}

			
			currentFolder.SetProps(lv.ToArray ());
			currentFolder.SaveChanges (0);

			//re-get UIDNEXT with updated value
			GetFolderProps ();
			
			return luidNext;
		}
		

		internal void BuildSequenceNumberList ()
		{
			sequenceNumberList = new SequenceNumberList();
			Trace.WriteLine ("Selectstart");
			IMapiTable contentsTable = null;
			try {
				currentFolderTable = currentFolder.GetContentsTable (Mapi.Unicode);
				Trace.WriteLine ("Select2");
			} catch (MapiException e) {
				if (e.HResult != Error.NoSupport)
					throw;
				return;
			}

			SPropValue uidProp = GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UID);
			SPropValue uidPathProp = GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UID_Path);
			SPropValue uidEntryIdProp = GetNamedProp (currentFolder, IMAPGatewayNamedProperty.UID_Creation_EntryId);
			
			currentFolderTable.SetColumns( 
				new SPropTagArray (
					new int[] { Property.EntryId, Property.InstanceKey, Property.Subject, uidProp.PropTag, 
								uidPathProp.PropTag, uidEntryIdProp.PropTag, Property.MsgStatus, Property.MessageFlags
							  }), 0);
			
			Trace.WriteLine ("Select1");
			while (true) {
				Trace.WriteLine ("Select3");
				Trace.WriteLine ("Select3b");
				SRowSet rows = currentFolderTable.QueryRows (10, Mapi.Unicode);
				Trace.WriteLine ("Select4");
				if (rows.Count == 0)
					break;
				foreach (SRow row in rows) {
					Trace.WriteLine ("Select5");
					SequenceNumberListItem snli = new SequenceNumberListItem ();
					SPropValue entryId = SPropValue.GetArrayProp(row.Props, 0);
						
					Trace.WriteLine ("Select5a");
					if (entryId != null) 
						snli.EntryId = entryId.Value.Binary;
					Trace.WriteLine ("Select5b");
						
					SPropValue val = SPropValue.GetArrayProp(row.Props, 1);
					if (val != null) snli.InstanceKey = val.Value.Binary;
						
					val = SPropValue.GetArrayProp(row.Props, 3);
					try {if (val != null) snli.UID = val.Value.l;}
						catch (Exception) {}
						
					val = SPropValue.GetArrayProp(row.Props, 4);
					if (val != null) snli.Path = val.Value.Unicode;
						
					val = SPropValue.GetArrayProp(row.Props, 5);
					if (val != null) snli.CreationEntryId = val.Value.Binary;
						
					val = SPropValue.GetArrayProp(row.Props, 6);
					if (val != null) snli.MsgStatus = (ulong) val.Value.l;
						
					val = SPropValue.GetArrayProp(row.Props, 7);
					if (val != null) snli.MessageFlags = (ulong) val.Value.l;

					Trace.WriteLine ("Select8");
					sequenceNumberList.Add (snli);
				}
			}
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
				SetUID (snli);
			}

			sequenceNumberList.Sort ();
			return cnt;
		}

		internal int SequenceNumberOf (SequenceNumberListItem snli)
		{
			return sequenceNumberList.FindIndex(delegate(SequenceNumberListItem snli1) {
											return snli1.UID == snli.UID;
										});
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
					Trace.WriteLine ("first: "+first);
					Trace.WriteLine ("second: "+second);
				/* handle cases */
				if (sel.Second != null) { /* Range */
					if (command.UIDCommand)
						query = from els in snl.AsQueryable ()
								where els.UID >= first && els.UID <= second
								select els;
					else
						query = from els in snl.AsQueryable ()
								where snl.IndexOfSNLI (els) >= first &&
									  snl.IndexOfSNLI (els) <= second
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

		public SPropValue GetNamedProp(IMapiProp mapiProperty, IMAPGatewayNamedProperty ignp)
		{
			Trace.WriteLine ("GetNamedProp 1");
			MapiNameId[] names = new MapiNameId[1];
			int type = 0;
			switch (ignp) {
			case IMAPGatewayNamedProperty.Subscriptions:
				names[0] = new MapiNameId ();
				names[0].Guid = Guids.PS_PUBLIC_STRINGS;
				names[0].UKind.StrName = "openmapi-root_folder-subscription_list";
				names[0].UlKind = MnId.String;
				type = PropertyType.MvUnicode.value__;
				break;
			case IMAPGatewayNamedProperty.UID:
				names[0] = new MapiNameId ();
				names[0].Guid = Guids.PS_PUBLIC_STRINGS;
				names[0].UKind.StrName = "openmapi-message-UID";
				names[0].UlKind = MnId.String;
				type= PropertyType.Long.value__;
				break;
			case IMAPGatewayNamedProperty.UID_Path:
				names[0] = new MapiNameId ();
				names[0].Guid = Guids.PS_PUBLIC_STRINGS;
				names[0].UKind.StrName = "openmapi-message-UID_Path";
				names[0].UlKind = MnId.String;
				type = PropertyType.Unicode.value__;
				break;
			case IMAPGatewayNamedProperty.UID_Creation_EntryId:
				names[0] = new MapiNameId ();
				names[0].Guid = Guids.PS_PUBLIC_STRINGS;
				names[0].UKind.StrName = "openmapi-message-UID_CreationEntryId";
				names[0].UlKind = MnId.String;
				type = PropertyType.Binary.value__;
				break;
			case IMAPGatewayNamedProperty.UIDNEXT:
				names[0] = new MapiNameId ();
				names[0].Guid = Guids.PS_PUBLIC_STRINGS;
				names[0].UKind.StrName = "openmapi-folder-UIDNEXT";
				names[0].UlKind = MnId.String;
				type= PropertyType.Long.value__;
				break;
			case IMAPGatewayNamedProperty.UIDVALIDITY:
				names[0] = new MapiNameId ();
				names[0].Guid = Guids.PS_PUBLIC_STRINGS;
				names[0].UKind.StrName = "openmapi-folder_UIDVALIDITY";
				names[0].UlKind = MnId.String;
				type= PropertyType.Long.value__;
				break;
			default:
				throw new Exception ("Named Property not defined");
			}
					
			Trace.WriteLine ("GetNamedProp 2");
			SPropTagArray spta = mapiProperty.GetIDsFromNames (names, NMAPI.MAPI_CREATE);
			int tag = spta.PropTagArray[0];
			Trace.WriteLine ("GetNamedProp 3");

			SPropValue prop = null;
			try {
				SPropTagArray xx = new SPropTagArray (new int [] {tag});
				SPropValue[] props = mapiProperty.GetProps (xx, Mapi.Unicode);
				Trace.WriteLine ("GetNamedProp 4");
				if (props.Length == 0 || props[0].Value.err != 0) {
					PropertyTypeHelper pth = new PropertyTypeHelper ();
					tag = pth.CHANGE_PROP_TYPE (tag, type);
					prop = new SPropValue (tag);
					Trace.WriteLine ("GetNamedProp 5");
				} else {
					prop = props[0];
				}
				
			} catch (Exception e) {
				throw new Exception ("GetNamedProp: Internal Problem: " + e.Message);
			}
			Trace.WriteLine ("GetNamedProp 6");
			return prop;
		}
	}
}
