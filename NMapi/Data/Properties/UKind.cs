//
// openmapi.org - NMapi C# Mapi API - UKind.cs
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

namespace NMapi.Properties {

	/// <summary>
	///  A helper for the MAPINAMEID structure.
	/// </summary>
	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class UKind
	{
		private int lID;
		private string lpwstrName;
	
		[DataMember (Name="ID")]
		public int ID {
			get { return lID; }
			set { lID = value; }
		}

		[DataMember (Name="StrName")]
		public string StrName {
			get { return lpwstrName; }
			set { lpwstrName = value; }
		}

		internal UKind()
		{
		}
	
	}
}
