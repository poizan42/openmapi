//
// openmapi.org - NMapi C# Mapi API - AdrList.cs
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
	///  The ADRLIST structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529331.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class AdrList : XdrAble
	{
		private AdrEntry [] aEntries;
	
		#region C#-ification

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="AEntries")]
		public AdrEntry [] AEntries {
			get { return aEntries; }
			set { aEntries = value; }
		}

		#endregion

		/// <summary>
		///  Allocates an empty AdrList. You have to allocate the members.
		/// </summary>
		public AdrList () 
		{
		}
	
		/// <summary>
		///  Allocates a AdrList from an <see cref="AdrEntry">AdrEntry</see> array.
		///  <param name="values">The AdrEntry array.</param>
		/// </summary>
		public AdrList (AdrEntry [] values)
		{
			aEntries = values;
		}
	
		public AdrList (SRowSet rows)
		{
			aEntries = new AdrEntry [rows.ARow.Length];
			for (int i = 0; i < rows.ARow.Length; i++)
			{
				aEntries[i] = new AdrEntry ();
				aEntries[i].PropVals = rows.ARow[i].Props;
				aEntries[i].Reserved1 = 0;
			}
		}
	
		/// <summary>
		///  Allocates a AdrList for <param name="count">count</param> 
		///  entries. The AEntries array also gets allocated, you only 
		///  have to provide the properties. 
		/// </summary>

		internal AdrList (int count)
		{
			aEntries = new AdrEntry [count];
			for (int i = 0; i < count; i++)
			{
				aEntries[i] = new AdrEntry ();
			}
		}

		[Obsolete]
		internal AdrList (XdrDecodingStream xdr) 
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			int _size = aEntries.Length;
			xdr.XdrEncodeInt(_size);
			for (int _idx = 0; _idx < _size; ++_idx) {
				aEntries[_idx].XdrEncode (xdr);
			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			int _size = xdr.XdrDecodeInt ();
			aEntries = new AdrEntry [_size];
			for (int _idx = 0; _idx < _size; ++_idx) {
				aEntries[_idx] = new AdrEntry (xdr);
			}
		}
	
	}

}
