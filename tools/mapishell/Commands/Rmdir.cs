//
// openmapi.org - NMapi C# Mapi API - Rmdir.cs
//
// Copyright 2008-2010 Topalis AG
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
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Tools.Shell {

	public sealed class RmdirCommand : AbstractBaseCommand
	{
		public override string Name {
			get {
				return "rmdir";
			}
		}

		public override string[] Aliases {
			get {
				return new string [0];
			}
		}

		public override string Description {
			get {
				return  "Deletes a folder.";
			}
		}

		public override string Manual {
			get {
				return null;
			}
		}

		public RmdirCommand (Driver driver, ShellState state) : base (driver, state)
		{
		}

		private void PrintErrorItemsExist (string items)
		{
			// TODO: "error"-call!
			driver.WriteLine ("Directory contains " + items + 
				" and the directory can't be deleted. " + 
				"Specify -R to still delete it.");
		}

		public override void Run (CommandContext context)
		{
			int flags = 0;
			bool deleteRecursive = false;
			
			// TODO: param option to recursively delete.
			
			if (deleteRecursive)
				flags |= NMAPI.DEL_MESSAGES | NMAPI.DEL_FOLDERS;
			
			Action<IMapiFolder, string> op = (parent, folderName) => {
				SBinary entryId = state.GetSubDirEntryId (parent, folderName);
				ShellProgressBar progressBar = driver.CreateProgressBar ("Deleting directory");
			
				if (entryId != null) {
					try {
						parent.DeleteFolder (entryId.ByteArray, progressBar, flags);						
					} catch (MapiHasMessagesException) {
						if (deleteRecursive)
							throw;
						PrintErrorItemsExist ("messages");
					} catch (MapiHasFoldersException) {
						if (deleteRecursive)
							throw;
						PrintErrorItemsExist ("subfolders");
					} finally {
						if (progressBar != null) {
							progressBar.Close ();
							progressBar = null;
						}
					}
				} else
					driver.WriteLine ("Directory not found."); // TODO: "error-call"!
			};
	
			state.PerformOperationOnFolder (context, op);
		}

	}
}
