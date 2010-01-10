//
// openmapi.org - NMapi C# Mapi API - StringAdapter.cs
//
// Copyright 2009 Topalis AG
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

		private Encoding GetSessionStrEncoding (string xdrCharacterEncoding)
		{
			if (xdrCharacterEncoding != null) {
				Console.WriteLine ("CHARACTER ENCODING !!! UNTESTED CODE -- TODO -- BE CAREFUL: " + xdrCharacterEncoding);
				return Encoding.GetEncoding (xdrCharacterEncoding);
			}
			return Encoding.GetEncoding ("windows-1252");
		}
		
		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrEncode called: " + this.GetType ().Name);
			if (value == null)
				xdr.XdrEncodeInt (~0);
			else {
				Encoding encoding = GetSessionStrEncoding (xdr.CharacterEncoding);
					
				// yes, it just HAS to be NULL-terminated. 
				// No, it wasn't my idea and we can't fix it because 
				// non-NMapi-components are depending on this.
				byte[] bytes = encoding.GetBytes (value + '\0'); 
				int len = bytes.Length;

				xdr.XdrEncodeInt (len-1);
				xdr.XdrEncodeOpaque (bytes, len);

			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrDecode called: " + this.GetType ().Name);
			int len = xdr.XdrDecodeInt ();
			if (len == ~0)
				value = null;
			else {
				Encoding encoding = GetSessionStrEncoding (xdr.CharacterEncoding);

				// Conversations is actually sending null-terminated strings 
				// _sometimes_ which led to bug when connecting to Outlook that took me _ages_
				// to reproduce. We fix this here, once and for all!
				string tmp = value = encoding.GetString (xdr.XdrDecodeOpaque(len+1));
				int index = tmp.IndexOf ('\0');
				if (index >= 0)
					tmp = tmp.Substring (0, index);
				this.value = tmp;
			}
		}
	}
}
