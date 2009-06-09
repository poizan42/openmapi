//
// openmapi.org - NMapi C# Mapi API - PropertyProblemArrayPtrAdapter.cs
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
	public sealed class PropertyProblemArrayPtrAdapter : IXdrAble
	{	
		public PropertyProblem[] value;

		/// <summary>
		///
		/// </summary>
		public PropertyProblem[] Value {
			get { return value; }
			set { this.value = value; }
		}

		/// <summary>
		///
		/// </summary>
		public PropertyProblemArrayPtrAdapter ()
		{
			value = null;
		}

		/// <summary>
		///
		/// </summary>
		public PropertyProblemArrayPtrAdapter (PropertyProblem[] value)
		{
			this.value = value;
		}

		/// <summary>
		///
		/// </summary>
		public PropertyProblemArrayPtrAdapter (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			Trace.WriteLine ("XdrEncode called: " + this.GetType ().Name);
			if (value == null)
				xdr.XdrEncodeInt (-1);
			else {
				xdr.XdrEncodeInt (value.Length);
				for (int i = 0; i < value.Length; i++)
					((IXdrEncodeable) value [i]).XdrEncode (xdr);
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
				value = new PropertyProblem [len];
				for (int idx = 0; idx < len; idx++)
					value [idx] = new PropertyProblem (xdr);
			}
		}
	}

}
