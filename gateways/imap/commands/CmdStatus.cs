// openmapi.org - NMapi C# IMAP Gateway - CmdStatus.cs
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
using NMapi.Table;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Gateways.IMAP;
using NMapi.DirectoryModel;


namespace NMapi.Gateways.IMAP {

	public sealed class CmdStatus : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "STATUS";
			}
		}

		public CmdStatus (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command) 
		{
			
			if (command.Mailbox1 != null) {
				try {
					string path = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + ConversionHelper.MailboxIMAPToUnicode (command.Mailbox1));
					Log ("Select: path = " + path);
					using (IMapiFolder folder = ServCon.FolderHelper.OpenFolder (path)) {

						// build sequence number list
						SequenceNumberList snl = ServCon.FolderHelper._BuildSequenceNumberList (folder);

						int recent = snl.Count ((x) => x.UID == null);
						
						int unseen = FlagHelper.GetUnseenIDFromSNL (snl);

						MyStringComparer mycomp = new MyStringComparer ();
						// write Responses
						Response r;
						r = new Response (ResponseState.NONE, "STATUS");
						r.AddResponseItem (command.Mailbox1);
						ResponseItemList ril = new ResponseItemList ();
						if (command.Status_list.Contains ("MESSAGES", mycomp)) {
							ril.AddResponseItem ("MESSAGES");
							ril.AddResponseItem (snl.Count.ToString ());
						}
						if (command.Status_list.Contains ("RECENT", mycomp)) {
							ril.AddResponseItem ("RECENT");
							ril.AddResponseItem (recent.ToString ());
						}
						if (command.Status_list.Contains ("UNSEEN", mycomp)) {
							ril.AddResponseItem ("UNSEEN");
							ril.AddResponseItem (unseen.ToString ());
						}
						if (command.Status_list.Contains ("UIDVALIDITY", mycomp) || command.Status_list.Contains ("UIDNEXT")) {
							long uidnext, uidvalidity;
							ServCon.FolderHelper._GetFolderProps (out uidvalidity, out uidnext, folder);
							if (command.Status_list.Contains ("UIDVALIDITY", mycomp)) {
								ril.AddResponseItem ("UIDVALIDITY");
								ril.AddResponseItem (uidvalidity.ToString ());
							}
							if (command.Status_list.Contains ("UIDNEXT", mycomp)) {
								ril.AddResponseItem ("UIDNEXT");
								ril.AddResponseItem (uidnext.ToString ());
							}
						}
						r.AddResponseItem (ril);
						state.ResponseManager.AddResponse (r);
						state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
						return;
					}
				} catch (Exception e) {
					state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
					Log (e.StackTrace);
					return;
				}
			}
			state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem ("not succeeded", ResponseItemMode.ForceAtom));
			return;
		}

	}
}
