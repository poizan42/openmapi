//
// openmapi.org - NMapi C# Mapi API - ObjectNotification.cs
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
	///  The OBJECT_NOTIFICATION structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527423.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class ObjectNotification : XdrAble
	{	
		private byte[]        lpEntryID;
		private int           ulObjType;
		private byte[]        lpParentID;
		private byte[]        lpOldID;
		private byte[]        lpOldParentID;
		private SPropTagArray lpPropTagArray;

		[DataMember (Name="EntryID")]
		public byte[] EntryID {
			get { return lpEntryID; }
			set { lpEntryID = value; }
		}

		[DataMember (Name="ObjType")]
		public int ObjType {
			get { return ulObjType; }
			set { ulObjType = value; }
		}

		[DataMember (Name="ParentID")]
		public byte[] ParentID {
			get { return lpParentID; }
			set { lpParentID = value; }
		}

		[DataMember (Name="OldID")]
		public byte[] OldID {
			get { return lpOldID; }
			set { lpOldID = value; }
		}

		[DataMember (Name="OldParentID")]
		public byte[] OldParentID {
			get { return lpOldParentID; }
			set { lpOldParentID = value; }
		}

		[DataMember (Name="PropTagArray")]
		public SPropTagArray PropTagArray {
			get { return lpPropTagArray; }
			set { lpPropTagArray = value; }
		}

		[Obsolete]
		public ObjectNotification (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeDynamicOpaque (lpEntryID);
			xdr.XdrEncodeInt (ulObjType);
			xdr.XdrEncodeDynamicOpaque (lpParentID);
			xdr.XdrEncodeDynamicOpaque (lpOldID);
			xdr.XdrEncodeDynamicOpaque (lpOldParentID);
			new LPSPropTagArray (lpPropTagArray).XdrEncode (xdr);
		}

		[Obsolete]
		public void XdrDecode(XdrDecodingStream xdr)
		{
			lpEntryID = xdr.XdrDecodeDynamicOpaque();
			ulObjType = xdr.XdrDecodeInt();
			lpParentID = xdr.XdrDecodeDynamicOpaque();
			lpOldID = xdr.XdrDecodeDynamicOpaque();
			lpOldParentID = xdr.XdrDecodeDynamicOpaque();
			lpPropTagArray = new LPSPropTagArray (xdr).Value;
		}
	
	}
}
