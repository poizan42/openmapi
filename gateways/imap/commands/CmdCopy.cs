// openmapi.org - NMapi C# IMAP Gateway - CmdCopy.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

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

	public sealed class CmdCopy : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "COPY";
			}
		}

		public CmdCopy (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{

			try {
				var slq = ServCon.FolderHelper.BuildSequenceSetQuery (command);
				// get sorted list, so we can generate optimized sequence sets in the copyuid reply
				var entry_ids = from eId in slq
								orderby eId.UID        // sort by uid!
								select eId.EntryId; 
				EntryList el = new EntryList (entry_ids.ToArray ()); 
				var uids = from eId in slq
								orderby eId.UID       // sort by uid!
								select eId.UID;
	
				string path = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + ConversionHelper.MailboxIMAPToUnicode (command.Mailbox1));

				using (FolderHelper destFH = new FolderHelper (ServCon, path)) {

					IMapiProgress imp = null;
					SBinary[] resultEntries= IMapiFolderExtender.CopyMessagesAndIdentify (
						ServCon.FolderHelper.CurrentFolder, 
						el, 
						null, 
						destFH.CurrentFolder, 
						imp, 
						0);
	//				ServCon.FolderHelper.CurrentFolder.CopyMessages (el, null, dest, imp, 0);

					// create UID's and collect them in a list
					List<long> newUids = new List<long> ();
					foreach (SBinary entryId in resultEntries) {
						using (IMessage im = (IMessage) destFH.CurrentFolder.OpenEntry 
								(entryId.ByteArray, null, Mapi.Unicode | Mapi.Modify)) {
							SequenceNumberListItem snli = destFH.AppendAndFixNewMessage (im);
							newUids.Add (snli.UID);					
						}
					}
					Response r = new Response (ResponseState.OK, Name, command.Tag);
					r.Val = new ResponseItemList ().SetSigns ("", "")
							.AddResponseItem ("COPYUID")
							.AddResponseItem (destFH.UIDVALIDITY.ToString ())
							.AddResponseItem (FolderHelper.BuildSequenceSetString (uids))
							.AddResponseItem (FolderHelper.BuildSequenceSetString (newUids));
					state.ResponseManager.AddResponse (r);
				}
			}
			catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				Log (e.StackTrace);
			}
		}

	}
}
