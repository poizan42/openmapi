//
// openmapi.org - OpenMapi Proxy Server - LifeCycle.cs
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
using System.Text;
using System.Net;

namespace NMapi.Server {

	/// <summary></summary>
	public sealed class LifeCycle
	{
		
		public event EventHandler<EventArgs> Start;
		
		public event EventHandler<EventArgs> Exit;


		internal void OnStart ()
		{
			if (Start != null)
				Start (this, null); // TODO: send eventargs.
		}
		
		internal void OnExit ()
		{
			if (Exit != null)
				Exit (this, null); // TODO: send eventargs.
		}

		
	}
}

