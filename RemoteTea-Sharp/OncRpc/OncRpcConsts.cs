//
// RemoteTea - OnRpcClient.cs
//
// C# port Copyright 2008 by Topalis AG
//
// Author: mazurin
//
// This library is based on the remotetea java library: 
//
// Copyright (c) 1999, 2000
// Lehrstuhl fuer Prozessleittechnik (PLT), RWTH Aachen
// D-52064 Aachen, Germany.
// All rights reserved.
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

namespace RemoteTea.OncRpc
{

	/// <summary>
	/// A collection of constants used to identify the acceptance status of
	/// ONC/RPC reply messages.
	/// </summary>
	public struct OncRpcAcceptStatus {
	
		/// <summary>
		/// The remote procedure was called and executed successfully.
		/// </summary>
	    public const int ONCRPC_SUCCESS = 0;
	
		/// <summary>
	    /// The program requested is not available. So the remote host
	    /// does not export this particular program and the ONC/RPC server
	    /// which you tried to send a RPC call message doesn't know of this
	    /// program either.
		/// </summary>
	    public const int ONCRPC_PROG_UNAVAIL = 1;
	
		/// <summary>
	    /// A program version number mismatch occured. The remote ONC/RPC
	    /// server does not support this particular version of the program.
		/// </summary>
	    public const int ONCRPC_PROG_MISMATCH = 2;
	
		/// <summary>
	    /// The procedure requested is not available. The remote ONC/RPC server
	    /// does not support this particular procedure.
		/// </summary>
	    public const int ONCRPC_PROC_UNAVAIL = 3;
	
		/// <summary>
	    /// The server could not decode the arguments sent within the ONC/RPC
	    /// call message.
		/// </summary>
	    public const int ONCRPC_GARBAGE_ARGS = 4;
	
		/// <summary>
	    /// The server encountered a system error and thus was not able to
	    /// process the procedure call. Causes might be memory shortage,
	    /// desinterest and sloth.
		/// </summary>
	    public const int ONCRPC_SYSTEM_ERR = 5;
	
	}
	
	/// <summary>
	/// A collection of constants related to authentication and generally usefull
	/// for ONC/RPC.
	/// </summary>
	internal struct OncRpcAuthConstants {
	
	    /// <summary>
	    /// Maximum length of opaque authentication information.
	    /// </summary>
	    public const int ONCRPC_MAX_AUTH_BYTES = 400;
	
	    /// <summary>
	    /// Maximum length of machine name.
	    /// </summary>
	    public const int ONCRPC_MAX_MACHINE_NAME = 255;
	
	    /// <summary>
	    /// Maximum allowed number of groups.
	    /// </summary>
	    public const int ONCRPC_MAX_GROUPS = 16;
	
	}
	
	/// <summary>
	/// A collection of constants used to identify the authentication status
	/// (or any authentication errors) in ONC/RPC replies of the corresponding
	/// ONC/RPC calls.
	/// </summary>
	internal struct OncRpcAuthStatus {
	
	    /// <summary>
	    /// There is no authentication problem or error.
	    /// </summary>
	    public const int ONCRPC_AUTH_OK = 0;
	
	    /// <summary>
	    /// The ONC/RPC server detected a bad credential (that is, the seal was
	    /// broken).
	    /// </summary>
	    public const int ONCRPC_AUTH_BADCRED = 1;
	
	    /// <summary>
	    /// The ONC/RPC server has rejected the credential and forces the caller
	    /// to begin a new session.
	    /// </summary>
	    public const int ONCRPC_AUTH_REJECTEDCRED = 2;
	
	    /// <summary>
	    /// The ONC/RPC server detected a bad verifier (that is, the seal was
	    /// broken).
	    /// </summary>
	    public const int ONCRPC_AUTH_BADVERF = 3;
	
	    /// <summary>
	    /// The ONC/RPC server detected an expired verifier (which can also happen
	    /// if the verifier was replayed).
	    /// </summary>
	    public const int ONCRPC_AUTH_REJECTEDVERF = 4;
	
	    /// <summary>
	    /// The ONC/RPC server rejected the authentication for security reasons.
	    /// </summary>
	    public const int ONCRPC_AUTH_TOOWEAK = 5;
	
	    /// <summary>
	    /// The ONC/RPC client detected a bogus response verifier.
	    /// </summary>
	    public const int ONCRPC_AUTH_INVALIDRESP = 6;
	
	    /// <summary>
	    /// Authentication at the ONC/RPC client failed for an unknown reason.
	    /// </summary>
	    public const int ONCRPC_AUTH_FAILED = 7;
	}

	/// <summary>
	/// A collection of constants used to identify the authentication schemes
	/// available for ONC/RPC. Please note that currently only
	/// <code>ONCRPC_AUTH_NONE</code> is supported by this Java package.
	/// </summary>
	internal struct OncRpcAuthType {
	
	    /// <summary>
	    /// No authentication scheme used for this remote procedure call.
	    /// </summary>
	    public const int ONCRPC_AUTH_NONE = 0;
	    /// <summary>
	    /// The so-called "Unix" authentication scheme is not supported. This one
	    /// only sends the users id as well as her/his group identifiers, so this
	    /// is simply far too weak to use in typical situations where
	    /// authentication is requested.
	    /// </summary>
	    public const int ONCRPC_AUTH_UNIX = 1;
	    /// <summary>
	    /// The so-called "short hand Unix style" is not supported.
	    /// </summary>
	    public const int ONCRPC_AUTH_SHORT = 2;
	    /// <summary>
	    /// The DES authentication scheme (using encrypted time stamps) is not
	    /// supported -- and besides, it's not a silver bullet either.
	    /// </summary>
	    public const int ONCRPC_AUTH_DES = 3;
	
	}
	
	/// <summary>
	/// A collection of constants used for ONC/RPC messages to identify the
	/// type of message. Currently, ONC/RPC messages can be either calls or
	/// replies. Calls are sent by ONC/RPC clients to servers to call a remote
	/// procedure (for you "ohohpies" that can be translated into the buzzword
	/// "method"). A server then will answer with a corresponding reply message
	/// (but not in the case of batched calls).
	/// </summary>
	public struct OncRpcMessageType {
	
	    /// <summary>
	    /// Identifies an ONC/RPC call. By a "call" a client request that a server
	    /// carries out a particular remote procedure.
	    /// </summary>
	    public const int ONCRPC_CALL = 0;
	
	    /// <summary>
	    /// Identifies an ONC/RPC reply. A server responds with a "reply" after
	    /// a client has sent a "call" for a particular remote procedure, sending
	    /// back the results of calling that procedure.
	    /// </summary>
	    public const int ONCRPC_REPLY = 1;
	
	}
	
	
	/// <summary>
	/// /// A collection of constants used for ONC/RPC messages to identify the
	/// /// remote procedure calls offered by ONC/RPC portmappers.
	/// </summary>
	public struct OncRpcPortmapServices {
	
	    /// <summary>
	    /// Procedure number of portmap service to register an ONC/RPC server.
	    /// </summary>
	    public const int PMAP_SET = 1;
	    /// <summary>
	    /// Procedure number of portmap service to unregister an ONC/RPC server.
	    /// </summary>
	    public const int PMAP_UNSET = 2;
	    /// <summary>
	    /// Procedure number of portmap service to retrieve port number of
	    /// a particular ONC/RPC server.
	    /// </summary>
	    public const int PMAP_GETPORT = 3;
	    /// <summary>
	    /// Procedure number of portmap service to return information about all
	    /// currently registered ONC/RPC servers.
	    /// </summary>
	    public const int PMAP_DUMP = 4;
	    /// <summary>
	    /// Procedure number of portmap service to indirectly call a remote
	    /// procedure an ONC/RPC server through the ONC/RPC portmapper.
	    /// </summary>
	    public const int PMAP_CALLIT = 5;
	
	}
	
	/// <summary>
	/// A collection of protocol constants used by the ONC/RPC package. Each
	/// constant defines one of the possible transport protocols, which can be
	/// used for communication between ONC/RPC clients and servers.
	/// </summary>
	public struct OncRpcProtocols {
	
	    /// <summary>
	    /// Use the UDP protocol of the IP (Internet Protocol) suite as the
	    /// network communication protocol for doing remote procedure calls.
	    /// This is the same as the IPPROTO_UDP definition from the famous
	    /// BSD socket API.
	    /// </summary>
	    public const int ONCRPC_UDP = 17;
	
	    /// <summary>
	    /// Use the TCP protocol of the IP (Internet Protocol) suite as the
	    /// network communication protocol for doing remote procedure calls.
	    /// This is the same as the IPPROTO_TCP definition from the famous
	    /// BSD socket API.
	    /// </summary>
	    public const int ONCRPC_TCP = 6;
	
	    /// <summary>
	    /// Use the HTTP application protocol for tunneling ONC remote procedure
	    /// calls. This is definetely not similiar to any definition in the
	    /// famous BSD socket API.
	    /// </summary>
	    public const int ONCRPC_HTTP = -42;
	
	}
	
	/// <summary>
	/// A collection of constants used to describe why a remote procedure call
	/// message was rejected. This constants are used in {@link OncRpcReplyMessage}
	/// objects, which represent rejected messages if their
	/// {@link OncRpcReplyMessage#replyStatus} field has the value
	/// {@link OncRpcReplyStatus#ONCRPC_MSG_DENIED}.
	/// </summary>
	public struct OncRpcRejectStatus {
	
	    /// <summary>
	    /// Wrong ONC/RPC protocol version used in call (it needs to be version 2).
	    /// </summary>
	    public const int ONCRPC_RPC_MISMATCH = 0;
	
	    /// <summary>
	    /// The remote ONC/RPC server could not authenticate the caller.
	    /// </summary>
	    public const int ONCRPC_AUTH_ERROR = 1;
	
	}

	/// <summary>
	/// A collection of constants used to identify the (overall) status of an
	/// ONC/RPC reply message. 
	/// </summary>
	public struct OncRpcReplyStatus {
	
	    /// <summary>
	    /// Reply status identifying that the corresponding message call was
	    /// accepted.
	    /// </summary>
	    public const int ONCRPC_MSG_ACCEPTED = 0;
	
	    /// <summary>
	    /// Reply status identifying that the corresponding message call was
	    /// denied.
	    /// </summary>
	    public const int ONCRPC_MSG_DENIED = 1;
	
	}
	
	/// <summary>
	/// A collection of constants used to identify the retransmission schemes
	/// when using {@link OncRpcUdpClient UDP/IP-based ONC/RPC clients}.
	/// </summary>
	public struct OncRpcUdpRetransmissionMode {
	
	    /// <summary>
	    /// In exponentional back-off retransmission mode, UDP/IP-based ONC/RPC
	    /// clients first wait a given retransmission timeout period before
	    /// sending the ONC/RPC call again. The retransmission timeout then is
	    /// doubled on each try.
	    /// </summary>
	    public const int EXPONENTIAL = 0;
	
	    /// <summary>
	    /// In fixed retransmission mode, UDP/IP-based ONC/RPC clients wait a
	    /// given retransmission timeout period before send the ONC/RPC call again.
	    /// The retransmission timeout is not changed between consecutive tries
	    /// but is fixed instead.
	    /// </summary>
	    public const int FIXED = 1;
	
	}

}
