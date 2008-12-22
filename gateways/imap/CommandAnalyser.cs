// openmapi.org - NMapi C# IMAP Gateway - CommandAnalyser.cs
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
using System.Collections;

namespace NMapi.Gateways.IMAP
{
	
	public delegate bool DelegateState ();
	
	public class CommandAnalyser
	{
		
		private CommandAnalyserParserInterface parseInt;
		private Queue commandQueue = new Queue ();

		private DelegateState stateNotAuthenticated;
		private DelegateState stateAuthenticated;
		private DelegateState stateSelected;
		private DelegateState stateLogout;

		public DelegateState StateNotAuthenticated {
			get { return stateNotAuthenticated; }
			set { stateNotAuthenticated = value; }
		}
		public DelegateState StateAuthenticated {
			get { return stateAuthenticated; }
			set { stateAuthenticated = value; }
		}
		public DelegateState StateSelected {
			get { return stateSelected; }
			set { stateSelected = value; }
		}
		public DelegateState StateLogout {
			get { return stateLogout; }
			set { stateLogout = value; }
		}

		public CommandAnalyser(ClientConnection client)
		{
			parseInt = new CommandAnalyserParserInterface (this, client);
		}

		public void CheckCommand()
		{
			Command cmd = parseInt.CheckCommand ();
			if (cmd != null)
			{
				commandQueue.Enqueue(cmd);
			}
		}
		
		public Queue CommandQueue { get { return commandQueue; } }
			
		
	}
}
