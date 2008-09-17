//
// openmapi.org - NMapi C# Mapi API - Guid.cs
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
	///  The GUID structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526752.aspx
	/// </remarks>
	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class NMapiGuid : XdrAble
	{
		[DataMember (Name="Data1")]
		public int     Data1 { get; set; }

		[DataMember (Name="Data2")]
		public short   Data2 { get; set; }

		[DataMember (Name="Data3")]
		public short   Data3 { get; set; }

		[DataMember (Name="Data4")]
		public byte [] Data4 { get; set; }

		public NMapiGuid () 
		{
		}

		public byte[] ToByteArray ()
		{
			byte[] result = new byte [16];
			byte[] part1 = BitConverter.GetBytes (Data1);
			Array.Copy (part1, 0, result, 0, 4);
			byte[] part2 = BitConverter.GetBytes (Data2);
			Array.Copy (part2, 0, result, 4, 2);
			byte[] part3 = BitConverter.GetBytes (Data3);
			Array.Copy (part3, 0, result, 6, 2);
			Array.Copy (Data4, 0, result, 8, 8);
			return result;
		}

		public NMapiGuid (byte[] bytes) 
		{
			Data1 = BitConverter.ToInt32 (bytes, 0);
			Data2 = BitConverter.ToInt16 (bytes, 4);
			Data2 = BitConverter.ToInt16 (bytes, 6);
			Array.Copy (bytes, 8, Data4, 0, 8);
		}

		internal NMapiGuid (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt (Data1);
			xdr.XdrEncodeShort (Data2);
			xdr.XdrEncodeShort (Data3);
			xdr.XdrEncodeOpaque (Data4, 8);
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			Data1 = xdr.XdrDecodeInt ();
			Data2 = xdr.XdrDecodeShort ();
			Data3 = xdr.XdrDecodeShort ();
			Data4 = xdr.XdrDecodeOpaque (8);
		}

	}

}
