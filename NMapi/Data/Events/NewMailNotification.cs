//
// openmapi.org - NMapi C# Mapi API - NewMailNotification.cs
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
	///  The NEWMAIL_NOTIFICATION structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527380.aspx
	/// </remarks>
	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class NewMailNotification : XdrAble
	{	
		private byte[] lpEntryID;
		private byte[] lpParentID;
		private int    ulFlags;
		private string lpszMessageClass;
		private int    ulMessageFlags;

		[DataMember (Name="EntryID")]
		public byte[] EntryID {
			get { return lpEntryID; }
			set { lpEntryID = value; }
		}

		[DataMember (Name="ParentID")]
		public byte[] ParentID {
			get { return lpParentID; }
			set { lpParentID = value; }
		}

		[DataMember (Name="Flags")]
		public int Flags {
			get { return ulFlags; }
			set { ulFlags = value; }
		}

		[DataMember (Name="MessageClass")]
		public string MessageClass {
			get { return lpszMessageClass; }
			set { lpszMessageClass = value; }
		}

		[DataMember (Name="MessageFlags")]
		public int MessageFlags {
			get { return ulMessageFlags; }
			set { ulMessageFlags = value; }
		}

		[Obsolete]
		public NewMailNotification (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeDynamicOpaque (lpEntryID);
			xdr.XdrEncodeDynamicOpaque (lpParentID);
			xdr.XdrEncodeInt (ulFlags);
			if ((ulFlags & 0x80000000) != 0)
				new LPWStr (lpszMessageClass).XdrEncode (xdr);
			else
				new LPStr (lpszMessageClass).XdrEncode (xdr);
			xdr.XdrEncodeInt (ulMessageFlags);
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			lpEntryID = xdr.XdrDecodeDynamicOpaque ();
			lpParentID = xdr.XdrDecodeDynamicOpaque ();
			ulFlags = xdr.XdrDecodeInt ();
			if ((ulFlags & 0x80000000) != 0)
				lpszMessageClass = new LPWStr (xdr).value;
			else
				lpszMessageClass = new LPStr (xdr).value;
			ulMessageFlags = xdr.XdrDecodeInt();
		}
	
	}

}
