// openmapi.org - NMapi C# IMAP Gateway - CmdClose.cs
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
using NMapi.Properties;

namespace NMapi.Gateways.IMAP {

	public sealed class CmdClose : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "CLOSE";
			}
		}

		public CmdClose (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			try {
				if (state.CurrentState == IMAPConnectionStates.SELECTED) {

					// execute Expunges
					CmdExpunge.DoExpunge (state, ServCon, command);
					
					// send response first, as it requires access to the Mapi Store for exists/expunges ..
					state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
					// ... then disconnect
					if (ServCon.CurrentFolder != null) {
						state.NotificationHandler = null;
						ServCon.CurrentFolder.Dispose ();
					}
					state.CurrentState = IMAPConnectionStates.AUTHENTICATED;
				} else {
					state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag)
															.AddResponseItem ("server is not in a selected state", ResponseItemMode.ForceAtom));
					state.CurrentState = IMAPConnectionStates.AUTHENTICATED;
				}					
			}
			catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				state.Log (e.StackTrace);
				state.CurrentState = IMAPConnectionStates.AUTHENTICATED;
			}
		}

	}
}
