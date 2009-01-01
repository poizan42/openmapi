//
// openmapi.org - CompactTeaSharp - OncRpcConsts.cs
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
	///  Constants used to identify the acceptance status of ONC/RPC reply messages.
	/// </summary>
	public enum OncRpcAcceptStatus
	{
		/// <summary>
		///  The remote procedure was called and executed successfully.
		/// </summary>
		Success = 0,

		/// <summary>
		///  The program requested is not available. The remote host
		///  does not export this particular program and the ONC/RPC server
		///  which you tried to send a RPC call message doesn't know of this
		///  program either.
		/// </summary>
		ProgUnavail,

		/// <summary>
		///  A program version number mismatch occured. The remote ONC/RPC
		///  server does not support this particular version of the program.
		/// </summary>
		ProgMismatch,

		/// <summary>
		///  The procedure requested is not available on the remote ONC server.
		/// </summary>
		ProcedureUnavailable,

		/// <summary>
		///  The server could not decode the arguments sent within the ONC/RPC call message.
		/// </summary>
		GarbageArgs,

		/// <summary>
		///  The server encountered a system error and was not able to process 
		///  the rpc call. Causes might be memory shortage, desinterest and sloth.
		/// </summary>
		SystemErr

	}

	/// <summary>
	///  Constants related to authentication and generally useful for ONC/RPC.
	/// </summary>
	public static class OncRpcAuthConstants
	{
		/// <summary>
		///  Maximum length of opaque authentication information.
		/// </summary>
		public const int ONCRPC_MAX_AUTH_BYTES = 400;

		/// <summary>
		///  Maximum length of machine name.
		/// </summary>
		public const int ONCRPC_MAX_MACHINE_NAME = 255;

		/// <summary>
		///  Maximum allowed number of groups.
		/// </summary>
		public const int ONCRPC_MAX_GROUPS = 16;

	}

	/// <summary>
	///  A collection of constants used to identify the authentication status
	///  (or errors) in ONC/RPC replies of the corresponding ONC/RPC calls.
	/// </summary>
	public enum OncRpcAuthStatus
	{
		/// <summary>
		///  There is no authentication problem or error.
		/// </summary>
		Ok = 0,

		/// <summary>
		///  The ONC/RPC server detected a bad credential.
		/// </summary>
		BadCred,

		/// <summary>
		///  The ONC/RPC server has rejected the credential and forces the caller to begin a new session.
		/// </summary>
		RejectedCred,

		/// <summary>
		///  The ONC/RPC server detected a bad verifier (that is, the seal was broken).
		/// </summary>
		BadVerifier,

		/// <summary>
		///  The ONC/RPC server detected an expired verifier (which can also happen if the verifier was replayed).
		/// </summary>
		RejectedVerifier,

		/// <summary>
		///  The ONC/RPC server rejected the authentication for security reasons.
		/// </summary>
		TooWeak,

		/// <summary>
		///  The ONC/RPC client detected a bogus response verifier.
		/// </summary>
		InvalidResponse,

		/// <summary>
		///  Authentication at the ONC/RPC client failed for an unknown reason.
		/// </summary>
		Failed
	}

	/// <summary>
	///  A collection of constants used to identify the authentication schemes
	///  available for ONC/RPC. Please note that currently only
	///  <code>ONCRPC_AUTH_NONE</code> is supported by this Java package.
	/// </summary>
	public enum OncRpcAuthType
	{
		/// <summary>
		///  No authentication scheme used for this remote procedure call.
		/// </summary>
		None = 0,
		/// <summary>
		///  The so-called "Unix" authentication scheme is not supported. This one
		///  only sends the users id as well as her/his group identifiers, so this
		///  is simply far too weak to use in typical situations where
		///  authentication is requested.
		/// </summary>
		Unix = 1,
		/// <summary>
		///  The so-called "short hand Unix style" is not supported.
		/// </summary>
		Short = 2,
		/// <summary>
		///  The DES authentication scheme (using encrypted time stamps) is not
		///  supported -- and besides, it's not a silver bullet either.
		/// </summary>
		AuthDES = 3
	}

	/// <summary>
	///  A collection of constants used for ONC/RPC messages to identify the
	///  type of message. Currently, ONC/RPC messages can be either calls or
	///  replies. Calls are sent by ONC/RPC clients to servers to call a remote
	///  procedure (for you "ohohpies" that can be translated into the buzzword
	///  "method"). A server then will answer with a corresponding reply message
	///  (but not in the case of batched calls).
	/// </summary>
	public enum OncRpcMessageType
	{
		NoValue = -1, // Only used for initialization
		
		/// <summary>
		///  Identifies an ONC/RPC call. By a "call" a client request that a server
		///  carries out a particular remote procedure.
		/// </summary>
		Call = 0,

		/// <summary>
		///  Identifies an ONC/RPC reply. A server responds with a "reply" after
		///  a client has sent a "call" for a particular remote procedure, sending
		///  back the results of calling that procedure.
		/// </summary>
		Reply
	}

	/// <summary>
	///  A collection of protocol constants used by the ONC/RPC package. Each
	///  constant defines one of the possible transport protocols, which can be
	///  used for communication between ONC/RPC clients and servers.
	/// </summary>
	public enum OncRpcProtocols
	{
		Udp = 17,
		Tcp = 6,

		/// <summary>
		///  Use the HTTP application protocol for tunneling ONC calls.
		/// </summary>
		Http = -42
	}

	/// <summary>
	///  A collection of constants used to describe why a remote procedure call
	///  message was rejected. This constants are used in {@link OncRpcReplyMessage}
	///  objects, which represent rejected messages if their
	///  {@link OncRpcReplyMessage#replyStatus} field has the value
	///  {@link OncRpcReplyStatus#ONCRPC_MSG_DENIED}.
	/// </summary>
	public enum OncRpcRejectStatus
	{
		Unknown = -1,
		
		/// <summary>
		/// Wrong ONC/RPC protocol version used in call (it needs to be version 2).
		/// </summary>
		RpcMismatch = 0,

		/// <summary>
		/// The remote ONC/RPC server could not authenticate the caller.
		/// </summary>
		AuthenticationError
	}

	/// <summary>
	///  A collection of constants used to identify the (overall) status of an
	///  ONC/RPC reply message. 
	/// </summary>
	public enum OncRpcReplyStatus
	{
		MsgAccepted = 0,
		MsgDenied
	}

}
