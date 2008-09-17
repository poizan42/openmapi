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
	/// The <code>OncRpcClientAuth</code> class is the base class for handling
	/// all protocol issues of ONC/RPC authentication on the client side. As it
	/// stands, it does not do very much with the exception of defining the contract
	/// for the behaviour of derived classes with respect to protocol handling
	/// issues.
	/// 
	/// <p>Authentication on the client side can be done as follows: just
	/// create an authentication object and hand it over to the ONC/RPC client
	/// object.
	/// 
	/// <pre>
	/// OncRpcClientAuth auth = new OncRpcClientAuthUnix(
	///                                 "marvin@ford.prefect",
	///                                 42, 1001, new int[0]);
	/// client.setAuth(auth);
	/// </pre>
	/// 
	/// The {@link OncRpcClientAuthUnix authentication <code>AUTH_UNIX</code>} will
	/// handle shorthand credentials (of type <code>AUTH_SHORT</code> transparently).
	/// If you do not set any authentication object after creating an ONC/RPC client
	/// object, <code>AUTH_NONE</code> is used automatically.
	/// </summary>
	public abstract class OncRpcClientAuth
	{
		/**
		* Encodes ONC/RPC authentication information in form of a credential
		* and a verifier when sending an ONC/RPC call message.
		*
		* @param xdr XDR stream where to encode the credential and the verifier
		*   to.
		*
		* @throws OncRpcException if an ONC/RPC error occurs.
		* @throws IOException if an I/O error occurs.
		*/
		internal abstract void XdrEncodeCredVerf (XdrEncodingStream xdr);

		/**
		* Decodes ONC/RPC authentication information in form of a verifier
		* when receiving an ONC/RPC reply message. 
		*
		* @param xdr XDR stream from which to receive the verifier sent together
		*   with an ONC/RPC reply message.
		*
		* @throws OncRpcAuthenticationException if the received verifier is
		*   not kosher.
		* @throws OncRpcException if an ONC/RPC error occurs.
		* @throws IOException if an I/O error occurs.
		*/
		internal abstract void XdrDecodeVerf (XdrDecodingStream xdr);
	
		/**
		* Indicates whether the ONC/RPC authentication credential can be
		* refreshed.
		*
		* @return true, if the credential can be refreshed
		*/
		internal abstract bool CanRefreshCred {
			get;
		}
		
	}
	
}
