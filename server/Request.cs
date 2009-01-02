//
// openmapi.org - NMapi C# Mapi API - Request.cs
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

using NMapi;

namespace NMapi.Server {

	public sealed class Request
	{
		private ProxySession session;
		private RemoteCall remoteCall;
		private int totalBytes;

		/// <summary>
		///  The ProxySession that represents the context of this request
		/// </summary>
		public ProxySession ProxySession {
			get { return session; }
		}

		/// <summary>
		///  Gets the name of the rpc call
		/// </summary>
		public RemoteCall RemoteCall {
			get { return remoteCall; }
		}

		/// <summary>
		///  Gets the number of bytes in the request.
		/// </summary>
		public int TotalBytes {
			get { return totalBytes; }
		}

		public Request (ProxySession session, RemoteCall call, int totalBytes)
		{
			this.session = session;
			this.remoteCall = call;
			this.totalBytes = totalBytes;
		}

	}
}
