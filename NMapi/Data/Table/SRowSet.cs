//
// openmapi.org - NMapi C# Mapi API - SRowSet.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;
using NMapi.Interop;

namespace NMapi.Table {

	/// <summary>
	///  The SRowSet structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527417.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class SRowSet : IEnumerable<SRow>
	{
		private SRow [] aRow;
	    
		/// <summary>
		///   
		/// </summary>
		[DataMember (Name="ARow")]
		public SRow [] ARow {
			get { return aRow; }
			set { aRow = value; }
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>
		///  This property is unique to NMapi.
		/// </remarks>
		public int Count {
			get {
				if (aRow == null)
					return 0;
				return aRow.Length;
			}
		}

		//  
		// provide some proxy access for aRow ...
		//

		public SRow this [int index]
		{
			get {
				if (aRow == null)
					return null;
				return aRow [index];
			}
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		public IEnumerator<SRow> GetEnumerator ()
		{
			if (aRow == null)
				yield break;
			foreach (SRow row in aRow)
				yield return row;
		}

		/// <summary>
		///
		/// </summary>
		public SRowSet ()
		{
		}

		/// <summary>
		///
		/// </summary>		
		public SRowSet (SRow [] values)
		{
			aRow = values;
		}
	}

}
