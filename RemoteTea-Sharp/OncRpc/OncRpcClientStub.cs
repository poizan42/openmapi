//
// openmapi.org - CompactTeaSharp - OnRpcClientStub.cs
//
// C# port Copyright 2008 by Topalis AG
//
// Author (C# port): mazurin, Johannes Roith
//
// This library is based on the RemoteTea java library:
//
//   Author: Harald Albrecht
//
//   Copyright (c) 1999, 2000
//   Lehrstuhl fuer Prozessleittechnik (PLT), RWTH Aachen
//   D-52064 Aachen, Germany. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify
// it under the terms of the GNU Library General Public License as
// published by the Free Software Foundation; either version 2 of the
// License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Library General Public License for more details.
//
// You should have received a copy of the GNU Library General Public
// License along with this program (see the file COPYING.LIB for more
// details); if not, write to the Free Software Foundation, Inc.,
// 675 Mass Ave, Cambridge, MA 02139, USA.
//

using System;
using System.IO;
using System.Net;

namespace CompactTeaSharp
{
	/// <summary>
	///  The abstract <code>OncRpcClientStub</code> class is the base class to
	///  build ONC/RPC-program specific clients upon. This class is typically
	///  only used by jrpcgen generated clients, which provide a particular
	///  set of remote procedures as defined in a x-file.
	/// 
	///  <p>When you do not need the client proxy object any longer, you should
	///  return the resources it occupies to the system. Use the {@link #close}
	///  method for this.
	/// 
	///  <pre>
	///  client.Close ();
	///  client = null; // Hint to the garbage collector
	/// </pre>
	/// </summary>
	// see OncRpcTcpClient
	public abstract class OncRpcClientStub
	{
		protected OncRpcClient client;
		
		/// <summary>
		///  Returns ONC/RPC client proxy object used for communication with a remote ONC/RPC server.
		/// </summary>
		public OncRpcClient Client {
			get { return client; }
		}

		/// <summary>
		///  Construct a new <code>OncRpcClientStub</code> for communication with
		///  a remote ONC/RPC server.
		/// </summary>
		/// <param name="host">Host address where the desired ONC/RPC server resides.</param>
		/// <param name="program">Program number of the desired ONC/RPC server.</param>
		/// <param name="version">Version number of the desired ONC/RPC server.</param>
		/// <param name="protocol">OncRpcProtocols Protocol to be used for
		///   ONC/RPC calls. This information is necessary, so port lookups through
		///   the portmapper can be done.</param>
		protected OncRpcClientStub (IPAddress host, int program, 
			int version, int port, OncRpcProtocols protocol)
		{
			client = OncRpcClient.NewOncRpcClient (host, program, version, port, protocol);
		}

		/// <summary>
		///  Construct a new <code>OncRpcClientStub</code> which uses the given
		///  client proxy object for communication with a remote ONC/RPC server.
		/// </summary>
		/// <param name="client">ONC/RPC client proxy object implementing a particular IP protocol.</param>
		protected OncRpcClientStub (OncRpcClient client)
		{
			this.client = client;
		}

		/// <summary>
		///  Close the connection the server and free network-related resources. 
		/// </summary>
		public void Close ()
		{
			if (client == null)
				return;
			try {
				client.Close ();
			} finally {
				client = null;
			}
		}
	}

}

