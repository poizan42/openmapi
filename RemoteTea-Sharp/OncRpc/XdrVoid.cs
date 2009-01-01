//
// openmapi.org - CompactTeaSharp - XdrVoid.cs
//
// C# port Copyright 2008 by Topalis AG
//
// Author: Johannes Roith
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

namespace CompactTeaSharp
{
	/// <summary>
	///  Instances of the class XdrVoid represent (de-)serializeable
	///  voids, which are especially useful in cases where no result is expected
	///  from a remote function call or no parameters are supplied.
	/// </summary>
	public class XdrVoid: IXdrAble
	{

		public void XdrEncode (XdrEncodingStream xdr)
		{
			// do nothing
		}

		public void XdrDecode (XdrDecodingStream xdr)
		{
			// do nothing
		}

		// Static instance, which can be used in cases where no data is to be 
		// serialized or deserialized but some ONC/RPC
		// function expects a reference to a XDR-able object.
		public static XdrVoid XDR_VOID = new XdrVoid ();

	}
	
}
