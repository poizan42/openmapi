//
// openmapi.org - CompactTeaSharp - OncRpcExceptions.cs
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

namespace CompactTeaSharp
{

	/// <summary>
	///  Indicates ONC/RPC conditions that a reasonable application might want to catch.
	/// </summary>
	/// <remarks>
	///  Also defines a set of ONC/RPC error codes as defined by RFC 1831. 
	///  Note that all these error codes are solely used on the client-side or 
	///  server-side, but never transmitted over the wire. For error codes 
	///  transmitted over the network, refer to OncRpcAcceptStatus and OncRpcRejectStatus.
	/// </remarks>
	public class OncRpcException : Exception
	{
	    private int reason;
	    private string message;
	
	    /// <summary>
	    ///  Returns the error message string of this ONC/RPC object.
	    /// </summary>
	    public string Message {
			get { return message; }
		}

	    /// <summary>
	    ///  Returns the error reason of this ONC/RPC exception object.
	    /// </summary>
	    public int Reason {
			get { return reason; }
		}
		
		public const int SUCCESS = 0;			// The remote procedure call was carried out successfully.
		public const int CANT_ENCODE_ARGS = 1;	// The client can not encode the argments to be sent for the remote procedure call.
		public const int CANT_DECODE_RES = 2;	// The client can not decode the result from the remote procedure call.
		public const int CANT_SEND = 3;			// Encoded information can not be sent.
		public const int CANT_RECV = 4;			// Information to be decoded can not be received.
		public const int TIMEDOUT = 5;			// The remote procedure call timed out.
		public const int VERS_MISMATCH = 6;		// ONC/RPC versions of server and client are not compatible.
		public const int AUTH_ERROR = 7;		// The ONC/RPC server did not accept the authentication sent by the client. Bad girl/guy!
		public const int PROG_UNAVAIL = 8;		// The ONC/RPC server does not support this particular program.
		public const int PROG_VERS_MISMATCH = 9;// The ONC/RPC server does not support this particular version of the program.
		public const int PROC_UNAVAIL = 10;		// The given procedure is not available at the ONC/RPC server.
		public const int CANT_DECODE_ARGS = 11;	// The ONC/RPC server could not decode the arguments sent within the call message.
		public const int SYSTEM_ERROR = 12;		// The ONC/RPC server encountered a system error and thus was not able to carry out the requested remote function call successfully.
		public const int UNKNOWN_PROTO = 17;	// The caller specified an unknown/unsupported IP protocol. Currently, only {@link OncRpcProtocols#ONCRPC_TCP} and {@link OncRpcProtocols#ONCRPC_UDP} are supported.
		public const int PMAP_FAILURE = 14;		// The portmapper could not be contacted at the given host.
		public const int PROG_NOT_REGISTERED = 15;// The requested program is not registered with the given host.
		public const int FAILED = 16;			// A generic ONC/RPC exception occured. Shit happens...
		public const int BUFFER_OVERFLOW = 42;	// A buffer overflow occured with an encoding XDR stream. This happens if you use UDP-based (datagram-based) XDR streams and you try to encode more data than can fit into the sending buffers.
		public const int BUFFER_UNDERFLOW = 43;	// A buffer underflow occured with an decoding XDR stream. This happens if you try to decode more data than was sent by the other communication partner.
		public const int WRONG_MESSAGE = 44;	// Either a ONC/RPC server or client received the wrong type of ONC/RPC message when waiting for a request or reply. Currently, only the decoding methods of the classes {@link OncRpcCallMessage} and {@link OncRpcReplyMessage} throw exceptions with this reason.
		public const int CANNOT_REGISTER = 45;	// Indicates that a server could not register a transport with the ONC/RPC port mapper.

		/// <summary>
		///  Constructs an OncRpcException with a reason of OncRpcException.FAILED.
		/// </summary>
		public OncRpcException (): this (OncRpcException.FAILED)
		{
		}

		/// <summary>
		///  Constructs an OncRpcException with the specified detail message.
		/// </summary>
		public OncRpcException (string detailMessage): base ()
		{
			reason = FAILED;
			message = detailMessage;
		}

		/// <summary>
		///  Constructs an OncRpcException with the specified detail
		///  reason and message. For possible reasons, see below.
		/// </summary>
		public OncRpcException (int detailReason, string detailMessage): base ()
		{
			reason = detailReason;
			message = detailMessage;
		}

		/// <summary>
	 	//   Constructs an OncRpcException with the specified detail
		///  reason. The detail message is derived from the reason.
		/// </summary>
		/// <param name="detailReason">The reason. This can be one of the 
		///   constants defined in this interface.</param>
		public OncRpcException (int detailReason): base ()
		{
			reason = detailReason;
			message = GetMessageText (detailReason);
		}

		private string GetMessageText (int reason)
		{
			switch (reason) {
				case CANT_ENCODE_ARGS: return "can not encode RPC arguments";
				case CANT_DECODE_RES: return "can not decode RPC result";
				case CANT_RECV: return "can not receive ONC/RPC data";
				case CANT_SEND: return "can not send ONC/RPC data";
				case TIMEDOUT: return "ONC/RPC call timed out";
				case VERS_MISMATCH: return "ONC/RPC version mismatch";
				case AUTH_ERROR: return "ONC/RPC authentification error";
				case PROG_UNAVAIL: return "ONC/RPC program not available";
				case CANT_DECODE_ARGS: return "can not decode ONC/RPC arguments";
				case PROG_VERS_MISMATCH: return "ONC/RPC program version mismatch";
				case PROC_UNAVAIL: return "ONC/RPC procedure not available";
				case SYSTEM_ERROR: return "ONC/RPC system error";
				case UNKNOWN_PROTO: return "unknown protocol";
				case PMAP_FAILURE: return "ONC/RPC portmap failure";
				case PROG_NOT_REGISTERED: return "ONC/RPC program not registered";
				case FAILED: return "ONC/RPC generic failure";
				case BUFFER_OVERFLOW: return "ONC/RPC buffer overflow";
				case BUFFER_UNDERFLOW: return "ONC/RPC buffer underflow";
				case WRONG_MESSAGE: return "wrong ONC/RPC message type received";
				case CANNOT_REGISTER: return "cannot register ONC/RPC port with local portmap";
				case SUCCESS:
					default: return null;
			}
		}

	}

	/// <summary>
	///  Indicates an authentication exception.
	/// </summary>
	public class OncRpcAuthenticationException : OncRpcException
	{
		private OncRpcAuthStatus authStatusDetail;

		/// <summary>
	 	///  Initializes an <code>OncRpcAuthenticationException</code>
		///  with a detail of {@link OncRpcException#RPC_AUTHERROR} and
		///  the specified {@link OncRpcAuthStatus authentication status} detail.
		/// </summary>
		public OncRpcAuthenticationException (OncRpcAuthStatus authStatus) : base (AUTH_ERROR)
		{
			authStatusDetail = authStatus;
		}

		/// <summary>
		///  The authentication status detail of this ONC/RPC exception.
		/// </summary>
		public OncRpcAuthStatus AuthStatus {
			get { return authStatusDetail; }
		}

	}

	/// <summary>
	///  Indicates a timed-out call exception.
	/// </summary>
	public class OncRpcTimeoutException : OncRpcException
	{
		public OncRpcTimeoutException () : base (TIMEDOUT)
		{
		}
	}

}
