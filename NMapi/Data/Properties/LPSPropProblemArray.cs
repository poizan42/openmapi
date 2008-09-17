//
// openmapi.org - NMapi C# Mapi API - LPSPropProblemArray.cs
//
// Copyright 2008 VipCom AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System;
using System.IO;

using RemoteTea.OncRpc;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Interop {

	/// <summary>
	///  For internal use only.
	/// </summary>
	public sealed class LPSPropProblemArray : XdrAble
	{	
		public SPropProblemArray value;

		/// <summary>
		///
		/// </summary>
		public SPropProblemArray Value {
			get { return value; }
			set { this.value = value; }
		}

		/// <summary>
		///
		/// </summary>
		public LPSPropProblemArray() {
			value = null;
		}

		/// <summary>
		///
		/// </summary>
		public LPSPropProblemArray (SPropProblemArray value)
		{
			this.value = value;
		}

		/// <summary>
		///
		/// </summary>
		public LPSPropProblemArray (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode(XdrEncodingStream xdr)
		{
			if (value == null)
				xdr.XdrEncodeInt (-1);
			else {
				xdr.XdrEncodeInt (value.AProblem.Length);
				for (int idx = 0; idx < value.AProblem.Length; idx++)
					value.AProblem [idx].XdrEncode (xdr);
			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			int len = xdr.XdrDecodeInt ();
			if (len == ~0)
				value = null;
			else {
				value = new SPropProblemArray ();
				value.AProblem = new SPropProblem[len];
				for (int idx = 0; idx < len; idx++)
					value.AProblem [idx] = new SPropProblem (xdr);
			}
		}
	}

}
