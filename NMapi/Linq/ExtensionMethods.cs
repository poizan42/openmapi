//
// openmapi.org - NMapi C# Mapi API - ExtensionMethods.cs
//
// Copyright 2008 Topalis AG
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
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using NMapi.Table;

namespace NMapi.Linq {

	/// <summary>
	///  Provides some extension methods on IQueryable.
	/// </summary>
	public static class IQueryableMapiExtender
	{
		private static MapiQuery<MEntity> EnsureMapiQuery<MEntity> (
			IQueryable<MEntity> query) where MEntity:class, IMapiEntity, new ()
		{
			MapiQuery<MEntity> mapiQuery = query as MapiQuery<MEntity>;
			if (mapiQuery == null)
				throw new NotSupportedException ("Only supported for Mapi-Linq-Objects!");
			return mapiQuery;
		}

		public static IEnumerable<MEntity> SharedRangeFromBookmark<MEntity> (
			this IQueryable<MEntity> query, TableBookmark bookmark, int rows)
			where MEntity:class, IMapiEntity, new ()
		{
			MapiQuery<MEntity> mapiQuery = EnsureMapiQuery (query);
			return mapiQuery.Proxy_SharedRangeFromBookmark (bookmark, rows);
		}


		public static IEnumerable<MEntity> PrivateRangeFromBookmark<MEntity> (
			this IQueryable<MEntity> query, TableBookmark bookmark, int rows)
			where MEntity:class, IMapiEntity, new ()
		{
			MapiQuery<MEntity> mapiQuery = EnsureMapiQuery (query);
			return mapiQuery.Proxy_PrivateRangeFromBookmark (bookmark, rows);
		}

		public static TableBookmark GetBookmarkCurrent<MEntity> (this IQueryable<MEntity> query)
			where MEntity:class, IMapiEntity, new ()
		{
			MapiQuery<MEntity> mapiQuery = EnsureMapiQuery (query);
			return mapiQuery.Proxy_GetBookmarkCurrent ();
		}

		public static TableBookmark CreateBookmark<MEntity> (this IQueryable<MEntity> query)
			where MEntity:class, IMapiEntity, new ()
		{
			MapiQuery<MEntity> mapiQuery = EnsureMapiQuery (query);
			return mapiQuery.Proxy_CreateBookmark ();
		}

		public static int MoveCursor<MEntity> (this IQueryable<MEntity> query, int rows)
			where MEntity:class, IMapiEntity, new ()
		{
			MapiQuery<MEntity> mapiQuery = EnsureMapiQuery (query);
			return mapiQuery.Proxy_MoveCursor (rows);
		}

		public static void MoveCursorApprox<MEntity> (this IQueryable<MEntity> query, int n, int d)
			where MEntity:class, IMapiEntity, new ()
		{
			MapiQuery<MEntity> mapiQuery = EnsureMapiQuery (query);
			mapiQuery.Proxy_MoveCursorApprox (n, d);
		}

	}

}
