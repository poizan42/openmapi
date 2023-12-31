//
// openmapi.org - CompactTeaSharp - OnRpcTcpClient.cs
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
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.IO;



namespace CompactTeaSharp
{
	/// <summary>
	///  ONC/RPC client which communicates with ONC/RPC servers over the network 
	///  using the stream-oriented protocol TCP/IP.
	/// </summary>
	public class OncRpcTcpClient: OncRpcClient
	{
		private TcpClient client; // TCP socket used for stream-oriented communication with an ONC/RPC server.
		protected XdrTcpEncodingStream sendingXdr; // XDR encoding stream used for sending requests via TCP/IP to an ONC/RPC server.
		protected XdrTcpDecodingStream receivingXdr; // XDR decoding stream used when receiving replies via TCP/IP from an ONC/RPC server.
		protected int transmissionTimeout = 30000; // Timeout during the phase where data is sent within calls, or data is received within replies.


		/// <summary>
		///  Set the timout for remote procedure calls to wait for an answer from
		///  the ONC/RPC server. If the timeout expires,
		///  #call (int, IXdrAble, IXdrAble) will raise a InterruptedIOException. 
		///  The default timeout value is 30 seconds (30,000 milliseconds). The 
		///  timeout must be > 0. A timeout of zero indicates a batched call, for 
		///  which no reply message is expected.
		/// </summary>
		/// <param name="milliseconds">Timeout in milliseconds. A timeout of zero indicates
		///   batched calls.</param>
		public override void SetTimeout (int milliseconds)
		{
			base.SetTimeout (milliseconds);
		}

		/// <summary>
		///  The current timeout used during transmission phases (call and reply phases).
		/// </summary>
		public int TransmissionTimeout {
			get { return transmissionTimeout; }
			set {
				// Set the timeout used during transmission of data. If the flow of data
				// when sending calls or receiving replies blocks longer than the given
				// timeout, an exception is thrown. The timeout must be > 0.
				if (value <= 0)
					throw new ArgumentException ("transmission timeout must be > 0");
				transmissionTimeout = value;
			}
		}

		/// <summary>
		///  Set the character encoding for (de-)serializing strings.
		/// </summary>
		/// <param name="characterEncoding"> the encoding to use for (de-)serializing strings.
		///   If null, the system's default encoding is to be used.</param>
		public override string CharacterEncoding {
			get { return receivingXdr.CharacterEncoding; }
			set {
				receivingXdr.CharacterEncoding = value;
				sendingXdr.CharacterEncoding = value;
			}
		}

		/// <summary>
		///  Constructs a new <code>OncRpcTcpClient</code> object, which connects
		///  to the ONC/RPC server at <code>host</code> for calling remote procedures
		///  of the given { program, version }.
		/// </summary>
		/// <param name="host">The host where the ONC/RPC server resides.</param>
		/// <param name="program">Program number of the ONC/RPC server to call.</param>
		/// <param name="version">Program version number.</param>
		// OncRpcException, IOException
		public OncRpcTcpClient (IPAddress host, int program, int version, bool useSsl):
			this (host, program, version, 0, 0, useSsl)
		{

		}

		/// <summary>
		///  Constructs a new <code>OncRpcTcpClient</code> object, which connects
		///  to the ONC/RPC server at <code>host</code> for calling remote procedures
		///  of the given { program, version }.
		/// <summary>
		/// <param name="host">The host where the ONC/RPC server resides.</param>
		/// <param name="program">Program number of the ONC/RPC server to call.</param>
		/// <param name="version">Program version number.</param>
		/// <param name="port">The port number where the ONC/RPC server can be contacted.
 		///   If 0, then the OncRpcUdpClient object will ask the portmapper at 
		///   host for the port number.</param>
		// OncRpcException, IOException
		public OncRpcTcpClient (IPAddress host, int program, int version, int port, bool useSsl):
			this (host, program, version, port, 0, useSsl)
		{

		}

		/// <summary>
		/// Constructs a new OncRpcTcpClient object, which connects
		/// to the ONC/RPC server at host for calling remote procedures
		/// of the given { program, version }.
		/// </summary>
		/// <param name="host">The host where the ONC/RPC server resides.</param>
		/// <param name="program">Program number of the ONC/RPC server to call.</param>
		/// <param name="version">Program version number.</param>
		/// <param name="port">The port number where the ONC/RPC server can be contacted.
		///   If 0, then the OncRpcUdpClient object will
		///   ask the portmapper at host for the port number.</param>
		/// <param name="bufferSize">Size of receive and send buffers. In contrast to
		///   UDP-based ONC/RPC clients, messages larger than the specified
		///   buffer size can still be sent and received. The buffer is only
		///   necessary to handle the messages and the underlaying streams will
		///   break up long messages automatically into suitable pieces.
		///   Specifying zero will select the default buffer size (currently
		///   8192 bytes).</param>
		// OncRpcException, IOException
		public OncRpcTcpClient (IPAddress host, int program, int version, 
			int port, int bufferSize, bool useSsl) : this (host, program, version, port, bufferSize, -1, useSsl)
		{

		}

		/// <summary>
		///  Constructs a new <code>OncRpcTcpClient</code> object, which connects
		///  to the ONC/RPC server at <code>host</code> for calling remote procedures
		///  of the given { program, version }.
		/// </summary>
		/// <param name="host">The host where the ONC/RPC server resides.</param>
		/// <param name="program">Program number of the ONC/RPC server to call.</param>
		/// <param name="version">Program version number.</param>
		/// <param name="port">The port number where the ONC/RPC server can be contacted.
		/// If <code>0</code>, then the <code>OncRpcUdpClient</code> object will
		/// ask the portmapper at <code>host</code> for the port number.</param>
		/// <param name="bufferSize">Size of receive and send buffers. In contrast to
		///   UDP-based ONC/RPC clients, messages larger than the specified
		///   buffer size can still be sent and received. The buffer is only
		///   necessary to handle the messages and the underlaying streams will
		///   break up long messages automatically into suitable pieces.
		///   Specifying zero will select the default buffer size (currently
		///   8192 bytes).</param>
		/// <param name="timeout">Maximum timeout in milliseconds when connecting to
		/// the ONC/RPC server. If negative, a default implementation-specific
		/// timeout setting will apply. <i>Note that this timeout only applies
		/// to the connection phase, but <b>not</b> to later communication.</i></param>
		// OncRpcException, IOException
		public OncRpcTcpClient (IPAddress host, int program, int version, 
			int port, int bufferSize, int timeout, bool useSsl) : this (host, program, 
				version, port, bufferSize, timeout, null, useSsl)
		{
		}

		public OncRpcTcpClient (IPAddress host, int program, int version, int port, 
			int bufferSize, int timeout, TcpClient tcpClient, bool useSsl) : base (host, program, version, port, OncRpcProtocols.Tcp)
		{
			//
			// Construct the inherited part of our object. This will also try to
			// lookup the port of the desired ONC/RPC server, if no port number
			// was specified (port = 0).
			//

			//
			// Let the host operating system choose which port (and network
			// interface) to use. Then set the buffer sizes for sending and
			// receiving UDP datagrams. Finally set the destination of packets.
			//

			if (bufferSize < 1024)
				bufferSize = 1024;

			//
			// Note that we use this.port at this time, because the superclass
			// might have resolved the port number in case the caller specified
			// simply 0 as the port number.
			//

			if (timeout < 0) // TODO: hack...
				timeout = 0;

			// If no tcp client was passed in, create one.
			if (tcpClient == null) {
				client = new TcpClient ();
				client.Connect (host, this.port);
			} else {
				client = tcpClient;
			}

			client.SendTimeout = timeout;
			client.ReceiveTimeout = timeout;
			client.NoDelay = true;
			if (client.ReceiveBufferSize < bufferSize)
				client.ReceiveBufferSize = bufferSize;
			if (client.SendBufferSize < bufferSize)
				client.SendBufferSize = bufferSize;
			//
			// Create the necessary encoding and decoding streams, so we can
			// communicate at all.
			//
			
			Stream stream = client.GetStream ();
			if (useSsl)
                stream = OncNetworkUtility.GetSslClientStream (stream, host);
                
			sendingXdr = new XdrTcpEncodingStream (client, stream, bufferSize);
			receivingXdr = new XdrTcpDecodingStream (client, stream, bufferSize);
		}
		
		/// <summary>
		///  Close the connection to an ONC/RPC server and free all 
		///  network-related resources.
		/// </summary>
		// OncRpcException
		public override void Close ()
		{
			if (client != null) {
				try {
					client.Close ();
				} catch {
					// do nothing
				}
				client = null;
			}
			if (sendingXdr != null) {
				try {
					sendingXdr.Close ();
				} catch {
					// do nothing
				}
				sendingXdr = null;
			}
			if (receivingXdr != null) {
				try {
					receivingXdr.Close ();
				} catch {
					// do nothing
				}
				receivingXdr = null;
			}
		}

		/// <summary>
		///  Calls a remote procedure on an ONC/RPC server.
		///
		///  Please note that while this method supports call batching by
		///  setting the communication timeout to zero you should better use
		///  BatchCall as it provides better control over when the
		///  batch should be flushed to the server.
		/// </summary>
		/// <param name="procedureNumber">Procedure number of the procedure to call.</param>
		/// <param name="versionNumber">Protocol version number.</param>
		/// <param name="params">The parameters of the procedure to call, contained
		///   in an object which implements the {@link IXdrAble} interface.</param>
		/// <param name="result">The object receiving the result of the procedure call.</param>
		// OncRpcException
		public override void Call (int procedureNumber, int versionNumber,
		                 IXdrAble @params, IXdrAble result)
		{
			lock (typeof (OncRpcTcpClient)) {
				Refresh:
				int refreshesLeft = 1;
				while (refreshesLeft-- >= 0) {
					// for ( int refreshesLeft = 1; refreshesLeft >= 0; --refreshesLeft ) {
					//
					// First, build the ONC/RPC call header. Then put the sending
					// stream into a known state and encode the parameters to be
					// sent. Finally tell the encoding stream to send all its data
					// to the server. Then wait for an answer, receive it and decode
					// it. So that's the bottom line of what we do right here.
					//
					NextXid();

					var callHeader = new OncRpcClientCallMessage (xid, program, versionNumber, procedureNumber, auth);
					OncRpcClientReplyMessage replyHeader = new OncRpcClientReplyMessage (auth);

					//
					// Send call message to server. If we receive an IOException,
					// then we'll throw the appropriate ONC/RPC (client) exception.
					// Note that we use a connected stream, so we don't need to
					// specify a destination when beginning serialization.
					//
					try {
						// TODO check next replaced code
						// socket.setSoTimeout(transmissionTimeout);
						
						if (transmissionTimeout < 0)		//TODO: hack
							transmissionTimeout = 0;
						client.SendTimeout = transmissionTimeout;
						sendingXdr.BeginEncoding (null, 0);
						callHeader.XdrEncode (sendingXdr);
						@params.XdrEncode (sendingXdr);
						if (timeout != 0)
							sendingXdr.EndEncoding ();
						else
							sendingXdr.EndEncoding (false);
					} catch (IOException e) {
						throw new OncRpcException (OncRpcException.CANT_SEND, e.Message);
					}

					//
					// Receive reply message from server -- at least try to do so...
					// In case of batched calls we don't need no stinkin' answer, so
					// we can do other, more interesting things.
					//
					if (timeout == 0)
						return;

					try {
						//
						// Keep receiving until we get the matching reply.
						//
						while (true) {
							// TODO check replaced code socket.setSoTimeout(timeout);

							if (timeout < 0) //TODO: hack
								timeout = 0;

							client.ReceiveTimeout = timeout;
							receivingXdr.BeginDecoding ();

							// TODO check replaced code socket.setSoTimeout(transmissionTimeout);

							if (transmissionTimeout < 0) //TODO: hack
								transmissionTimeout = 0;

							client.ReceiveTimeout = transmissionTimeout;

							//
							// First, pull off the reply message header of the
							// XDR stream. In case we also received a verifier
							// from the server and this verifier was invalid, broken
							// or tampered with, we will get an
							// OncRpcAuthenticationException right here, which will
							// propagate up to the caller. If the server reported
							// an authentication problem itself, then this will
							// be handled as any other rejected ONC/RPC call.
							//
							try {
								replyHeader.XdrDecode (receivingXdr);
							} catch (OncRpcException) {
								//
								// ** SF bug #1262106 **
								//
								// We ran into some sort of trouble. Usually this will have
								// been a buffer underflow. Whatever, end the decoding process
								// and ensure this way that the next call has a chance to start
								// from a clean state.
								//
								receivingXdr.EndDecoding ();
								throw;
							}
							//
							// Only deserialize the result, if the reply matches the
							// call. Otherwise skip this record.
							//
							if (replyHeader.MessageId == callHeader.MessageId)
							  break;
							receivingXdr.EndDecoding ();
						}
						
						//
						// Make sure that the call was accepted. In case of unsuccessful
						// calls, throw an exception, if it's not an authentication
						// exception. In that case try to refresh the credential first.
						//
						if ( !replyHeader.SuccessfullyAccepted () ) {
							receivingXdr.EndDecoding ();

							//
							// Check whether there was an authentication
							// problem. In this case first try to refresh the
							// credentials.
							//
							if ( (refreshesLeft > 0)
							   && (replyHeader.ReplyStatus == OncRpcReplyStatus.MsgDenied)
							   && (replyHeader.RejectStatus == OncRpcRejectStatus.AuthenticationError)
							   && (auth != null)
							   && auth.CanRefreshCred)
							{
								goto Refresh;
							}
							// Nope. No chance. This gets tough.
							throw replyHeader.newException ();
						}
						try {
							result.XdrDecode (receivingXdr);
						} catch (OncRpcException e) {
							//
							// ** SF bug #1262106 **
							//
							// We ran into some sort of trouble. Usually this will have
							// been a buffer underflow. Whatever, end the decoding process
							// and ensure this way that the next call has a chance to start
							// from a clean state.
							//
							receivingXdr.EndDecoding ();
							throw e;
						}
						//
						// Free pending resources of buffer and exit the call loop,
						// returning the reply to the caller through the result
						// object.
						//
						receivingXdr.EndDecoding ();
							return;
						} catch (SocketException) {
							// In case our time run out, we throw an exception.
							throw new OncRpcTimeoutException ();
						} catch (IOException e) {
							//
							// Argh. Trouble with the transport. Seems like we can't
							// receive data. Gosh. Go away!
							//
							throw new OncRpcException (OncRpcException.CANT_RECV, e.Message);
						}
				} // for ( refreshesLeft )
			}
		}

		/// <summary>
		/// Issues a batched call for a remote procedure to an ONC/RPC server.
		/// Below is a small example (exception handling ommited for clarity):
		///
		/// <code>
		///  OncRpcTcpClient client = new OncRpcTcpClient(
		///   InetAddress.getByName("localhost"),
		///   myprogramnumber, myprogramversion,
		///   OncRpcProtocols.Tcp);
		///   client.callBatch(42, myparams, false);
		///   client.callBatch(42, myotherparams, false);
		///   client.callBatch(42, myfinalparams, true);
		/// </code>
		/// 
		/// In the example above, three calls are batched in a row and only be sent
		/// all together with the third call. Note that batched calls must not expect
		/// replies, with the only exception being the last call in a batch:
		/// 
		/// <code>
		/// client.callBatch(42, myparams, false);
		/// client.callBatch(42, myotherparams, false);
		/// client.call(43, myfinalparams, myfinalresult);
		/// </code>
		/// </summary>
		/// <param name="procedureNumber">Procedure number of the procedure to call.</param>
		/// <param name="params">The parameters of the procedure to call, contained
		/// in an object which implements the {@link IXdrAble} interface.</param>
		/// <param name="flush">Make sure that all pending batched calls are sent to
		/// the server.</param>
		/// 
		// OncRpcException
		public void BatchCall (int procedureNumber,
			IXdrAble prms, bool flush)
		{
			lock (typeof (OncRpcTcpClient)) {
				//
				// First, build the ONC/RPC call header. Then put the sending
				// stream into a known state and encode the parameters to be
				// sent. Finally tell the encoding stream to send all its data
				// to the server. We don't then need to wait for an answer. And
				// we don't need to take care of credential refreshes either.
				//
				NextXid ();

				var callHeader = new OncRpcClientCallMessage (
				xid, program, version, procedureNumber, auth);

				//
				// Send call message to server. If we receive an IOException,
				// then we'll throw the appropriate ONC/RPC (client) exception.
				// Note that we use a connected stream, so we don't need to
				// specify a destination when beginning serialization.
				//
				try {
					// TODO check replaced code socket.setSoTimeout(transmissionTimeout);

					if (timeout < 0) // TODO: hack...
						timeout = 0;

					client.SendTimeout = transmissionTimeout;
					sendingXdr.BeginEncoding (null, 0);
					callHeader.XdrEncode (sendingXdr);
					prms.XdrEncode (sendingXdr);
					sendingXdr.EndEncoding (flush);
				} catch (IOException e) {
						throw new OncRpcException (OncRpcException.CANT_SEND, e.Message);
				}
			}
		}
		
	}
}
