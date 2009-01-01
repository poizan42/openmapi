//
// openmapi.org - CompactTeaSharp - OncRpcServerAuthShort.cs
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
	///   Handles all protocol issues of the ONC/RPC authentication AUTH_SHORT on the server side.
	/// </summary>
	public class OncRpcServerAuthShort : OncRpcServerAuth
	{
		private byte [] shorthandCred;
		private byte [] shorthandVerf;
	

		/// <summary>
		///   Constructs an <code>OncRpcServerAuthShort</code> object and pulls its
		///   state off an XDR stream.
		/// </summary>
		/// <param xdr XDR stream to retrieve the object state from.
		// throws OncRpcException, IOException
		public OncRpcServerAuthShort (XdrDecodingStream xdr)
		{
			XdrDecodeCredVerf (xdr);
		}

		/// <summary>
		///   Returns the type (flavor) of OncRpcAuthType authentication used.
		/// </summary>
		public override OncRpcAuthType GetAuthenticationType ()
		{
			return OncRpcAuthType.Short;
		}

		/// <summary>
		///   Returns the shorthand credential sent by the caller.
		/// </summary>
		public byte [] GetShorthandCred ()
		{
			return shorthandCred;
		}

		/// <summary>
		///   Sets shorthand verifier to be sent back to the caller. The caller then
		///   can use this shorthand verifier as the new credential with the next
		///   ONC/RPC calls. If you don't set the verifier or set it to null, then 
		///   the verifier returned to the caller will be of type AUTH_NONE.
		/// <summary>
		public byte [] ShorthandVerifier {
			get {
				return shorthandVerf;
			}
			set {
				this.shorthandVerf = value;
			}
		}

		/// <summary>
		///   Decodes an ONC/RPC authentication object
		///   (credential & verifier) on the server side.
		/// </summary>
		// throws OncRpcException, IOException
		public override void XdrDecodeCredVerf (XdrDecodingStream xdr)
		{
			//
			// Reset the authentication object's state properly...
			//
			shorthandCred = null;
			shorthandVerf = null;
			//
			// Pull off the shorthand credential information (opaque date) of
			// the XDR stream...
			//
			shorthandCred = xdr.XdrDecodeDynamicOpaque ();
			if ( shorthandCred.Length >
				OncRpcAuthConstants.ONCRPC_MAX_AUTH_BYTES) {
					throw new OncRpcAuthenticationException (
						OncRpcAuthStatus.BadCred);
			}
			//
			// We also need to decode the verifier. This must be of type
			// AUTH_NONE too. For some obscure historical reasons, we have to
			// deal with credentials and verifiers, although they belong together,
			// according to Sun's specification.
			//
			if ( (xdr.XdrDecodeInt () != (int) OncRpcAuthType.None) ||
				(xdr.XdrDecodeInt() != 0) ) {
					throw new OncRpcAuthenticationException(
						OncRpcAuthStatus.BadVerifier);
			}
		}

		/// <summary>
		///   Encodes an ONC/RPC authentication object (its verifier) on the server side.
		/// </summary>
		// throws OncRpcException, IOException
		public override void XdrEncodeVerf (XdrEncodingStream xdr)
		{
			if (shorthandVerf != null) {
				//
				// Encode AUTH_SHORT shorthand verifier (credential).
				//
				xdr.XdrEncodeInt ((int) OncRpcAuthType.Short);
				xdr.XdrEncodeDynamicOpaque (shorthandVerf);
			} else {
				//
				// Encode an AUTH_NONE verifier with zero length, if no shorthand
				// verifier (credential) has been supplied by now.
				//
				xdr.XdrEncodeInt ((int) OncRpcAuthType.None);
				xdr.XdrEncodeInt (0);
			}
		}

	}

}
