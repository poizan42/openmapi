//
// openmapi.org - NMapi C# Mapi API - FileTime.cs
//
// Copyright 2008 Topalis AG
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

	/// <summary>
	///  
	/// </summary>
	public partial class FileTime : IComparable
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="d"></param>
		public FileTime (DateTime d)
		{
            LongValue = d.ToFileTime();
		}

		/// <summary>
		///  Get/Set the 64bit long value
		/// </summary>
        public long LongValue
        {
			get {
                return (((long)dwHighDateTime << 32) | (long) dwLowDateTime & 0xffffffffL);
            }

            set {
                dwHighDateTime = (int) (value >> 32);
                dwLowDateTime  = (int) (value & 0xffffffffL);
            }
        }

		/// <summary>
		///  Gets a DateTime structure from a FileTime.
		/// </summary>
		public DateTime DateTime
		{
			get {
                return DateTime.FromFileTime (LongValue);
			}
		}
		
		/// <summary>Implementation of the IComparable interface.</summary>
		/// <remarks></remarks>
		/// <param name="obj"></param>
		public int CompareTo (object obj)
		{
			if (!(obj is FileTime))
				throw new ArgumentException ("Not a FileTime object.");
            FileTime other = (FileTime) obj;
            return (dwHighDateTime == other.dwHighDateTime) ?
                (dwLowDateTime - other.dwLowDateTime) : (dwHighDateTime - other.dwHighDateTime);
		}
		
		
	}

}
