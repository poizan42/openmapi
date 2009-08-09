// openmapi.org - NMapi C# IMAP Gateway - CmdExpunge.cs
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

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Gateways.IMAP;


namespace NMapi.Gateways.IMAP {

	public sealed class CmdExpunge : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "EXPUNGE";
			}
		}

		public CmdExpunge (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{

			// current solution will work with Evolution, because Evolution
			// will set all delete-Flags just before issuing Expunge.
			// we can therefor store the flags in the SequenceNumberList.
			// TeamXchange currently cannot store Data on the MessageFlags
			// Property permanently. (See CmdStore)

			try {
				if (command.UIDCommand) {
					DoUIDExpunge (state, ServCon, command);
					state.ResponseManager.AddResponse (new Response (ResponseState.OK, "UID EXPUNGE", command.Tag));
				} else {
					DoExpunge (state, ServCon, command);
					state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
				}
			} catch (Exception e) { 
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				Log (e.StackTrace);
			}
				
		}

		public static void DoExpunge (IMAPConnectionState state, ServerConnection servCon, Command command) {
			
			// get all snlis with deleted flag
			List<SequenceNumberListItem> snlDel = FlagHelper.DeletableMessagesAsSequenceNumberList (servCon.FolderHelper.SequenceNumberList);

			// delete messages
			var entry_ids = from snl in snlDel
							select snl.EntryId;
			EntryList el = new EntryList (entry_ids.ToArray ());
			servCon.FolderHelper.CurrentFolder.DeleteMessages (el, null, 0);
			servCon.FolderHelper.CurrentFolder.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
			
			// handle Expunge responses and manage SequenceNumberList
			foreach (SequenceNumberListItem snli in snlDel) {
				state.AddExpungeRequest (snli);
			}
		}

		public static void DoUIDExpunge (IMAPConnectionState state, ServerConnection servCon, Command command) {
			
state.Log ("uidexpunge0");
			// get all snlis with deleted flag
			List<SequenceNumberListItem> snlDel = FlagHelper.DeletableMessagesAsSequenceNumberList (servCon.FolderHelper.SequenceNumberList);
state.Log ("uidexpunge1");
			// subtract all Entry-IDs which have not been named by the sequence-set
			var querySequenceSet = servCon.FolderHelper.BuildSequenceSetQuery (command);
			var query2 = from snl in querySequenceSet
							where (0 != snlDel.Count ((x) => x.UID == snl.UID))
							select snl;

state.Log ("uidexpunge2");
			// delete Messages
			var entry_ids = from snl in query2
							select snl.EntryId;
			EntryList el = new EntryList (entry_ids.ToArray ());
state.Log ("uidexpunge3");
			servCon.FolderHelper.CurrentFolder.DeleteMessages (el, null, 0);
			servCon.FolderHelper.CurrentFolder.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
			
state.Log ("uidexpunge4");
			// handle Expunge responses and manage SequenceNumberList
			foreach (SequenceNumberListItem snli in query2) {
				state.AddExpungeRequest (snli);
			}
		}

	}
}
