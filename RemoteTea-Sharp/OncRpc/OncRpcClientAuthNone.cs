//
// openmapi.org - CompactTeaSharp - OnRpcClientAuthNone.cs
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
using System.IO;
using System.Net;

namespace CompactTeaSharp
{
	/// <summary>
	///  The OncRpcClientAuthNone class handles protocol issues 
	///  of ONC/RPC AUTH_NONE authentication.
	/// </summary>
	public class OncRpcClientAuthNone : OncRpcClientAuth
	{
		/// <summary>
		///  Contains a singleton which comes in handy if you just need an
		///  AUTH_NONE authentification for an ONC/RPC client.
		/// </summary>
		public static readonly OncRpcClientAuthNone AUTH_NONE = new OncRpcClientAuthNone ();

		internal override bool CanRefreshCred {
			get { return false; }
		}
		
		internal override void XdrEncodeCredVerf (XdrEncodingStream xdr)
		{
			//
			// The credential only consists of the indication of AUTH_NONE with
			// no opaque authentication data following.
			//
			xdr.XdrEncodeInt ((int) OncRpcAuthType.None);
			xdr.XdrEncodeInt (0);
			//
			// But we also need to encode the verifier. 
			// This is always of type AUTH_NONE too.
			//
			xdr.XdrEncodeInt ((int) OncRpcAuthType.None);
			xdr.XdrEncodeInt (0);
		}

		internal override void XdrDecodeVerf (XdrDecodingStream xdr)
		{
			//
			// Make sure that we received a AUTH_NONE verifier and that it
			// does not contain any opaque data. Anything different from this
			// is not kosher and an authentication exception will be thrown.
			//
			if ( (xdr.XdrDecodeInt () != (int) OncRpcAuthType.None) ||
				(xdr.XdrDecodeInt () != 0) ) {
					throw new OncRpcAuthenticationException (OncRpcAuthStatus.Failed);
			}
		}

	}

}
