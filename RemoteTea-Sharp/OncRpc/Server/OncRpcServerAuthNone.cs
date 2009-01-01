//
// openmapi.org - CompactTeaSharp - OncRpcServerAuthNone.cs
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
	///  Handles all protocol issues of the ONC/RPC authentication 
	///  AUTH_NONE on the server side.
	/// <summary>
	public sealed class OncRpcServerAuthNone : OncRpcServerAuth
	{
		/// <summary>
		///  Singleton to use when an authentication object for AUTH_NONE is needed.
		/// <summary>
		public static readonly OncRpcServerAuthNone AUTH_NONE = new OncRpcServerAuthNone();
		
		/// <summary>
		///  Returns the type (flavor) of {@link OncRpcAuthType authentication}
	 	///  used by this authentication object.
		/// <summary>
		public override OncRpcAuthType GetAuthenticationType ()
		{
			return OncRpcAuthType.None;
		}

		/// <summary>
		///  Decodes an ONC/RPC authentication object (credential & verifier) 
		///  on the server side.
		/// </summary>
		// throws OncRpcException, IOException
		public override void XdrDecodeCredVerf (XdrDecodingStream xdr)
		{
			//
			// As the authentication type has already been pulled off the XDR
			// stream, we only need to make sure that really no opaque data follows.
			//
			if (xdr.XdrDecodeInt () != 0)
				throw new OncRpcAuthenticationException (OncRpcAuthStatus.BadCred);

			//
			// We also need to decode the verifier. This must be of type
			// AUTH_NONE too. For some obscure historical reasons, we have to
			// deal with credentials and verifiers, although they belong together,
			// according to Sun's specification.
			//
			
			if ((xdr.XdrDecodeInt () != (int) OncRpcAuthType.None) ||
				(xdr.XdrDecodeInt () != 0))
			{
				throw new OncRpcAuthenticationException (OncRpcAuthStatus.BadVerifier);
			}
		}

		/// <summary>
		//   Encodes an ONC/RPC authentication object (its verifier) on the server side.
		/// </summary>
		// throws OncRpcException, IOException
		public override void XdrEncodeVerf (XdrEncodingStream xdr)
		{
			//
			// Encode an AUTH_NONE verifier with zero length.
			//
			xdr.XdrEncodeInt ((int) OncRpcAuthType.None);
			xdr.XdrEncodeInt (0);
		}

	}

}
