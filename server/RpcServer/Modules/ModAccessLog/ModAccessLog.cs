//
// openmapi.org - NMapi C# Mapi API - ModAccessLog.cs
//
// Copyright 2010 Topalis AG
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
using System.IO;

using NMapi;

namespace NMapi.Server {

	/// <summary>
	///  
	/// </summary>
	public class ModAccessLog : IServerModule
	{		
		public string Name {
			get { return "NMapi Access Logging Module"; }
		}

		public string ShortName {
			get { return "ModAccessLog"; }
		}

		public Version Version {
			get { return new Version (0, 1); }

		}
		
		public ModAccessLog (LifeCycle lifeCycle)
		{
		}
	}

}
