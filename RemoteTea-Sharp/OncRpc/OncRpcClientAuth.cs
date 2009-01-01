//
// openmapi.org - CompactTeaSharp - OnRpcClientAuth.cs
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
	/// The base class for handling all protocol issues of ONC/RPC 
	/// authentication on the client side. It defines the contract for the 
	/// behaviour of derived classes with respect to protocol handling issues.
	/// 
	/// Authentication on the client side can be done as follows:
	///
	/// <code>
	/// var auth = new OncRpcClientAuthUnix ("marvin@ford.prefect", 42, 1001, new int [0]);
	/// client.SetAuth (auth);
	/// </code>
	/// 
	/// The OncRpcClientAuthUnix authentication AUTH_UNIX will handle shorthand 
	/// credentials (of type AUTH_SHORT) transparently.
	/// If you do not set any authentication object after creating an ONC/RPC client
	/// object, <code>AUTH_NONE</code> is used automatically.
	/// </summary>
	public abstract class OncRpcClientAuth
	{
		/// <summary>
		///  Returns true if the authentication credential can be refreshed.
		/// </summary>
		internal abstract bool CanRefreshCred { get; }
		
		/// <summary>
		///  Encodes ONC/RPC authentication information as a credential and a 
		///  verifier when sending an ONC/RPC call message.
		/// </summary>
		/// <param name="xdr">XDR stream where to encode the credential and the verifier to.</param>
		internal abstract void XdrEncodeCredVerf (XdrEncodingStream xdr);

		/// <summary>
		///  Decodes ONC/RPC authentication information as a verifier when 
		///  receiving an ONC/RPC reply message. 
		/// </summary>
		/// <param name="xdr">XDR stream from which to receive the verifier 
		///   sent together with an ONC/RPC reply message.</param>
		///
		/// throws OncRpcAuthenticationException if the received verifier is not kosher.
		internal abstract void XdrDecodeVerf (XdrDecodingStream xdr);
		
	}
	
}
