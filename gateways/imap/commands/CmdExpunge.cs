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
				DoExpunge (state, ServCon, command);
				state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
			} catch (Exception e) { 
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				state.Log (e.StackTrace);
			}
				
		}

		public static void DoExpunge (IMAPConnectionState state, ServerConnection servCon, Command command) {
			
			EntryList el = FlagHelper.DeletableMessages (servCon.FolderHelper.SequenceNumberList);
			servCon.FolderHelper.CurrentFolder.DeleteMessages (el, null, 0);
			servCon.FolderHelper.CurrentFolder.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
			
			// handle Expunge responses and manage SequenceNumberList
			var query2 = from toDel in servCon.FolderHelper.SequenceNumberList
						where (FlagHelper.IsDeleteMarked (toDel))
						select toDel;
			foreach (SequenceNumberListItem snli in query2) {
				state.AddExpungeRequest (snli);
			}
		}

	}
}
