//
// openmapi.org - NMapi C# Mapi API - SCommentRestriction.cs
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
	///  The SCommentRestriction structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527928.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class SCommentRestriction : XdrAble
	{	
		private SPropValue[] lpProp;
		private SRestriction lpRes;

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="Prop")]
		public SPropValue[] Prop {
			get { return lpProp; }
			set { lpProp = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="Res")]
		public SRestriction Res {
			get { return lpRes; }
			set { lpRes = value; }
		}

		public SCommentRestriction() 
		{
		}

		internal SCommentRestriction (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			int _size = lpProp.Length;
			xdr.XdrEncodeInt(_size);
			for (int _idx = 0; _idx < _size; ++_idx)
				lpProp[_idx].XdrEncode (xdr);
			if (lpRes != null) {
				xdr.XdrEncodeBoolean (true);
				lpRes.XdrEncode (xdr);
			} else
				xdr.XdrEncodeBoolean (false);
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			int _size = xdr.XdrDecodeInt ();
			lpProp = new SPropValue[_size];
			for (int _idx = 0; _idx < _size; ++_idx)
				lpProp[_idx] = new SPropValue (xdr);
			lpRes = xdr.XdrDecodeBoolean() ? new SRestriction (xdr) : null;
		}

	}

}
