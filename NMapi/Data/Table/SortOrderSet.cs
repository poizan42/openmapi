//
// openmapi.org - NMapi C# Mapi API - SSortOrderSet.cs
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

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;
using NMapi.Interop;

namespace NMapi.Table {


	/// <summary></summary>
	public sealed class CategorySortInfo
	{
		private SortOrder sortOrder;
		private bool isExpanded;
		
		/// <summary></summary>
		public bool IsExpanded {
			get { return isExpanded; }
		}
		
		/// <summary></summary>
		public SortOrder SortOrder {
			get { return sortOrder; }
		}
		
		/// <summary></summary>
		internal CategorySortInfo (SortOrder so, bool isExpanded)
		{
			this.sortOrder = so;
			this.isExpanded = isExpanded;
		}
		
		// TODO: equality & co.
		
	}



	/// <summary>
	///  The SortOrderSet structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530988.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class SortOrderSet
	{
		private int cCategories;
		private int cExpanded;
		private SortOrder [] aSort;


		/// <summary></summary>
		public bool HasCategories {
			get { return cCategories > 0; }
		}
		
		/// <summary>If not categorized, null is returned.</summary>
		public CategorySortInfo[] GetCategorySort ()
		{
			if (!HasCategories || aSort == null)
				return null;

			int length = Math.Min (cCategories, aSort.Length);
			
			CategorySortInfo[] result = new CategorySortInfo [length];
			for (int i=0; i < length; i++) {
				bool isExpanded = (cExpanded >= (i+1));
				result [i] = new CategorySortInfo (aSort [i], isExpanded);
			}
			return result;			
		}
		
		
		public SortOrderSet ()
		{
			// uninitialized case ... -- remove ?
			// (currently used by NMapi/Linq/TableQuerier.cs)
		}
		
		
		
		public SortOrderSet (int numberOfCategories, int numberOfExpanded, SortOrder[] so)
		{
			this.cCategories = numberOfCategories;
			this.cExpanded = numberOfExpanded;
			this.aSort = so;
		}
		
		
		/// <summary></summary>
		[DataMember (Name="CCategories")]
		public int CCategories {
			get { return cCategories; }
			set { cCategories = value; }
		}

		/// <summary></summary>
		[DataMember (Name="CExpanded")]
		public int CExpanded {
			get { return cExpanded; }
			set { cExpanded = value; }
		}

		/// <summary></summary>
		[DataMember (Name="ASort")]
		public SortOrder [] ASort {
			get { return aSort; }
			set { aSort = value; }
		}
		

	}

}
