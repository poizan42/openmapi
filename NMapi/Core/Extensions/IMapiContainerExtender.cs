//
// openmapi.org - NMapi C# Mapi API - IMapiContainerExtender.cs
//
// Copyright 2009 Topalis AG
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

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi {

	/// <summary>
	///  Provides some extension methods on IMapiContainer.
	/// </summary>
	public static class IMapiContainerExtender
	{
		
		
		
		/*

		OpenEntry
		SetSearchCriteria
		GetSearchCriteria		

		
		*/
		
		
		
		
		
/*		ACLTABLE_FREEBUSY

		MAPI_DEFERRED_ERRORS
*/

		/// <summary>
		///  Returns the (unicode) contents table.
		/// </summary>
		/// <returns>The contents table of the current container.</returns>
		/// <exception cref="MapiNoSupportException">The container does not support the contents table.</exception>
		public static IMapiTable GetNormalContentsTable (this IMapiContainer prop)
		{
			return prop.GetContentsTable (Mapi.Unicode);
		}
		

		/// <summary>
		///  Returns the (unicode) contents table.
		/// </summary>
		/// <param name="showSoftDeletes">If true, include items that are marked as soft-deleted.</param>
		/// <returns>The contents table of the current container.</returns>
		/// <exception cref="MapiNoSupportException">The container does not support the contents table.</exception>
		public static IMapiTable GetNormalContentsTable (this IMapiContainer prop, bool showSoftDeletes)
		{
			int flags = Mapi.Unicode | 
						((showSoftDeletes) ? NMAPI.SHOW_SOFT_DELETES : 0);
			return prop.GetContentsTable (flags);
		}
		
		/// <summary>
		///  Returns the associated (unicode) contents table.
		/// </summary>
		/// <returns>The associated (!) contents table of the current container.</returns>
		/// <exception cref="MapiNoSupportException">The container does not support the contents table.</exception>
		public static IMapiTable GetAssociatedContentsTable (this IMapiContainer prop)
		{
			return prop.GetContentsTable (NMAPI.MAPI_ASSOCIATED | Mapi.Unicode);
		}
		
		
		/// <summary>
		///  Returns the associated (unicode) contents table.
		/// </summary>
		/// <param name="showSoftDeletes">If true, include items that are marked as soft-deleted.</param>
		/// <returns>The associated (!) contents table of the current container.</returns>
		/// <exception cref="MapiNoSupportException">The container does not support the contents table.</exception>
		public static IMapiTable GetAssociatedContentsTable (this IMapiContainer prop, bool showSoftDeletes)
		{
			int flags = NMAPI.MAPI_ASSOCIATED | Mapi.Unicode | 
						((showSoftDeletes) ? NMAPI.SHOW_SOFT_DELETES : 0);
			return prop.GetContentsTable (flags);
		}
		
		
		
		
		/// <summary>
		///  Returns the (unicode) hierarchy table using a depth of one.
		/// </summary>
		/// <returns>An IMapiTableReader object that provides access to the child containers of the current container.</returns>
		/// <exception cref="MapiBadCharWidthException">The OpenMapi provider does not support Unicode.</exception>
		/// <exception cref="MapiNoSupportException">The object does not support the hierarchy table.</exception>
		public static IMapiTableReader GetHierarchyTable (this IMapiContainer prop)
		{
			return prop.GetHierarchyTable (false);
		}
	
		/// <summary>
		///  Returns the (unicode) hierarchy table.
		/// </summary>		
		/// <param name="convenientDepth">If true the hierarchy table may contain multiple levels (or not, depending on the provider).</param>		
		/// <returns>An IMapiTableReader object that provides access to the child containers of the current container.</returns>
		/// <exception cref="MapiBadCharWidthException">The OpenMapi provider does not support Unicode.</exception>
		/// <exception cref="MapiNoSupportException">The object does not support the hierarchy table.</exception>
		public static IMapiTableReader GetHierarchyTable (this IMapiContainer prop, bool convenientDepth)
		{
			return prop.GetHierarchyTable (convenientDepth, false);
		}

		/// <summary>
		///  Returns the (unicode) hierarchy table.
		/// </summary>		
		/// <param name="convenientDepth">If true the hierarchy table may contain multiple levels (or not, depending on the provider).</param>		
		/// <param name="showSoftDeletes">If true, objects that have been soft-deleted will be shown.</param>
		/// <returns>An IMapiTableReader object that provides access to the child containers of the current container.</returns>
		/// <exception cref="MapiBadCharWidthException">The OpenMapi provider does not support Unicode.</exception>
		/// <exception cref="MapiNoSupportException">The object does not support the hierarchy table.</exception>
		public static IMapiTableReader GetHierarchyTable (this IMapiContainer prop, bool convenientDepth, bool showSoftDeletes)
		{
			return prop.GetHierarchyTable (convenientDepth, showSoftDeletes, true, false);
		}

		private static IMapiTableReader GetHierarchyTable (this IMapiContainer prop, bool convenientDepth, 
			bool showSoftDeletes, bool unicode, bool deferredErrors)
		{
			int flags = ((deferredErrors) ? NMAPI.MAPI_DEFERRED_ERRORS : 0) | 
			 			((unicode) ? Mapi.Unicode : 0) | 
						((convenientDepth) ? NMAPI.CONVENIENT_DEPTH : 0) | 
						((showSoftDeletes) ? NMAPI.SHOW_SOFT_DELETES : 0);
			return prop.GetHierarchyTable (flags);
		}
		
	}

}
