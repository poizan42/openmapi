//
// RemoteTea - OnRpcBroadcastEvent.cs
//
// C# port Copyright 2008 by Topalis AG
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
// Java source:
// $Header: /cvsroot/remotetea/remotetea/src/org/acplt/oncrpc/OncRpcClientAuthNone.java,v 1.1.1.1 2003/08/13 12:03:40 haraldalbrecht Exp $
// 


using System;
using System.IO;
using System.Net;

namespace RemoteTea.OncRpc
{

	/**
	 * The <code>OncRpcClientAuthNone</code> class handles protocol issues of
	 * ONC/RPC <code>AUTH_NONE</code> authentication.
	 *
	 * @version $Revision: 1.1.1.1 $ $Date: 2003/08/13 12:03:40 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 */
	public class OncRpcClientAuthNone : OncRpcClientAuth
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
		internal override void XdrEncodeCredVerf (XdrEncodingStream xdr)
		{
			//
			// The credential only consists of the indication of AUTH_NONE with
			// no opaque authentication data following.
			//
			xdr.XdrEncodeInt (OncRpcAuthType.ONCRPC_AUTH_NONE);
			xdr.XdrEncodeInt (0);
			//
			// But we also need to encode the verifier. This is always of type
			// AUTH_NONE too. For some obscure historical reasons, we have to
			// deal with credentials and verifiers, although they belong together,
			// according to Sun's specification.
			//
			xdr.XdrEncodeInt (OncRpcAuthType.ONCRPC_AUTH_NONE);
			xdr.XdrEncodeInt (0);
		}

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
		internal override void XdrDecodeVerf (XdrDecodingStream xdr)
		{
			//
			// Make sure that we received a AUTH_NONE verifier and that it
			// does not contain any opaque data. Anything different from this
			// is not kosher and an authentication exception will be thrown.
			//
			if ( (xdr.XdrDecodeInt () != OncRpcAuthType.ONCRPC_AUTH_NONE) ||
				(xdr.XdrDecodeInt () != 0) ) {
					throw new OncRpcAuthenticationException(
						OncRpcAuthStatus.ONCRPC_AUTH_FAILED);
			}
		}

		/**
		* Indicates whether the ONC/RPC authentication credential can be
		* refreshed.
		*
		* @return true, if the credential can be refreshed
		*/
		internal override bool CanRefreshCred {
			//
			// Nothing to do here, as AUTH_NONE doesn't know anything of
			// credential refreshing. How refreshing...
			//
			get { return false; }
		}

		/**
		* Contains a singleton which comes in handy if you just need an
		* AUTH_NONE authentification for an ONC/RPC client.
		*/
		public static readonly OncRpcClientAuthNone AUTH_NONE = new OncRpcClientAuthNone ();

	}

}
