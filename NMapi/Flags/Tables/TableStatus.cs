//
// openmapi.org - NMapi C# Mapi API - TableStatus.cs
//
// Copyright 2008-2010 Topalis AG
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

namespace NMapi.Flags {

	/// <summary>
	///  Indicates the current state of a table that can be retrieved 
	///  by getting <see cref="NMapi.IMapiTable.Status" /> C# property.
	/// </summary>
	/// <remarks>
	///  The client might want to look at this, if the provider does not 
	///  support events to poll for compeletion of asynchronous Sorting/Restrict 
	///  operations. It may also want to check the state when preparing for 
	///  a new restriction or when it is critical that the data in the table is 
	///  consistent with the sort-order and current restrictions.
	/// </remarks>
	public enum TableStatus
	{
		/// <summary>
		///  All asynchronous operations like <see cref="M:NMapi.IMapiTable.Sort" /> 
		///  or <see cref="M:NMapi.IMapiTable.Restrict" />, if they had been 
		///  called have completed.
		/// </summary>
		/// <remarks>The classic MAPI name for this value is TBLSTAT_COMPLETE.</remarks>
		Complete = 0,

		/// <summary>
		///  The data of objects in the table (including those that would be visible, 
		///  were it not for a restriction that they can't fulfill) underlying the table 
		///  has changed. If the <see cref="NMapi.Flags.TableType" /> is not dynamic 
		///  the client might have to deal with this fact manually. 
		/// </summary>
		/// <remarks>The classic MAPI name for this value is TBLSTAT_QCHANGED.</remarks>
		QChanged = 7,

		/// <summary>The table is being sorted currently.</summary>
		/// <remarks>The classic MAPI name for this value is TBLSTAT_SORTING.</remarks>
		Sorting = 9,

		/// <summary>A sort operation failed.</summary>
		/// <remarks>The classic MAPI name for this value is TBLSTAT_SORT_ERROR.</remarks>
		SortError = 10,

		/// <summary>Columns are currently being set on the table.</summary>
		/// <remarks>The classic MAPI name for this value is TBLSTAT_SETTING_COLS.</remarks>
		SettingCols = 11,

		/// <summary>Changing the set of columns on the table failed.</summary>
		/// <remarks>The classic MAPI name for this value is TBLSTAT_SETCOL_ERROR.</remarks>
		SetColError = 13,

		/// <summary>The table is currently being filtered/restricted.</summary>
		/// <remarks>The classic MAPI name for this value is TBLSTAT_RESTRICTING.</remarks>
		Restricting = 14,

		/// <summary>A restrict operation failed.</summary>
		/// <remarks>The classic MAPI name for this value is TBLSTAT_RESTRICT_ERROR.</remarks>
		RestrictError = 15
	}
}
