//
// openmapi.org - NMapi C# Mapi API - LPWStr.cs
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
using System.Text;
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
	public sealed class LPWStr : IXdrAble
	{
		public string value;
		
		public string Value {
			get { return value; }
			set { this.value = value; }
		}
		
		public LPWStr ()
		{
			value = null;
		}

		public LPWStr (string value)
		{
			this.value = value;
		}

		public LPWStr (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			Trace.WriteLine ("XdrEncode called: " + this.GetType ().Name);
			if (value == null)
				xdr.XdrEncodeInt (~0);
			else {
				string val = value + "\0";
				int len = val.Length;
				int idx;

				xdr.XdrEncodeInt (len - 1);
				for (idx = 0; idx < len; idx++)
					xdr.XdrEncodeShort ((short) val [idx]);
			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			Trace.WriteLine ("XdrDecode called: " + this.GetType ().Name);
			int len = xdr.XdrDecodeInt ();
			if (len == ~0)
				value = null;
			else {
				value = "";
				for (int idx = 0; idx < len; idx++)
					value += (char) xdr.XdrDecodeShort();
				xdr.XdrDecodeShort ();
			}
		}
	}
}
