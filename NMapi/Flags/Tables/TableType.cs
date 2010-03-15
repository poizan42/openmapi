//
// openmapi.org - NMapi C# Mapi API - TableType.cs
//
// Copyright 2010 Topalis AG
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
	///  Possible Values from the type of an IMapiTable that can be retrieved 
	///  by getting <see cref="NMapi.IMapiTable.State" /> of the IMapiTable object.
	/// </summary>
	/// <remarks>
	///  MAPI tables can be dynamically updated by design. However not all tables 
	///  always provide support for that or just partially support it. The TableType 
	///  allows the client to distinguish between those and act accordingly.
	/// </remarks>
	public enum TableType
	{
		/// <summary>
		///  Indicates, that the content of the table represents a snapshot of 
		///  the state of the underlying objects at the time when the table was 
		///  created. It therefore may not reflect the latest state if one of 
		///  these objects does change.
		/// </summary>
		/// <remarks>In Classic MAPI this constant is called TBLTYPE_SNAPSHOT.</remarks>
		Snapshot = 0,
		
		/// <summary>
		///  <para>
		///   Indicates that the size of the table is fixed after it has been 
		///   created, but the content of the rows refers to the latest state.
		///  </para>
		///  <para>
		///   No rows will be added or removed, even if changes to other objects 
		///   or creation of new ones allows for them to be included in the table. 
		///   Likewise, rows of objects that have been deleted or do not match 
		///   the restriction anymore are not removed. 
		///  </para>
		///  <para>
		///   When rows are retrieved with <see cref="IMapiTable.QueryRows" /> 
		///   the properties returned are built from the latest data of these 
		///   objects. Therefore, it is theoretically possible that data is 
		///   returned that does not match the specified restriction.
		///  </para>
		/// </summary>
		/// <remarks>In Classic MAPI this constant is called TBLTYPE_KEYSET.</remarks>
		KeySet,
		
		/// <summary>
		///  Indicates that the data currently in the table is being constantly 
		///  updated, when changes to the underlying objects are made.
		/// </summary>
		/// <remarks>In Classic MAPI this constant is called TBLTYPE_DYNAMIC.</remarks>
		Dynamic
	}
	
}
