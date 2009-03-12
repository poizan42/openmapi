// openmapi.org - NMapi C# IMAP Gateway - CmdSelect.cs
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
using System.Diagnostics;

using NMapi;
using NMapi.Table;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Gateways.IMAP;
using NMapi.DirectoryModel;

namespace NMapi.Gateways.IMAP {

	public class CmdSelect : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "SELECT";
			}
		}

		public CmdSelect (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			if (state.CurrentState == IMAPConnectionStates.AUTHENTICATED ||
			    state.CurrentState == IMAPConnectionStates.SELECTED) {
				state.CurrentState = IMAPConnectionStates.AUTHENTICATED;
				bool res = DoRun(command, false);
				if (res)
					state.CurrentState = IMAPConnectionStates.SELECTED;
			}
		}

		protected bool DoRun (Command command, bool examine) 
		{
			if (command.Mailbox1 != null) {
				try {
					string path = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + ConversionHelper.MailboxIMAPToUnicode (command.Mailbox1));
					state.Log ("Select: path = " + path);					
					if (!ServCon.ChangeDir (path)) {
						state.ResponseManager.AddResponse (
							new Response (ResponseState.NO, Name, command.Tag).AddResponseItem ("given folder does not exist"));
						return false;
					}

					// clear ExpungeRequestList
					state.ResetExpungeRequests ();
					
					// clear ExistsRequestList
					state.ResetExistsRequests ();
					
					// if UIDNEXT/UIDVALIDITY is not set, go fix that and UIDVALIDITY
					if (ServCon.UIDNEXT == 0 || ServCon.UIDVALIDITY == 0)
						ServCon.UpdateNextUid ();

					// build sequence number list
					int recent = ServCon.RebuildSequenceNumberListPlusUIDFix ();
					
					// connect notification handler. Need to wait until SequenceNumberList is finally prepared
					new NotificationHandler (state);
					
					int unseen = FlagHelper.GetUnseenIDFromSNL (ServCon.SequenceNumberList);

	
					// write Responses
					Response r;
					r = new Response (ResponseState.NONE, "EXISTS");
					r.Val = new ResponseItemText(ServCon.SequenceNumberList.Count.ToString ());
					state.ResponseManager.AddResponse (r);
					r = new Response (ResponseState.NONE, "RECENT");   //TODO: need to make sure, the messages return the recent flag in search/fetch
					r.Val = new ResponseItemText(recent.ToString ());
					state.ResponseManager.AddResponse (r);
					r = new Response (ResponseState.OK, "UNSEEN");
					r.Val = new ResponseItemText(unseen.ToString ());
					state.ResponseManager.AddResponse (r);
					r = new Response (ResponseState.OK, "UIDVALIDITY");
					r.Val = new ResponseItemText(ServCon.UIDVALIDITY.ToString ());
					state.ResponseManager.AddResponse (r);
					r = new Response (ResponseState.OK, "UIDNEXT");
					r.Val = new ResponseItemText(ServCon.UIDNEXT.ToString ());
					state.ResponseManager.AddResponse (r);
					r = new Response (ResponseState.NONE, "FLAGS");
					r.AddResponseItem(new ResponseItemList ().AddResponseItem ("\\Answered", ResponseItemMode.ForceAtom) 
															.AddResponseItem ("\\Flagged", ResponseItemMode.ForceAtom) 
															.AddResponseItem ("\\Deleted", ResponseItemMode.ForceAtom) 
															.AddResponseItem ("\\Seen", ResponseItemMode.ForceAtom) 
															.AddResponseItem ("\\Draft", ResponseItemMode.ForceAtom)); 
					state.ResponseManager.AddResponse (r);
					r = new Response (ResponseState.OK, "PERMANENTFLAGS");
					r.Val= new ResponseItemList().AddResponseItem ("\\Answered", ResponseItemMode.ForceAtom)
												.AddResponseItem ("\\Flagged", ResponseItemMode.ForceAtom) 
												.AddResponseItem ("\\Deleted", ResponseItemMode.ForceAtom) 
												.AddResponseItem ("\\Seen", ResponseItemMode.ForceAtom) 
												.AddResponseItem ("\\Draft", ResponseItemMode.ForceAtom); 
					state.ResponseManager.AddResponse (r);
					r = new Response (ResponseState.OK, Name, command.Tag);
					r.Val= new ResponseItemText((examine) ? "READ-ONLY" : "READ-WRITE");
					state.ResponseManager.AddResponse (r);
					return true;
				} catch (Exception e) {
					state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
					state.Log (e.StackTrace);
					return false;
				}
			}
			state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem ("not succeeded", ResponseItemMode.ForceAtom));
			return false;
		}


	}
}
