//
// openmapi.org - NMapi C# Mapi API - SRow.cs
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

namespace NMapi.Table {

	/// <summary>
	///  The SRow structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526769.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class Row : IXdrAble
	{	
		public PropertyValue[] lpProps;

		public Row () 
		{
			lpProps = new PropertyValue [0];
		}
	
		public Row (PropertyValue [] values)
		{
			lpProps = values;
		}

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="Props")]
		public PropertyValue [] Props
		{
			get { return lpProps; }
			set { lpProps = value; }
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>
		///  This method is unique to NMapi.
		/// </remarks>
		public PropertyValue FindProperty (int propertyName)
		{
			int index = FindPropertyIndex (propertyName);
			if (index >= 0)
				return lpProps [index];
			return null;
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>
		///  This method is unique to NMapi.
		/// </remarks>
    	public int FindPropertyIndex (int propertyName)
		{
			if (lpProps == null)
				return -1;
			int i = 0;
			foreach (PropertyValue prop in lpProps) {
				if (prop != null && prop.PropTag == propertyName)
					return i;
				i++;
			}
			return -1;
		}

		internal Row (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		void IXdrEncodeable.XdrEncode (XdrEncodingStream xdr)
		{
			XdrEncode (xdr);
		}

		[Obsolete]
		void IXdrDecodeable.XdrDecode (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		private void XdrEncode (XdrEncodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrEncode called: " + this.GetType ().Name);
			int size = lpProps.Length;
			xdr.XdrEncodeInt (size);
			for (int i = 0; i < size; i++)
				((IXdrEncodeable) lpProps [i]).XdrEncode (xdr); // TODO: check for null values!
		}

		[Obsolete]
		private void XdrDecode (XdrDecodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrDecode called: " + this.GetType ().Name);
			int size = xdr.XdrDecodeInt ();
			lpProps = new PropertyValue [size];
			for (int i = 0; i < size; i++)
				lpProps [i] = PropertyValue.Decode (xdr);
		}
	
	}

}
