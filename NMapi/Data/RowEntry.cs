//
// openmapi.org - NMapi C# Mapi API - RowEntry.cs
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

using System.Diagnostics;
using CompactTeaSharp;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi {

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class RowEntry : IXdrAble
	{
		[DataMember (Name="RowFlags")]
		public int ulRowFlags;

		[DataMember (Name="PropVals")]
		public SPropValue [] rgPropVals;
	
		private const int EMPTY = 5;

		public RowEntry () {
			ulRowFlags = EMPTY;
		}

		public RowEntry (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			Trace.WriteLine ("XdrEncode called: " + this.GetType ().Name);
			xdr.XdrEncodeInt (ulRowFlags);
			if (ulRowFlags != EMPTY) {
				int i, len = rgPropVals.Length;
				xdr.XdrEncodeInt(len);
				for (i = 0; i < len; i++)
					rgPropVals[i].XdrEncode(xdr);
			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			Trace.WriteLine ("XdrDecode called: " + this.GetType ().Name);
			ulRowFlags = xdr.XdrDecodeInt ();
			if (ulRowFlags == EMPTY) 
				rgPropVals = null;
			else {
				int i, len = xdr.XdrDecodeInt ();
				if (len == ~0)
					rgPropVals = null;
				else {
					rgPropVals = new SPropValue [len];
					for (i = 0; i < len; i++)
						rgPropVals[i] = SPropValue.Decode (xdr);
				}
			}
		}
	}
}
