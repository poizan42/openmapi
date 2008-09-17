//
// openmapi.org - NMapi C# Mapi API - GetNamesFromIDsResult.cs
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
	///  The result of the <see cref="M:IMapiProp.GetNamesFromIDs()">
	///  IMapiProp.GetNamesFromIDs</see> method.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class GetNamesFromIDsResult
	{	
		private SPropTagArray _lpPropTags;
		private MapiNameId [] _lpPropNames;
	
		/// <summary>
		///  An SPropTagArray containing the PropTags that match 
		///  the requested Property Names returned in "PropNames".
		/// </summary>
		[DataMember (Name="PropTags")]
		public SPropTagArray PropTags {
			get { return _lpPropTags; }
			set { _lpPropTags = value; }
		}

		/// <summary>
		///  The Property Names for the Properties listed in "PropTags".
		/// </summary>
		[DataMember (Name="PropNames")]
		public MapiNameId [] PropNames {
			get { return _lpPropNames; }
			set { _lpPropNames = value; }
		}


		public GetNamesFromIDsResult ()
		{
		}
	
	}
}


