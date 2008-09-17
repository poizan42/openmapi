//
// openmapi.org - NMapi C# Mapi API - ExtendedNotification.cs
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
	///  The EXTENDED_NOTIFICATION structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528829.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class ExtendedNotification : XdrAble
	{
		private int    ulEvent;
		private byte[] pbEventParameters;

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="Event")]
		public int Event {
			get { return ulEvent; }
			set { ulEvent = value; }
		}

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="EventParameters")]
		public byte [] EventParameters {
			get { return pbEventParameters; }
			set { pbEventParameters = value; }
		}

		internal ExtendedNotification (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr) {
			xdr.XdrEncodeInt (ulEvent);
			xdr.XdrEncodeDynamicOpaque (pbEventParameters);
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr) {
			ulEvent = xdr.XdrDecodeInt ();
			pbEventParameters = xdr.XdrDecodeDynamicOpaque ();
		}

	}

}
