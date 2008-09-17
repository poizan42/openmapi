//
// openmapi.org - NMapi C# Mapi API - SComparePropsRestriction.cs
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
using System.Runtime.Serialization;

using RemoteTea.OncRpc;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;
using NMapi.Interop;

namespace NMapi.Table {

	/// <summary>
	///  The SComparePropsRestriction structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530910.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class SComparePropsRestriction : XdrAble
	{
		private RelOp relop;
		private int ulPropTag1;
		private int ulPropTag2;

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="RelOp")]
		public RelOp RelOp {
			get { return relop; }
			set { relop = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="PropTag1")]
		public int PropTag1 {
			get { return ulPropTag1; }
			set { ulPropTag1 = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="PropTag2")]
		public int PropTag2 {
			get { return ulPropTag2; }
			set { ulPropTag2 = value; }
		}

		public SComparePropsRestriction () 
		{
		}

		internal SComparePropsRestriction (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt ((int) relop);
			xdr.XdrEncodeInt (ulPropTag1);
			xdr.XdrEncodeInt (ulPropTag2);
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			relop = (RelOp) xdr.XdrDecodeInt ();
			ulPropTag1 = xdr.XdrDecodeInt ();
			ulPropTag2 = xdr.XdrDecodeInt ();
		}

	}

}
