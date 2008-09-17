/*
 * $Header: /cvsroot/remotetea/remotetea/src/org/acplt/oncrpc/server/OncRpcServerAuthShort.java,v 1.1.1.1 2003/08/13 12:03:51 haraldalbrecht Exp $
 *
 * Copyright (c) 1999, 2000
 * Lehrstuhl fuer Prozessleittechnik (PLT), RWTH Aachen
 * D-52064 Aachen, Germany.
 * All rights reserved.
 *
 * This library is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Library General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Library General Public License for more details.
 *
 * You should have received a copy of the GNU Library General Public
 * License along with this program (see the file COPYING.LIB for more
 * details); if not, write to the Free Software Foundation, Inc.,
 * 675 Mass Ave, Cambridge, MA 02139, USA.
 */



using System;
using System.Net;
using System.IO;
using RemoteTea.OncRpc;

namespace RemoteTea.OncRpc.Server
{

	/**
	 * The <code>OncRpcServerAuthShort</code> class handles all protocol issues
	 * of the ONC/RPC authentication <code>AUTH_SHORT</code> on the server
	 * side.
	 *
	 * @version $Revision: 1.1.1.1 $ $Date: 2003/08/13 12:03:51 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 */
	public class OncRpcServerAuthShort : OncRpcServerAuth
	{

		/**
		* Constructs an <code>OncRpcServerAuthShort</code> object and pulls its
		* state off an XDR stream.
		*
		* @param xdr XDR stream to retrieve the object state from.
		*
		* @throws OncRpcException if an ONC/RPC error occurs.
		* @throws IOException if an I/O error occurs.
		*/
		public OncRpcServerAuthShort(XdrDecodingStream xdr)
		{
			XdrDecodeCredVerf (xdr);
		}

		/**
		* Returns the type (flavor) of {@link OncRpcAuthType authentication}
		* used.
		*
		* @return Authentication type used by this authentication object.
		*/
		public override int GetAuthenticationType ()
		{
			return OncRpcAuthType.ONCRPC_AUTH_SHORT;
		}

		/**
		* Returns the shorthand credential sent by the caller.
		*/
		public byte [] GetShorthandCred ()
		{
			return shorthandCred;
		}

		/**
		* Sets shorthand verifier to be sent back to the caller. The caller then
		* can use this shorthand verifier as the new credential with the next
		* ONC/RPC calls. If you don't set the verifier or set it to
		* <code>null</code>, then the verifier returned to the caller will be
		* of type <code>AUTH_NONE</code>.
		*/
		public byte [] ShorthandVerifier {
			get {
				return shorthandVerf;
			}
			set {
				this.shorthandVerf = value;
			}
		}

		/**
		* Decodes -- that is: deserializes -- an ONC/RPC authentication object
		* (credential & verifier) on the server side.
		*
		* @throws OncRpcException if an ONC/RPC error occurs.
		* @throws IOException if an I/O error occurs.
		*/
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
				OncRpcAuthConstants.ONCRPC_MAX_AUTH_BYTES ) {
					throw new OncRpcAuthenticationException (
						OncRpcAuthStatus.ONCRPC_AUTH_BADCRED);
			}
			//
			// We also need to decode the verifier. This must be of type
			// AUTH_NONE too. For some obscure historical reasons, we have to
			// deal with credentials and verifiers, although they belong together,
			// according to Sun's specification.
			//
			if ( (xdr.XdrDecodeInt() != OncRpcAuthType.ONCRPC_AUTH_NONE) ||
				(xdr.XdrDecodeInt() != 0) ) {
					throw new OncRpcAuthenticationException(
						OncRpcAuthStatus.ONCRPC_AUTH_BADVERF);
			}
		}

		/**
		* Encodes -- that is: serializes -- an ONC/RPC authentication object
		* (its verifier) on the server side.
		*
		* @throws OncRpcException if an ONC/RPC error occurs.
		* @throws IOException if an I/O error occurs.
		*/
		public override void XdrEncodeVerf (XdrEncodingStream xdr)
		{
			if (shorthandVerf != null) {
				//
				// Encode AUTH_SHORT shorthand verifier (credential).
				//
				xdr.XdrEncodeInt (OncRpcAuthType.ONCRPC_AUTH_SHORT);
				xdr.XdrEncodeDynamicOpaque (shorthandVerf);
			} else {
				//
				// Encode an AUTH_NONE verifier with zero length, if no shorthand
				// verifier (credential) has been supplied by now.
				//
				xdr.XdrEncodeInt (OncRpcAuthType.ONCRPC_AUTH_NONE);
				xdr.XdrEncodeInt (0);
			}
		}

		/**
		* Contains the shorthand credential sent by the caller.
		*/
		private byte [] shorthandCred;

		/**
		* Contains the shorthand authentication verifier (credential) to return
		* to the caller to be used with the next ONC/RPC calls.
		*/
		private byte [] shorthandVerf;

	}

}
