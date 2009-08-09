// openmapi.org - NMapi C# IMAP Gateway - AbstractBaseCommandProcessor.cs
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

namespace NMapi.Gateways.IMAP {

	public abstract class AbstractBaseCommandProcessor
	{
		internal IMAPConnectionState state;
		protected ServerConnection ServCon { 
			get { return state.ServerConnection; } 
		}

		public abstract string Name { get; }
		public abstract void Run (Command command);

		public AbstractBaseCommandProcessor (IMAPConnectionState state)
		{
			this.state = state;
		}

		public void Log (string text)
		{
			state.Log (text, null);
		}
		
		public void Log (string text, string tag)
		{
			state.Log (text, tag);
		}
		
	}

}
