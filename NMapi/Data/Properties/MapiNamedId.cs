//
// openmapi.org - NMapi C# Mapi API - MapiNameId.cs
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

namespace NMapi.Properties {

	/// <summary>
	///  The MAPINAMEID structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526422.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class MapiNameId : XdrAble
	{
		private NMapiGuid lpguid;
		private MnId _ulKind;
		private UKind _kind; // This collides with the C#-ification rules....
	
		#region C#-ifycation

		[DataMember (Name="Guid")]
		public NMapiGuid  Guid {
			get { return lpguid; }
			set { lpguid = value; }
		}

		/// <summary>
		///  The usual naming conventions can't be used here!
		///  UlKind is the same as "ulKind" in jumapi.
		/// </summary>
		[DataMember (Name="UlKind")]
		public MnId  UlKind {
			get { return _ulKind; }
			set { _ulKind = value; }
		}
		
		/// <summary>
		///  The usual naming conventions can't be used here!
		///  UKind is the same as "Kind" in jumapi.
		/// </summary>
		[DataMember (Name="UKind")]
		public UKind  UKind {
			get { return _kind; }
			set { _kind = value; }
		}
		
		
		#endregion C#-ifycation


		public MapiNameId () 
		{
			_kind = new UKind ();
		}
	
		/// <summary>
		///   Allocates a MapiNameId array. All MapiNameId 
		///   elements are initialized.
		/// </summary>
		/// <param name="count">The size of the array</param>
		/// <returns>MapiNameId</returns>
		public static MapiNameId [] HrAllocMapiNameIdArray (int count)
		{
			MapiNameId [] ret = new MapiNameId [count];
			for (int i = 0; i < count; i++)
				ret[i] = new MapiNameId ();
			return ret;
		}

		[Obsolete]
		public MapiNameId (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode(XdrEncodingStream xdr)
		{
			new LPGuid (lpguid).XdrEncode(xdr);
			xdr.XdrEncodeInt ( (int) _ulKind);
			if (_ulKind == MnId.Id)
				xdr.XdrEncodeInt (_kind.ID);
			else
				new LPWStr (_kind.StrName).XdrEncode(xdr);
		}
	
		[Obsolete]
		public void XdrDecode(XdrDecodingStream xdr)
		{
			_kind = new UKind ();
			lpguid = new LPGuid (xdr).value;
			_ulKind = (MnId) xdr.XdrDecodeInt ();
			if (_ulKind == MnId.Id)
				_kind.ID = xdr.XdrDecodeInt ();
			else
				_kind.StrName = new LPWStr (xdr).value;
		}
	
	}

}
