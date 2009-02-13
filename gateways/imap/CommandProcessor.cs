// openmapi.org - NMapi C# IMAP Gateway - CommandProcessor.cs
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
using System.Diagnostics;

using NMapi.Utility;

namespace NMapi.Gateways.IMAP {

	public class CommandProcessor
	{
		IMAPConnectionState imapConnectionState;

		protected ServerConnection ServCon {
			get { return imapConnectionState.ServerConnection; }
		}

		public CommandProcessor(IMAPConnectionState imapConn) {
			imapConnectionState = imapConn;
		}
	
		public void ProcessCommand (Command command)
		{
			// check for parse errors and stop processing:
			if (command.Parse_error != null) {
				if (command.Command_name != null && command.Tag != null) {
					imapConnectionState.ResponseManager.AddResponse (
						new Response (ResponseState.BAD, command.Command_name, command.Tag)
							.AddResponseItem (command.Parse_error, ResponseItemMode.ForceAtom));
				} else {
					imapConnectionState.ResponseManager.AddResponse (
						new Response (ResponseState.BAD, "XXX", command.Tag)
							.AddResponseItem ("Unrecognized Command", ResponseItemMode.ForceAtom));
				}					
				return;
			}
			
			// special handling for Mailbox INBOX
			if (ServCon != null) {
				command.Mailbox1 = ConversionHelper.MailboxIMAPToUnicode (command.Mailbox1);
				command.Mailbox2 = ConversionHelper.MailboxIMAPToUnicode (command.Mailbox2);
				command.List_mailbox = ConversionHelper.MailboxIMAPToUnicode (command.List_mailbox);
				
				// prohibit activities in the namespace of the MAPI receive folder.
				// it is being mapped to the name INBOX below
				if (command.Command_name.ToUpper () != "UNSUBSCRIBE" &&
				    (imapConnectionState.FolderMappingAgent.MapiFolderIsMapped (command.Mailbox1) ||
					 imapConnectionState.FolderMappingAgent.MapiFolderIsMapped (command.Mailbox2) ||
				     imapConnectionState.FolderMappingAgent.MapiFolderIsMapped (command.List_mailbox))) {
				    imapConnectionState.ResponseManager.AddResponse (
						new Response (ResponseState.NO, command.Command_name, command.Tag).AddResponseItem ("actions on this mailbox are disallowed", ResponseItemMode.ForceAtom));
					return;
				}
				// map the INBOX to the MAPI receive folder
				command.Mailbox1 = imapConnectionState.FolderMappingAgent.MapIMAPToMAPI (command.Mailbox1);
				command.Mailbox2 = imapConnectionState.FolderMappingAgent.MapIMAPToMAPI (command.Mailbox2);
				command.List_mailbox = imapConnectionState.FolderMappingAgent.MapIMAPToMAPI (command.List_mailbox);
			}

ObjectDumper.Write(command,3);			

			
			AbstractBaseCommandProcessor cmd = null;
			switch (command.Command_name) {
			case "APPEND": cmd = new CmdAppend (imapConnectionState); break;
			case "CAPABILITY": cmd = new CmdCapability (imapConnectionState); break;
			case "CHECK": cmd = new CmdCheck (imapConnectionState); break;
			case "CLOSE": cmd = new CmdClose (imapConnectionState); break;
			case "COPY": cmd = new CmdCopy (imapConnectionState); break;
			case "CREATE": cmd = new CmdCreate (imapConnectionState); break;
			case "DELETE": cmd = new CmdDelete (imapConnectionState); break;
			case "EXAMINE": cmd = new CmdExamine (imapConnectionState); break;
			case "EXPUNGE": cmd = new CmdExpunge (imapConnectionState); break;
			case "FETCH": cmd = new CmdFetch (imapConnectionState); break;
			case "LOGIN": cmd = new CmdLogin (imapConnectionState); break;
			case "LOGOUT": cmd = new CmdLogout (imapConnectionState); break;
			case "LIST": cmd = new CmdList (imapConnectionState); break;
			case "LSUB": cmd = new CmdLsub (imapConnectionState); break;
			case "NOOP": cmd = new CmdNoop (imapConnectionState); break;
			case "RENAME": cmd = new CmdRename (imapConnectionState); break;
			case "SELECT": cmd = new CmdSelect (imapConnectionState); break;
			case "STORE": cmd = new CmdStore (imapConnectionState); break;
			case "STATUS": cmd = new CmdStatus (imapConnectionState); break;
			case "SUBSCRIBE": cmd = new CmdSubscribe (imapConnectionState); break;
			case "UNSUBSCRIBE": cmd = new CmdUnsubscribe (imapConnectionState); break;
			default:
				Response r = new Response (ResponseState.BAD, command.Command_name, command.Tag);
				r.AddResponseItem ("command is not supported", ResponseItemMode.ForceAtom);
				imapConnectionState.ResponseManager.AddResponse (r);
				break;
			}
			if (cmd != null)
				cmd.Run (command);
		}

	}

}
