//
// openmapi.org - NMapi C# Mapi API - InternalCallsClient.cs
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

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;

namespace NMapi.Server.ICalls {

	/// <summary>
	///  each reference on the client will have it's own instance of this.
	/// </summary>
	public sealed class InternalCallsClient
	{
		public static IInternalCalls GetNewClient ()
		{
			return (IInternalCalls) Activator.GetObject
				(typeof (IInternalCalls),
					"ipc://mapiproxy/icall");
		}

	}
}

