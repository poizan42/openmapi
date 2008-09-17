//
// openmapi.org - NMapi C# Mapi API - TableNotification.cs
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

namespace NMapi.Events {

	/// <summary>
	///  The TABLE_NOTIFICATION structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528405.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class TableNotification : XdrAble
	{	
		private TableNotificationType ulTableEvent;
		private int        hResult;
		private SPropValue propIndex;
		private SPropValue propPrior;
		private SRow       row;

		[DataMember (Name="TableEvent")]
		public TableNotificationType TableEvent {
			get { return ulTableEvent; }
			set { ulTableEvent = value; }
		}

		[DataMember (Name="HResult")]
		public int HResult {
			get { return hResult; }
			set { hResult = value; }
		}

		[DataMember (Name="PropIndex")]
		public SPropValue PropIndex {
			get { return propIndex; }
			set { propIndex = value; }
		}

		[DataMember (Name="PropPrior")]
		public SPropValue PropPrior {
			get { return propPrior; }
			set { propPrior = value; }
		}

		[DataMember (Name="Row")]
		public SRow Row {
			get { return row; }
			set { row = value; }
		}

		public TableNotification () 
		{
		}

		internal TableNotification (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt ((int)ulTableEvent);
			xdr.XdrEncodeInt (hResult);
			propIndex.XdrEncode (xdr);
			propPrior.XdrEncode (xdr);
			row.XdrEncode (xdr);
		}

		[Obsolete]
		public void XdrDecode(XdrDecodingStream xdr) 
		{
			ulTableEvent = (TableNotificationType) xdr.XdrDecodeInt ();
			hResult = xdr.XdrDecodeInt ();
			propIndex = new SPropValue (xdr);
			propPrior = new SPropValue (xdr);
			row = new SRow (xdr);
		}
	}

}
