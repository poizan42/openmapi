//
// openmapi.org - NMapi C# Mapi API - RowSetPtrAdapter.cs
//
// Copyright 2008 VipCom AG
// Copyright 2009 Topalis AG
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
	public sealed class RowSetPtrAdapter : IXdrAble
	{
		private RowSet _value;

		/// <summary>
		///   
		/// </summary>
		public RowSet Value {
			get { return _value; }
			set { _value = value; }
		}

		/// <summary>
		///
		/// </summary>
		public RowSetPtrAdapter ()
		{
		}

		/// <summary>
		///
		/// </summary>
		public RowSetPtrAdapter (RowSet value)
		{
			this._value = value;
		}

		/// <summary>
		///
		/// </summary>
		public RowSetPtrAdapter (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrEncode called: " + this.GetType ().Name);
			if (_value == null)
				xdr.XdrEncodeInt (~0);
			else {
				xdr.XdrEncodeInt (_value.ARow.Length);
				for (int idx = 0; idx < _value.ARow.Length; idx++)
					((IXdrEncodeable) _value.ARow [idx]).XdrEncode (xdr);	// TODO: we need to check for a NULL value here (and throw exceptions!)
			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrDecode called: " + this.GetType ().Name);
			int len = xdr.XdrDecodeInt ();
			if (len == ~0)
				_value = null;
			else {
				_value = new RowSet ();
				_value.ARow = new Row [len];
				for (int idx = 0; idx < len; idx++)
					_value.ARow [idx] = new Row (xdr);
			}
		}
	}

}
