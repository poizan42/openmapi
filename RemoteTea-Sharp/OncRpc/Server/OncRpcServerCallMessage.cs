//
// openmapi.org - CompactTeaSharp - OncRpcServerCallMessage.cs
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
	///  Represents an ONC/RPC call message on the server side. For this reasons 
	///  it just handles decoding of call messages but can not do any encoding. 
	///  This class is also responsible for pulling off authentication information 
	///  from the wire and converting it into appropriate authentication protocol 
	///  handling objects. As with all good management, this class therefor delegates 
	///  this to the server-side authentication protocol handling classes.
	/// </summary>
	public class OncRpcServerCallMessage : OncRpcCallMessage
	{
		private OncRpcServerAuth auth;
	
		/// <summary>
		///  Contains the authentication protocol handling object retrieved together
		///  with the call message itself.
		/// </summary>
		public OncRpcServerAuth Auth {
			get { return auth; }
			set { auth = value; }
		}

		/// <summary>
		///  Decodes a ONC/RPC message header object from a XDR stream according to RFC 1831.
		/// </summary>
		///  <param name="xdr">A decoding XDR stream from which to receive all the mess.</param>
		// throws OncRpcException, IOException
		public void XdrDecode (XdrDecodingStream xdr)
		{
			MessageId = xdr.XdrDecodeInt ();

			// Ensure that we are really decoding an ONC/RPC message call header.
			MessageType = (OncRpcMessageType) xdr.XdrDecodeInt ();
			if (MessageType != OncRpcMessageType.Call)
				throw new OncRpcException (OncRpcException.WRONG_MESSAGE);

			// Make sure that the other side is talking version 2 of ONC/RPC.
			OncRpcVersion = xdr.XdrDecodeInt ();
			if (OncRpcVersion != ONCRPC_VERSION)
				throw new OncRpcException (OncRpcException.VERS_MISMATCH);

			// Decode the remaining fields of the call header.
			Program = xdr.XdrDecodeInt ();
			Version = xdr.XdrDecodeInt ();
			Procedure = xdr.XdrDecodeInt ();

			//
			// Last comes the authentication data. Note that the "factory" hidden
			// within XdrNew () will graciously recycle any old authentication
			// protocol handling object if it is of the same authentication type
			// as the new one just coming in from the XDR wire.
			//
			auth = OncRpcServerAuth.XdrNew (xdr, auth);
		}
		
	}

}
