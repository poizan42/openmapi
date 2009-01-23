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
				ulong flags = snli.MessageFlags;
				Trace.WriteLine ("flags: " + flags);
				flags = ProcessBit (flags, 0x00000001 /*MSGFLAG_READ*/, command.Flag_sign, "\\seen", command);
				flags = ProcessBit (flags, 0x00000008 /*MESSAGE_FLAG_UNSENT*/, command.Flag_sign, "\\draft", command);

				Trace.WriteLine ("flags: " + flags);

				ulong status =  snli.MsgStatus;
				Trace.WriteLine ("status: " + status);
				status = ProcessBit (status, NMAPI.MSGSTATUS_DELMARKED, command.Flag_sign, "\\deleted", command);
				status = ProcessBit (status, 0x00000200 /*MSGSTATUS_ANSWERED*/, command.Flag_sign, "\\answered", command);
				status = ProcessBit (status, 0x00000002/*MSGSTATUS_TAGGED*/, command.Flag_sign, "\\flagged", command);
				Trace.WriteLine ("status: " + status);

				// update the sequence number list

				snli.MessageFlags = flags;
				snli.MsgStatus = status;
				
				// store the changes
				try {
					Trace.WriteLine ("Store uid: " + snli.UID);
					IMessage msg = (IMessage) ServCon.Store.OpenEntry (snli.EntryId.ByteArray, null, Mapi.Modify);

					// special handling read-Flag
					if ((flags & 0x00000001 /*MSGFLAG_READ*/) == 0)
						msg.SetReadFlag (NMAPI.CLEAR_READ_FLAG);
					else
						msg.SetReadFlag (0);
					// rest of flags in regular Propertyhandling
					IntProperty flagsProp = new IntProperty ();
					flagsProp.PropTag = Property.MessageFlags;
					flagsProp.Value = (int) flags;
//					msg.HrSetOneProp (flagsProp);    // can't be done, Store answeres MAPE_E_COMPUTED
					msg.SaveChanges (NMAPI.FORCE_SAVE);
					                 
					//status changes
					// TODO: setMessageStatus does not work for TeamXchange currently.
					// TODO: determine a solution somewhen
					
					//SPropValue eid = ServCon.CurrentFolder.HrGetOneProp (Property.EntryId);
					//IMapiFolder fldr = (IMapiFolder) ServCon.Store.OpenEntry (eid.Value.Binary.ByteArray, null, Mapi.Modify).Unk;
					//fldr.SetMessageStatus (
					//	snli.EntryId.Value.Binary.ByteArray, 
					//	(int) status,
					//	NMAPI.MSGSTATUS_DELMARKED | 0x00000200 /*MSGSTATUS_ANSWERED*/ | 0x00000002/*MSGSTATUS_TAGGED*/);
					//fldr.SaveChanges (NMAPI.FORCE_SAVE);
				} catch (Exception e) { 
					state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
					return;
				}
				
			}

			state.AddExistsRequestDummy ();
			
			Response r = new Response (ResponseState.OK, Name, command.Tag);
			r.UIDResponse = command.UIDCommand;
			state.ResponseManager.AddResponse (r);
		}

		private ulong ProcessBit (ulong flags, ulong mask, string sign, string key, Command command)
		{
			if (sign == "+" && command.Flag_list.Contains (key))
				flags = flags | mask;
			else if (sign == "-" && command.Flag_list.Contains (key))
				flags = flags & ~mask;
			else if (sign == null && command.Flag_list.Contains (key))
				flags = flags | mask;
			return flags;
		}


		
	}
}
