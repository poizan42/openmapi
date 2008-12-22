// openmapi.org - NMapi C# IMAP Gateway - CmdExamine.cs
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

	public sealed class CmdExamine : CmdSelect
	{
		public override string Name {
			get {
				return "EXAMINE";
			}
		}

		public CmdExamine (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			if (state.CurrentState == IMAPConnectionStates.AUTHENTICATED ||
			    state.CurrentState == IMAPConnectionStates.SELECTED) {
				state.CurrentState = IMAPConnectionStates.AUTHENTICATED;
				bool res = DoRun(command);
				if (res)
					state.CurrentState = IMAPConnectionStates.SELECTED;
			}
		}


	}
}
