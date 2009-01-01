//
// openmapi.org - CompactTeaSharp - OncRpcServerTransport.cs
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
using System.Net;
using CompactTeaSharp;

namespace CompactTeaSharp.Server
{
	
	/// <summary>
	///  Instances of class <code>OncRpcServerTransport</code> encapsulate XDR
	///  streams of ONC/RPC servers. Using server transports, ONC/RPC calls are
	///  received and the corresponding replies are later sent back after
	///  handling.
	/// 
	///  Note that the server-specific dispatcher handling requests
	///  (done through {@link IOncRpcDispatchable} will only
	///  directly deal with {@link OncRpcCallInformation} objects. These
	///  call information objects reference OncRpcServerTransport object, but
	///  the server programmer typically will never touch them, as the call
	///  information object already contains all necessary information about
	///  a call, so replies can be sent back (and this is definetely a sentence
	///  containing too many words).
	/// </summary>
	/// see OncRpcCallInformation
	/// see IOncRpcDispatchable
	public abstract class OncRpcServerTransport
	{
		protected IOncRpcDispatchable dispatcher;
		protected int port;

		/// <summary>
		///  Returns port number of socket this server transport listens on for
		///  incoming ONC/RPC calls.
		/// </summary>
		public int Port {
			get { return port; }
		}

		/// <summary>
		///  Get/Set the character encoding for (de-)serializing strings.
		/// </summary>
		/// <param name="characterEncoding">The encoding to use for (de-)serializing strings.
		///   If null, the system's default encoding is to be used.</param>
		public abstract string CharacterEncoding {
			get; set;
		}
		
		/// <summary>
		///   Create a new instance of a <code>OncRpcServerTransport</code> which
		///   encapsulates XDR streams of an ONC/RPC server. Using a server transport,
		///   ONC/RPC calls are received and the corresponding replies are sent back.
		///  
		///   We do not create any XDR streams here, as it is the responsibility
		///   of derived classes to create appropriate XDR stream objects for the
		///   respective kind of transport mechanism used (like TCP/IP and UDP/IP).
		/// </summary>
		/// <param name="dispatcher">Reference to interface of an object capable of
		///     dispatching (handling) ONC/RPC calls.</param>
		/// <param name="port">Number of port where the server will wait for incoming calls.</param>
		/// <param name="info">Array of program and version number tuples of the ONC/RPC
		///     programs and versions handled by this transport.</param>
		protected OncRpcServerTransport (IOncRpcDispatchable dispatcher, int port)
		{
			this.dispatcher = dispatcher;
			this.port = port;
		}

		/// <summary>
		///  Close the server transport and free any resources associated with it.
		///  
		///  <p>Note that the server transport is <b>not deregistered</b>. You'll
		///  have to do it manually if you need to do so. The reason for this
		///  behaviour is, that the portmapper removes all entries regardless of
		///  the protocol (TCP/IP or UDP/IP) for a given ONC/RPC program number
		///  and version.
		///  
		///  <p>Derived classes can choose between different behaviour for
		///  shuting down the associated transport handler threads:
		///  <ul>
		///  <li>Close the transport immediately and let the threads stumble on the
		///    closed network connection.
		///  <li>Wait for handler threads to complete their current ONC/RPC request
		///    (with timeout), then close connections and kill the threads.
		///  </ul>
		/// </summary>
		public abstract void Close ();

		/// <summary>
		///  Creates a new thread and uses this thread to listen to incoming
		///  ONC/RPC requests, then dispatches them and finally sends back the
		///  appropriate reply messages.
		///  
		///  Note that you have to supply an implementation for this abstract
		///  method in derived classes. Your implementation needs to create a new
		///  thread to wait for incoming requests. The method has to return
		///  immediately for the calling thread.
		/// </summary>
		public abstract void Listen ();

	  	/// <summary>
	  	///  Send back an ONC/RPC reply to the original caller. This is rather a
	  	///  low-level method, typically not used by applications. Dispatcher handling
	  	///  ONC/RPC calls have to use the OncRpcCallInformation#reply(IXdrAble) 
		///  method instead on the call object supplied to the handler.
	  	///
	  	///  An appropriate implementation has to be provided in derived classes
	  	///  as it is dependent on the type of transport used.
	  	/// </summary>
	  	/// <param name="callInfo">information about the original call, which are necessary
	  	///   to send back the reply to the appropriate caller.</param>
	  	/// <param name="state">ONC/RPC reply message header indicating success or failure
	  	///   and containing associated state information.</param>
	  	/// <param name="reply">If not <code>null</code>, then this parameter references
	  	///   the reply to be serialized after the reply message header.</param>
	  	/// @see OncRpcCallInformation
	  	/// @see IOncRpcDispatchable
	  	////
		/// OncRpcException, IOException
		public abstract void Reply (OncRpcCallInformation callInfo,
			  OncRpcServerReplyMessage state, IXdrAble reply);

		public abstract void RetrieveCall (IXdrAble call);

		public abstract XdrDecodingStream GetXdrDecodingStream ();
		
		public abstract XdrEncodingStream GetXdrEncodingStream ();

		public abstract void EndDecoding ();
		
		public abstract void BeginEncoding (OncRpcCallInformation callInfo,
			OncRpcServerReplyMessage state);
		
		public abstract void EndEncoding ();
			
	}

}

