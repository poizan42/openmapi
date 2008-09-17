//
// openmapi.org - NMapi C# Mapi API - QueryPositionResult.cs
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
	///  The result of {@link IMAPITable#QueryPosition} method.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class QueryPositionResult
	{	
		private int _ulRow;
		private int _ulNumerator;
		private int _ulDenominator;
	
		/// <summary>
		///
		/// </summary>
		[DataMember (Name="Row")]
		public int Row {
			get { return _ulRow; }
			set { _ulRow = value; }
		}

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="Numerator")]
		public int Numerator {
			get { return _ulNumerator; }
			set { _ulNumerator = value; }
		}

		/// <summary>
		///
		/// </summary>
		[DataMember (Name="Denominator")]
		public int Denominator {
			get { return _ulDenominator; }
			set { _ulDenominator = value; }
		}

		public QueryPositionResult ()
		{
		}
	
	}
}
