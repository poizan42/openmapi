//
// openmapi.org - NMapi C# Mapi API - AdrEntry.cs
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
using System.Runtime.Serialization;
using System.IO;
using RemoteTea.OncRpc;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi {

	/// <summary>
	///  The ADRENTRY structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529873.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class AdrEntry : XdrAble
	{
		private int          ulReserved1;
		private SPropValue[] rgPropVals;

		#region C#-ification

		/// <summary>
		///  This value is reserved.
		/// </summary>
		[DataMember (Name="Reserved1")]
		public int Reserved1 {
			get { return ulReserved1; }
			set { ulReserved1 = value; }
		}

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="PropVals")]
		public SPropValue[] PropVals {
			get { return rgPropVals; }
			set { rgPropVals = value; }
		}

		#endregion

		public AdrEntry ()
		{
			ulReserved1 = 0;
		}
	
		public AdrEntry (SPropValue [] values)
		{
			ulReserved1 = 0;
			rgPropVals = values;
		}

		internal AdrEntry (XdrDecodingStream xdr)
		{
			ulReserved1 = 0;
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			int _size = rgPropVals.Length;
			xdr.XdrEncodeInt(_size);
			for (int _idx = 0; _idx < _size; ++_idx) {
				rgPropVals[_idx].XdrEncode(xdr);
			}
		}


		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			int _size = xdr.XdrDecodeInt();
			rgPropVals = new SPropValue [_size];
			for (int _idx = 0; _idx < _size; ++_idx) {
				rgPropVals[_idx] = new SPropValue (xdr);
			}
		}
	
	}

}

