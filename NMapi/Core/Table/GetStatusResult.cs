//
// openmapi.org - NMapi C# Mapi API - GetStatusResult.cs
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
	///  The result of the {@link IMAPITable#GetStatus} method.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class GetStatusResult
	{	
		private int _ulTableStatus;
		private int _ulTableType;
	
		/// <summary>
		///
		/// </summary>
		public int TableStatus {
			get { return _ulTableStatus; }
			set { _ulTableStatus = value; }
		}

		/// <summary>
		///
		/// </summary>
		public int TableType {
			get { return _ulTableType; }
			set { _ulTableType = value; }
		}

		public GetStatusResult ()
		{
		}
	
	}

}
