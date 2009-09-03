//
// openmapi.org - CompactTeaSharp - OncRpcTcpServerTransport.cs
//
// C# port Copyright 2008 by Topalis AG
//
// Author (C# port): Johannes Roith
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
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using CompactTeaSharp;

namespace CompactTeaSharp.Server
{
	
	/// <summary>
	///  
	/// <summary>
	public class ConnectionClosedEventArgs : EventArgs
	{
		public IPAddress IPAddress { get; set; }
		public int Port { get; set; }

		public ConnectionClosedEventArgs (IPAddress ip, int port)
		{
			this.IPAddress = ip;
			this.Port = port;
		}

	}
	
	/// <summary>
	///  Instances of class <code>OncRpcTcpServerTransport</code> encapsulate
	///  TCP/IP-based XDR streams of ONC/RPC servers. This server transport class
	///  is responsible for accepting new ONC/RPC connections over TCP/IP.
	/// </summary>
	/// see OncRpcServerTransport
	/// see OncRpcTcpConnectionServerTransport
	public sealed class OncRpcTcpServerTransport : OncRpcServerTransport
	{
		private bool useSsl;
		private SslStore sslParams;
		
		private TcpListener socket; // TCP socket used for stream-based communication with ONC/RPC clients.
		private int bufferSize;  // Size of send/receive buffers to use when encoding/decoding XDR data.
		private LinkedList<OncRpcTcpConnectionServerTransport> openTransports;
		private int transmissionTimeout = 30000;
		private string characterEncoding = null;
		
		/// <summary>
		///  The current timeout used during transmission phases (call and 
		///  reply phases) in milliseconds.
		/// </summary>
		public int TransmissionTimeout {
			get { return transmissionTimeout; }
			set {
				if (value <= 0)
					throw new ArgumentException ("transmission timeout must be > 0");
				transmissionTimeout = value;
			}
		}

		/// <summary>
		///  The character encoding currently used for (de-)serializing strings.
		///  If <code>null</code>, then the system's default encoding is used.
		/// </summary>
		public override string CharacterEncoding {
			get { return characterEncoding; }
			set { this.characterEncoding = value; }
		}

		/// <summary>
		///  Called when the tcp connection is closed.
		/// </summary>
		public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;
		
		/// <summary>
		///   Create a new instance of a <code>OncRpcTcpServerTransport</code> which
		///   encapsulates TCP/IP-based XDR streams of an ONC/RPC server. This
		///   particular server transport only waits for incoming connection requests
		///   and then creates {@link OncRpcTcpConnectionServerTransport} server transports
		///   to handle individual connections.
		///   This constructor is a convenience constructor for those transports
		///   handling only a single ONC/RPC program and version number.
		/// </summary>
		/// <param name="dispatcher">Reference to interface of an object capable of
		///   dispatching (handling) ONC/RPC calls.</param>
		/// <param name="port">Number of port where the server will wait for incoming
		///   calls.</param>
		/// <param name="program">Number of ONC/RPC program handled by this server
		///   transport.</param>
		/// <param name="version">Version number of ONC/RPC program handled.</param>
		/// <param name="bufferSize">Size of buffer used when receiving and sending
		///   chunks of XDR fragments over TCP/IP. The fragments built up to
		///   form ONC/RPC call and reply messages.</param>
		// throws OncRpcException, IOException 
		public OncRpcTcpServerTransport (IOncRpcDispatchable dispatcher, int port, 
			int program, int version, int bufferSize) : this (dispatcher, port, bufferSize)
		{
		}

		/// <summary>
		///  Create a new instance of a <code>OncRpcTcpServerTransport</code> which
		///  encapsulates TCP/IP-based XDR streams of an ONC/RPC server. This
		///  particular server transport only waits for incoming connection requests
		///  and then creates {@link OncRpcTcpConnectionServerTransport} server transports
		///  to handle individual connections.
		/// </summary>
		///  <param name="dispatcher">Reference to interface of an object capable of
		///    dispatching (handling) ONC/RPC calls.</param>
		///  <param name="port">Port where the server will wait for incoming calls.</param>
		///  <param name="info">Array of program and version number tuples of the ONC/RPC
		///    programs and versions handled by this transport.</param>
		///  <param name="bufferSize">Size of buffer used when receiving and sending
		///    chunks of XDR fragments over TCP/IP. The fragments built up to
		///    form ONC/RPC call and reply messages.</param>
		// throws OncRpcException, IOException 
		public OncRpcTcpServerTransport (IOncRpcDispatchable dispatcher,
			int port, int bufferSize) : this (dispatcher, null, port, bufferSize)
		{
		}

		/// <summary>
		///  Create a new instance of a <code>OncRpcTcpServerTransport</code> which
		///  encapsulates TCP/IP-based XDR streams of an ONC/RPC server. This
		///  particular server transport only waits for incoming connection requests
		///  and then creates {@link OncRpcTcpConnectionServerTransport} server transports
		///  to handle individual connections.
		/// </summary>
		/// <param name="dispatcher">Reference to interface of an object capable of
		///     dispatching (handling) ONC/RPC calls.</param>
		/// <param name="bindAddr">The local Internet Address the server will bind to.</param>
		/// <param name="port">Number of port where the server will wait for incoming
		///     calls.</param>
		/// <param name="info">Array of program and version number tuples of the ONC/RPC
		///     programs and versions handled by this transport.</param>
		/// <param name="bufferSize">Size of buffer used when receiving and sending
		///     chunks of XDR fragments over TCP/IP. The fragments built up to
		///     form ONC/RPC call and reply messages.</param>
		// throws OncRpcException, IOException
		public OncRpcTcpServerTransport (IOncRpcDispatchable dispatcher,
			IPAddress bindAddr, int port, int bufferSize) : this (dispatcher,
				bindAddr, port, bufferSize, false, null)
		{
		}
		
		public OncRpcTcpServerTransport (IOncRpcDispatchable dispatcher,
			IPAddress bindAddr, int port, int bufferSize, bool useSsl, 
			SslStore sslParams) : base (dispatcher, port)
		{
			this.useSsl = useSsl;
			this.sslParams = sslParams;
			
			openTransports = new LinkedList<OncRpcTcpConnectionServerTransport> ();
				
			//
			// Make sure the buffer is large enough and resize system buffers
			// accordingly, if possible.
			//
			if (bufferSize < 1024)
				bufferSize = 1024;
			this.bufferSize = bufferSize;
			socket = new TcpListener (bindAddr, port);

			socket.Start (); // TODO: remove!

			if (port == 0)
				this.port = ((IPEndPoint) socket.LocalEndpoint).Port;
		}

		/// <summary>
		///  Close the server transport and free any resources associated with it.
		///  
		///  Note that the server transport is <b>not deregistered</b>. You'll
		///  have to do it manually if you need to do so. The reason for this
		///  behaviour is, that the portmapper removes all entries regardless of
		///  the protocol (TCP/IP or UDP/IP) for a given ONC/RPC program number
		///  and version.
		///  
		///  Calling this method on a <code>OncRpcTcpServerTransport</code>
		///  results in the listening TCP network socket immediately being closed.
		///  In addition, all server transports handling the individual TCP/IP
		///  connections will also be closed. The handler threads will therefore
		///  either terminate directly or when they try to sent back replies.
		/// </summary>
		public override void Close ()
		{
			if (socket != null) {
				//
				// Since there is a non-zero chance of getting race conditions,
				// we now first set the socket instance member to null, before
				// we close the corresponding socket. This avoids null-pointer
				// exceptions in the method which waits for connections: it is
				// possible that that method is awakened because the socket has
				// been closed before we could set the socket instance member to
				// null. Many thanks to Michael Smith for tracking down this one.
				//
				TcpListener deadSocket = socket;
				socket = null;
				try {
					deadSocket.Stop ();
				} catch (IOException) {
					// Do nothing
				}
			}
			//
			// Now close all per-connection transports currently open...
			//
			lock (openTransports) {
				while (openTransports.Count > 0) {
					OncRpcTcpConnectionServerTransport transport = openTransports.First.Value;
					transport.Close ();
					openTransports.RemoveFirst ();
				}
			}
		}

		/// <summary>
		///  Removes a TCP/IP server transport from the list of currently open
		///  transports.
		/// </summary>
		/// <param name="transport">Server transport to remove from the list of currently
		///   open transports for this listening transport.</param>
		public void RemoveTransport (OncRpcTcpConnectionServerTransport transport)
		{
			lock (openTransports) {
				openTransports.Remove (transport);
			}
		}

		private NotSupportedException GetException ()
		{
			throw new NotSupportedException ("Not supported by TCP Transport");
		}

		public override void RetrieveCall (IXdrAble call)
		{
			throw GetException ();
		}
		
		public override XdrDecodingStream GetXdrDecodingStream ()
		{
			throw GetException ();
		}
		
		public override XdrEncodingStream GetXdrEncodingStream ()
		{
			throw GetException ();
		}

		public override void Reply (OncRpcCallInformation callInfo,
			  OncRpcServerReplyMessage state, IXdrAble reply)
		{
			throw GetException ();
		}
		
		public override void BeginEncoding (OncRpcCallInformation callInfo,
			OncRpcServerReplyMessage state)
		{
			throw GetException ();
		}
		
		public override void EndEncoding ()
		{
			throw GetException ();
		}
		
		public override void EndDecoding () {
			throw GetException ();
		}

		private void OnClosed (ConnectionClosedEventArgs e)
		{
			if (ConnectionClosed != null)
				ConnectionClosed (this, e);
		}
		
		public void SendClosed (IPAddress ip, int port)
		{
			OnClosed (new ConnectionClosedEventArgs (ip, port));
		}

		/// <summary>
		///  Creates a new thread and uses this thread to listen to incoming
		///  ONC/RPC requests, dispatches them and sends back the appropriate 
		///  reply messages. For every incomming TCP/IP connection a handler 
		///  thread is created to handle RPC calls on this particular connection.
		/// </summary>
		public override void Listen ()
		{
			Thread listenThread = new Thread (ListenerThread);
			listenThread.Name = "TCP server transport listener thread";
			listenThread.IsBackground = true;
			listenThread.Start ();
		}
		
		private void ListenerThread ()
		{
			while (true) {
				try {
					//
					// Now wait for (new) connection requests to come in.
					//
					TcpListener myServerSocket = socket;
					if (myServerSocket == null)
						return;

					TcpClient newSocket = myServerSocket.AcceptTcpClient ();
						
					Stream stream = newSocket.GetStream ();
					if (useSsl)
						stream = OncNetworkUtility.GetSslServerStream (stream, sslParams);
					
					var transport = new OncRpcTcpConnectionServerTransport (
						dispatcher, newSocket, bufferSize, this, transmissionTimeout, stream);
					lock (openTransports) {
						openTransports.AddFirst (transport);
					}
					//
					// Let the newly created transport object handle this
					// connection. Note that it will create its own
					// thread for handling.
					//
					transport.Listen ();

				} catch (OncRpcException) {
					// Do nothing
				} catch (IOException e) {
					//
					// We ignore most of the IOExceptions as that might be thrown, 
					// for instance, if a client attempts a connection and resets 
					// it before it is pulled off by accept ().

#if DEBUG
					Console.WriteLine (e);
#endif

					// If the socket has been gone away after an IOException 
					// this means that the transport has been closed, so we end this thread
					// gracefully.
					if (socket == null)
						return;
				}
			}
		}
		
	}

}

