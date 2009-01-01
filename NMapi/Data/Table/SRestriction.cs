//
// openmapi.org - NMapi C# Mapi API - SRestriction.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
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

using System.Diagnostics;
using CompactTeaSharp;


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
	public abstract class SRestriction : IXdrAble
	{
		public static SRestriction Decode (XdrDecodingStream xdr)
		{
			RestrictionType rt = (RestrictionType) xdr.XdrDecodeInt ();
			switch (rt) {
				case RestrictionType.CompareProps: return new SComparePropsRestriction (xdr);
				case RestrictionType.And: return new SAndRestriction (xdr);
				case RestrictionType.Or: return new SOrRestriction (xdr);
				case RestrictionType.Not: return new SNotRestriction (xdr);
				case RestrictionType.Content: return new SContentRestriction (xdr);
				case RestrictionType.Property: return new SPropertyRestriction (xdr);
				case RestrictionType.Bitmask: return new SBitMaskRestriction (xdr);
				case RestrictionType.Size: return new SSizeRestriction (xdr);
				case RestrictionType.Exist: return new SExistRestriction (xdr);
				case RestrictionType.SubRestriction: return new SSubRestriction (xdr);
				case RestrictionType.Comment: return new SCommentRestriction (xdr);
			}
			throw new Exception ("Shouldn't get here°!");
		}

		[Obsolete] public virtual void XdrEncode (XdrEncodingStream xdr) { }
		[Obsolete] public virtual void XdrDecode (XdrDecodingStream xdr) { }
	
	}

}
