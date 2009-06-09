//
// openmapi.org - NMapi C# Mapi API - ModLoginMapper.cs
//
// Copyright 2009 Topalis AG
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

	/// <summary>
	///  Maps the Logon call the correct backend host as specified in the 
	///  configuration. This module is REQUIRED or the server will try to 
	///  connect to itself (which, obviously will fail in the end).
	/// </summary>
	public class ModLoginMapper : IServerModule
	{		
		public string Name {
			get { return "NMapi LoginMapper Module"; }
		}

		public string ShortName {
			get { return "ModLoginMapper"; }
		}

		public Version Version {
			get { return new Version (0, 1); }

		}

		[PreCall (RemoteCall.IMapiSession_Logon)]
		public void AdaptLogin (ref Request request, ref IMapiSession obj, ref string host, 
			ref int sessionFlags, ref string user, ref string password, ref int codePage)
		{
			Console.WriteLine ("DEBUG -- LOGIN FROM PLUGIN !!! "); // DEBUG
			Console.WriteLine ("CODEPAGE: " + codePage); // DEBUG
			
			request.ProxySession.LoginName = user;
			host = request.ProxySession.Rpc.TargetHost;
			int targetPort = request.ProxySession.Rpc.TargetPort;
			if (targetPort > 0)
				host +=  ":" + targetPort;
		}
	}

}
