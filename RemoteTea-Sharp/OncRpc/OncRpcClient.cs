//
// openmapi.org - CompactTeaSharp - OnRpcClient.cs
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
using System.Net;

namespace CompactTeaSharp
{
	
	/// <summary>
	///  Foundation for protcol-specific ONC/RPC clients. It encapsulates 
	///  protocol-independent functionality. This class provides the 
	///  method skeleton, for instance for executing procedure calls.
	/// </summary>
	public abstract class OncRpcClient
	{
		protected IPAddress host; // Internet address of the host where the ONC/RPC server we want to communicate with is located at.

		/// <summary>
		///  ONC/RPC calls through the {@link #call(int, IXdrAble, IXdrAble)} method
		///  will throw an exception if no answer from the ONC/RPC server is
		///  received within the timeout time span.
		/// </summary>
		protected int timeout = 30000; // Timeout (in milliseconds) for communication with an ONC/RPC server.

		protected int program, version, port, xid; // The message id (also called "transaction id") used for the next call message.
		protected OncRpcClientAuth auth; // Authentication protocol object to be used when issuing ONC/RPC calls.


		/// <summary>
		///  Set the timout for remote procedure calls to wait for an answer from
		///  the ONC/RPC server. If the timeout expires,
		///  {@link #call(int, IXdrAble, IXdrAble)} will raise a
		///  {@link java.io.InterruptedIOException}. The default timeout value is
		///  30 seconds (30,000 milliseconds). The timeout must be > 0.
		///  A timeout of zero indicated batched calls, for which no reply message is expected.
		/// </summary>
		///  <param name="milliseconds">Timeout in milliseconds. A timeout of zero indicates
		///   batched calls.</param>
		public virtual void SetTimeout (int milliseconds) 
		{
			if (milliseconds < 0)
				throw new Exception ("timeouts can not be negative.");
			timeout = milliseconds;
		}

		/// <summary>
		///  Retrieve the current timeout set for remote procedure calls. A timeout
		///  of zero indicates batching calls (no reply message is expected).
		/// </summary>
		public int Timeout {
			get { return timeout; }
		}

		/// <summary>
		///  Returns the program number of the ONC/RPC server specified when creating this client.
		/// </summary>
		public int Program {
			get { return program; }
		}

		/// <summary>
		///  Returns the version number of the ONC/RPC server  specified when creating this client.
		/// </summary>
		public int Version {
			get { return version; }
		}

		/// <summary>
		///  Returns the IP address of the server's host this client is connected to.
		/// </summary>
		public IPAddress Host {
			get { return host; }
		}

		/// <summary>
		///  Returns port number of the server this client is connected to.
		/// </summary>
		public int Port {
			get { return port; }
		}

		/// <summary>
		///  Sets the authentication to be used when making ONC/RPC calls.
		/// </summary>
		public void setAuth (OncRpcClientAuth auth)
		{
			this.auth = auth;
		}

		/// <summary>
		///  Returns the current authentication.
		/// </summary>
		public OncRpcClientAuth getAuth ()
		{
			return auth;
		}

		/// <summary>
		///   The character encoding for (de-)serializing strings.
		/// </summary>
		public abstract string CharacterEncoding { get; set; }

		/// <summary>
		///  Create next message identifier. Message identifiers are used to match
		///  corresponding ONC/RPC call and reply messages.
		/// </summary>
		protected void NextXid ()
		{
			xid++;
		}


		/// <summary>
		///  Constructs an <code>OncRpcClient</code> object (the generic part).
		/// </summary>
		/// <param name="host">Host address where the desired ONC/RPC server resides.</param>
		/// <param name="program">Program number of the desired ONC/RPC server.</param>
		/// <param name="version">Version number of the desired ONC/RPC server.</param>
		/// <param name="port"></param>
		/// <param name="protocol">OncRpcProtocols Protocol to be used for calls.</param>
		protected OncRpcClient (IPAddress host, int program, int version, int port, 
		OncRpcProtocols protocol)
		{
			this.host =  host;
			this.program = program;
			this.version = version;

			// Initialize the message identifier with some random value.
			long seed = System.DateTime.Now.ToBinary ();
			xid = ((int) seed) ^ ((int) (seed >> 32));
			this.port = port;
		}

		/// <summary>
		///  Creates a new client object, which can handle the requested protocol.
		/// </summary>
		/// <param name="host">Host address where the desired ONC/RPC server resides.
		/// <param name="program">Program number of the desired ONC/RPC server.
		/// <param name="version">Version number of the desired ONC/RPC server.
		/// <param name="protocol">ncRpcProtocols Protocol to be used for ONC/RPC calls.</params>
		// throws OncRpcException, IOException
		public static OncRpcClient NewOncRpcClient (IPAddress host, 
			int program, int version, OncRpcProtocols protocol)
		{
			return NewOncRpcClient (host, program, version, 0, protocol);
		}

		/// <summary>
		///  Creates a new cient object, which can handle the requested protocol.
		/// </summary>
		/// <param name="host"> Host address where the desired ONC/RPC server resides.</param>
		/// <param name="program"> Program number of the desired ONC/RPC server.</param>
		/// <param name="version"> Version number of the desired ONC/RPC server.</param>
		/// <param name="port"> Port number of the ONC/RPC server. Specifiy 0
		///  if this is not known and the portmap process located at host should
		///  be contacted to find out the port.</param>
		/// <param name="protocol">OncRpcProtocols Protocol to be used for ONC/RPC calls.</param>
		// throws OncRpcException, IOException
		public static OncRpcClient NewOncRpcClient (IPAddress host,
			int program, int version, int port, OncRpcProtocols protocol)
		{
			if (protocol == OncRpcProtocols.Tcp)
				return new OncRpcTcpClient (host, program, version, port, false);
			if (protocol == OncRpcProtocols.SslTcp)
				return new OncRpcTcpClient (host, program, version, port, true);
			throw new OncRpcException (OncRpcException.UNKNOWN_PROTO);
		}

		/// <summary>
		///  Close the connection to an ONC/RPC server and free all 
		///  network-related resources
		/// </summary>
		// throws OncRpcException
		public virtual void Close ()
		{
		}

		/// <summary>
		///  Calls a remote procedure on an ONC/RPC server.
		///  The <code>OncRpcUdpClient</code> uses a similar timeout scheme as
		///  the genuine Sun C implementation of ONC/RPC: it starts with a timeout
		///  of one second when waiting for a reply. If no reply is received within
		///  this time frame, the client doubles the timeout, sends a new request
		///  and then waits again for a reply. In every case the client will wait
		///  no longer than the total timeout set through the
		///  link #setTimeout(int)} method.
		/// </summary>
		///  <param name="procedureNumber">Procedure number of the procedure to call.</param>
		///  <<param name="prms">The parameters of the procedure to call, contained
		///  in an object which implements the {@link IXdrAble} interface.</param>
		///  <param name="result">The object receiving the result of the procedure call.</param>
		///  
		// throws OncRpcException
		// TODO synchronized 
		public virtual void Call (int procedureNumber, IXdrAble prms, IXdrAble result)
		{
			// Use the default version number as specified for this client.
			Call (procedureNumber, version, prms, result);
		}

		/// <summary>
		///  Calls a remote procedure on an ONC/RPC server.
		/// </summary>
		///  <param name="procedureNumber">Procedure number of the procedure to call.
		///  <param name="versionNumber">Protocol version number.</param>
		///  <param name="parameters">The parameters of the procedure to call, contained
		///    in an object which implements the IXdrAble interface.</param>
		///  <param name="result">The object receiving the result of the procedure call.</param>
		// throws OncRpcException
		public abstract void Call (int procedureNumber, int versionNumber, 
									IXdrAble parameters, IXdrAble result);

	}
}
