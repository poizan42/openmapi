//
// openmapi.org - NMapi C# Mapi API - StringAdapter.cs
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
	public sealed class StringAdapter : IXdrAble
	{
		public string value;
		
		public string Value {
			get { return value; }
			set { this.value = value; }
		}
		
		/// <summary>
		///
		/// </summary>
		public StringAdapter ()
		{
		}
		
		/// <summary>
		///
		/// </summary>
		public StringAdapter (string value)
		{
			this.value = value;
		}
		
		public StringAdapter (XdrDecodingStream xdr)
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
				
				Console.WriteLine ("DEBUG (encoding...): " + value);
				
//				Encoding encoding = (characterEncoding != null) ? 
//					Encoding.GetEncoding (characterEncoding) : Encoding.GetEncoding (0);
				Encoding encoding = Encoding.GetEncoding ("windows-1252");
					
				byte[] bytes = encoding.GetBytes (value + '\0');
				int len = bytes.Length;

				xdr.XdrEncodeInt (len-1);
				xdr.XdrEncodeOpaque (bytes, len);

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
				
//				Encoding encoding = (characterEncoding != null) ? 
//					Encoding.GetEncoding (characterEncoding) : Encoding.GetEncoding (0);
				Encoding encoding = Encoding.GetEncoding ("windows-1252");
				value = encoding.GetString (xdr.XdrDecodeOpaque(len+1)); // TODO: Potiential BUG: Null terminated strings!
			}
		}
	}
}
