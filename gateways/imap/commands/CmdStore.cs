// openmapi.org - NMapi C# IMAP Gateway - CmdStore.cs
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


namespace NMapi.Gateways.IMAP {

	public sealed class CmdStore : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "STORE";
			}
		}

		public CmdStore (IMAPConnectionState state) : base (state)
		{


		}

		public override void Run (Command command)
		{
			var slq = ServCon.BuildSequenceSetQuery(command);
			foreach (SequenceNumberListItem snli in slq) {
Console.WriteLine ("Store command loop");			

				FlagHelper fh = new FlagHelper (snli);
				fh.ProcessFlagChangesStoreCommand (command);
				fh.FillFlagsIntoSNLI (snli);
					
				IMessage msg = null;
				// store the changes
			
				try {
					// try block, to ignore if an email cannot be found
					state.Log ("Store uid: " + snli.UID);
					msg = (IMessage) ServCon.Store.OpenEntry (snli.EntryId.ByteArray, null, Mapi.Modify);
				
				} catch (Exception e) {
					state.Log ("CmdStore " + e.Message);
					state.Log (e.StackTrace);
				}
				fh.SaveFlagsIntoIMessage (msg, state.ServerConnection);
				
			}

			state.AddExistsRequestDummy ();
			
			Response r = new Response (ResponseState.OK, Name, command.Tag);
			r.UIDResponse = command.UIDCommand;
			state.ResponseManager.AddResponse (r);
		}



		
	}
}
