//
// openmapi.org - NMapi C# Mapi API - ShellState.cs
//
// Copyright 2008 Topalis AG
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
using System.Linq;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;

using NMapi.DirectoryModel;

namespace NMapi.Tools.Shell {

	public sealed class ShellState
	{
		private Driver driver;

		private KeyList keyList;
		private Dictionary<string, AbstractBaseCommand> commands;
		private TypeResolver resolver;
		private PropertyLookup propertyLookup;

		private Variables variables;
		private Functions functions;

		private string host;
		private string user;
		private string currentPath;

		private string storeStr;
		private IMsgStore store;

		private IMapiFolder currentFolder;

		private Stack<string> pathStack;

		private Dictionary<string, string[]> providers;
		private bool loggedOn;
		private bool logging;

		private IMapiFactory factory;
		private IMapiSession session;
		private MapiContext mapiContext;

		private int currentScope = 0;

		internal void EnterScope ()
		{
			currentScope++;
		}

		internal void ExitScope ()
		{
			// for this to work the variables in the currentScope
			// must be on the top of the stack.
			variables.DropCurrentScope ();

			currentScope--;
		}

		internal Driver Driver {
			get { return driver; }
		}


		/// <summary>
		/// 
		/// </summary>
		internal Dictionary<string, AbstractBaseCommand> Commands {
			get { return commands; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal TypeResolver Resolver {
			get { return resolver; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal PropertyLookup PropertyLookup {
			get { return propertyLookup; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal string Host {
			get { return host; }
			set { host = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal string User {
			get { return user; }
			set { user = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal string CurrentPath {
			get { return currentPath; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal string StoreStr {
			get { return storeStr; }
			set { storeStr = value; }
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
		internal IMapiFolder CurrentFolder {
			get { return currentFolder; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal Stack<string> PathStack {
			get { return pathStack; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal Dictionary<string, string[]> Providers {
			get { return providers; }
			set { providers = value; }
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
		internal bool Logging {
			get { return logging; }
			set { logging = value; }
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

		/// <summary>
		/// 
		/// </summary>
		internal Variables Variables {
			get { return variables; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal Functions Functions {
			get { return functions; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal int CurrentScope {
			get { return currentScope; }
		}

		/// <summary>
		/// 
		/// </summary>
		internal KeyList KeyList {
			get { return keyList; }
		}

		internal bool CheckSessionMsg ()
		{
			if (session == null) {
				driver.WriteLine ("Session must be open!");
				return false;
			}
			return true;
		}

		internal bool CheckLoggedOnMsg ()
		{
			if (!loggedOn) {
				driver.WriteLine ("ERROR: Not logged on!");
				return false;
			}
			return true;
		}

		internal bool CheckStore ()
		{
			if (store == null) {
				driver.WriteLine ("ERROR: Message-Store not open!");
				return false;
			}
			return true;
		}

		internal string FullPath {
			get {
				if (storeStr == null || storeStr == String.Empty)
					return "";

				string colon = ":";
				if (currentPath == null || currentPath == String.Empty)
					colon = "";
				return storeStr + colon + currentPath;
			}
		}

		internal bool CommandExists (string name)
		{
			return commands.ContainsKey (name);
		}


		public ShellState (Driver driver)
		{
			this.driver = driver;
			this.logging = true;
			this.pathStack = new Stack<string> ();

			this.resolver = new TypeResolver ();
			this.propertyLookup = new PropertyLookup (resolver);
			this.commands = new Dictionary<string, AbstractBaseCommand> ();

			this.keyList = new KeyList (this);
			this.variables = new Variables (this);
			this.functions = new Functions (driver, this);
		}


		internal string PropTag2Name (int propTag)
		{
			string name = propertyLookup.GetName (propTag);
			if (name != null)
				return name;
			return "" + propTag;
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
				
					PropertyValue name = PropertyValue.GetArrayProp (row.Props, nameIndex);
					PropertyValue eid  = PropertyValue.GetArrayProp (row.Props, entryIdIndex);

					if (name != null && ((UnicodeProperty) name).Value == match)
						return action (parent, ((BinaryProperty) eid).Value);
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
			string[] parts = PathHelper.Path2Array (path);

			if (path == String.Empty)
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
			if (!CheckStore())
				return false;
			IMapiFolder newFolder = null;
			newFolder = OpenFolder (path);
			if (newFolder == null) {
				driver.WriteLine ("cd: " + path + ": No such folder.");
				return false;
			}
			currentFolder = newFolder;
			currentPath = path;


			// TODO: Reset keyList dictionary ??? - scope ?


			return true;
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
				RowSet rows = tableReader.GetRows (10);
				if (rows.Count == 0)
					break;
				int nameIndex = -1;
				foreach (Row row in rows) {
					if (nameIndex == -1)
					nameIndex = PropertyValue.GetArrayIndex (row.Props, Property.DisplayNameW);
					PropertyValue name = PropertyValue.GetArrayProp (row.Props, nameIndex);
					names.Add (((UnicodeProperty) name).Value);
				}
			}
			return names.ToArray ();
		}


		// TODO: support named properties!
		// TODO: InteractiveResolveEntryID doesn't work with objects outside current working dir!

		internal IMapiProp OpenPropObj (string keyName)
		{
			SBinary entryID = keyList.InteractiveResolveEntryID (keyName);
			if (entryID == null)
				return null;
			return (IMapiProp) currentFolder.OpenEntry  (entryID.ByteArray); // TODO: IMapi Prop assumed!
		}


		internal void PerformOperationOnFolder (CommandContext context, Action<IMapiFolder, string> operation)
		{
			if (context.Param == String.Empty) {
				AbstractBaseCommand.RequireMsg (driver, "path");
				return;
			}

			if (!CheckStore())
				return;

			string param1 = ShellUtil.SplitParams (context.Param) [0].Trim ();
			string path = Input2AbsolutePath (param1);
			string parentPath = PathHelper.GetParent (path);
			string folderName = PathHelper.GetLast (path);

			IMapiFolder parent = null;
			try {
				parent = OpenFolder (parentPath);
				operation (parent, folderName);
			} catch (MapiException e) {
				if (e.HResult == Error.NotFound) {
					driver.WriteLine ("Parent not found!");
					return;
				} else if (e.HResult == Error.NoAccess) {
					driver.WriteLine ("No permission to create folder!");
					return;
				} else
					throw;
			} finally {
				if (parent != null)
					parent.Close ();
			}
		}

		internal void CopyOrMove (string srcPath, string destPath, bool move)
		{
			string srcParentPath = PathHelper.GetParent (srcPath);
			string srcFolderOrFileName = PathHelper.GetLast (srcPath);

			string destParentPath = PathHelper.GetParent (destPath);
			string destFolderOrFileName = PathHelper.GetLast (destPath);

			IMapiFolder destFolder = null;
			try {
				destFolder = OpenFolder (destParentPath);

				IMapiFolder parent = null;
				try {
					parent = OpenFolder (srcPath);

					SBinary entryId = null;
					if (parent != null) {
						// Source is a folder!
						var prop = new MapiPropHelper (parent).HrGetOneProp (Property.EntryId);
						entryId = ((BinaryProperty) prop).Value;
						parent.Close ();
						parent = OpenFolder (srcParentPath);

						int flags = 0;
						if (move)
							flags = NMAPI.FOLDER_MOVE;
						parent.CopyFolder (entryId.ByteArray, null, 
							destFolder, destFolderOrFileName, null, flags);
					} else {
						// Source is a file
						parent = OpenFolder (srcParentPath);
						entryId = keyList.InteractiveResolveEntryID (parent, srcFolderOrFileName);
						if (entryId == null) {
							driver.WriteLine ("ERROR: Source file not found!");
							return;
						}
						EntryList list = new EntryList (new SBinary [] { entryId });
						int flags = 0;
						if (move)
							flags = NMAPI.MESSAGE_MOVE;
						parent.CopyMessages (list, null, destFolder, null, flags);
					}
				
				} catch (MapiException e) {
					if (e.HResult == Error.NotFound) {
						driver.WriteLine ("Parent not found!");
						return;
					} else if (e.HResult == Error.NoAccess) {
						driver.WriteLine ("No permission to create folder!");
						return;
					} else
						throw;
				} finally {
					if (parent != null)
						parent.Close ();
				}

			} catch (MapiException e) {
				if (e.HResult == Error.NotFound) {
					driver.WriteLine ("Parent not found!");
					return;
				} else if (e.HResult == Error.NoAccess) {
					driver.WriteLine ("No permission to read target folder!");
					return;
				} else
					throw;
			} finally {
				if (destFolder != null)
					destFolder.Close ();
			}
			
		}
	}

}
