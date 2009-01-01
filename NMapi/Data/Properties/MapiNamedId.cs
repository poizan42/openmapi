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

using System.Diagnostics;
using CompactTeaSharp;

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
	public abstract class MapiNameId : IXdrEncodeable
	{
		private NMapiGuid lpguid;
		private MnId ulKind;

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
			get { return ulKind; }
			set { ulKind = value; }
		}
		
/*

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

*/

		public MapiNameId ()
		{
		}

		[Obsolete]
		public MapiNameId (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		public static MapiNameId Decode (XdrDecodingStream xdr)
		{
			Trace.WriteLine ("XdrDecode called: MapiNameId");
			
			NMapiGuid guid = new LPGuid (xdr).Value;
			MnId ulKind = (MnId) xdr.XdrDecodeInt ();
			MapiNameId result = null;
			switch (ulKind) {
				case MnId.String: result = new StringMapiNameId (xdr); break;
				case MnId.Id: result = new NumericMapiNameId (xdr); break;
			}
			result.Guid = guid;
			result.UlKind = ulKind;
			return result;			
		}
		
		[Obsolete]
		public virtual void XdrEncode (XdrEncodingStream xdr)
		{
			Trace.WriteLine ("XdrEncode called: MapiNameId");
			// must be called by derived classes!
			new LPGuid (lpguid).XdrEncode (xdr);
			xdr.XdrEncodeInt ((int) ulKind);
		}
		
		[Obsolete] public virtual void XdrDecode (XdrDecodingStream xdr)
		{
		}
	}

}
