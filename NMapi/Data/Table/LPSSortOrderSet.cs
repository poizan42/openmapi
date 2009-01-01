//
// openmapi.org - NMapi C# Mapi API - SPSSortOrderSet.cs
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

using System.Diagnostics;
using CompactTeaSharp;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Interop {

	/// <summary>
	///  For internal use only.
	/// </summary>
	public sealed class LPSSortOrderSet : IXdrAble
	{
		private SSortOrderSet _value;

		/// <summary>
		///   
		/// </summary>
		public SSortOrderSet Value {
			get { return _value; }
			set { _value = value; }
		}

		/// <summary>
		///
		/// </summary>
		public LPSSortOrderSet()
		{
		}

		/// <summary>
		///
		/// </summary>
		public LPSSortOrderSet(SSortOrderSet value)
		{
			this._value = value;
		}

		public LPSSortOrderSet(XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			Trace.WriteLine ("XdrEncode called: " + this.GetType ().Name);
			if (_value == null)
				xdr.XdrEncodeInt(~0);
			else {
				xdr.XdrEncodeInt (_value.ASort.Length);
				xdr.XdrEncodeInt (_value.CCategories);
				xdr.XdrEncodeInt (_value.CExpanded);
				for (int idx = 0; idx < _value.ASort.Length; idx++)
					_value.ASort[idx].XdrEncode (xdr);
			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			Trace.WriteLine ("XdrDecode called: " + this.GetType ().Name);
			int len = xdr.XdrDecodeInt();
			if (len == ~0)
				_value = null;
			else {
				_value = new SSortOrderSet();
				_value.CCategories = xdr.XdrDecodeInt();
				_value.CExpanded = xdr.XdrDecodeInt();
				_value.ASort = new SSortOrder[len];
				for (int idx = 0; idx < len; idx++)
					_value.ASort[idx] = new SSortOrder (xdr);
			}
		}
	}

}
