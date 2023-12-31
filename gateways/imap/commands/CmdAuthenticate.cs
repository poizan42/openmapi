// openmapi.org - NMapi C# IMAP Gateway - CmdAuthenticate.cs
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

	public sealed class CmdAuthenticate : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "AUTHENTICATE";
			}
		}

		public CmdAuthenticate (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			try {
				state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
			}
			catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				Log (e.StackTrace);
			}
		}

	}
}
