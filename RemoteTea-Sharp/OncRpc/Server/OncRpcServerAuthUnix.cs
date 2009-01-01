//
// openmapi.org - CompactTeaSharp - OncRpcServerAuthUnix.cs
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

	/**
	 * Handles all protocol issues of the ONC/RPC authentication AUTH_UNIX on the server side.
	 */
	public sealed class OncRpcServerAuthUnix : OncRpcServerAuth
	{
		/// <summary>
		///  Contains the shorthand authentication verifier (credential) to return
		///  to the caller to be used with the next ONC/RPC calls.
		/// </summary>
		private byte [] shorthandVerf { get; set; }
		
		/// <summary>
		///  Contains timestamp as supplied through credential.
		/// </summary>
		public int stamp { get; set; }

		/// <summary>
		///  Contains the machine name of caller supplied through credential.
		/// </summary>
		public string machinename { get; set; }

		/// <summary>
		///  Contains the user ID of caller supplied through credential.
		/// </summary>
		public int uid { get; set; }

		/// <summary>
		///  Contains the group ID of caller supplied through credential.
		/// </summary>
		public int gid { get; set; }

		/// <summary>
		///  Contains a set of group IDs the caller belongs to, as supplied
		///  through credential.
		/// </summary>
		public int [] gids { get; set; }

		/// <summary>
		///  Constructs an OncRpcServerAuthUnix object and pulls its
		///  state off an XDR stream.
		/// </summary>
		/// <param name="xdr">XDR stream to retrieve the object state from.</param>
		// throws OncRpcException, IOException
		public OncRpcServerAuthUnix (XdrDecodingStream xdr)
		{
			XdrDecodeCredVerf (xdr);
		}

		/// <summary>
		///  Returns the type (flavor) of OncRpcAuthType authentication used.
		/// </summary>
		/// <return>Authentication type used by this auth object.</return>
		public override OncRpcAuthType GetAuthenticationType () {
			return OncRpcAuthType.Unix;
		}

		/// <summary>
		///  Sets shorthand verifier to be sent back to the caller. The caller then
		///  can use this shorthand verifier as the new credential with the next
		///  ONC/RPC calls to speed up things up (hopefully).
		/// </summary>
		public byte [] ShorthandVerifier {
			get { return shorthandVerf; }
			set {
				this.shorthandVerf = value;
			}
		}

		/// <summary>
		///  Decodes an ONC/RPC authentication object
		///  (credential & verifier) on the server side.
		/// </summary>
		// throws OncRpcException, IOException
		public override void XdrDecodeCredVerf (XdrDecodingStream xdr)
		{
			//
			// Reset some part of the object's state...
			//
			shorthandVerf = null;
			//
			// Now pull off the object state of the XDR stream...
			//
			int realLen = xdr.XdrDecodeInt();
			stamp = xdr.XdrDecodeInt();
			machinename = xdr.XdrDecodeString();
			uid = xdr.XdrDecodeInt();
			gid = xdr.XdrDecodeInt();
			gids = xdr.XdrDecodeIntVector();
			//
			// Make sure that the indicated length of the opaque data is kosher.
			// If not, throw an exception, as there is something strange going on!
			//
			int len =   4 // length of stamp
				+ ((machinename.Length + 7) & ~3) // len string incl. len
				+ 4 // length of uid
				+ 4 // length of gid
				+ (gids.Length * 4) + 4 // length of vector of gids incl. len
				;
			if (realLen != len) {
				if (realLen < len)
					throw new OncRpcException (OncRpcException.BUFFER_UNDERFLOW);
				else
					throw new OncRpcException (OncRpcException.AUTH_ERROR);
			}
			//
			// We also need to decode the verifier. This must be of type
			// AUTH_NONE too. For some obscure historical reasons, we have to
			// deal with credentials and verifiers, although they belong together,
			// according to Sun's specification.
			//
			if ( (xdr.XdrDecodeInt() != (int) OncRpcAuthType.None) ||
				(xdr.XdrDecodeInt() != 0) ) {
					throw new OncRpcAuthenticationException (OncRpcAuthStatus.BadVerifier);
			}
		}

		/// <summary>
		///  Encodes an ONC/RPC authentication object (its verifier) on the server side.
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
