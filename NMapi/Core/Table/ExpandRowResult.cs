//
// openmapi.org - NMapi C# Mapi API - ExpandRowResult.cs
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

using System.Runtime.Serialization;

namespace NMapi.Table {

	/// <summary>
	///  The result of the {@link IMAPITable#ExpandRow} method.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class ExpandRowResult
	{	
		private SRowSet _lpRows;
		private int _ulMoreRows;
	

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="Rows")]
		public SRowSet Rows {
			get { return _lpRows; }
			set { _lpRows = value; }
		}

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="MoreRows")]
		public int MoreRows {
			get { return _ulMoreRows; }
			set { _ulMoreRows = value; }
		}

		public ExpandRowResult ()
		{
		}
	
	}

}
