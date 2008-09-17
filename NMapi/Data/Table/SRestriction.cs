//
// openmapi.org - NMapi C# Mapi API - SRestriction.cs
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
	///  The SRestriction structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529087.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class SRestriction : XdrAble
	{	
		private RestrictionType rt;
		private URestriction res;

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="Rt")]
		public RestrictionType Rt {
			get { return rt; }
			set { rt = value; }
		}

		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="Res")]
		public URestriction Res {
			get { return res; }
			set { res = value; }
		}

		public SRestriction() 
		{
			res = new URestriction ();
		}

		internal SRestriction(XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode(XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt ((int)rt);
			switch (rt) {
			case RestrictionType.CompareProps:
				res.ResCompareProps.XdrEncode(xdr);
				break;
			case RestrictionType.And:
				res.ResAnd.XdrEncode(xdr);
				break;
			case RestrictionType.Or:
				res.ResOr.XdrEncode(xdr);
				break;
			case RestrictionType.Not:
				res.ResNot.XdrEncode(xdr);
				break;
			case RestrictionType.Content:
				res.ResContent.XdrEncode(xdr);
				break;
			case RestrictionType.Property:
				res.ResProperty.XdrEncode(xdr);
				break;
			case RestrictionType.Bitmask:
				res.ResBitMask.XdrEncode(xdr);
				break;
			case RestrictionType.Size:
				res.ResSize.XdrEncode(xdr);
				break;
			case RestrictionType.Exist:
				res.ResExist.XdrEncode(xdr);
				break;
			case RestrictionType.SubRestriction:
				res.ResSub.XdrEncode(xdr);
				break;
			case RestrictionType.Comment:
				res.ResComment.XdrEncode(xdr);
				break;
			}
		}

		[Obsolete]
		public void XdrDecode(XdrDecodingStream xdr)
		{
			rt = (RestrictionType) xdr.XdrDecodeInt ();
			res = new URestriction ();
			switch (rt) {
			case RestrictionType.CompareProps:
				res.ResCompareProps = new SComparePropsRestriction(xdr);
				break;
			case RestrictionType.And:
				res.ResAnd = new SAndRestriction(xdr);
				break;
			case RestrictionType.Or:
				res.ResOr = new SOrRestriction(xdr);
				break;
			case RestrictionType.Not:
				res.ResNot = new SNotRestriction(xdr);
				break;
			case RestrictionType.Content:
				res.ResContent = new SContentRestriction(xdr);
				break;
			case RestrictionType.Property:
				res.ResProperty = new SPropertyRestriction(xdr);
				break;
			case RestrictionType.Bitmask:
				res.ResBitMask = new SBitMaskRestriction(xdr);
				break;
			case RestrictionType.Size:
				res.ResSize = new SSizeRestriction(xdr);
				break;
			case RestrictionType.Exist:
				res.ResExist = new SExistRestriction(xdr);
				break;
			case RestrictionType.SubRestriction:
				res.ResSub = new SSubRestriction(xdr);
				break;
			case RestrictionType.Comment:
				res.ResComment = new SCommentRestriction (xdr);
				break;
			}
		}
	
	}

}
