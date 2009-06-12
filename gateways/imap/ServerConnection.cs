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
		private bool loggedOn;
		private string user;
		private string rootDir;
		private string inboxPath;
		private FolderHelper folderHelper;
		

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

		internal FolderHelper FolderHelper {
			get { return folderHelper; }
			set { folderHelper = value; }
		}

		internal IMAPConnectionState State {
			get { return state; }
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

			folderHelper = new FolderHelper (this, string.Empty + PathHelper.PathSeparator); // Change to root dir
			
			state.Log ("Server6");
		
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
				return folderHelper.CurrentPath;
			}
		}

		internal void CloseSession ()
		{
			loggedOn = false;
			if (mapiContext != null)
				mapiContext.Dispose ();
			
			if (folderHelper.CurrentFolder != null)
				folderHelper.CurrentFolder.Dispose ();
			
			if (store != null)
				store.Dispose ();
			
			if (session != null)
				session.Dispose ();
			
			mapiContext = null;
			folderHelper = null;
			session = null;
			store = null;
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
					mapiProperty.GetIDsFromNames (mnids, Mapi.Create),
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
					tag = PropertyTypeHelper.CHANGE_PROP_TYPE (tag, type);
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
