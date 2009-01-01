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

namespace CompactTeaSharp
{
	/// <summary>
	///  The OncRpcReplyMessage class represents an ONC/RPC reply message as 
	///  defined by ONC/RPC in RFC 1831. Such messages are sent back by
	///  ONC/RPC to servers to clients and contain (in case of real success) the
	///  result of a remote procedure call.
	///
	///  The decision to define only one single class for the accepted and
	///  rejected replies was driven by the motivation not to use polymorphism
	///  and thus have to upcast and downcast references all the time.
	///
	///  The derived classes are only provided for convinience on the server side.
	/// </summary>
	public abstract class OncRpcReplyMessage : OncRpcMessage
	{
		/// <summary>
		///  Dummy, which can be used to identify unused parameters when constructing
		///  OncRpcReplyMessage objects.
		/// </summary>
		public const int UNUSED_PARAMETER = 0;

		/// <summary>
		///  The reply status of the reply message. This can be either
		///  OncRpcReplyStatus.ONCRPC_MSG_ACCEPTED or OncRpcReplyStatus.ONCRPC_MSG_DENIED. 
		///  Depending on the value of this field, other fields of an instance of
		///  OncRpcReplyMessage become important.
		/// </summary>
		public OncRpcReplyStatus ReplyStatus { get; set; }

		/// <summary>
		///  Acceptance status in case this reply was sent in response to an
		///  accepted call (OncRpcReplyStatus.ONCRPC_MSG_ACCEPTED). This
		///  field can take any of the values defined in the
		///  link OncRpcAcceptStatus} interface.
		///  
		///  Note that even for accepted calls that only in the case of
		///  link OncRpcAcceptStatus.ONCRPC_SUCCESS result data will follow
		///  the reply message header.
		/// </summary>
		public OncRpcAcceptStatus AcceptStatus { get; set; }

		/// <summary>
		///  Rejectance status in case this reply sent in response to a
		///  rejected call (OncRpcReplyStatus.MsgDenied). This field can take 
		///  any of the values defined in the OncRpcRejectStatus interface.
		/// </summary>
		public OncRpcRejectStatus RejectStatus { get; set; }

		/// <summary>
		///  Lowest supported version in case of
		///  OncRpcRejectStatus.ONCRPC_RPC_MISMATCH and
		///  OncRpcAcceptStatus.ONCRPC_PROG_MISMATCH.
		/// </summary>
		public int LowVersion { get; set; }
		
		/// <summary>
		///  Highest supported version in case of
		///  OncRpcRejectStatus.ONCRPC_RPC_MISMATCH and
		///  OncRpcAcceptStatus.ONCRPC_PROG_MISMATCH.
		/// </summary>
		public int HighVersion { get; set; }

		/// <summary>
		///  Contains the reason for authentification failure in the case
		///  of OncRpcRejectStatus.ONCRPC_AUTH_ERROR.
		/// </summary>
		public OncRpcAuthStatus AuthStatus { get; set; }

		/// <summary>
		///  Initializes a new <code>OncRpcReplyMessage</code> object to represent
		///  an invalid state. This default constructor should only be used if in the
		///  next step the real state of the reply message is immediately decoded
		///  from a XDR stream.
		/// </summary>
		protected OncRpcReplyMessage () : base (0)
		{
			this.MessageType  = OncRpcMessageType.Reply;
			this.ReplyStatus  = OncRpcReplyStatus.MsgAccepted;
			this.AcceptStatus = OncRpcAcceptStatus.SystemErr;
			this.RejectStatus = UNUSED_PARAMETER;
			this.AuthStatus   = UNUSED_PARAMETER;
		}

		/// <summary>
		///  Initializes a new <code>OncRpcReplyMessage</code> object and initializes
		///  its complete state from the given parameters.
		///  Note that depending on the reply, acceptance and rejectance status
		///  some parameters are unused and can be specified as UNUSED_PARAMETER.
		/// </summary>
		/// <param name="call">The ONC/RPC call this reply message corresponds to.</param>
		/// <param name="lowVersion">Lowest supported version.</param>
		/// <param name="highVersion">Highest supported version.</param>
		/// <param name="authStatus">The autentication state (see {@link OncRpcAuthStatus}).</param>
		protected OncRpcReplyMessage (OncRpcCallMessage call, OncRpcReplyStatus replyStatus, 
			OncRpcAcceptStatus acceptStatus, OncRpcRejectStatus rejectStatus, int lowVersion, 
			int highVersion, OncRpcAuthStatus authStatus) : base (call.MessageId)
		{
			this.MessageType  = OncRpcMessageType.Reply;
			this.ReplyStatus  = replyStatus;
			this.AcceptStatus = acceptStatus;
			this.RejectStatus = rejectStatus;
			this.LowVersion   = lowVersion;
			this.HighVersion  = highVersion;
			this.AuthStatus   = authStatus;
		}
	}
}
