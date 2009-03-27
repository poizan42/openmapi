// openmapi.org - NMapi C# IMAP Gateway - CmdRename.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
//         Michael Kukat <michael.kukat@to.com>
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
using System.Linq;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Gateways.IMAP;
using NMapi.DirectoryModel;

namespace NMapi.Gateways.IMAP {

	public sealed class CmdRename : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "RENAME";
			}
		}

		public CmdRename (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			try {
				// check arguments
				if (command.Mailbox1 == command.Mailbox2) {
					state.ResponseManager.AddResponse (
						new Response (ResponseState.NO, Name, command.Tag).AddResponseItem ("destination name has to differ from source name"));
					return;
				}

				// check if folder is mapped
				if (state.FolderMappingAgent.MapiFolderIsMapped (ConversionHelper.MailboxIMAPToUnicode (command.Mailbox1))) {
					state.ResponseManager.AddResponse (
						new Response (ResponseState.NO, Name, command.Tag).AddResponseItem ("folder may not be renamed"));
					return;
				}

				// calculate all necessary path elements
				string srcPath = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + ConversionHelper.MailboxIMAPToUnicode (command.Mailbox1));
				string srcParentPath = PathHelper.GetParent (srcPath);
				string srcFolderName = PathHelper.GetLast (srcPath);

				string destPath = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + ConversionHelper.MailboxIMAPToUnicode (command.Mailbox2));
				string destParentPath = PathHelper.GetParent (destPath);
				string destFolderName = PathHelper.GetLast (destPath);

				// try to open folders
				IMapiFolder destFolder = null;
				destFolder = ServCon.OpenFolder (destParentPath);

				IMapiFolder parent = null;
				parent = ServCon.OpenFolder (srcPath);

				SBinary entryId = null;

				// if opening folders was successful, do the rename/move
				if(parent != null && destFolder != null) {
					// get entry ID
					var prop = new MapiPropHelper (parent).HrGetOneProp (Property.EntryId);
					entryId = ((BinaryProperty) prop).Value;

					// close parent (which in fact was source) and open source parent
					parent.Close ();
					parent = ServCon.OpenFolder (srcParentPath);

					// move folder to destination, use unicode-flag to make sure the name doesn't get garbled
					parent.CopyFolder (entryId.ByteArray, null, destFolder, destFolderName, null, NMAPI.FOLDER_MOVE | Mapi.Unicode);

					// if we came here, return success
					state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
				} else {
					// one of the folders wasn't a folder
					state.ResponseManager.AddResponse (
						new Response (ResponseState.NO, Name, command.Tag).AddResponseItem ("at least one of the arguments is not a folder"));
					return;
				}

			}
			catch (Exception e) {
				// XXX better error handling would be nice
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				state.Log (e.StackTrace);
			}
		}
	}
}
