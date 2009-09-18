//
// openmapi.org - CompactTeaSharp - OnRpcClientCallMessage.cs
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
	///  Represents a remote procedure call message on the client side.
	/// </summary>
	public class OncRpcClientCallMessage: OncRpcCallMessage
	{
		/// <summary>
		///  Client-side authentication protocol handling object to use when
		///  decoding the reply message.
		/// </summary>
		protected OncRpcClientAuth auth;
			
		/// <summary>
		///  Constructs and initialises a new ONC/RPC call message header.
		/// </summary>
		/// <param name="messageId">An identifier choosen by an ONC/RPC client to uniquely
		///    identify matching call and reply messages.</param>
		/// <param name="program"> Program number of the remote procedure to call.</param>
		/// <param name="version"> Program version number of the remote procedure to call.</param>
		/// <param name="procedure"> Procedure number (identifier) of the procedure to call.</param>
		/// <param name="auth"> Authentication protocol handling object.</param>
		public OncRpcClientCallMessage (int messageId, int program, int version, 
			int procedure, OncRpcClientAuth auth) : 
			base (messageId, program, version, procedure)
		{
			this.auth = auth;
		}

		/// <summary>
		///  Encodes an ONC/RPC message header object into a XDR stream according to RFC 1831.
		/// </summary>
		/// <param name="xdr">An encoding XDR stream where to put the mess in.</param>
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt (MessageId);
			xdr.XdrEncodeInt ((int) MessageType);
			xdr.XdrEncodeInt (OncRpcVersion);
			xdr.XdrEncodeInt (Program);
			xdr.XdrEncodeInt (Version);
			xdr.XdrEncodeInt (Procedure);
		
			//
			// Now encode the authentication data. If we have an authentication
			// protocol handling object at hand, then we let do the dirty work
			// for us. Otherwise, we fall back to AUTH_NONE handling.
			//
			if (auth != null)
				auth.XdrEncodeCredVerf (xdr);
			else {
				xdr.XdrEncodeInt ((int) OncRpcAuthType.None);
				xdr.XdrEncodeInt (0);
				xdr.XdrEncodeInt ((int) OncRpcAuthType.None);
				xdr.XdrEncodeInt (0);
			}
		}
	}
}
