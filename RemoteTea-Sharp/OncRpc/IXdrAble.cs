//
// RemoteTea - IXdrAble.cs
//
// Copyright 2008 by Topalis AG
//
// Author: Johannes Roith
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

namespace CompactTeaSharp
{
	/// <summary>
	///  Defines the interface for all classes that should be able to be
	///  serialized into and constructed from XDR streams.
	/// </summary>
	public interface IXdrAble : IXdrDecodeable, IXdrEncodeable
	{
	}
	
	/// <summary>
	///  Encoding subset of IXdrAble
	/// </summary>
	public interface IXdrEncodeable
	{
		/// <summary>
	 	///  Encodes an object into a XDR stream in compliance to RFC 1832.
		/// </summary>
		/// <param name="xdr">XDR stream to which information is sent for encoding.</param>
		void XdrEncode (XdrEncodingStream xdr);
	}
	
	/// <summary>
	///  Decoding subset of IXdrAble
	/// </summary>
	public interface IXdrDecodeable
	{
		/// <summary>
	 	///  Decodes an object into a XDR stream in compliance to RFC 1832.
		/// </summary>
		/// <param name="xdr">XDR stream from which decoded information is retrieved.</param>
		void XdrDecode (XdrDecodingStream xdr);
	}
	
}
