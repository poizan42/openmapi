//
// openmapi.org - NMapi C# Mapi API - GetSearchCriteriaResult.cs
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

using NMapi.Table;
using System.Runtime.Serialization;

namespace NMapi.Properties.Special {

	/// <summary>
	///  The result of <see cref="M:IMapiContainer.GetSearchCriteria()">
	///  IMapiContainer.GetSearchCriteria()</see> method.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class GetSearchCriteriaResult
	{	
		private Restriction _lpRestriction;
		private EntryList    _lpContainerList;
		private int          _ulSearchState;
	
		/// <summary>
		///  The restriction currently set on the container.
		/// </summary>
		[DataMember (Name="Restriction")]
		public Restriction Restriction {
			get { return _lpRestriction; }
			set { _lpRestriction = value; }
		}
	
		/// <summary>
		///  A list of contains that are to be searched.
		/// </summary>
		[DataMember (Name="ContainerList")]
		public EntryList ContainerList {
			get { return _lpContainerList; }
			set { _lpContainerList = value; }
		}
	
		/// <summary>
		///  Represents the current state of the search. 
		///  The data is stored as several flags that can be checked.
		/// </summary>
		[DataMember (Name="SearchState")]
		public int SearchState {
			get { return _ulSearchState; }
			set { _ulSearchState = value; }
		}

		public GetSearchCriteriaResult ()
		{
		}
	
	}
}
