//
// openmapi.org - NMapi C# Mapi API - Restriction.cs
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
	///  The Restriction structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529087.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public abstract class Restriction : IXdrAble
	{
		public static Restriction Decode (XdrDecodingStream xdr)
		{
			RestrictionType rt = (RestrictionType) xdr.XdrDecodeInt ();
			switch (rt) {
				case RestrictionType.CompareProps: return new ComparePropsRestriction (xdr);
				case RestrictionType.And: return new AndRestriction (xdr);
				case RestrictionType.Or: return new OrRestriction (xdr);
				case RestrictionType.Not: return new NotRestriction (xdr);
				case RestrictionType.Content: return new ContentRestriction (xdr);
				case RestrictionType.Property: return new PropertyRestriction (xdr);
				case RestrictionType.Bitmask: return new BitMaskRestriction (xdr);
				case RestrictionType.Size: return new SizeRestriction (xdr);
				case RestrictionType.Exist: return new ExistRestriction (xdr);
				case RestrictionType.SubRestriction: return new SubRestriction (xdr);
				case RestrictionType.Comment: return new CommentRestriction (xdr);
			}
			throw new Exception ("Shouldn't get here!");
		}

		[Obsolete] public virtual void XdrEncode (XdrEncodingStream xdr) { }
		[Obsolete] public virtual void XdrDecode (XdrDecodingStream xdr) { }
		
	}

}
