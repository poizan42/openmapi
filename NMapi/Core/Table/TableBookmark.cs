//
// openmapi.org - NMapi C# Mapi API - TableBookmark.cs
//
// Copyright 2008 Topalis AG
//
// Author:    Johannes Roith <johannes@jroith.de>
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

namespace NMapi.Table {

	using System;
	using NMapi.Flags;

	/// <summary>
	///  Represents a bookmark in a Table. This class warps the 
	///  integer usually used by Mapi to represent a bookmark, 
	///  for bettery type safety.
	/// </summary>
	public class TableBookmark : IDisposable
	{
		private IMapiTable associatedTable;
		private int bookmarkID;

		/// <summary>
		///  The integer representing the bookmark.
		/// </summary>
		public int Id {
			get { return bookmarkID; }
		}

		public TableBookmark (IMapiTable associatedTable)
		{
			this.associatedTable = associatedTable;
		}

		public TableBookmark (IMapiTable associatedTable, int bookmarkID)
		{
			this.associatedTable = associatedTable;
			this.bookmarkID = bookmarkID;
		}

		/// <summary>
		///  Returns true if the bookmark represents the bookmarks 
		///  Bookmark.Beginning, Bookmark.Current or Bookmark.End.
		/// </summary>
		public bool IsPredefined {
			get {
				return (bookmarkID == Bookmark.Beginning || 
					bookmarkID == Bookmark.Current || 
					bookmarkID == Bookmark.End);
			}
		}

		public void Dispose ()
		{
			if (!IsPredefined && associatedTable != null)
				associatedTable.FreeBookmark (bookmarkID);
		}

	}

}
