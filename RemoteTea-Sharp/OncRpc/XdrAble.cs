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
	/// Defines the interface for all classes that should be able to be
	/// serialized into XDR streams, and deserialized or constructed from
	/// XDR streams.
	/// </summary>
	public interface XdrAble
	{
		
    /**
     * Encodes -- that is: serializes -- an object into a XDR stream in
     * compliance to RFC 1832.
     *
     * @param xdr XDR stream to which information is sent for encoding.
     *
     * @throws OncRpcException if an ONC/RPC error occurs.
     * @throws IOException if an I/O error occurs.
     */
    void XdrEncode(XdrEncodingStream xdr);
           //throws OncRpcException, IOException;

    /**
     * Decodes -- that is: deserializes -- an object from a XDR stream in
     * compliance to RFC 1832.
     *
     * @param xdr XDR stream from which decoded information is retrieved.
     *
     * @throws OncRpcException if an ONC/RPC error occurs.
     * @throws IOException if an I/O error occurs.
     */
    void XdrDecode(XdrDecodingStream xdr);
           //throws OncRpcException, IOException;
		
	}
}
