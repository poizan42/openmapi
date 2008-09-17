//
// openmapi.org - NMapi C# Mapi API - SPropertyRestriction.cs
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
	///  The SPropertyRestriction structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529141.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class SPropertyRestriction : XdrAble
	{
		private RelOp relop;
		private int ulPropTag;
		private SPropValue lpProp;

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="RelOp")]
		public RelOp RelOp {
			get { return relop; }
			set { relop = value; }
		}

		/// <summary>
		///   Must match Prop.PropTag!
		/// </summary>
		[DataMember (Name="PropTag")]
		public int PropTag {
			get { return ulPropTag; }
			set { ulPropTag = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="Prop")]
		public SPropValue Prop {
			get { return lpProp; }
			set { lpProp = value; }
		}


		public SPropertyRestriction() 
		{
		}

		internal SPropertyRestriction(XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt ((int) relop);
			xdr.XdrEncodeInt (ulPropTag);
			new LPSPropValue (lpProp).XdrEncode (xdr);
		}

		[Obsolete]
		public void XdrDecode(XdrDecodingStream xdr)
		{
			relop = (RelOp) xdr.XdrDecodeInt ();
			ulPropTag = xdr.XdrDecodeInt ();
			lpProp = new LPSPropValue (xdr).value;
		}
	
	}

}
