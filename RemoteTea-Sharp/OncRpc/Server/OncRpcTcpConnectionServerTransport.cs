//
// openmapi.org - CompactTeaSharp - OncRpcTcpConnectionServerTransport.cs
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
using System.Net.Sockets;
using System.Threading;
using CompactTeaSharp;

namespace CompactTeaSharp.Server
{
   	/// <summary>
	///  Instances of class <code>OncRpcTcpServerTransport</code> encapsulate
	///  TCP/IP-based XDR streams of ONC/RPC servers. This server transport class
	///  is responsible for receiving ONC/RPC calls over TCP/IP.
   	/// </summary>
	/// see OncRpcServerTransport
	/// see OncRpcTcpServerTransport
	public class OncRpcTcpConnectionServerTransport : OncRpcServerTransport
	{
	   	/// <summary>
	   	///  TCP client used for stream-based communication with ONC/RPC
	   	///  clients.
	   	/// </summary>
		private TcpClient tcpClient;

	   	/// <summary>
	   	///  XDR encoding stream used for sending replies via TCP/IP back to an
	   	///  ONC/RPC client.
	   	/// </summary>
		private XdrTcpEncodingStream sendingXdr;

	   	/// <summary>
	   	///  XDR decoding stream used when receiving requests via TCP/IP from
	   	///  ONC/RPC clients.
	   	/// </summary>
		private XdrTcpDecodingStream receivingXdr;

	   	/// <summary>
		///  Indicates that <code>BeginDecoding</code> has been called for the
		///  receiving XDR stream, so that it should be closed later using
		///  EndDecoding.
		/// </summary>
		private bool pendingDecoding = false;

	   	/// <summary>
		///  Indicates that <code>BeginEncoding</code> has been called for the
		///  sending XDR stream, so in face of exceptions we can not send an
		///  error reply to the client but only drop the connection.
		/// </summary>
		private bool pendingEncoding = false;

	   	/// <summary>
		///  Reference to the TCP/IP transport which created us to handle a
		///  new ONC/RPC connection.
		/// </summary>
		private OncRpcTcpServerTransport parent;

	   	/// <summary>
		///  Timeout during the phase where data is received within calls, 
		///  or data is sent within replies.
		/// </summary>
		protected int transmissionTimeout;

	   	/// <summary>
		///  Set the character encoding for (de-)serializing strings.
	   	/// <summary>
		///  <param name="characterEncoding">The encoding to use for 
		///    (de-)serializing strings. If null, the system's default encoding 
		///    is to be used.</param>
		public override string CharacterEncoding {
			get { return sendingXdr.CharacterEncoding; }
			set {
				sendingXdr.CharacterEncoding = value;
				receivingXdr.CharacterEncoding = value;
			}
		}
		
	   	/// <summary>
	   	///   Create a new instance of a <code>OncRpcTcpSConnectionerverTransport</code>
	   	///   which encapsulates TCP/IP-based XDR streams of an ONC/RPC server. This
	   	///   particular server transport handles individual ONC/RPC connections over
	   	///   TCP/IP.
	   	/// </summary>
	   	///  <param name="dispatcher">Reference to interface of an object capable of
	   	///    dispatching (handling) ONC/RPC calls.</param>
	   	///  <param name="tcpClient">TCP/IP-based client of new connection.</param>
	   	///  <param name="info">Array of program and version number tuples of the ONC/RPC
	   	///    programs and versions handled by this transport.</param>
	   	///  <param name="bufferSize">Size of buffer used when receiving and sending
	   	///    chunks of XDR fragments over TCP/IP. The fragments built up to
	   	///    form ONC/RPC call and reply messages.</param>
	   	///  <param name="parent">Parent server transport which created us.</param>
	   	///  <param name="transmissionTimeout">Inherited transmission timeout.</param>
		// throws OncRpcException, IOException 
		public OncRpcTcpConnectionServerTransport (IOncRpcDispatchable dispatcher,
			TcpClient tcpClient, int bufferSize, OncRpcTcpServerTransport parent,
			int transmissionTimeout) : base (dispatcher, 0)
		{
			this.parent = parent;
			this.transmissionTimeout = transmissionTimeout;
			//
			// Make sure the buffer is large enough and resize system buffers
			// accordingly, if possible.
			//
			if ( bufferSize < 1024 )
				bufferSize = 1024;
			this.tcpClient = tcpClient;
			this.port = ((IPEndPoint) tcpClient.Client.LocalEndPoint).Port;
			if ( tcpClient.SendBufferSize < bufferSize ) {
				tcpClient.SendBufferSize = bufferSize;
			}
			if ( tcpClient.ReceiveBufferSize < bufferSize ) {
				tcpClient.ReceiveBufferSize = bufferSize;
			}
			//
			// Create the necessary encoding and decoding streams, so we can
			// communicate at all.
			//
			sendingXdr = new XdrTcpEncodingStream (tcpClient, bufferSize);
			receivingXdr = new XdrTcpDecodingStream (tcpClient, bufferSize);
			//
			// Inherit the character encoding setting from the listening
			// transport (parent transport).
			//
			if (parent != null)
				CharacterEncoding = parent.CharacterEncoding;
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
		///  The handler thread will therefore either terminate directly or when
		///  it tries to sent back replies.
	   	/// </summary>
		public override void Close ()
		{
			IPAddress ip = null;
			int port = -1;
			try {
				if (receivingXdr != null && tcpClient != null) {
					ip = receivingXdr.GetSenderAddress ();
					port = receivingXdr.GetSenderPort ();
				}
			} catch (Exception) {
				// Do nothing
			}
			
			if (tcpClient != null) {
				//
				// Since there is a non-zero chance of getting race conditions,
				// we now first set the socket instance member to null, before
				// we close the corresponding socket. This avoids null-pointer
				// exceptions in the method which waits for new requests: it is
				// possible that this method is awakened because the socket has
				// been closed before we could set the socket instance member to
				// null. Many thanks to Michael Smith for tracking down this one.
				//
				TcpClient deadTcpClient = tcpClient;
				tcpClient = null;
				try {
					deadTcpClient.Close ();
				} catch (SocketException) {
					// Do nothing
				}
			}
			if ( sendingXdr != null ) {
				XdrEncodingStream deadXdrStream = sendingXdr;
				sendingXdr = null;
				try {
					deadXdrStream.Close();
				} catch ( IOException) {
					// Do nothing
				} catch ( OncRpcException) {
					// Do nothing
				}
			}
			if ( receivingXdr != null ) {
				XdrDecodingStream deadXdrStream = receivingXdr;
				receivingXdr = null;
				try {
					deadXdrStream.Close ();
				} catch (IOException) {
					// Do nothing
				} catch (OncRpcException) {
					// Do nothing
				}
			}
			if ( parent != null ) {
				parent.RemoveTransport (this);
				parent.SendClosed (ip, port);
				parent = null;
			}
		}

		/// <summary>
		///  Finalize object by making sure that we're removed from the list
		///  of open transports which our parent transport maintains.
		/// </summary>
		~OncRpcTcpConnectionServerTransport ()
		{
			if (parent != null)
				parent.RemoveTransport (this);
		}

		/// <summary>
		///  Retrieves the parameters sent within an ONC/RPC call message. It also
		///  makes sure that the deserialization process is properly finished after
		///  the call parameters have been retrieved. Under the hood this method
		///  therefore calls {@link XdrDecodingStream#endDecoding} to free any
		///  pending resources from the decoding stage.
		/// </summary>
		///  throws OncRpcException, IOException 
		public override void RetrieveCall (IXdrAble call)
		{
			call.XdrDecode (receivingXdr);
			if (pendingDecoding) {
				pendingDecoding = false;
				receivingXdr.EndDecoding ();
			}
		}

		/// <summary>
		///  Returns XDR stream which can be used for deserializing the parameters
		///  of this ONC/RPC call. This method belongs to the lower-level access
		///  pattern when handling ONC/RPC calls.
		/// </summary>
		public override XdrDecodingStream GetXdrDecodingStream ()
		{
			return receivingXdr;
		}


		/// <summary>
		///  Finishes call parameter deserialization. Afterwards the XDR stream
		///  returned by {@link #getXdrDecodingStream} must not be used any more.
		///  This method belongs to the lower-level access pattern when handling
		///  ONC/RPC calls.
		/// </summary>
		// throws OncRpcException, IOException 
		public override void EndDecoding ()
		{
			if (pendingDecoding) {
				pendingDecoding = false;
				receivingXdr.EndDecoding ();
			}
		}

		/// <summary>
		///  Returns XDR stream which can be used for eserializing the reply
		///  to this ONC/RPC call. This method belongs to the lower-level access
		///  pattern when handling ONC/RPC calls.
		/// </summary>
		public override XdrEncodingStream GetXdrEncodingStream ()
		{
			return sendingXdr;
		}

		/// <summary>
		///  Begins the sending phase for ONC/RPC replies.
		///  This method belongs to the lower-level access pattern when handling
		///  ONC/RPC calls.
		/// </summary>
		///  <param name="callInfo">Information about ONC/RPC call for which we are about
		///    to send back the reply.</param>
		///  <param name="state">ONC/RPC reply header indicating success or failure.</param>
		// OncRpcException, IOException 
		public override void BeginEncoding (OncRpcCallInformation callInfo,
			OncRpcServerReplyMessage state)
		{
			//
			// In case decoding has not been properly finished, do it now to
			// free up pending resources, etc.
			//
			if (pendingDecoding) {
				pendingDecoding = false;
				receivingXdr.EndDecoding ();
			}
			//
			// Now start encoding using the reply message header first...
			//
			pendingEncoding = true;
			sendingXdr.BeginEncoding (callInfo.PeerAddress, callInfo.PeerPort);
			state.XdrEncode (sendingXdr);
		}

	   	/// <summary>
	   	///  Finishes encoding the reply to this ONC/RPC call. Afterwards you must
	   	///  not use the XDR stream returned by {@link #getXdrEncodingStream} any
	   	///  longer.
	   	/// </summary>
	   	/// throws OncRpcException, IOException 
		public override void EndEncoding ()
		{
			//
			// Close the case. Finito.
			//
			sendingXdr.EndEncoding ();
			pendingEncoding = false;
		}

	   	/// <summary>
		///  Send back an ONC/RPC reply to the original caller. This is rather a
		///  low-level method, typically not used by applications. Dispatcher handling
		///  ONC/RPC calls have to use the OncRpcCallInformation#reply(IXdrAble) method 
		///  instead on the call object supplied to the handler.
	   	/// </summary>
		///  <param name="callInfo">information about the original call, which are necessary
		///    to send back the reply to the appropriate caller.</param>
		///  <param name="state">ONC/RPC reply message header indicating success or failure
		///    and containing associated state information.</param>
		///  <param name="reply">If not <code>null</code>, then this parameter references
		///    the reply to be serialized after the reply message header.</param>
		///  see OncRpcCallInformation
		///  see IOncRpcDispatchable
		// throws OncRpcException, IOException 
		public override void Reply (OncRpcCallInformation callInfo,
			OncRpcServerReplyMessage state, IXdrAble reply)
		{
			BeginEncoding (callInfo, state);
			if (reply != null)
				reply.XdrEncode (sendingXdr);
			EndEncoding ();
		}

		/// <summary>
		///  Creates a new thread and uses this thread to handle the new connection
		///  to receive ONC/RPC requests, then dispatching them and finally sending
		///  back reply messages. Control in the calling thread immediately
		///  returns after the handler thread has been created.
		///  
		///  Currently only one call after the other is dispatched, so no
		///  multithreading is done when receiving multiple calls. Instead, later
		///  calls have to wait for the current call to finish before they are
		///  handled.
		/// </summary>
		public override void Listen ()
		{
			Thread listener = new Thread (this._listen);
			listener.IsBackground = true;
			listener.Start ();
		}
		
		
		private const int _ok = 0;
		private const int _doBreak = 1;
		private const int _doContinue = 2;
		
	   	/// <summary>
		///  The real workhorse handling incoming requests, dispatching them 
		///  and sending back replies.
	   	/// </summary>
		private void _listen ()
		{
			var callInfo = new OncRpcCallInformation (this);
			while (true) {
				if (!StartDecoding (callInfo))
					return;
				int result = DecodeHeader (callInfo);
				if (result == _doBreak)
					return;
				else if (result == _doContinue)
					continue;
				if (!DispatchAndDecodeCall (callInfo))
					return;
			}
		}
		
		
		private bool StartDecoding (OncRpcCallInformation callInfo)
		{
			// Start decoding the incomming call. This involves remembering
			// from whom we received the call so we can later send back the
			// appropriate reply message.
			try {
				tcpClient.SendTimeout = 0;
				pendingDecoding = true;
				receivingXdr.BeginDecoding ();
				callInfo.PeerAddress = receivingXdr.GetSenderAddress ();
				callInfo.PeerPort = receivingXdr.GetSenderPort ();
				tcpClient.SendTimeout = transmissionTimeout;
			} catch (IOException) {
				// In case of I/O Exceptions (especially socket exceptions) 
				// close the file and leave the stage.
				Close ();
				return false;
			} catch (OncRpcException) {
				Close (); // In case of ONC/RPC exceptions at this stage kill the connection.
				return false;
			}
			return true;
		}
		
		private int DecodeHeader (OncRpcCallInformation callInfo)
		{
			try {
				//
				// Pull off the ONC/RPC call header of the XDR stream.
				//
				callInfo.CallMessage.XdrDecode (receivingXdr);
			} catch (IOException) {
				// In case of I/O Exceptions (especially socket exceptions) 
				// close the file and leave the stage.
				Close ();
				return 1;
			} catch (OncRpcException) {
				//
				// In case of ONC/RPC exceptions at this stage we're silently
				// ignoring that there was some data coming in, as we're not
				// sure we got enough information to send a matching reply
				// message back to the caller.
				//
				if (pendingDecoding) {
					pendingDecoding = true;
					try {
						receivingXdr.EndDecoding ();
					} catch (IOException) {
						Close ();
						return _doBreak;
 					} catch (OncRpcException) {
						// Do nothing
					}
				}
				return _doContinue; // continue
			}
			return _ok;
		}
		
		private bool DispatchAndDecodeCall (OncRpcCallInformation callInfo)
		{
			
			try {
				//
				// Let the dispatcher retrieve the call parameters, work on
				// it and send back the reply.
				// To make it once again clear: the dispatch called has to
				// pull off the parameters of the stream!
				//
									
				dispatcher.DispatchOncRpcCall (callInfo,
					callInfo.CallMessage.Program,
					callInfo.CallMessage.Version,
					callInfo.CallMessage.Procedure);
			
			} catch (Exception e) {
				//
				// In case of some other runtime exception, we report back to
				// the caller a system error. We can not do this if we don't
				// got the exception when serializing the reply, in this case
				// all we can do is to drop the connection. If a reply was not
				// yet started, we can safely send a system error reply.
				//
				if (pendingEncoding) {
					Close (); // Drop the connection...
					return false; // ...and kill the transport.
				}
				//
				// Looks safe, so we try to send back an error reply.
				//
				if (pendingDecoding) {
					pendingDecoding = false;
					try {
						receivingXdr.EndDecoding ();
					} catch (IOException) {
						Close ();
						return false;
					} catch (OncRpcException) {
						// Do nothing
					}
				}
				//
				// Check for authentication exceptions, which are reported back
				// as is. Otherwise, just report a system error
				// -- very generic, indeed.
				//
				try {
					if (e is OncRpcAuthenticationException) {
						callInfo.FailAuthenticationFailed (
							((OncRpcAuthenticationException) e).AuthStatus);
					} else {
						callInfo.FailSystemError ();
					}
				} catch (IOException) {
					Close ();
					return false;
				} catch (OncRpcException) {
					// Do nothing
				}
				//
				// Phew. Done with the error reply. So let's wait for new
				// incoming ONC/RPC calls...
				//
			}
			return true;
		}

	}
	
}

