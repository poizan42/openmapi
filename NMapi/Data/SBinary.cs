//
// openmapi.org - NMapi C# Mapi API - SBinary.cs
//
// Copyright 2008 VipCom AG, Topalis AG
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
using System.Runtime.Serialization;
using System.IO;

using System.Diagnostics;
using CompactTeaSharp;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi {

	// TODO: Optimize this class for best performance possible.

	/// <summary>
	///  The SBinary structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528837.aspx
	/// </remarks>
	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class SBinary : IXdrAble, ICloneable, IComparable
	{
		private string hexString = null;
		private byte[] _lpb; // Do NOT access directly!

		[DataMember (Name="lpb")]
		public byte[] lpb {
			get { return _lpb; }
			set {
				_lpb = value;
				hexString = null;
			}
		}
		
		/// <summary>
		///  
		/// </summary>
		public byte[] ByteArray {
			get { return lpb; }
			set { lpb = value; }
		}


		public SBinary () 
		{
			lpb = null;
		}

		public SBinary (byte[] value) 
		{
			lpb = value;
		}
		
		/// <summary>
		///  
		/// </summary>
		public bool Equals (SBinary bin2)
		{
			if (bin2 == null)
				return false;

			if (_lpb == null && bin2._lpb == null) // both null
				return true;
			if (_lpb != null && bin2._lpb != null) { // both NOT null
				if (_lpb.Length != bin2._lpb.Length)
					return false;
				for (int i=0;i<_lpb.Length;i++)
					if (_lpb [i] != bin2._lpb [i])
						return false;
				return true;
			}
			return false; // one is NOT null!
		}
		
		/// <summary>
		///  
		/// </summary>
		public override bool Equals (object o)
		{
			if (o == this)
				return true;
			SBinary bin2 = o as SBinary;
			if (bin2 == null)
				return false;
			return this.Equals (bin2);
		}
		
		/// <summary>
		///  
		/// </summary>
		public string ToHexString ()
		{
			if (hexString != null)
				return hexString;
			StringBuilder result = new StringBuilder ();
			if (lpb != null) {
				for (int i = 0; i < lpb.Length; i++) {
					int c = ((int) lpb[i]) & 0xff;
					string num = c.ToString ("x");;
					if (num.Length < 2)
						num = "0" + num;
					result.Append (num);
				}
				hexString = result.ToString ();
			}
			return (hexString != null) ? hexString : "";
		}
		
		public override string ToString ()
		{
			return "{SBinary: " + ToHexString () + "}";
		}
		

		public override int GetHashCode ()
		{
			int hash = 0;
			if (_lpb != null) {
				for (int i = 0; i < _lpb.Length; i++)
					hash = 31 * hash + _lpb [i];
			}
			return hash;
		}

		public SBinary (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
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

		internal void XdrEncode (XdrEncodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled) {
			//			Trace.WriteLine ("XdrEncode called: " + this.GetType ().Name);
				Trace.WriteLine ("XdrEncode-DEBUG: " + ((lpb == null) ? " NULL " : new SBinary (lpb).ToHexString ()));
			}
			if (lpb == null)
				xdr.XdrEncodeDynamicOpaque (new byte[0]);
			else
				xdr.XdrEncodeDynamicOpaque (lpb);
		}

		internal void XdrDecode (XdrDecodingStream xdr)
		{
//			if (NMapi.Utility.Debug.XdrTrace.Enabled)
//				Trace.WriteLine ("XdrDecode called: " + this.GetType ().Name);
			lpb = xdr.XdrDecodeDynamicOpaque ();
			if (lpb.Length == 0)
				lpb = null;
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrDecode-DEBUG: " + ((lpb == null) ? " NULL " : new SBinary (lpb).ToHexString ()));
		}
		
		/// <summary>
		///  Implementation of the IComparable interface.
		/// </summary>
		public int CompareTo (object obj)
		{
			if (!(obj is SBinary))
				throw new ArgumentException ("Not an SBinary object.");

			byte[] b1 = ByteArray;
			byte[] b2 = ((SBinary) obj).ByteArray;
			if (b1 == null && b2 == null)
				return 0;
			if (b1 == null && b2 != null)
				return -1;
			if (b1 != null && b2 == null)
				return 1;

			// Neither one is NULL => we now check each byte.
			// If two bytes in the iteration are different, we are done.
			// If one array is shorter and there is still no difference, the length is compared.
			int shorterLength = Math.Min (b1.Length, b2.Length);
			
			for (int i=0; i < shorterLength; i++)
				if (!b1 [i].Equals (b2 [i]))
					return b1 [i].CompareTo (b2 [i]);
			return b1.Length.CompareTo (b2.Length);
		}
		
		/// <summary>
		///  Implementation of the ICloneable interface.
		/// </summary>
		public object Clone ()
		{
			byte[] lpbCopy = null;
			if (lpb != null) {
				lpbCopy = new byte [lpb.Length];
				Array.Copy (lpb, 0, lpbCopy, 0, lpb.Length);
			}
			return new SBinary (lpbCopy);
		}
	
	}

}
