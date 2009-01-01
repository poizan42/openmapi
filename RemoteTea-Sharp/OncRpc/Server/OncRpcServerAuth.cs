//
// openmapi.org - CompactTeaSharp - OncRpcServerAuth.cs
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
	///  Base class and factory for handling all protocol issues of ONC/RPC 
	///  authentication on the server side.
	/// </summary>
	public abstract class OncRpcServerAuth
	{
		/// <summary>
		///  Returns the type (flavor) of {@link OncRpcAuthType authentication}
		///  used by this authentication object.
		/// </summary>
		public abstract OncRpcAuthType GetAuthenticationType ();

		/// <summary>
		///   Restores (deserializes) an authentication object from an XDR stream.
		/// </summary>
		/// <param name="xdr">XDR stream from which the authentication object is restored.</param>
		/// <param name="recycle">Old authtentication object which is intended to be
		///     reused in case it is of the same authentication type as the new
		///     one just arriving from the XDR stream.</param>
		/// <return>Authentication information encapsulated in an object, whose class
		///   is derived from OncRpcServerAuth</return>
		// throws OncRpcException, IOException
		public static OncRpcServerAuth XdrNew (XdrDecodingStream xdr,
			OncRpcServerAuth recycle)
		{

			OncRpcServerAuth auth;
			//
			// In case we got an old authentication object and we are just about
			// to receive an authentication with the same type, we reuse the old
			// object.
			//
			OncRpcAuthType authType = (OncRpcAuthType) xdr.XdrDecodeInt ();
			if ( (recycle != null)
				&& (recycle.GetAuthenticationType () == authType) )
			{
				//
				// Simply recycle authentication object and pull its new state
				// of the XDR stream.
				//
				auth = recycle;
				auth.XdrDecodeCredVerf (xdr);
			} else {
				//
				// Create a new authentication object and pull its state off
				// the XDR stream.
				//
				switch (authType) {
					case OncRpcAuthType.None:
						auth = new OncRpcServerAuthNone ();
						auth.XdrDecodeCredVerf (xdr);
					break;
					case OncRpcAuthType.Short:
						auth = new OncRpcServerAuthShort (xdr);
					break;
					case OncRpcAuthType.Unix:
						auth = new OncRpcServerAuthUnix (xdr);
					break;
					default:
						//
						// In case of an unknown or unsupported type, throw an exception.
						// Note: using AUTH_REJECTEDCRED is in sync with the way Sun's
						// ONC/RPC implementation does it.
						//
						throw new OncRpcAuthenticationException (
							OncRpcAuthStatus.RejectedCred);
				}
			}
			return auth;
		}

		/// <summary>
		///  Decodes an ONC/RPC authentication object (credential & verifier) on the server side.
		/// </summary>
		// throws OncRpcException, IOException
		public abstract void XdrDecodeCredVerf (XdrDecodingStream xdr);

		/// <summary>
		///  Encodes an ONC/RPC authentication object (its verifier) on the server side.
		/// </summary>
		// throws OncRpcException, IOException
		public abstract void XdrEncodeVerf (XdrEncodingStream xdr);

	}

}
