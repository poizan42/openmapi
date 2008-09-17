//
// openmapi.org - NMapi C# Mapi API - FileTime.cs
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
	///  The FILETIME structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530421.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class FileTime : XdrAble
	{
		private int dwHighDateTime;
		private int dwLowDateTime;

		[DataMember (Name="HighDateTime")]
		public int HighDateTime {
			get { return dwHighDateTime; }
			set { dwHighDateTime = value; }
		}

		[DataMember (Name="LowDateTime")]
		public int LowDateTime {
			get { return dwLowDateTime; }
			set { dwLowDateTime = value; }
		}

		private const long TICKSPERSEC = 10000000L;
		private const long SECSPERDAY = 86400;
		/* 1601 to 1970 is 369 years plus 89 leap days */
		private const long SECS_1601_TO_1970 = ((369 * 365 + 89) * SECSPERDAY);
		private const long TICKS_1601_TO_1970 = (SECS_1601_TO_1970 * TICKSPERSEC);
		private const long l32 = 0x100000000L;

		/// <summary>
		///
		/// </summary>
		public FileTime () 
		{
			dwHighDateTime = 0;
			dwLowDateTime  = 0;
		}

		/// <summary>
		///
		/// </summary>
		public FileTime (DateTime d)
		{
			// das geht genauer ...
			DateTime origin = new DateTime (1970, 1, 1, 0, 0, 0, 0);
			TimeSpan diff = d - origin;
			long timestamp = Convert.ToInt64 (Math.Floor (diff.TotalSeconds));

			long l = timestamp * TICKSPERSEC + TICKS_1601_TO_1970;

			dwHighDateTime = (int)(l >> 32);
			dwLowDateTime  = (int)(l & 0xffffffffL);
		}

		/// <summary>
		///  Gets a DateTime structure from a FileTime.
		/// </summary>
		public DateTime DateTime
		{
			get {
				// das geht genauer ...
				long l = ((((long)dwHighDateTime << 32) | 
						(long) dwLowDateTime & 0xffffffffL) - 
						TICKS_1601_TO_1970)/TICKSPERSEC;
				DateTime origin = new DateTime (1970, 1, 1, 0, 0, 0, 0);
				return origin.AddSeconds (l);
			}
		}

		internal FileTime (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt (dwHighDateTime);
			xdr.XdrEncodeInt (dwLowDateTime);
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			dwHighDateTime = xdr.XdrDecodeInt ();
			dwLowDateTime = xdr.XdrDecodeInt ();
		}

	}

}
