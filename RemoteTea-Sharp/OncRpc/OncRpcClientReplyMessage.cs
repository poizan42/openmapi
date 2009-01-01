//
// openmapi.org - CompactTeaSharp - OnRpcClientReplyMessage.cs
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
	///  Represents an ONC/RPC reply message as defined by ONC/RPC in RFC 1831. 
	///  Such messages are sent back by ONC/RPC to servers to clients and contain 
	///  (in case of real success) the result of a remote procedure call.
	///
	///  The derived classes are only provided for convinience on the server side.
	/// </summary>
	public class OncRpcClientReplyMessage : OncRpcReplyMessage
	{
		/// <summary>
		///  Client-side authentication protocol handling object to use when
		///  decoding the reply message.
		/// </summary>
		protected OncRpcClientAuth auth;

		/// <summary>
		///  Initializes a new OncRpcReplyMessage object to represent
		///  an invalid state. This default constructor should only be used if in the
		///  next step the real state of the reply message is immediately decoded
		///  from a XDR stream.
		/// </summary>
		/// <param name="auth">Client-side authentication protocol handling object which
		///   is to be used when decoding the verifier data contained in the reply.</param>
		public OncRpcClientReplyMessage (OncRpcClientAuth auth): base ()
		{
			this.auth = auth;
		}

		/// <summary>
		///  Check whether this <code>OncRpcReplyMessage</code> represents an
		///  accepted and successfully executed remote procedure call.
		/// </summary>
		/// <return>True if rpc call was accepted and successfully executed.</return>
		public bool SuccessfullyAccepted ()
		{
			return (ReplyStatus == OncRpcReplyStatus.MsgAccepted)
				&& (AcceptStatus == OncRpcAcceptStatus.Success);
		}

		/// <summary>
		///  Return an appropriate exception object according to the state this
		///  reply message header object is in. The exception object then can be thrown.
		/// </summary>
		//// <return>Exception object of class OncRpcException or a subclass thereof.</return>
		public OncRpcException newException () 
		{
			switch (ReplyStatus) {
				case OncRpcReplyStatus.MsgAccepted:
					switch (AcceptStatus) {
						case OncRpcAcceptStatus.Success: return new OncRpcException (OncRpcException.SUCCESS);
						case OncRpcAcceptStatus.ProcedureUnavailable: return new OncRpcException (OncRpcException.PROC_UNAVAIL);
						case OncRpcAcceptStatus.ProgMismatch: return new OncRpcException (OncRpcException.PROG_VERS_MISMATCH);
						case OncRpcAcceptStatus.ProgUnavail: return new OncRpcException (OncRpcException.PROG_UNAVAIL);
						case OncRpcAcceptStatus.GarbageArgs: return new OncRpcException (OncRpcException.CANT_DECODE_ARGS);
						case OncRpcAcceptStatus.SystemErr: return new OncRpcException (OncRpcException.SYSTEM_ERROR);
					}
				break;
				case OncRpcReplyStatus.MsgDenied:
					switch (RejectStatus) {
						case OncRpcRejectStatus.AuthenticationError: return new OncRpcAuthenticationException (AuthStatus);
						case OncRpcRejectStatus.RpcMismatch: return new OncRpcException (OncRpcException.FAILED);
					}
				break;
			}
			return new OncRpcException ();
		}

		///
		/// Decodes a ONC/RPC message header object from an XDR stream.
		///
		public void XdrDecode (XdrDecodingStream xdr)
		{
			MessageId = xdr.XdrDecodeInt ();
			//
			// Make sure that we are really decoding an ONC/RPC message call
			// header. Otherwise, throw the appropriate OncRpcException exception.
			//
			MessageType = (OncRpcMessageType) xdr.XdrDecodeInt ();
			if (MessageType != OncRpcMessageType.Reply)
				throw new OncRpcException (OncRpcException.WRONG_MESSAGE);
			ReplyStatus = (OncRpcReplyStatus) xdr.XdrDecodeInt ();
			switch (ReplyStatus) {
				case OncRpcReplyStatus.MsgAccepted: DecodeMsgAccepted (xdr); break;
				case OncRpcReplyStatus.MsgDenied: DecodeMsgDenied (xdr); break;
			}
		}

		private void DecodeMsgAccepted (XdrDecodingStream xdr)
		{
			//
			// Decode the information returned for accepted message calls.
			// If we have an associated client-side authentication protocol
			// object, we use that. Otherwise we fall back to the default
			// handling of only the AUTH_NONE authentication.
			//
			if (auth != null)
				auth.XdrDecodeVerf (xdr);
			else {
				//
				// If we don't have a protocol handler and the server sent its
				// reply using another authentication scheme than AUTH_NONE, we
				// will throw an exception. Also we check that no-one is
				// actually sending opaque information within AUTH_NONE.
				//
				if (xdr.XdrDecodeInt () != (int) OncRpcAuthType.None)
					throw new OncRpcAuthenticationException (OncRpcAuthStatus.Failed);

				if (xdr.XdrDecodeInt () != 0)
					throw new OncRpcAuthenticationException (OncRpcAuthStatus.Failed);
			}
			//
			// Even if the call was accepted by the server, it can still
			// indicate an error. Depending on the status of the accepted
			// call we will receive an indication about the range of
			// versions a particular program (server) supports.
			//
			AcceptStatus = (OncRpcAcceptStatus) xdr.XdrDecodeInt ();
			if (AcceptStatus == OncRpcAcceptStatus.ProgMismatch) {
				LowVersion = xdr.XdrDecodeInt ();
				HighVersion = xdr.XdrDecodeInt ();
			} else {
				//
				// Otherwise "open ended set of problem", like the author
				// of Sun's ONC/RPC source once wrote...
				//
			}

		}

		private void DecodeMsgDenied (XdrDecodingStream xdr)
		{
			RejectStatus = (OncRpcRejectStatus) xdr.XdrDecodeInt ();
			switch (RejectStatus) {
				case OncRpcRejectStatus.RpcMismatch:
					LowVersion = xdr.XdrDecodeInt ();
					HighVersion = xdr.XdrDecodeInt ();
				break;
				case OncRpcRejectStatus.AuthenticationError:
					AuthStatus = (OncRpcAuthStatus) xdr.XdrDecodeInt ();
				break;
			}
		}
	}
}