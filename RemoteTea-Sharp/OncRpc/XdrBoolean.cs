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
	/**
	 * Instances of the class <code>XdrBoolean</code> represent (de-)serializeable
	 * booleans, which are especially useful in cases where a result with only a
	 * single boolean is expected from a remote function call or only a single
	 * boolean parameter needs to be supplied.
	 *
	 * <p>Please note that this class is somewhat modelled after Java's primitive
	 * data type wrappers. As for these classes, the XDR data type wrapper classes
	 * follow the concept of values with no identity, so you are not allowed to
	 * change the value after you've created a value object.
	 *
	 * @version $Revision: 1.1.1.1 $ $Date: 2003/08/13 12:03:43 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 */
	public class XdrBoolean: XdrAble
	{
	    /**
	     * Constructs and initializes a new <code>XdrBoolean</code> object.
	     *
	     * @param value Boolean value.
	     */
	    public XdrBoolean(Boolean value) {
	        this.value = value;
	    }
	
	    /**
	     * Constructs and initializes a new <code>XdrBoolean</code> object.
	     */
	    public XdrBoolean() {
	        this.value = false;
	    }
	
	    /**
	     * Returns the value of this <code>XdrBoolean</code> object as a boolean
	     * primitive.
	     *
	     * @return  The primitive <code>boolean</code> value of this object.
	     */
	    public Boolean BooleanValue() {
	        return this.value;
	    }
	
	    /**
	     * Encodes -- that is: serializes -- a XDR boolean into a XDR stream in
	     * compliance to RFC 1832.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncode(XdrEncodingStream xdr)
	    {
	        xdr.XdrEncodeBoolean(value);
	    }
	
	    /**
	     * Decodes -- that is: deserializes -- a XDR boolean from a XDR stream in
	     * compliance to RFC 1832.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrDecode(XdrDecodingStream xdr)
	    {
	        value = xdr.XdrDecodeBoolean();
	    }
	
	    /**
	     * The encapsulated boolean value itself.
	     */
	    private Boolean value;
	}
}
