//
// openmapi.org - CompactTeaSharp - OncRpcServerReplyMessage.cs
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
using System.Net;
using System.IO;
using CompactTeaSharp;

namespace CompactTeaSharp.Server
{

	/// <summary>
	///  Represents an ONC/RPC reply message as defined by ONC/RPC in RFC 1831. 
	///  Such messages are sent back by ONC/RPC to servers to clients and contain 
	///  (in case of real success) the result of a remote procedure call.
	///  
	///  This class and all its derived classes can be encoded only. They are
	///  not able to encode themselves, because they are used solely on the
	///  server side of an ONC/RPC connection.
	///  
	///  The decision to define only one single class for the accepted and
	///  rejected replies was driven by the motivation not to use polymorphism
	///  and thus have to upcast and downcast references all the time.
	/// </summary>
	public class OncRpcServerReplyMessage : OncRpcReplyMessage
	{
		private OncRpcServerAuth auth;
		
		/// <summary>
		///  Initializes a new <code>OncRpcReplyMessage</code> object and initializes
		///  its complete state from the given parameters.
		///  
		///  Note that depending on the reply, acceptance and rejectance status
		///  some parameters are unused and can be specified as UNUSED_PARAMETER.
		/// </summary>
		///  <param name="call">The ONC/RPC call this reply message corresponds to.</param>
		///  <param name="replyStatus The reply status (see OncRpcReplyStatus).</param>
		///  <param name="acceptStatus The acceptance state (see OncRpcAcceptStatus).</param>
		///  <param name="rejectStatus The rejectance state (see OncRpcRejectStatus).</param>
		///  <param name="lowVersion lowest supported version.</param>
		///  <param name="highVersion highest supported version.</param>
		///  <param name="authStatus The autentication state (see OncRpcAuthStatus).</param>
		public OncRpcServerReplyMessage (OncRpcServerCallMessage call,
			OncRpcReplyStatus replyStatus, OncRpcAcceptStatus acceptStatus, 
			OncRpcRejectStatus rejectStatus, int lowVersion, int highVersion, 
			OncRpcAuthStatus authStatus) : base (call, replyStatus, acceptStatus, 
				rejectStatus, lowVersion, highVersion, authStatus)
		{
			this.auth = call.Auth;
		}

		/// <summary>
		///  Encodes an ONC/RPC reply header object into a XDR stream.
		/// </summary>
		// throws OncRpcException, IOException
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt (MessageId);
			xdr.XdrEncodeInt ((int) MessageType);
			xdr.XdrEncodeInt ((int) ReplyStatus);
			switch (ReplyStatus) {
				case OncRpcReplyStatus.MsgAccepted: RespondToAccepted (xdr); break;
				case OncRpcReplyStatus.MsgDenied: RespondToDenied (xdr); break;
			}
		}
		
		private void RespondToAccepted (XdrEncodingStream xdr)
		{
			// First encode the authentification data. If someone has
			// nulled (nuked?) the authentication protocol handling object
			// from the call information object, then we can still fall back
			// to sending AUTH_NONE replies...
			//
			if (auth != null)
				auth.XdrEncodeVerf (xdr);
			else {
				xdr.XdrEncodeInt ((int) OncRpcAuthType.None);
				xdr.XdrEncodeInt (0);
			}
			//
			// Even if the call was accepted by the server, it can still
			// indicate an error. Depending on the status of the accepted
			// call we have to send back an indication about the range of
			// versions we support of a particular program (server).
			//
			xdr.XdrEncodeInt ((int) AcceptStatus);
			switch (AcceptStatus) {
				case OncRpcAcceptStatus.ProgMismatch:
					xdr.XdrEncodeInt (LowVersion);
					xdr.XdrEncodeInt (HighVersion);
				break;
				default:
					//
					// Otherwise "open ended set of problem", like the author
					// of Sun's ONC/RPC source once wrote...
					//
				break;
			}
		}

		private void RespondToDenied (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt ((int) RejectStatus);
			switch (RejectStatus) {
				case OncRpcRejectStatus.RpcMismatch:
					xdr.XdrEncodeInt (LowVersion);
					xdr.XdrEncodeInt (HighVersion);
				break;
				case OncRpcRejectStatus.AuthenticationError:
					xdr.XdrEncodeInt ((int) AuthStatus);
				break;
			}	
		}

	}

}
