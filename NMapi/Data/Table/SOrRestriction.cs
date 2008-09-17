//
// openmapi.org - NMapi C# Mapi API - SOrRestriction.cs
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
	///  The SOrRestriction structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530693.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class SOrRestriction : XdrAble
	{
		private SRestriction[] lpRes;

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="Res")]
		public SRestriction[] Res {
			get { return lpRes; }
			set { lpRes = value; }
		}

		public SOrRestriction() 
		{
		}
	
		public SOrRestriction (SRestriction [] values)
		{
			lpRes = values;
		}

		internal SOrRestriction(XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			int _size = lpRes.Length;
			xdr.XdrEncodeInt(_size);
			for (int _idx = 0; _idx < _size; ++_idx) {
				lpRes[_idx].XdrEncode (xdr);
			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			int _size = xdr.XdrDecodeInt ();
			lpRes = new SRestriction[_size];
			for (int _idx = 0; _idx < _size; ++_idx) {
				lpRes [_idx] = new SRestriction (xdr);
			}
		}
	
	}

}
