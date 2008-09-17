//
// openmapi.org - NMapi C# Mapi API - EntryList.cs
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
	///  The ENTRYLIST structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531220.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class EntryList : XdrAble
	{
		private SBinary [] lpbin;
		
		#region C#-ification

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="Bin")]
		public SBinary [] Bin {
			get { return lpbin; }
			set { lpbin = value; }
		}

		#endregion

		/// <summary>
		///  Allocates a empty EntryList.
		///  You have to allocate the members.
		/// </summary>
		public EntryList () 
		{
		}
	

		/// <summary>
		///  Allocates an EntryList from a <see cref="SBinary">SBinary</see> array.
		///  <param name="values">The SBinary array.</param>
		/// </summary>
		public EntryList (SBinary [] values) 
		{
			this.lpbin = values;
		}
	
		/// <summary>
		///  Allocates an ENTRYLIST for <param name="count">count</param> 
		///  entries. The lbin array also gets allocated, you only have 
		///  to provide the lpb of <see cref="SBinary">SBinary</see>
		/// </summary>
	
		public EntryList (int count)
		{
			lpbin = new SBinary [count];
		}

		internal EntryList (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			int _size = lpbin.Length;
			xdr.XdrEncodeInt (_size);
			for (int idx = 0; idx < _size; ++idx) {
				lpbin [idx].XdrEncode (xdr);
			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			int _size = xdr.XdrDecodeInt();
			lpbin = new SBinary [_size];
			for (int _idx = 0; _idx < _size; ++_idx) {
				lpbin[_idx] = new SBinary(xdr);
			}
		}
	
	}

}
