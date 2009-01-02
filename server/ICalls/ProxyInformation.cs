//
// openmapi.org - NMapi C# Mapi API - ProxyInformation.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;

namespace NMapi.Server.ICalls {

	/// <summary>
	///  
	/// </summary>
	public sealed class ProxyInformation : MarshalByRefObject
	{
		private DateTime initTime;
		private DateTime lastLoginAdmin;
		private DateTime lastStartTime;
		private int timesRestarted;

		public DateTime InitTime {
			get { return initTime; }
		}

		public DateTime LastLoginAdmin {
			get { return lastLoginAdmin; }
		}

		public DateTime LastStartTime {
			get { return lastStartTime; }
		}

		public int TimesRestarted {
			get { return timesRestarted; }
		}

		public TimeSpan Uptime {
			get {
				return DateTime.Now - LastStartTime;
			}
		}


		public ProxyInformation ()
		{
			initTime = DateTime.Now;
			UpdateLastLogin ();
			lastStartTime = DateTime.Now;
			timesRestarted = 0;
		}

		public void UpdateLastLogin ()
		{
			lastLoginAdmin = DateTime.Now;
		}

	}
}

