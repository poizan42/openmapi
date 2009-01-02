//
// openmapi.org - OpenMapi Proxy Server - CallAttribute.cs
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

namespace NMapi {

	public abstract class CallAttribute : Attribute
	{
		protected RemoteCall call;
		protected bool any;

		public bool Any {
			get { return any; }
		}

		public RemoteCall RemoteCall {
			get { return call; }
		}

	}

}
